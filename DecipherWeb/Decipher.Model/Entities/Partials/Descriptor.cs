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
    [MetadataType(typeof(DescriptorJson))]
    public partial class Descriptor
    {
        public bool Selected { get; set; }

        public string SelectedAttr
        {
            get
            {
                if (Selected)
                {
                    return "selected";
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public int NumSelected { get; set; }

        public int TotalSelected { get; set; }

        public double NumPct
        {
            get
            {
                try
                {
                    var pct = (Convert.ToDouble(NumSelected) / Convert.ToDouble(TotalSelected)) * 100;
                    if(pct < 35)
                    {
                        pct = 35;
                    }
                    return pct;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int PointsSafe
        {
            get
            {
                if (Points.HasValue)
                {
                    return Points.Value;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public class DescriptorJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<ReviewResponse> ReviewResponses { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<UserDescriptor> UserDescriptors { get; set; }
    }
}
