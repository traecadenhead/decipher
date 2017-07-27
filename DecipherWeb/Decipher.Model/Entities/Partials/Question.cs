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
    [MetadataType(typeof(QuestionJson))]
    public partial class Question
    {
        public List<Descriptor> Descriptors { get; set; }
        public string Detail { get; set; }
        public List<string> Details { get; set; }
    }

    public class QuestionJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ReviewResponse> ReviewResponses { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual QuestionSet QuestionSet { get; set; }
    }
}
