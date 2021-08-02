using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using videoapp.api.Services;

namespace videoapp.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieTrailersController : ControllerBase
    {
        private readonly MovieTrailerResolver Resolver;
        private readonly MovieTrailerQueryResultsCache Cache;

        public MovieTrailersController
        (
            MovieTrailerQueryResultsCache cache,
            MovieTrailerResolver resolver
        )
        {
            Resolver = resolver;
            Cache = cache;
        }

        // query movie trailers
        [HttpGet]
        public async Task<IEnumerable<MovieTrailerQueryResult>> Query
        (
            [FromQuery] string searchString,
            [FromQuery] int results = 10,
            [FromQuery] int page = 0
        )
        {
            if (Cache.AreResultsCached(searchString, results, page, out IEnumerable<MovieTrailerQueryResult> previousResults))
                return previousResults;

            var web_results = await Resolver.Resolve(searchString, results, page);
            Cache.Member(searchString, results, page, web_results);
            return web_results;
        }
    }
}
