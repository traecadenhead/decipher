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
        #region Google Places API

        private string GooglePlacesAPIRequest(string action, Dictionary<string, string> parameters = null)
        {
            try
            {
                string str = "key=" + GoogleAPIKey;
                if(parameters != null && parameters.Count > 0)
                {
                    foreach(string key in parameters.Keys)
                    {
                        str += "&"+ key + "=" + parameters[key];
                    }
                }
                string url = "https://maps.googleapis.com/maps/api/place/" + action + "/json?" + str;
                return GetContentFromURL(url);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        private PlaceResult SearchPlacesFromGoogle(Search search)
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                if (!String.IsNullOrEmpty(search.Token))
                {
                    dict.Add("pagetoken", search.Token);
                }
                else
                {
                    dict.Add("location", search.Location.Latitude.ToString() + "," + search.Location.Longitude.ToString());                    
                    if (search.Distance.HasValue)
                    {
                        // convert miles to meters since that's the format Google wants
                        dict.Add("radius", Convert.ToInt32(ConvertMilesToMeters(search.Distance.Value)).ToString());
                    }
                    else
                    {
                        // default to city radius
                        dict.Add("radius", Convert.ToInt32(ConvertMilesToMeters(search.City.Radius)).ToString());
                    }
                    if (!String.IsNullOrEmpty(search.TypeID))
                    {
                        dict.Add("type", search.TypeID);
                        if (!dict.ContainsKey("rankby"))
                        {
                            dict.Add("rankby", "distance");
                            // can't send radius if we're ranking by distance
                            dict.Remove("radius");
                        }
                    }
                    if (!String.IsNullOrEmpty(search.Keyword))
                    {
                        dict.Add("keyword", search.Keyword);
                        if (!dict.ContainsKey("rankby"))
                        {
                            dict.Add("rankby", "distance");
                            // can't send radius if we're ranking by distance
                            dict.Remove("radius");
                        }
                    }                    
                }
                string response = GooglePlacesAPIRequest("nearbysearch", dict);
                JObject json = JObject.Parse(response);
                return new PlaceResult
                {
                    Results = ParseGooglePlaces(json),
                    NextToken = SafeJsonString(json, "next_page_token")
                };
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return new PlaceResult
            {
                Results = new List<Place>()
            };
        }

        private List<Place> ImportPlacesFromGoogle(int cityID, GeoCoordinate location, float radiusinMiles, string type = null)
        {
            List<Place> list = new List<Place>();
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("location", location.Latitude.ToString() + "," + location.Longitude.ToString());
                // convert miles to meters since that's the format Google wants
                dict.Add("radius", Convert.ToInt32(ConvertMilesToMeters(radiusinMiles)).ToString());
                if (!String.IsNullOrEmpty(type))
                {
                    dict.Add("type", type);
                }
                string response = GooglePlacesAPIRequest("nearbysearch", dict);
                JObject json = JObject.Parse(response);
                var places = ParseGooglePlaces(json);
                if (SaveListOfPlaces(cityID, places))
                {
                    string next = SafeJsonString(json, "next_page_token");
                    if (!String.IsNullOrEmpty(next))
                    {
                        ImportPlacesFromToken(cityID, next);
                    }
                } 
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        private async void ImportPlacesFromToken(int cityID, string token)
        {
            await Task.Delay(2000);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("pagetoken", token);
            string response = GooglePlacesAPIRequest("nearbysearch", dict);
            JObject json = JObject.Parse(response);
            var places = ParseGooglePlaces(json);
            if (SaveListOfPlaces(cityID, places))
            {
                string next = SafeJsonString(json, "next_page_token");
                if (!String.IsNullOrEmpty(next))
                {
                    ImportPlacesFromToken(cityID, token);
                }
            }
        }

        private List<Place> ParseGooglePlaces(JObject json)
        {
            List<Place> list = new List<Place>();
            try
            {
                foreach (var item in (JArray)json["results"])
                {
                    JToken geo = item["geometry"];
                    JToken loc = geo["location"];
                    List<string> types = new List<string>();
                    foreach (string t in item["types"])
                    {
                        types.Add(t);
                    }
                    Place place = new Place
                    {
                        PlaceID = SafeJsonString(item, "place_id"),
                        Latitude = SafeJsonFloat(loc, "lat"),
                        Longitude = SafeJsonFloat(loc, "lng"),
                        Name = SafeJsonString(item, "name"),
                        GoogleRating = SafeJsonDecimal(item, "rating"),
                        Cost = SafeJsonInt(item, "price_level").ToString(),
                        Address = SafeJsonString(item, "vicinity"),
                        TypesList = types
                    };
                    list.Add(place);
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        private Place GetPlaceDetailFromGoogle(string placeID)
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("placeid", placeID);
                string response = GooglePlacesAPIRequest("details", dict);
                JObject json = JObject.Parse(response);
                JToken item = (JToken)json["result"];
                string zip = null;
                foreach(var c in item["address_components"].Children())
                {
                    bool isZip = false;
                    foreach(string t in c["types"])
                    {
                        if(t == "postal_code")
                        {
                            isZip = true;
                        }
                    }
                    if (isZip)
                    {
                        zip = SafeJsonString(c, "short_name");
                        if(zip.Length > 5)
                        {
                            zip = zip.Substring(0, 5);
                        }
                    }
                }
                if (!String.IsNullOrEmpty(zip))
                {
                    JToken geo = item["geometry"];
                    JToken loc = geo["location"];
                    List<string> types = new List<string>();
                    foreach (string t in item["types"])
                    {
                        types.Add(t);
                    }
                    Place place = new Place
                    {
                        PlaceID = placeID,
                        Name = SafeJsonString(item, "name"),
                        Address = SafeJsonString(item, "formatted_address"),
                        Zip = zip,
                        Latitude = SafeJsonFloat(loc, "lat"),
                        Longitude = SafeJsonFloat(loc, "lng"),
                        GoogleRating = SafeJsonDecimal(item, "rating"),
                        Cost = SafeJsonInt(item, "price_level").ToString(),
                        TypesList = types
                    };
                    return place;
                }
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
