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
    [MetadataType(typeof(UserJson))]
    public partial class User
    {
        public List<Descriptor> Descriptors { get; set; }
    }

    public class UserJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<UserDescriptor> UserDescriptors { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
