using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher.Model.Entities
{
    public class ReviewContainer
    {
        public Review Review { get; set; }
        public ICollection<ReviewResponse> Responses { get; set; }
        public User User { get; set; }
        public ICollection<UserDescriptor> UserDescriptors { get; set; }
    }
}
