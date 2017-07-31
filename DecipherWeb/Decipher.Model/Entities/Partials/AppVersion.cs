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
    [MetadataType(typeof(AppVersionJson))]
    public partial class AppVersion
    {
    }

    public class AppVersionJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<UserDevice> UserDevices { get; set; }
    }
}
