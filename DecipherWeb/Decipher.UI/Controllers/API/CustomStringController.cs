using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Decipher.Model.Entities;

namespace Decipher.UI.Controllers.API
{
    public class CustomStringController : BaseController
    {
        [HttpGet]
        public List<CustomString> List(string language = "en")
        {
            return db.GetCustomStrings(language);
        }
    }
}
