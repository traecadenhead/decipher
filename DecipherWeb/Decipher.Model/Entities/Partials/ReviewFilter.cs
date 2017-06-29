using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher.Model.Entities
{
    public class ReviewFilter
    {
        public List<Descriptor> Descriptors { get; set; }
        public List<City> Cities { get; set; }
        public List<Zip> Zips { get; set; }
        public List<Place> Places { get; set; }
    }
}
