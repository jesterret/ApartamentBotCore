using System;
using System.Collections.Generic;
using System.Text;

namespace ApartamentBotCore.WebModules.SearchProvider
{
    [DictionaryKey("OtoDom")]
    class OtoDomSearch : SearchBase
    {
        protected override string SearchUrl => "https://www.otodom.pl/wynajem/mieszkanie/krakow/?search%5Bfilter_float_price%3Ato%5D=2000&search%5Bdescription%5D=1&search%5Bdist%5D=0&search%5Bsubregion_id%5D=410&search%5Bcity_id%5D=38";

        protected override string ItemListXPath => "//header[@class='offer-item-header']/h3/a";

        protected override string NextPageXPath => "//a[@data-dir='next']";
    }
}
