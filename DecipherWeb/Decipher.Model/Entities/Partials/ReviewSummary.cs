using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher.Model.Entities
{
    public class ReviewSummary
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; }
        public List<Descriptor> UserDescriptors { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
