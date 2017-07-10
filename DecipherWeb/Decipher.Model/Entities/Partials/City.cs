﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Decipher.Model.Entities
{
    [MetadataType(typeof(CityJson))]
    public partial class City
    {
        public string Status
        {
            get
            {
                if (ZipsAssociated)
                {
                    return "Active";
                }
                else
                {
                    return "Inactive";
                }
            }
        }

        public bool Selected { get; set; }
    }

    public class CityJson
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Zip> Zips { get; set; }
    }
}