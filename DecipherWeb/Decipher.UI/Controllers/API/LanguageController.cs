﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Decipher.Model.Entities;

namespace Decipher.UI.Controllers.API
{
    public class LanguageController : BaseController
    {
        [HttpGet]
        public List<Language> List()
        {
            return db.Languages.OrderBy(n => n.Name).ToList();
        }
    }
}
