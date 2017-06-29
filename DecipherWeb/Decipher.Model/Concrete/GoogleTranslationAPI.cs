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
        #region Google Translation API

        private string GoogleTranslationAPIRequest(Dictionary<string, string> parameters = null)
        {
            try
            {
                string str = "key=" + GoogleAPIKey;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (string key in parameters.Keys)
                    {
                        str += "&" + key + "=" + parameters[key];
                    }
                }
                string url = "https://translation.googleapis.com/language/translate/v2?" + str;
                return GetContentFromURL(url);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public string GoogleTranslateString(string sourceString, string toLanguage, string fromLanguage = "en")
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("q", sourceString);
                dict.Add("target", toLanguage);
                dict.Add("source", fromLanguage);
                string data = GoogleTranslationAPIRequest(dict);
                JObject json = JObject.Parse(data);
                JToken jData = json["data"];
                List<string> translations = new List<string>();
                foreach(var jItem in (JArray)jData["translations"])
                {
                    translations.Add(SafeJsonString(jItem, "translatedText"));
                }
                return translations.FirstOrDefault();
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        #endregion
    }
}
