using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher.Model.Entities
{
    public class APIResponse
    {
        public bool Success { get; set; }
        public object Result { get; set; }
    }
}
