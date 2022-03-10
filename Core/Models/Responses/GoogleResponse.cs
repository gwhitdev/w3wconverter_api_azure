using System;
using System.Collections.Generic;
namespace Core.Models.Responses
{
    public class GoogleResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Northeast
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Southwest
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Viewport
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
            public Viewport viewport { get; set; }
        }

        public class Candidate
        {
            public Geometry geometry { get; set; }
        }

        public class Root
        {
            public List<Candidate> candidates { get; set; }
            public string status { get; set; }
        }


    }
}
