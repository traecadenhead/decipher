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
    [MetadataType(typeof(TypeJson))]
    public partial class Type
    {
    }

    public class TypeJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<PlaceType> PlaceTypes { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ZipType> ZipTypes { get; set; }
    }
}
