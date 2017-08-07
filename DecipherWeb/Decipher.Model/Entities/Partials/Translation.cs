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
    [MetadataType(typeof(TranslationJson))]
    public partial class Translation
    {
        public string TranslationBeginID { get; set; }
        public bool TinyMCE { get; set; }
    }

    public class TranslationJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Language Language { get; set; }
    }
}
