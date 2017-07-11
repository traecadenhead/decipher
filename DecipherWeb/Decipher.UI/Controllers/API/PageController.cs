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
    public class PageController : BaseController
    {
        [HttpGet]
        public List<Page> List(string language = "en")
        {
            return db.GetPages(language);
        }

        [HttpGet]
        public Page Get(int? id, string language = "en")
        {
            if (id.HasValue)
            {
                return db.GetPage(id.Value, language);
            }
            return null;
        }
    }
}
