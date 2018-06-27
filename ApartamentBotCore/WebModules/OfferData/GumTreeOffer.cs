using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartamentBotCore.WebModules.OfferData
{
    [DictionaryKey("www.gumtree.pl")]
    class GumTreeOffer : OfferBase
    {
        public GumTreeOffer(string url) : base(url)
        {
        }
        public override async Task Load()
        {
            await base.Load();
            var doc = _document.DocumentNode;
            _title = doc.SelectSingleNode("//span[@class='myAdTitle']").InnerText;
            _text = doc.SelectSingleNode("//span[@class='pre']").InnerText;
            var pricetext = doc.SelectSingleNode("//span[@class='amount']").InnerText;
            int.TryParse(Utils.GetNumbers(pricetext), out _price);

            var kv = doc.SelectNodes("//div[@class='attribute']")
                ?.ToDictionary(a => a.Elements("span").First().InnerText, a => a.Elements("span").Last().InnerText);

            if (kv.TryGetValue("Wielkość (m2)", out var space))
                float.TryParse(Utils.GetNumbers(space), out _space);

            if (kv.TryGetValue("Do wynajęcia przez", out var type))
                _type = type == "Agencja" ? OfferType.Bussines : OfferType.Private;

            if (kv.TryGetValue("Liczba pokoi", out var roomCount))
                int.TryParse(Utils.GetNumbers(roomCount), out _roomCount);

            var mapnode = doc.SelectSingleNode("//span[@class='google-maps-link']/img");
            if (mapnode != null)
            {
                var x = mapnode.GetAttributeValue("src", null);
                if (x != null)
                {
                    _maploc = x.Replace("300x300", "640x640");
                }
            }
            _floor = 0;
            _document = null;
        }

        public override string Title => _title;
        public override string Text => _text;
        public override string MapUrl => _maploc;
        public override int Price => _price;
        public override float Space => _space;
        public override int RoomCount => _roomCount;
        public override int Floor => _floor;
        public override OfferType Offer => _type;
        public override MapLocation Location => throw new NotImplementedException();

        string _title;
        string _text;
        string _maploc;
        int _price;
        float _space;
        int _roomCount;
        int _floor;
        OfferType _type;
    }
}
