using System;
using System.Collections.Generic;

namespace CivicPlusSearch.Library.models
{
    public class CivicPlusSearchResponse
    {
        private string _numberOfResults;
        public string SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public double TotalNumberOfPages { get; set; }
        public string NumberOfResults { get { return _numberOfResults; } set { _numberOfResults = GetNumberOfResults(value); } }
        public List<SearchResult> Data { get; set; }

        public string GetNumberOfResults(string parseString)
        {
            var start = parseString.IndexOf("of ") + "of ".Length;
            var end = parseString.IndexOf(" results ");
            var numResults = parseString.Substring(start, end - start);

            TotalNumberOfPages = Math.Ceiling(Double.Parse(numResults) / 10);

            return numResults;
        }
    }


}