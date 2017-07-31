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
    [MetadataType(typeof(UserNotificationJson))]
    public partial class UserNotification
    {
    }

    public class UserNotificationJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Notification Notification { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual User User { get; set; }
    }
}
