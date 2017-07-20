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
            return ModelState.IsValid;
        }

        public bool SaveCity(City entity)
        {
            try
            {
                if (ValidateCity(entity))
                {
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

        public bool AssociateAllCityZips()
        {
            try
            {
                var cities = Cities.Where(n => n.ZipsAssociated == false).ToList();
                cities.ForEach(n => AssociateCityZips(n));
                return true;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        public bool AssociateCityZips(int cityID)
        {
            try
            {
                var city = Cities.Where(n => n.CityID == cityID).FirstOrDefault();
                if(city != null)
                {
                    return AssociateCityZips(city);
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        private bool AssociateCityZips(City entity)
        {
            try
            {
                List<string> zips = RequestZipsNearZip(entity.CenterZip, entity.Radius);
                foreach(string z in zips)
                {
                    Zip zip = Zips.Where(n => n.Zip1 == z).FirstOrDefault();
                    if(zip != null)
                    {
                        zip.CityID = entity.CityID;
                        if (!zip.Latitude.HasValue || !zip.Longitude.HasValue)
                        {
                            zip = RequestZipLocation(zip);
                        }
                        SaveZip(zip);
                    }
                }
                entity.ZipsAssociated = true;
                return SaveCity(entity);
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        public City DetermineNearestCity(GeoCoordinate location)
        {
            try
            {
                var cities = Cities.ToList();
                return cities.Where(n => new GeoCoordinate(n.Latitude, n.Longitude).GetDistanceTo(location) <= ConvertMilesToMeters(n.Radius)).FirstOrDefault();
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public City GetDefaultCity()
        {
            try
            {
                string defaultCity = GetConfig("DefaultCity", "Austin, TX");
                var city = Cities.Where(n => n.Name == defaultCity).FirstOrDefault();
                return city;
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
