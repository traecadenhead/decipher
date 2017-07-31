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

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Review> Reviews { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Language Language { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual City City { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<UserDevice> UserDevices { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }
}
