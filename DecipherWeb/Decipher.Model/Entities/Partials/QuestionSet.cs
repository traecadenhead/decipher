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
    [MetadataType(typeof(QuestionSetJson))]
    public partial class QuestionSet
    {
        
    }

    public class QuestionSetJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<City> Cities { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Question> Questions { get; set; }
    }
}
