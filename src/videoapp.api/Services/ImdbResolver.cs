using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using videoapp.api.Services.Imdb;

namespace videoapp.api.Services
{
    public sealed class ImdbResolver
    {
        private readonly string Lang;
        private readonly string ApiKey;
        private readonly ILogger<ImdbResolver> Logger;
        private const string RatingsEndpoint = "Ratings";
        private const string SearchMoviesEndpoint = "SearchMovie";
        private readonly string BaseUrlTemplate = "https://imdb-api.com/{lang_code}/API/{endpoint}/{api_key}";

        public ImdbResolver
        (
            IConfiguration config, ILogger<ImdbResolver> logger, string languageCode = null
        )
        {
            Logger = logger;
            ApiKey = config.GetSection("Imdb").GetValue<string>("ApiKey");
            Lang = languageCode ?? config.GetValue<string>("DefaultLanguage");
        }

        private string GetBaseUrl(string endpoint)
        {
            return BaseUrlTemplate.Replace("{lang_code}", Lang).Replace("{endpoint}", endpoint).Replace("{api_key}", ApiKey);
        }

        public async Task<IList<ImdbQueryResult>> QueryImdb(string searchString, int results, int page)
        {
            try
            {
                var movies_query = WebUtility.UrlEncode(searchString).Replace("+", "%20");
                var movies_query_url = string.Join('/', GetBaseUrl(SearchMoviesEndpoint), movies_query);

                var movies = await QueryImdbMovies(movies_query_url);
                if (movies != null && movies.errorMessage == "" && movies.results.Count > 0)
                {
                    var paged_movies = movies.results.Skip(page * results).Take(results); // add state for total results count
                    var details_tasks = paged_movies.AsParallel().Select(async (movie) =>
                    {
                        var rating_query = WebUtility.UrlEncode(movie.id);
                        var rating_query_url = string.Join('/', GetBaseUrl(RatingsEndpoint), rating_query);

                        var rating = await QueryImdbRating(rating_query_url);

                        return new ImdbQueryResult(movie, rating);
                    })
                    .ToArray();

                    var details_results = await Task.WhenAll(details_tasks);
                    return details_results.Where(r => r != null).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "error while quering imdb api!");
            }
            return new List<ImdbQueryResult>();
        }

        private async Task<ImdbRatingsResult> QueryImdbRating(string url)
        {
            return await SimpleGetter.RetrieveAsync<ImdbRatingsResult>(url, Logger);
        }

        private async Task<ImdbSearchMoviesResult> QueryImdbMovies(string url)
        {
            return await SimpleGetter.RetrieveAsync<ImdbSearchMoviesResult>(url, Logger);
        }
    }
}
