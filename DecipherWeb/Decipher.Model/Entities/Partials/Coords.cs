using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.Device.Location;

namespace Decipher.Model.Entities
{
    public class Coords
    {
        public string Language { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoCoordinate Coordinates
        {
            get
            {
                return new GeoCoordinate { Latitude = Latitude, Longitude = Longitude };
            }
        }
    }
}
