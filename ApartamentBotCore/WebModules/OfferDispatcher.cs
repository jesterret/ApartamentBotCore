using ApartamentBotCore.WebModules.OfferData;
using ApartamentBotCore.WebModules.SearchProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartamentBotCore.WebModules
{
    class OfferDispatcher
    {
        public static Dictionary<string, Type> Offers { get; } = typeof(OfferBase).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(OfferBase)))
            .ToDictionary(t => (t.GetCustomAttributes(false).Where(a => a is DictionaryKeyAttribute).Single() as DictionaryKeyAttribute).Key);
        public static Dictionary<string, Type> Searches { get; } = typeof(SearchBase).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(SearchBase)))
            .ToDictionary(t => (t.GetCustomAttributes(false).Where(a => a is DictionaryKeyAttribute).Single() as DictionaryKeyAttribute).Key);

        public static OfferBase GetOffer(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var Url = new Uri(url);
            try
            {
                var x = Offers.Keys.SingleOrDefault(k => k.Contains(Url.Host));
                if (Offers.TryGetValue(Url.Host, out Type type))
                {
                    var offer = Activator.CreateInstance(type, new[] { url }) as OfferBase;
                    return offer;
                }
            }
            catch
            {
            }

            return null;
        }
        public static async Task<OfferBase> GetOfferLoaded(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var Url = new Uri(url);
            try
            {
                var x = Offers.Keys.SingleOrDefault(k => k.Contains(Url.Host));
                if (Offers.TryGetValue(Url.Host, out Type type))
                {
                    var offer = Activator.CreateInstance(type, new[] { url }) as OfferBase;
                    await offer.Load();
                    return offer;
                }
            }
            catch
            {
            }

            return null;
        }

        public static SearchBase GetSearch(string url)
        {
            try
            {
                if (Searches.TryGetValue(url, out Type type))
                {
                    return Activator.CreateInstance(type) as SearchBase;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
