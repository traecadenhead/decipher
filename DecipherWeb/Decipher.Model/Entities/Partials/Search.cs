using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace Decipher.Model.Entities
{
    public class Search
    {
        public GeoCoordinate Location { get; set; }
        public int? Distance { get; set; }
        public string TypeID { get; set; }
        public double? Diversity { get; set; }
        public string Keyword { get; set; }
        public string Token { get; set; }
        public City City { get; set; }
        public string Term { get; set; }
        public User User { get; set; }
    }
}
