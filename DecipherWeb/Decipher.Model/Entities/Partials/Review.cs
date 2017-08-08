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

        public string IdentifierList
        {
            get
            {
                string str = "";
                if(CurrentUser != null && CurrentUser.Descriptors != null)
                {
                    foreach(var desc in CurrentUser.Descriptors)
                    {
                        if (!String.IsNullOrEmpty(str))
                        {
                            str += ", ";
                        }
                        str += desc.Name;
                    }
                }
                return str;
            }
        }

        public City City{ get; set; }

        public string Language { get; set; }
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
