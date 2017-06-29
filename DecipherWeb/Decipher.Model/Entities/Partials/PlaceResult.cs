using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher.Model.Entities
{
    public class PlaceResult
    {
        public List<Place> Results { get; set; }
        public string NextToken { get; set; }
        public int NumResults
        {
            get
            {
                if(Results != null)
                {
                    return Results.Count;
                }
                return 0;
            }
        }

        public string Response { get; set; }

        public Search Search { get; set; }

        public double? MaxDistance
        {
            get
            {
                if(Results != null && Results.Count > 0)
                {
                    return Results.OrderByDescending(n => n.Distance).Select(n => n.Distance).FirstOrDefault();
                }
                return null;
            }
        }

        public int Zoom
        {
            get
            {
                if (MaxDistance.HasValue)
                {
                    if (MaxDistance.Value >= 10)
                    {
                        return 9;
                    }
                    else if(MaxDistance.Value >= 5)
                    {
                        return 10;
                    }
                    else if(MaxDistance.Value >= 3)
                    {
                        return 12;
                    }
                    else
                    {
                        return 13;
                    }
                }
                else
                {
                    return 13;
                }       
            }
        }
    }
}
