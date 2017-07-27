using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;

namespace Decipher.Model.Entities
{
    [MetadataType(typeof(ZipJson))]
    public partial class Zip
    {
        public string DiversityStr
        {
            get
            {
                return (DiversityIndex * 100).ToString("0") + "%";
            }
        }

        public GeoCoordinate Coordinates
        {
            get
            {
                if(Latitude.HasValue && Longitude.HasValue)
                {
                    return new GeoCoordinate(Latitude.Value, Longitude.Value);
                }
                return null;
            }
        }

        public bool Selected { get; set; }        
    }

    public class ZipJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ZipDemographic> ZipDemographics { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual City City { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ZipType> ZipTypes { get; set; }
    }
}
