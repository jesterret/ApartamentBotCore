using System;
using System.Collections.Generic;
using System.Text;

namespace ApartamentBotCore.WebModules.SearchProvider
{
    [DictionaryKey("OLX")]
    class OLXSearch : SearchBase
    {
        protected override string SearchUrl => "https://www.olx.pl/nieruchomosci/mieszkania/wynajem/krakow/?search%5Bfilter_float_price%3Ato%5D=2000&search%5Bfilter_enum_floor_select%5D%5B0%5D=floor_0&search%5Bfilter_enum_floor_select%5D%5B1%5D=floor_1&search%5Bfilter_enum_floor_select%5D%5B2%5D=floor_2&search%5Bfilter_enum_floor_select%5D%5B3%5D=floor_3&search%5Bfilter_enum_floor_select%5D%5B4%5D=floor_4&search%5Bfilter_enum_floor_select%5D%5B5%5D=floor_5&search%5Bfilter_enum_furniture%5D%5B0%5D=yes&search%5Bfilter_enum_builttype%5D%5B0%5D=loft&search%5Bfilter_enum_builttype%5D%5B1%5D=apartamentowiec&search%5Bfilter_enum_builttype%5D%5B2%5D=szeregowiec&search%5Bfilter_enum_builttype%5D%5B3%5D=wolnostojacy&search%5Bfilter_enum_builttype%5D%5B4%5D=blok&search%5Border%5D=created_at%3Adesc";

        protected override string ItemListXPath => "//a[contains(@class, 'link')]/strong/..";

        protected override string NextPageXPath => "//a[contains(@class, 'pageNextPrev')]";
    }
}
