using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartamentBotCore.WebModules.OfferData
{
    [DictionaryKey("www.olx.pl")]
    class OLXOffer : OfferBase
    {
        public OLXOffer(string url) : base(url)
        {
        }

        public override async Task Load()
        {
            await base.Load();
            var doc = _document.DocumentNode;
            _title = doc.SelectSingleNode("//div[@class='offer-titlebox']/h1").InnerText.Trim();
            _text = doc.SelectSingleNode("//div[@id='textContent']/p")?.InnerText.Trim();
            var pricedata = Utils.GetNumbers(doc.SelectSingleNode("//div[@class='price-label']/strong")?.InnerText);
            if (pricedata != null)
                int.TryParse(pricedata, out _price);

            var details = doc.SelectNodes("//table[@class='item']");
            var dict = details
                ?.ToDictionary(x => x.SelectSingleNode(".//th").InnerText, x => x.SelectSingleNode(".//strong").InnerText);

            if (dict != null)
            {
                if (dict.ContainsKey("Powierzchnia"))
                {
                    float.TryParse(Utils.GetNumbers(dict["Powierzchnia"]).Replace(",", "."), out _space);
                }

                if (dict.ContainsKey("Poziom"))
                    int.TryParse(Utils.GetNumbers(dict["Poziom"]), out _floor);

                if (dict.ContainsKey("Liczba pokoi"))
                    int.TryParse(Utils.GetNumbers(dict["Liczba pokoi"]), out _roomCount);

                if (dict.ContainsKey("Oferta od"))
                    _type = dict["Oferta od"].Contains("Osoby prywatnej") ? OfferType.Private : OfferType.Bussines;
            }
            var mapnode = doc.SelectSingleNode("//div[@id='mapcontainer']");
            if (mapnode != null)
            {
                _loc = new MapLocation()
                {
                    From = new MapLocation.Coords
                    {
                        Latitude = mapnode.GetAttributeValue("data-lat", string.Empty),
                        Longtitude = mapnode.GetAttributeValue("data-lon", string.Empty)
                    },
                    To = Program.TargetLocation,
                    Radius = mapnode.GetAttributeValue("data-rad", 999999)
                };
            }
            _document = null;
        }

        public override string Title => _title;
        public override string Text => _text;
        public override string MapUrl
        {
            get
            {
                if (_loc != null)
                    return $"https://maps.googleapis.com/maps/api/staticmap?center={_loc.To}&zoom=13&size=640x640&markers=color:red|{_loc.From}";

                return null;
            }
        }

            
        public override int Price => _price;
        public override float Space => _space;
        public override int RoomCount => _roomCount;
        public override int Floor => _floor;
        public override OfferType Offer => _type;
        public override MapLocation Location => _loc;

        string _title;
        string _text;
        int _price;
        float _space;
        int _roomCount;
        int _floor;
        OfferType _type;
        MapLocation _loc;
    }
}
