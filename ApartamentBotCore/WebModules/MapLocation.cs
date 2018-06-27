using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ApartamentBotCore.WebModules
{
    class MapLocation
    {
        private const string APIUrl = "https://maps.googleapis.com/maps/api/distancematrix/xml?origins={from}&destinations={to}&key=AIzaSyDXfD53QgdwsXQD4SuUjlKTg18xFHy_Src";

        public class Coords
        {
            public string Latitude { get; set; }
            public string Longtitude { get; set; }

            public override string ToString()
            {
                return string.Format("{0},{1}", Latitude, Longtitude);
            }

        }
        public class DistanceInfo
        {
            public string Distance { get; set; }
            public string Duration { get; set; }
        }

        public Coords From { get; set; }
        public Coords To { get; set; }
        public int Radius { get; set; }
        public bool IsExact => Radius == 0;

        public DistanceInfo GetData()
        {
            var stream = Program.GetRawString(APIUrl.Replace("{from}", From.ToString()).Replace("{to}", To.ToString()));
            var doc = XDocument.Parse(stream);
            return new DistanceInfo()
            {
                Duration = (string)doc.Root
                                      .Element("row")
                                      .Element("element")
                                      .Element("duration")
                                      .Element("text"),
                Distance = (string)doc.Root
                                      .Element("row")
                                      .Element("element")
                                      .Element("distance")
                                      .Element("text")
            };
        }
    }
}
