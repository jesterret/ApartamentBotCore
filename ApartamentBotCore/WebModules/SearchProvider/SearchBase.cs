using ApartamentBotCore.WebModules.OfferData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartamentBotCore.WebModules.SearchProvider
{
    abstract class SearchBase
    {
        protected HtmlAgilityPack.HtmlDocument Page = new HtmlAgilityPack.HtmlDocument();

        protected abstract string SearchUrl { get; }

        protected abstract string ItemListXPath { get; }

        protected abstract string NextPageXPath { get; }

        private int NextDepthCount = 0;
        const int MaxDepthCount = 10;

        protected virtual IEnumerable<string> GetItemUrls()
        {
            return Page.DocumentNode.SelectNodes(ItemListXPath)
                .Select(x => x.GetAttributeValue("href", string.Empty));
        }

        protected virtual string GetNextPageUrl()
        {
            return Page.DocumentNode.SelectSingleNode(NextPageXPath)
                    ?.GetAttributeValue("href", string.Empty);
        }

        protected HtmlAgilityPack.HtmlNode GetNextPage()
        {
            if (NextDepthCount < MaxDepthCount)
            {
                var url = GetNextPageUrl();
                if (url != null)
                {
                    NextDepthCount++;
                    Page.LoadHtml(Program.GetRawString(url));
                    return Page.DocumentNode;
                }
            }
            return null;
        }
        
        public IEnumerable<OfferBase> Get()
        {
            Page.LoadHtml(Program.GetRawString(SearchUrl));
            var doc = Page.DocumentNode;
            while (doc != null)
            {
                var urls = GetItemUrls();
                foreach (var url in urls)
                {
                    if (OfferDispatcher.GetOffer(url) is OfferBase offer)
                        yield return offer;
                }

                doc = GetNextPage();
            }
            Page = null;
        }
        public IEnumerable<OfferBase> GetLoaded()
        {
            Page.LoadHtml(Program.GetRawString(SearchUrl));
            var doc = Page.DocumentNode;
            while (doc != null)
            {
                var urls = GetItemUrls();
                foreach (var url in urls)
                {
                    if (OfferDispatcher.GetOfferLoaded(url).Result is OfferBase offer)
                        yield return offer;
                }

                doc = GetNextPage();
            }
            Page = null;
        }
    }
}
