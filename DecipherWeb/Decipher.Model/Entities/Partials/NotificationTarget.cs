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
    [MetadataType(typeof(NotificationTargetJson))]
    public partial class NotificationTarget
    {
    }

    public class NotificationTargetJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Notification Notification { get; set; }
    }
}
