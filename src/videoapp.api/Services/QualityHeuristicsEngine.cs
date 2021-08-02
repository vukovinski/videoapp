using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace videoapp.api.Services
{
    public static class QualityHeuristicsEngine
    {
        public static IList<MovieTrailerQueryResult> Filter(IList<MovieTrailerQueryResult> results)
        {
            // for each movie
            //     - at least one trailer must be found
            //     - imdb data should not contain error message
            // for each movie trailer:
            //     - exact movie name must appear in video title
            //     - movie trailer should be published in the year the movie releases or in the previous 2 years

            var movie_results_to_remove = new List<int>();
            var trailer_results_to_remove = new List<int>();
            for (int i = 0; i < results.Count; i++)
            {
                var movie = results[i];
                var trailers = movie.youtubeTrailersData.items;
                for (int t = 0; t < trailers.Count; t++)
                {
                    if (movie.imdbData.Rating.title != null && movie.imdbData.Rating.year != null)
                    {
                        var trailer = trailers[t];

                        if (!trailer.snippet.title.ToLower().Contains(movie.imdbData.Rating.title.ToLower()))
                        {
                            trailer_results_to_remove.Add(t);
                            continue;
                        }

                        if (trailer.snippet.publishedAt.AddYears(-2).Year > int.Parse(movie.imdbData.Rating.year))
                        {
                            trailer_results_to_remove.Add(t);
                            continue;
                        }
                    }
                }

                movie.youtubeTrailersData.items = movie.youtubeTrailersData.items.Select((item, index) =>
                {
                    return trailer_results_to_remove.Contains(index) ? null : item;
                })
                .Where(item => item != null)
                .ToList();

                trailer_results_to_remove.Clear();

                if (movie.youtubeTrailersData.items.Count == 0)
                {
                    movie_results_to_remove.Add(i);
                }

                if (movie.imdbData.Rating.errorMessage != "")
                {
                    movie_results_to_remove.Add(i);
                }
            }

            return results.Select((item, index) =>
            {
                return movie_results_to_remove.Contains(index) ? null : item;
            })
            .Where(item => item != null)
            .ToList();
        }
    }
}
