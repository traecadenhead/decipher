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
using System.Security.Cryptography;
using System.Device.Location;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Decipher.Model.Concrete
{
    #region Helpers
    public partial class Repository : IDataRepository
    {
        public string GetConfig(string name)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[name]))
            {
                return ConfigurationManager.AppSettings[name];
            }
            else
            {
                return "";
            }
        }

        public string GetConfig(string name, string defaultValue)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[name]))
            {
                return ConfigurationManager.AppSettings[name];
            }
            else
            {
                return defaultValue;
            }
        }

        public int GetConfigInt(string name)
        {
            try
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings[name]);
            }
            catch
            {
                return 0;
            }
        }

        public int GetConfigInt(string name, int defaultValue)
        {
            try
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[name]))
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings[name]);
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

        public bool GetConfigBool(string key, bool defaultValue = false)
        {
            try
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                {
                    return Convert.ToBoolean(ConfigurationManager.AppSettings[key]);
                }
            }
            catch { }
            return defaultValue;
        }

        private string SafeXMLString(XElement el)
        {
            try
            {
                return Convert.ToString(el.Value);
            }
            catch
            {
                return "";
            }
        }

        private bool SafeXMLBoolean(XElement el)
        {
            try
            {
                return Convert.ToBoolean(el.Value);
            }
            catch
            {
                return false;
            }
        }

        private int SafeXMLInt(XElement el)
        {
            try
            {
                return Convert.ToInt32(el.Value);
            }
            catch
            {
                return 0;
            }
        }

        private DateTime SafeXMLDateTime(XElement el)
        {
            try
            {
                return Convert.ToDateTime(el.Value);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private string SafeString(object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch
            {
                return "";
            }
        }

        public string GetContentFromURL(string url, string postData = null, string requestMethod = "GET", string contentType = "text/json")
        {
            try
            {
                HttpContext.Current.Trace.Warn(url);
                byte[] buf = new byte[8192];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = requestMethod;
                request.ContentLength = 0;
                if (GetConfigInt("HttpTimeout") > 0)
                {
                    request.Timeout = GetConfigInt("HttpTimeout");
                }
                else
                {
                    request.Timeout = 5000;
                }
                if (!String.IsNullOrEmpty(postData))
                {
                    request.Method = "POST";
                    byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postData);
                    request.ContentType = contentType;
                    request.ContentLength = postBytes.Length;
                    Stream str = request.GetRequestStream();
                    HttpContext.Current.Trace.Warn("post url and data and type" + request.RequestUri.ToString() + " / " + postData + " / " + contentType);
                    str.Write(postBytes, 0, postBytes.Length);
                    str.Close();
                }
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream);
                    string str = reader.ReadToEnd();
                    HttpContext.Current.Trace.Warn("response: " + str);
                    return str;
                }
                catch (WebException ex)
                {
                    HttpContext.Current.Trace.Warn(ex.ToString());
                    using (var stream = ex.Response.GetResponseStream())
                    using (var read = new StreamReader(stream))
                    {
                        string str = read.ReadToEnd();
                        HttpContext.Current.Trace.Warn("response err: " + str);
                        return str;
                    }
                }
            }
            catch (Exception ex)
            {
                //HandleError(new Exception(url + ": " + ex.ToString()));
                HttpContext.Current.Trace.Warn(ex.ToString());
                return String.Empty;
            }
        }

        public static List<SelectListItem> CreateList(List<string> items, string defaultValue, string emptyTitle, bool allowEmptyTitle = false, bool allowZero = false)
        {
            if (defaultValue == null || (defaultValue == "0" && !allowZero))
            {
                defaultValue = "";
            }
            List<SelectListItem> list = new List<SelectListItem>();
            if (!String.IsNullOrEmpty(emptyTitle) || allowEmptyTitle)
            {
                if (emptyTitle == null)
                {
                    emptyTitle = String.Empty;
                }
                list.Add(new SelectListItem { Value = "", Text = emptyTitle });
            }
            items.ForEach(n => list.Add(new SelectListItem { Text = n, Value = n, Selected = IsSelected(n, defaultValue) }));
            return list;
        }

        public static List<SelectListItem> CreateList(Dictionary<string, string> items, string defaultValue, string emptyTitle)
        {
            return CreateList(items, defaultValue, emptyTitle, false);
        }

        public static List<SelectListItem> CreateList(Dictionary<string, string> items, string defaultValue, string emptyTitle, bool allowEmptyTitle, bool allowZero = false)
        {
            if (defaultValue == null || (defaultValue == "0" && !allowZero))
            {
                defaultValue = "";
            }
            List<SelectListItem> list = new List<SelectListItem>();
            if (!String.IsNullOrEmpty(emptyTitle) || allowEmptyTitle)
            {
                if (emptyTitle == null)
                {
                    emptyTitle = String.Empty;
                }
                list.Add(new SelectListItem { Value = "", Text = emptyTitle });
            }
            foreach (string key in items.Keys)
            {
                list.Add(new SelectListItem { Text = items[key], Value = key, Selected = IsSelected(key, defaultValue) });
            }
            return list;
        }

        private static bool IsSelected(string itemValue, string defaultValue)
        {
            HttpContext.Current.Trace.Warn("Value:" + itemValue);
            HttpContext.Current.Trace.Warn("Default:" + defaultValue);
            if (itemValue == defaultValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string InfoMessage(string message)
        {
            return Message("info", message);
        }

        public string WarningMessage(string message)
        {
            return Message("warning", message);
        }

        public string ErrorMessage(string message)
        {
            return Message("error", message);
        }

        public string Message(string type, string message)
        {
            string imageRoot = SiteAddress + "/Content/Images/icons";
            return "<div class=\"messagelist\" ><p ><img src=\"" + imageRoot + "/" + type + ".gif\" align=\"absmiddle\" style=\"border-width:0px; margin-right: 10px; margin-bottom: 10px;\" />" + message + "</p></div>";
        }

        private string SiteAddress
        {
            get
            {
                return GetConfig("SiteAddress");
            }
        }

        public bool SafeJsonBool(JToken obj, string key)
        {
            try
            {
                return (bool)obj[key];
            }
            catch (Exception ex)
            {
                try
                {
                    return Convert.ToBoolean((string)obj[key]);
                }
                catch
                {
                    HttpContext.Current.Trace.Warn(ex.ToString());
                    return false;
                }
            }
        }

        public decimal SafeJsonDecimal(JToken obj, string key)
        {
            try
            {
                return (decimal)obj[key];
            }
            catch
            {
                return 0m;
            }
        }

        public double SafeJsonDouble(JToken obj, string key)
        {
            try
            {
                return (double)obj[key];
            }
            catch
            {
                return 0;
            }
        }

        public int SafeJsonInt(JToken obj, string key)
        {
            try
            {
                return (int)obj[key];
            }
            catch
            {
                return 0;
            }
        }

        public int? SafeJsonIntNull(JToken obj, string key)
        {
            try
            {
                try
                {
                    return (int)obj[key];
                }
                catch
                {
                    string val = (string)obj[key];
                    return Convert.ToInt32(val);
                }
            }
            catch
            {
                return null;
            }
        }

        public float SafeJsonFloat(JToken obj, string key)
        {
            try
            {
                return (float)obj[key];
            }
            catch
            {
                return 0;
            }
        }

        private long SafeJsonLong(JToken obj, string key)
        {
            try
            {
                return (long)obj[key];
            }
            catch
            {
                return 0;
            }
        }

        public string SafeJsonString(JToken obj, string key)
        {
            try
            {
                return (string)obj[key];
            }
            catch
            {
                try
                {
                    int val = (int)obj[key];
                    return val.ToString();
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Trace.Warn(key + ": " + ex.ToString());
                    return "";
                }
            }
        }

        private DateTime SafeJsonDateTime(JToken obj, string key)
        {
            try
            {
                return Convert.ToDateTime((string)obj[key]);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private DateTime? SafeJsonDateTimeNull(JToken obj, string key)
        {
            try
            {
                return Convert.ToDateTime((string)obj[key]);
            }
            catch
            {
                return null;
            }
        }

        private decimal SafeXMLDecimal(XElement el)
        {
            try
            {
                return Convert.ToDecimal(el.Value);
            }
            catch
            {
                return 0m;
            }
        }

        private string SafeXMLString(XElement el, string defaultValue)
        {
            try
            {
                return Convert.ToString(el.Value);
            }
            catch
            {
                return defaultValue;
            }
        }

        private double SafeXMLDouble(XElement el)
        {
            try
            {
                return Convert.ToDouble(el.Value);
            }
            catch
            {
                return 0;
            }
        }

        private DateTime ConvertFromUnixTimestamp(string timeStamp)
        {
            try
            {
                double timestamp = Convert.ToDouble(timeStamp);
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return origin.AddSeconds(timestamp);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private bool SafeXMLBoolean(XElement el, bool defaultValue)
        {
            try
            {
                return Convert.ToBoolean(el.Value);
            }
            catch
            {
                return defaultValue;
            }
        }

        private bool SafeXMLBooleanTrue(XElement el)
        {
            try
            {
                return Convert.ToBoolean(el.Value);
            }
            catch
            {
                return true;
            }
        }

        private int? SafeXMLIntNull(XElement el)
        {
            try
            {
                return Convert.ToInt32(el.Value);
            }
            catch
            {
                return null;
            }
        }

        private DateTime? SafeXMLDateTimeNull(XElement el)
        {
            try
            {
                return Convert.ToDateTime(el.Value);
            }
            catch
            {
                return null;
            }
        }

        public string CreateSHAHash(string phrase)
        {
            SHA512Managed HashTool = new SHA512Managed();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(phrase));
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            return Convert.ToBase64String(EncryptedBytes);
        }

        public GeoCoordinate Geocode(string address)
        {
            try
            {
                string url = "http://services.gisgraphy.com//geocoding/geocode?format=JSON&address='" + address + "'";
                string data = GetContentFromURL(url);
                JObject json = JObject.Parse(data);
                if(SafeJsonInt(json, "numFound") > 0)
                {
                    // get first result
                    JToken loc = json["result"].FirstOrDefault();
                    if(loc != null)
                    {
                        return new GeoCoordinate
                        {
                            Latitude = SafeJsonDouble(loc, "lat"),
                            Longitude = SafeJsonDouble(loc, "lng")
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        private double ConvertMilesToMeters(double miles)
        {
            return miles * 1609.34;
        }

        private string GoogleAPIKey
        {
            get
            {
                return GetConfig("GoogleAPIKey");
            }
        }
    }
    #endregion
}