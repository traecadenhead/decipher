using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Decipher.Model.Abstract;
using Decipher.Model.Entities;
using System.Web;
using System.Net;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Data;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        private ModelStateDictionary ModelState;
        private Decipher.Model.Entities.DecipherEntities db;

        public Repository(ModelStateDictionary modelState)
        {
            ModelState = modelState;
            db = new DecipherEntities();
        }

        public Repository()
        {
            ModelState = new ModelStateDictionary();
            db = new DecipherEntities();
        }
    }
}