using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;

namespace Decipher.Model.Entities
{
    [MetadataType(typeof(PlaceJson))]
    public partial class Place
    {
        public Double? DistanceInMeters { get; set; }

        public Double? Distance
        {
            get
            {
                if (DistanceInMeters.HasValue)
                {
                    return DistanceInMeters.Value * 0.000621371;
                }
                return null;
            }
        }

        public Double? DistanceInFeet
        {
            get
            {
                if(DistanceInMeters.HasValue)
                {
                    return DistanceInMeters.Value * 3.28084;
                }
                return null;
            }
        }

        public string DistanceStr
        {
            get
            {
                if (Distance.HasValue)
                {
                    if (DistanceInMeters.Value <= 500)
                    {
                        return DistanceInFeet.Value.ToString("0") + " feet";
                    }
                    else
                    {
                        return Distance.Value.ToString("0.0") + " miles";
                    }
                }
                else
                {
                    return "NA";
                }
            }
        }

        public Zip DefaultZip { get; set; }

        public List<Type> Types { get; set; }

        public string TypeStr
        {
            get
            {
                string str = "";
                if(Types != null && Types.Count > 0)
                {
                    foreach (var type in Types)
                    {
                        if (str.Length > 0)
                        {
                            str += ", ";
                        }
                        str += type.Name;
                    }
                }
                return str;
            }
        }

        public GeoCoordinate Location
        {
            get
            {
                return new GeoCoordinate(Latitude, Longitude);
            }
        }

        public List<Question> Questions { get; set; }

        public List<string> TypesList { get; set; }

        public City City { get; set; }
        
        public bool HasReviews { get; set; }

        public List<Descriptor> Descriptors { get; set; }

        public bool Selected { get; set; }

        public double? AvgScore { get; set; }

        public string AvgScoreStr
        {
            get
            {
                if (AvgScore.HasValue)
                {
                    return AvgScore.Value.ToString("0.00");
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public string Description
        {
            get
            {
                return "X% Positive Experience";
            }
        }
    }

    public class PlaceJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Zip Zip1 { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<PlaceType> PlaceTypes { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
