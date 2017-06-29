using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher.Model.Entities
{
    public class ListHolder
    {
        public List<ListItem> Distance { get; set; }
        public List<ListItem> Types { get; set; }
        public List<ListItem> Diversity { get; set; }
    }
}
