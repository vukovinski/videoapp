using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace videoapp.api.Services
{
    // Thread-safe, query results cache with internal maintenance.
    public sealed class MovieTrailerQueryResultsCache
    {
        private readonly int TTL_Minutes;
        private readonly object Maintenance = new();
        private readonly ILogger<MovieTrailerQueryResultsCache> Logger;
        private readonly ConcurrentDictionary<int, CacheFrame> Store = new();

        public MovieTrailerQueryResultsCache
        (
            IConfiguration config,
            ILogger<MovieTrailerQueryResultsCache> logger
        )
        {
            TTL_Minutes = config.GetValue<int>("CacheMinutes");
            Logger = logger;
        }

        public void Member(string searchString, int results, int page, IEnumerable<MovieTrailerQueryResult> queryResults)
        {
            var frame = new CacheFrame(queryResults);
            var hash = CalculateQueryHash(searchString, results, page);
            if (Store.TryAdd(hash, frame))
                Logger.LogDebug($"Hash: {hash}, item added to cache");
        }

        public bool AreResultsCached(string searchString, int results, int page, out IEnumerable<MovieTrailerQueryResult> previousResults)
        {
            bool success;
            previousResults = null;
            var hash = CalculateQueryHash(searchString, results, page);
            if (Store.TryGetValue(hash, out CacheFrame cached) && HasntExpired(cached))
            {
                Logger.LogDebug($"Hash: {hash}, cache hit!");
                previousResults = cached.Results;
                success = true;
            }
            else
            {
                Logger.LogDebug($"Hash: {hash}, cache miss!");                
                success = false;
            }

            // always run background maintenance
            Task.Run(() => RunMaintenance());

            return success;
        }

        private int CalculateQueryHash(string searchString, int results, int page)
        {
            return searchString.GetHashCode() ^ results.GetHashCode() ^ page.GetHashCode();
        }

        #region maintenance
        private void RunMaintenance()
        {
            // check for expirations to reduce memory usage,
            // only iff maintenance not in progress
            try
            {
                if (Monitor.TryEnter(Maintenance))
                {
                    var to_remove = new List<int>();
                    var under_inspection = Store.ToArray();
                    for (int i = 0; i < under_inspection.Length; i++)
                    {
                        var hash = under_inspection[i].Key;
                        var frame = under_inspection[i].Value;
                        if (!HasntExpired(frame))
                        {
                            to_remove.Add(hash);
                        }
                    }

                    foreach (var key_to_remove in to_remove)
                    {
                        // removal may fail because of other threads, so we have to loop
                        while (Store.ContainsKey(key_to_remove))
                        {
                            if (Store.TryRemove(key_to_remove, out CacheFrame _))
                            {
                                Logger.LogDebug($"Hash: {key_to_remove}, cache maintenance evicted item");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // OK
            }
            finally
            {
                if (Monitor.IsEntered(Maintenance))
                {
                    Monitor.Exit(Maintenance);
                }
            }
        }
        #endregion

        private bool HasntExpired(CacheFrame frame)
        {
            return DateTime.UtcNow < frame.CachedAt.AddMinutes(TTL_Minutes);
        }

        sealed class CacheFrame
        {
            public readonly DateTime CachedAt;
            public readonly List<MovieTrailerQueryResult> Results;

            public CacheFrame(IEnumerable<MovieTrailerQueryResult> results)
            {
                Results = results.ToList();
                CachedAt = DateTime.UtcNow;
            }
        }
    }
}
