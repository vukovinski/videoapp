using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace videoapp.api.Services
{
    public sealed class MovieTrailerResolver
    {
        private readonly ImdbResolver ImdbResolver;
        private readonly YoutubeResolver YoutubeResolver;
        private readonly ILogger<MovieTrailerResolver> Logger;

        public MovieTrailerResolver
        (
            ImdbResolver imdbResolver,
            YoutubeResolver youtubeResolver,
            ILogger<MovieTrailerResolver> logger
        )
        {
            Logger = logger;
            ImdbResolver = imdbResolver;
            YoutubeResolver = youtubeResolver;
        }

        public async Task<IList<MovieTrailerQueryResult>> Resolve(string searchString, int results, int page)
        {
            try
            {
                // query imdb api for search results, assume authorativness on existence of movies
                var imdb_results = await ImdbResolver.QueryImdb(searchString, results, page);

                // make partial objects with query results, saving metadata,
                // cross-query youtube api for potential trailers, choose best and save to partial object
                var youtube_enriched_results = await YoutubeResolver.QueryYoutubeTrailers(imdb_results);

                // filter only valid-by-result-quality
                var final_results = QualityHeuristicsEngine.Filter(youtube_enriched_results);

                // return 
                return final_results;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"error resolving movie trailers for query: {searchString}, page: {page}, n: {results}!");
            }
            return new List<MovieTrailerQueryResult>();
        }
    }
}
