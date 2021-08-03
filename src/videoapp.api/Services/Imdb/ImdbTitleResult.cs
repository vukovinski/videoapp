using System.Collections.Generic;

namespace videoapp.api.Services.Imdb
{
    public class ImdbTitleResult
    {
        public string id { get; set; }
        public string title { set; get; }
        public string originalTitle { get; set; }
        public string fullTitle { set; get; }
        public string year { set; get; }
        public string releaseDate { set; get; }
        public string runtimeMins { set; get; }
        public string runtimeStr { set; get; }
        public string plot { set; get; } // IMDb Plot allways en, TMDb Plot translate
        public string plotLocal { set; get; }
        public bool plotLocalIsRtl { set; get; }
        public string awards { set; get; }
        public string image { get; set; }
        public string type { set; get; }
        public string directors { set; get; }
        public List<StarShort> directorList { get; set; }
        public string writers { set; get; }
        public List<StarShort> writerList { get; set; }
        public string stars { set; get; }
        public List<StarShort> starList { get; set; }
        //public List<ActorShort> ActorList { get; set; }
        //public FullCastData FullCast { get; set; }
        public string genres { set; get; }
        //public List<KeyValueItem> GenreList { get; set; }
        public string companies { get; set; }
        public List<CompanyShort> companyList { get; set; }
        public string countries { set; get; }
        //public List<KeyValueItem> CountryList { set; get; }
        public string languages { set; get; }
        //public List<KeyValueItem> LanguageList { set; get; }
        public string contentRating { get; set; }
        public string imDbRating { get; set; }
        public string imDbRatingVotes { get; set; }
        public string metacriticRating { set; get; }
        //public RatingData Ratings { set; get; }
        //public WikipediaData Wikipedia { set; get; }
        //public PosterData Posters { get; set; }
        //public ImageData Images { get; set; }
        //public TrailerData Trailer { get; set; }
        public BoxOfficeShort boxOffice { get; set; }
        public string tagline { get; set; }
        public string keywords { get; set; }
        public List<string> keywordList { get; set; }
        public List<SimilarShort> similars { get; set; }
        public TvSeriesInfo tvSeriesInfo { get; set; }
        public TvEpisodeInfo tvEpisodeInfo { get; set; }
        public string errorMessage { get; set; }
    }

    public class PosterDataItem
    {
        public string id { get; set; }
        public string link { get; set; }
        public double aspectRatio { get; set; }
        public string language { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class TvSeriesInfo
    {
        public string yearEnd { set; get; }
        public string creators { set; get; }
        public List<StarShort> creatorList { get; set; }
        public List<string> seasons { get; set; }
    }

    public class TvEpisodeInfo
    {
        public string seriesId { get; set; }
        public string seriesTitle { get; set; }
        public string seriesFullTitle { get; set; }
        public string seriesYear { get; set; }
        public string seriesYearEnd { get; set; }

        public string seasonNumber { get; set; }
        public string episodeNumber { get; set; }

        public string previousEpisodeId { get; set; }
        public string nextEpisodeId { get; set; }
    }

    public class SimilarShort
    {
        public string id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
    }

    public class StarShort
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class BoxOfficeShort
    {
        public string budget { get; set; }
        public string openingWeekendUSA { get; set; }
        public string grossUSA { get; set; }
        public string cumulativeWorldwideGross { get; set; }
    }

    public class CompanyShort
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
