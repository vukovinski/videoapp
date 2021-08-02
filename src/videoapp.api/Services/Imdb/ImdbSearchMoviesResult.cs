﻿using System.Collections.Generic;

namespace videoapp.api.Services.Imdb
{
    public class ImdbSearchMoviesResult
    {
        public string searchType { get; set; }
        public string expression { get; set; }

        public List<ImdbSearchMoviesItem> results { get; set; }

        public string errorMessage { get; set; }
    }

    public class ImdbSearchMoviesItem
    {
        public string id { get; set; }
        public string resultType { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }

    public enum SearchType
    {
        Movie = 2
    }
}
