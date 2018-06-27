using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApartamentBotCore.WebModules.SearchProvider
{
    [DictionaryKey("GumTree")]
    class GumTreeSearch : SearchBase
    {
        protected override string SearchUrl => "https://www.gumtree.pl/s-mieszkania-i-domy-do-wynajecia/krakow/v1c9008l3200208p1?pr=,2000";

        protected override string ItemListXPath => "//a[@class='href-link']";

        protected override string NextPageXPath => "//a[contains(@class,'next')]";

        protected override IEnumerable<string> GetItemUrls() => base.GetItemUrls().Select(url => "https://www.gumtree.pl" + url);

        protected override string GetNextPageUrl() => "https://www.gumtree.pl" + base.GetNextPageUrl();
    }
}
