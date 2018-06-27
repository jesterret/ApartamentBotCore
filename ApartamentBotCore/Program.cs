using ApartamentBotCore.WebModules;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApartamentBotCore
{
    class Program
    {
        public static MapLocation.Coords TargetLocation = new MapLocation.Coords
        {
            Latitude = "50.0463395",
            Longtitude = "19.9542531"
        };
        public const string TelegramApiKey = "";
        public static ManualResetEvent ShouldQuit = new ManualResetEvent(false);
        public static long PrivateChatID = 0;

        static public string GetRawString(string Url) => GetRawStringAsync(Url).ConfigureAwait(false).GetAwaiter().GetResult();
        static public Task<string> GetRawStringAsync(string Url)
        {
            var cl = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            cl.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36");
            return cl.DownloadStringTaskAsync(Url);
        }
        static public string GetDirectory() => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static void Main(string[] args)
        {
            using (var telegram = new TelegramManager())
            {
                ShouldQuit.WaitOne();
                Thread.Sleep(5000);
            }
        }
    }
}
