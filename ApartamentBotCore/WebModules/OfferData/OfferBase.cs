using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApartamentBotCore.WebModules.OfferData
{
    abstract partial class OfferBase
    {
        string _url;
        protected bool IsLoaded { get; private set; }
        protected Task IsLoading { get; private set; }
        protected HtmlAgilityPack.HtmlDocument _document { get; set; } = new HtmlAgilityPack.HtmlDocument();

        public OfferBase(string url)
        {
            _url = url;
        }
        public Uri Url => new Uri(_url);
        public abstract string Title { get; }
        public abstract string Text { get; }
        public abstract string MapUrl { get; }
        public abstract int Price { get; }
        public abstract float Space { get; }
        public abstract int RoomCount { get; }
        public abstract int Floor { get; }
        public abstract OfferType Offer { get; }
        public abstract MapLocation Location { get; }
        public virtual async Task Load()
        {
            if (!IsLoaded)
                await LoadAsync();
        }
        public virtual Task LoadAsync() => IsLoading ?? (IsLoading = Program.GetRawStringAsync(Url.ToString())
                                                        .ContinueWith(t =>
                                                        {
                                                            IsLoaded = true;
                                                            _document.LoadHtml(t.GetAwaiter().GetResult());
                                                        }));
        public string GetHtml()
        {
            string html = string.Empty;
            var mapurl = MapUrl;
            if (!string.IsNullOrEmpty(mapurl))
            {
                html += $"<a href=\"{mapurl}\">Map</a>\n";
            }
            return html + $"<b>{Title}</b>\nPrice: {Price}\nSpace: {Space}\nOffer: {Offer}\n\n<pre>{Text}</pre>";
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }
        public override bool Equals(object other)
        {
            if (other is OfferBase x)
                return Url == x.Url;
            return false;
        }
    }
}
