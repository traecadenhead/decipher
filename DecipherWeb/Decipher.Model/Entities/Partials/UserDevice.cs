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
    [MetadataType(typeof(UserDeviceJson))]
    public partial class UserDevice
    {
        public List<Descriptor> Descriptors { get; set; }
    }

    public class UserDeviceJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual AppVersion AppVersion1 { get; set; }
        public virtual User User { get; set; }
    }
}
