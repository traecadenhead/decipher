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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region Zip Code API    

        private string ZipCodeAPIKey
        {
            get
            {
                return GetConfig("ZipCodeAPIKey");
            }
        }

        private string ZIPCodeAPIRequest(string request)
        {
            try
            {
                string url = "https://www.zipcodeapi.com/rest/" + ZipCodeAPIKey + "/" + request;
                return GetContentFromURL(url);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public List<string> RequestZipsNearZip(string centerZip, int distanceInMiles)
        {
            List<string> list = new List<string>();
            try
            {
                string request = "radius.json/" + centerZip + "/" + distanceInMiles + "/miles";
                string response = ZIPCodeAPIRequest(request);
                JObject json = JObject.Parse(response);
                foreach(var item in (JArray)json["zip_codes"])
                {
                    string zip = SafeJsonString(item, "zip_code");
                    list.Add(zip);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        public Zip RequestZipLocation(Zip zip)
        {
            try
            {
                string request = "info.json/" + zip.Zip1 + "/degrees";
                string response = ZIPCodeAPIRequest(request);
                JObject json = JObject.Parse(response);
                zip.Latitude = SafeJsonFloat(json, "lat");
                zip.Longitude = SafeJsonFloat(json, "lng");
                return zip;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }
        
        # endregion
    }
}
