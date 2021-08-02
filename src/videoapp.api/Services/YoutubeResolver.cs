using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

using videoapp.api.Services.Imdb;
using videoapp.api.Services.Youtube;

namespace videoapp.api.Services
{
    public sealed class YoutubeResolver
    {
        private readonly string Lang;
        private readonly string ApiKey;
        private readonly ILogger<YoutubeResolver> Logger;
        private const string SearchEndpoint = "https://www.googleapis.com/youtube/v3/search";

        public YoutubeResolver
        (
            IConfiguration config,
            ILogger<YoutubeResolver> logger,
            string languageCode = null
        )
        {
            Logger = logger;
            ApiKey = config.GetSection("Youtube").GetValue<string>("ApiKey");
            Lang = languageCode ?? config.GetValue<string>("DefaultLanguage");

            if (Lang == "en") Lang = "US";
            Lang = Lang.ToUpper();
        }

        public async Task<IList<MovieTrailerQueryResult>> QueryYoutubeTrailers(IList<ImdbQueryResult> imdb_results)
        {
            var resolver_tasks = imdb_results.AsParallel().Select(async (imdb_movie) =>
            {
                return await QueryYoutubeTrailers(imdb_movie);
            });

            var resolver_results = await Task.WhenAll(resolver_tasks);
            return resolver_results.Where(r => r != null).ToList();
        }

        private async Task<MovieTrailerQueryResult> QueryYoutubeTrailers(ImdbQueryResult movie)
        {
            try
            {
                var search_url = GetSearchUrl($"{movie.Movie.title} {movie.Rating.year}");
                var youtube_results = await SimpleGetter.RetrieveAsync<YoutubeSearchResults>(search_url);
                if (youtube_results != null)
                    return new MovieTrailerQueryResult(movie, youtube_results);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"error finding youtube trailers for {movie.Movie.title}");
            }
            return null;
        }

        private string GetSearchUrl(string searchString)
        {
            var query = WebUtility.UrlEncode(string.Join(' ', searchString, "Trailer"));
            var query_strings = new Dictionary<string, string>()
            {
                { "q", query },
                { "part", "snippet" },
                { "key", ApiKey },
                { "order", "relevance" },
                { "regionCode", Lang },
                { "type", "video" },
                { "videoDuration", "short" },
                { "videoEmbeddable", "true" }
            };
            return QueryHelpers.AddQueryString(SearchEndpoint, query_strings);
        }
    }
}
