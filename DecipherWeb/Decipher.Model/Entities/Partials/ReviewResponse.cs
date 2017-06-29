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
    [MetadataType(typeof(ReviewResponseJson))]
    public partial class ReviewResponse
    {
        public Descriptor CurrentDescriptor { get; set; }
    }

    public class ReviewResponseJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Descriptor Descriptor { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Question Question { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Review Review { get; set; }
    }
}
