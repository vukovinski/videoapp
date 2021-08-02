namespace videoapp.api.Services.Imdb
{
    public class ImdbQueryResult
    {
        public ImdbQueryResult(ImdbSearchMoviesItem movie, ImdbRatingsResult rating)
        {
            Movie = movie;
            Rating = rating;
        }

        public ImdbSearchMoviesItem Movie { get; init; }
        public ImdbRatingsResult Rating { get; init; }
    }
}
