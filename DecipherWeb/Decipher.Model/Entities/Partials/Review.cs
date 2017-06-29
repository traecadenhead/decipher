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
    [MetadataType(typeof(ReviewJson))]
    public partial class Review
    {
        public List<ReviewResponse> Responses { get; set; }
        public User CurrentUser { get; set; }
        public Place CurrentPlace { get; set; }
        public List<Question> Questions { get; set; }
        public bool Report { get; set; }

        public string DateCreatedStr
        {
            get
            {
                return DateCreated.ToShortDateString();
            }
        }
    }

    public class ReviewJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Place Place { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ReviewResponse> ReviewResponses { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual User User { get; set; }
    }
}
