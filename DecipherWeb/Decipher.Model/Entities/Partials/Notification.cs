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
    [MetadataType(typeof(NotificationJson))]
    public partial class Notification
    {
    }

    public class NotificationJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<NotificationTarget> NotificationTargets { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }
}
