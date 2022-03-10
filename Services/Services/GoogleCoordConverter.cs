using System;
using Core.Models.Objects;

namespace Services.Services
{
    public class GoogleCoordConverter
    {
        public Coords Coords { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public GoogleCoordConverter(string lat, string lng)
        {
            Lat = lat;
            Lng = lng;
            Coords = new();
        }

        public Coords Convert()
        {
            Coords.Lat = Lat;
            Coords.Lng = Lng;
            return Coords;
        }
    }
}
