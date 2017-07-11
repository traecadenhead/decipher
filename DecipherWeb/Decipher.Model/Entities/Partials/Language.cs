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
    [MetadataType(typeof(LanguageJson))]
    public partial class Language
    {
        
    }

    public class LanguageJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Translation> Translations { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<User> Users { get; set; }
    }
}
