using System;
using System.Threading.Tasks;
using CivicPlusSearch.Library;

namespace CivicPlusSearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var searchResults = await Search.get("[CIVIC_PLUS_SITE]", "[SEARCH_PHRASE]");
            Console.WriteLine(searchResults);
        }
    }
}
