using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Device.Location;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region Translations

        public string TranslateString(string sourceString, string toLanguage, string fromLanguage = "en")
        {
            try
            {
                return GoogleTranslateString(sourceString, toLanguage, fromLanguage);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        #endregion
    }
}
