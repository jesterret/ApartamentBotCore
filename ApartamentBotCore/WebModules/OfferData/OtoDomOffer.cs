using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApartamentBotCore.WebModules.OfferData
{
    [DictionaryKey("www.otodom.pl")]
    class OtoDomOffer : OfferBase
    {
        public OtoDomOffer(string url) : base(url)
        {
        }
        private string GetParam(HtmlAgilityPack.HtmlNode doc, string param)
        {
            return doc.SelectSingleNode($"//li[@class='{param}']/span/strong")?.InnerText;
        }
        public override async Task Load()
        {
            await base.Load();
            var doc = _document.DocumentNode;
            _title = doc.SelectSingleNode("//h1[@itemprop='name']").InnerText;
            _text = doc.SelectSingleNode("//div[@itemprop='description']").InnerText;
            var offertype = doc.SelectSingleNode("//h6[@class='box-contact-info-type']");
            _type = offertype != null && offertype.InnerText.Contains("prywatna") ? OfferType.Private : OfferType.Bussines;

            int.TryParse(Utils.GetNumbers(GetParam(doc, "param_price")), out _price);
            float.TryParse(Utils.GetNumbers(GetParam(doc, "param_m")), out _space);

            var roomcountnode = doc.SelectSingleNode("//div[@class='room-lane']/span[@class='big']");
            if (roomcountnode != null)
                int.TryParse(roomcountnode.InnerText, out _roomCount);

            // Not required to specify
            int.TryParse(GetParam(doc, "param_floor_no") ?? string.Empty, out _floor);

            var mapnode = doc.SelectSingleNode("//div[@id='adDetailInlineMap']");
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
                    Radius = mapnode.GetAttributeValue("data-radius", 0xffffff)
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
