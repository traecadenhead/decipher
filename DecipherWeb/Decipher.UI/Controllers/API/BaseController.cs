using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Configuration;
using Decipher.Model.Abstract;
using Decipher.Model.Concrete;

namespace Decipher.UI.Controllers.API
{
    public class BaseController : ApiController
    {
        public IDataRepository db;

        public BaseController()
        {
            db = new Repository();
        }

        public string GetConfig(string key, string defaultValue = "")
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
            {
                return ConfigurationManager.AppSettings[key];
            }
            else
            {
                return defaultValue;
            }
        }

        public int GetConfigInt(string key, int defaultValue = 0)
        {
            try
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings[key]);
                }
                else
                {
                    return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
