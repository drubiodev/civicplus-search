using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CivicPlusSearch.Library.models;
using HtmlAgilityPack;

namespace CivicPlusSearch.Library
{
    public static class Search
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly HtmlDocument doc = new HtmlDocument();
        public static async Task<CivicPlusSearchResponse> get(string domain, string searchPhrase, int pageNumber = 1)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();

            string url = $"{domain}/Search/Results?searchPhrase={searchPhrase}&pageNumber={pageNumber}&perPage=10&departmentId=-1";
            var request = await client.GetStringAsync(url);
            cancellationToken.Token.ThrowIfCancellationRequested();

            var getNumberOfResults = request.ParseResponse("//p[@class='cp-search-status']/text()");
            var data = request.ParseResponseList();
            return new CivicPlusSearchResponse
            {
                Data = data,
                SearchPhrase = searchPhrase,
                PageNumber = pageNumber,
                NumberOfResults = getNumberOfResults,
            };
        }

        private static string ParseResponse(this string response, string pattern)
        {
            doc.LoadHtml(response);

            return doc.DocumentNode.SelectSingleNode(pattern).InnerText;
        }

        private static List<SearchResult> ParseResponseList(this string response)
        {
            var results = new List<SearchResult>();
            doc.LoadHtml(response);
            var x = doc.DocumentNode.Descendants("div")
                    .Where(n => n.GetAttributeValue("class", "").Contains("cp-search-result "));
            // var x = doc.DocumentNode.SelectNodes(pattern);

            foreach (var item in x)
            {
                var title = item.Descendants("div")
                 .Where(n => n.GetAttributeValue("class", "").Contains("cp-search-resultTitle")).First()
                 .Descendants("h3").First()
                 .Descendants("a").First().InnerText;

                var link = item.Descendants("div")
                 .Where(n => n.GetAttributeValue("class", "").Contains("cp-search-resultURL")).First().InnerText;

                var datePosted = item.Descendants("div")
                 .Where(n => n.GetAttributeValue("class", "").Contains("cp-search-resultTitle")).First()
                 .Descendants("span").Where(d => d.GetAttributeValue("class", "").Contains("cp-search-resultDate"))
                 .First().Attributes["title"].Value;

                var description = item.Descendants("div")
                                 .Where(n => n.GetAttributeValue("class", "").Contains("cp-search-resultDesc")).First().InnerHtml;

                results.Add(
                    new SearchResult
                    {
                        Title = title,
                        Link = link,
                        DatePosted = DateTime.Parse(datePosted),
                        Description = description
                    });
            }

            return results;
        }
    }
}