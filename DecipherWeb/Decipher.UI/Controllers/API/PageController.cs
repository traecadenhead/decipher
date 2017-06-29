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
        public List<Page> List()
        {
            return db.Pages.OrderBy(n => n.Ordinal).ToList();
        }

        [HttpGet]
        public Page Get(int? id)
        {
            if (id.HasValue)
            {
                return db.Pages.Where(n => n.PageID == id.Value).FirstOrDefault();
            }
            return null;
        }
    }
}
