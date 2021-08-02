using System;

using videoapp.api.Services.Imdb;
using videoapp.api.Services.Youtube;

namespace videoapp.api
{
    public class MovieTrailerQueryResult
    {
        public DateTime retrievedAt { get; set; }
        public ImdbQueryResult imdbData { get; set; }
        public YoutubeSearchResults youtubeTrailersData { get; set; }

        public MovieTrailerQueryResult(ImdbQueryResult imdb_data, YoutubeSearchResults youtube_data)
        {
            imdbData = imdb_data;
            youtubeTrailersData = youtube_data;

            retrievedAt = DateTime.UtcNow;
        }
    }
}
