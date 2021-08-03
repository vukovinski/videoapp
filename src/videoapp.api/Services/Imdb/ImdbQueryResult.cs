namespace videoapp.api.Services.Imdb
{
    public class ImdbQueryResult
    {
        public ImdbQueryResult(ImdbSearchMoviesItem movie, ImdbRatingsResult rating, ImdbTitleResult title)
        {
            Movie = movie;
            Title = title;
            Rating = rating;
        }

        public ImdbSearchMoviesItem Movie { get; init; }
        public ImdbRatingsResult Rating { get; init; }
        public ImdbTitleResult Title { get; init; }

        public bool IsValid => Movie != null && Rating != null && Title != null;
    }
}
