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
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region Cities

        public IQueryable<City> Cities
        {
            get
            {
                return db.Cities.AsQueryable();
            }
        }

        public bool ValidateCity(City entity)
        {
            bool valid = true;
            if(entity.QuestionSetID == 0)
            {
                HttpContext.Current.Trace.Warn("QuestionSetID not found");
                valid = false;
            }
            if (entity.Name == null || entity.Name.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("Name not found");
                valid = false;
            }
            if (entity.DisplayName == null || entity.DisplayName.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("DisplayName not found");
                valid = false;
            }
            if (entity.Latitude == 0)
            {
                HttpContext.Current.Trace.Warn("Latitude not found");
                valid = false;
            }
            if (entity.Longitude == 0)
            {
                HttpContext.Current.Trace.Warn("Longitude not found");
                valid = false;
            }
            return valid;
        }

        public bool SaveCity(City entity)
        {
            try
            {
                if (ValidateCity(entity))
                {
                    // set defaults for fields we may not need anymore
                    entity.DateModified = DateTime.Now;
                    var original = db.Cities.Where(n => n.CityID == entity.CityID).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Cities.Add(entity);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
                foreach(var err in db.GetValidationErrors())
                {
                    foreach(var e in err.ValidationErrors)
                    {
                        HttpContext.Current.Trace.Warn(e.PropertyName + ": " + e.ErrorMessage);
                    }
                }
            }
            return false;
        }

        public bool DeleteCity(int id)
        {
            try
            {
                var entity = db.Cities.Find(id);
                if (entity != null)
                {
                    db.Cities.Remove(entity);
                    if (db.SaveChanges() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        //public bool AssociateAllCityZips()
        //{
        //    try
        //    {
        //        var cities = Cities.Where(n => n.ZipsAssociated == false).ToList();
        //        cities.ForEach(n => AssociateCityZips(n));
        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        HttpContext.Current.Trace.Warn(ex.ToString());
        //    }
        //    return false;
        //}

        //public bool AssociateCityZips(int cityID)
        //{
        //    try
        //    {
        //        var city = Cities.Where(n => n.CityID == cityID).FirstOrDefault();
        //        if(city != null)
        //        {
        //            return AssociateCityZips(city);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        HttpContext.Current.Trace.Warn(ex.ToString());
        //    }
        //    return false;
        //}

        //private bool AssociateCityZips(City entity)
        //{
        //    try
        //    {
        //        List<string> zips = RequestZipsNearZip(entity.CenterZip, entity.Radius);
        //        foreach(string z in zips)
        //        {
        //            Zip zip = Zips.Where(n => n.Zip1 == z).FirstOrDefault();
        //            if(zip != null)
        //            {
        //                zip.CityID = entity.CityID;
        //                if (!zip.Latitude.HasValue || !zip.Longitude.HasValue)
        //                {
        //                    zip = RequestZipLocation(zip);
        //                }
        //                SaveZip(zip);
        //            }
        //        }
        //        entity.ZipsAssociated = true;
        //        return SaveCity(entity);
        //    }
        //    catch(Exception ex)
        //    {
        //        HttpContext.Current.Trace.Warn(ex.ToString());
        //    }
        //    return false;
        //}

        public City DetermineNearestCity(GeoCoordinate location, string language = "en")
        {
            try
            {
                var cities = Cities.ToList();
                HttpContext.Current.Trace.Warn(cities.Count + " cities");
                var city = cities.OrderBy(n => new GeoCoordinate(n.Latitude, n.Longitude).GetDistanceTo(location)).FirstOrDefault();
                if (city != null)
                {
                    return TranslateCity(city, language);
                }
                else
                {
                    HttpContext.Current.Trace.Warn("no city found");
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public City GetDefaultCity(string language = "en")
        {
            try
            {
                string defaultCity = GetConfig("DefaultCity", "Austin, TX");
                var city = Cities.Where(n => n.Name == defaultCity).FirstOrDefault();
                return TranslateCity(city, language);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public City GetCity(int cityID, string language = "en")
        {
            try
            {
                var city = Cities.Where(n => n.CityID == cityID).FirstOrDefault();
                return TranslateCity(city, language);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public City TranslateCity(City city, string language = "en", List<Translation> translations = null)
        {
            try
            {
                if (translations == null)
                {
                    translations = Translations.Where(n => n.TranslationID.IndexOf("Cities." + city.CityID) == 0).Where(n => n.LanguageID == language).ToList();
                }
                var entity = new City { CityID = city.CityID };
                entity = (City)UpdateObject(entity, city, "CityID");
                entity.Name = TranslateString("Cities", city.CityID.ToString(), "Name", city.Name, language, translations);
                entity.DisplayName = TranslateString("Cities", city.CityID.ToString(), "DisplayName", city.DisplayName, language, translations);
                entity.ReportName = TranslateString("Cities", city.CityID.ToString(), "ReportName", city.ReportName, language, translations);
                return entity;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return city;
        }

        public City GetCityFromURL(string path)
        {
            try
            {
                var cities = Cities.ToList();
                var city = cities.Where(n => n.DisplayName.Replace(" ", "").ToLower() == path.ToLower()).FirstOrDefault();
                if(city != null)
                {
                    return city;
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        # endregion
    }
}
