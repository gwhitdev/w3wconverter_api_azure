using System;
using System.Collections.Generic;

namespace Core.Models.Objects
{
    public class Output
    {
        public List<string> Postcodes { get; set; }
        public List<string> Uids { get; set; }
        public List<Coord> Coords { get; set; }
        public List<W3WAddress> W3WAddress { get; set; }

        
        public class Coord
        {
            public string Lat { get; set; }
            public string Lng { get; set; }

        }
    }

    
}
