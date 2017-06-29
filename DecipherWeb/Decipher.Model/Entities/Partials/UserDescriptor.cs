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
    [MetadataType(typeof(UserDescriptorJson))]
    public partial class UserDescriptor
    {
    }

    public class UserDescriptorJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Descriptor Descriptor { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual User User { get; set; }
    }
}
