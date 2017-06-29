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
    [MetadataType(typeof(PlaceTypeJson))]
    public partial class PlaceType
    {
    }

    public class PlaceTypeJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Place Place { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Type Type { get; set; }
    }
}
