using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Decipher.Model.Entities
{
    [MetadataType(typeof(CityJson))]
    public partial class City
    {
        public string Status
        {
            get
            {
                if (ZipsAssociated)
                {
                    return "Active";
                }
                else
                {
                    return "Inactive";
                }
            }
        }

        public bool Selected { get; set; }

        public double RadiusInMeters
        {
            get
            {
                return Radius * 1609.34;
            }
        }

        public double UserDistance { get; set; }
    }

    public class CityJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Zip> Zips { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Place> Places { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<User> Users { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual QuestionSet QuestionSet { get; set; }
    }
}
