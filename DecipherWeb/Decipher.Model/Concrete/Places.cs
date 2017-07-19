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
        #region Places

        public IQueryable<Place> Places
        {
            get
            {
                return db.Places.AsQueryable();
            }
        }

        public bool ValidatePlace(Place entity)
        {
            if (entity.Name == null || entity.Name.Trim().Length == 0)
            {
                ModelState.AddModelError("Name", "Name is a required field.");
            }
            if (entity.Zip == null || entity.Zip.Trim().Length == 0)
            {
                ModelState.AddModelError("Zip", "Zip is a required field.");
            }
            if (entity.Address == null || entity.Address.Trim().Length == 0)
            {
                ModelState.AddModelError("Address", "Address is a required field.");
            }
            if (entity.Longitude == 0)
            {
                ModelState.AddModelError("Longitude", "Longitude is a required field.");
            }
            if (entity.Latitude == 0)
            {
                ModelState.AddModelError("Latitude", "Latitude is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SavePlace(Place entity)
        {
            try
            {
                if (ValidatePlace(entity))
                {
                    var types = entity.Types;
                    entity.DateModified = DateTime.Now;
                    var original = db.Places.Find(entity.PlaceID);
                    if (original != null)
                    {
                        entity.DateCreated = original.DateCreated;
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Places.Add(entity);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        if(types != null && types.Count > 0)
                        {
                            // save place types
                            types.ForEach(n => SavePlaceType(new PlaceType { PlaceID = entity.PlaceID, TypeID = n.TypeID }));
                        }
                        return true;
                    }
                }
                else
                {
                    HttpContext.Current.Trace.Warn("couldn't validate");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        public bool DeletePlace(int id)
        {
            try
            {
                var entity = db.Places.Find(id);
                if (entity != null)
                {
                    db.Places.Remove(entity);
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

        public List<SelectListItem> ListPlaceDistances(string defaultValue = "", string emptyText= "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("2", "Within 2 Miles");
            dict.Add("5", "Within 5 Miles");
            dict.Add("10", "Within 10 Miles");
            dict.Add("20", "Within 20 Miles");
            return CreateList(dict, defaultValue, emptyText);
        }

        public PlaceResult NearbyPlaces(Search entity)
        {
            try
            {
                entity.City = DetermineNearestCity(entity.Location);
                if (entity.City != null)
                {
                    // Search Google Places API
                    var results = NearbyPlacesFromGoogle(entity);
                    if (results.NumResults > 0)
                    {
                        var rtn = new PlaceResult
                        {
                            NextToken = results.NextToken,
                            Results = new List<Place>(),
                            Response = "Results"
                        };
                        var zips = Zips.Where(n => n.CityID == entity.City.CityID).ToList();
                        var types = Types.ToList();
                        var reviews = Reviews.Where(n => n.Place.Zip1.CityID == entity.City.CityID).ToList();
                        foreach (var place in results.Results)
                        {
                            // For each result, get nearest zip / demographics and distance
                            place.Zip = DetermineNearestZipForPlace(place, entity.City.CityID, zips);
                            //if (!String.IsNullOrEmpty(place.Zip))
                            //{
                            //    place.DefaultZip = zips.Where(n => n.Zip1 == place.Zip).FirstOrDefault();
                            //}
                            // LATER: use Translated Name for other languages. We don't want to replace Name since it's saving
                            place.TranslatedName = place.Name;
                            place.DistanceInMeters = place.Location.GetDistanceTo(entity.Location);
                            place.Types = new List<Entities.Type>();
                            foreach (string typeID in place.TypesList)
                            {
                                var type = types.Where(n => n.TypeID == typeID).FirstOrDefault();
                                if (type != null)
                                {
                                    place.Types.Add(type);
                                }
                            }
                            if (reviews.Where(n => n.PlaceID == place.PlaceID).Count() > 0)
                            {
                                place.HasReviews = true;
                                place.AvgScore = Convert.ToDouble(reviews.Where(n => n.PlaceID == place.PlaceID).Select(n => n.Score).Sum()) / Convert.ToDouble(reviews.Where(n => n.PlaceID == place.PlaceID).Select(n => n.Score).Count());
                            }
                            rtn.Results.Add(place);
                        }
                        if (String.IsNullOrEmpty(entity.Term))
                        {
                            rtn.Results = rtn.Results.Where(n => n.Distance.GetValueOrDefault() <= 1).OrderBy(n => n.Distance.GetValueOrDefault()).ToList();
                        }
                        rtn.Search = entity;
                        return rtn;
                    }
                }
                else
                {
                    return new PlaceResult
                    {
                        Results = new List<Place>(),
                        Response = "No City"
                    };
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
                return new PlaceResult
                {
                    Results = new List<Place>(),
                    Response = "Error"
                };
            }
            return new PlaceResult
            {
                Results = new List<Place>(),
                Response = "No Results"
            };
        }

        public PlaceResult SearchPlaces(Search entity)
        {
            try
            {
                // first determine the city
                entity.City = DetermineNearestCity(entity.Location);
                if (entity.City != null)
                {
                    // Search Google Places API
                    var results = SearchPlacesFromGoogle(entity);
                    if (results.NumResults > 0)
                    {
                        var rtn = new PlaceResult
                        {
                            NextToken = results.NextToken,
                            Results = new List<Place>(),
                            Response = "Results"
                        };
                        var zips = Zips.Where(n => n.CityID == entity.City.CityID).ToList();
                        var types = Types.ToList();
                        var reviews = Reviews.Where(n => n.Place.Zip1.CityID == entity.City.CityID).ToList();
                        foreach (var place in results.Results)
                        {
                            // For each result, get nearest zip / demographics and distance
                            place.Zip = DetermineNearestZipForPlace(place, entity.City.CityID, zips);
                            if (!String.IsNullOrEmpty(place.Zip))
                            {
                                place.DefaultZip = zips.Where(n => n.Zip1 == place.Zip).FirstOrDefault();
                            }
                            place.DistanceInMeters = place.Location.GetDistanceTo(entity.Location);
                            if ((!entity.Distance.HasValue || place.Distance <= entity.Distance.Value) && (!entity.Diversity.HasValue || place.DefaultZip.DiversityIndex >= entity.Diversity.Value))
                            {
                                place.Types = new List<Entities.Type>();
                                foreach (string typeID in place.TypesList)
                                {
                                    var type = types.Where(n => n.TypeID == typeID).FirstOrDefault();
                                    if (type != null)
                                    {
                                        place.Types.Add(type);
                                    }
                                }
                                if(reviews.Where(n => n.PlaceID == place.PlaceID).Count() > 0)
                                {
                                    place.HasReviews = true;
                                    place.AvgScore = Convert.ToDouble(reviews.Where(n => n.PlaceID == place.PlaceID).Select(n => n.Score).Sum()) / Convert.ToDouble(reviews.Where(n => n.PlaceID == place.PlaceID).Select(n => n.Score).Count());
                                }
                                rtn.Results.Add(place);
                            }
                        }
                        rtn.Search = entity;
                        return rtn;
                    }
                }
                else
                {
                    return new PlaceResult
                    {
                        Results = new List<Place>(),
                        Response = "No City"
                    };
                }         
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
                return new PlaceResult
                {
                    Results = new List<Place>(),
                    Response = "Error"
                };
            }
            return new PlaceResult
            {
                Results = new List<Place>(),
                Response = "No Results"
            };
        }

        public Place GetPlaceForReview(string placeID)
        {
            try
            {
                var place = GetPlace(placeID);
                if (place != null)
                {                    
                    place.Questions = Questions.OrderBy(n => n.Ordinal).ToList();
                    var descriptors = Descriptors.Where(n => n.DescriptorType == "Question").OrderBy(n => n.Name).ToList();
                    foreach(var question in place.Questions)
                    {
                        question.Descriptors = descriptors.Where(n => n.AssociatedID == question.QuestionID).OrderBy(n => n.Ordinal).ThenBy(n => n.Name).ToList();
                    }
                    return place;
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public bool ImportPlace(string placeID)
        {
            try
            {
                var place = GetPlaceDetailFromGoogle(placeID);
                if(place != null)
                {
                    if (SavePlace(place))
                    {
                        if (place.TypesList.Count > 0)
                        {
                            // clear previous types list in case anything is outdated
                            var oldTypes = PlaceTypes.Where(n => n.PlaceID == placeID).ToList();
                            if(oldTypes.Count > 0)
                            {
                                db.PlaceTypes.RemoveRange(oldTypes);
                            }
                            var supportedTypes = Types.Select(n => n.TypeID).Distinct().ToList();
                            foreach (string type in place.TypesList)
                            {
                                if (supportedTypes.Contains(type))
                                {
                                    db.PlaceTypes.Add(new PlaceType
                                    {
                                        PlaceID = place.PlaceID,
                                        TypeID = type,
                                        DateCreated = DateTime.Now,
                                        DateModified = DateTime.Now
                                    });
                                }
                            }
                            db.SaveChanges();
                        }
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        public bool ImportPlacesInCity(int cityID, string type = null)
        {
            try
            {
                var city = Cities.Where(n => n.CityID == cityID).FirstOrDefault();
                if(city != null)
                {
                    return ImportPlacesInCity(city);
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        private bool ImportPlacesInCity(City entity)
        {
            try
            {
                var zips = Zips.Where(n => n.CityID == entity.CityID).ToList();
                var types = Types.Where(n => n.Active == true).ToList();
                var zipTypes = ZipTypes.Where(n => n.Zip1.CityID == entity.CityID).ToList();
                DateTime lastDate = DateTime.Now.AddMonths(-1);
                foreach (var zip in zips)
                {
                    foreach (var type in types)
                    {
                        if (zipTypes.Where(n => n.Zip == zip.Zip1).Where(n => n.TypeID == type.TypeID).Where(n => n.DateModified >= lastDate).FirstOrDefault() == null)
                        {
                            // for now taking a 5 mile radius around zip
                            // TO DO: make sure it's ordered by proximity
                            ImportPlacesFromGoogle(entity.CityID, zip.Coordinates, 5, type.TypeID);
                            // TO DO: Save the zip type so it won't download again for a month
                        }
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        public bool SaveListOfPlaces(int cityID, List<Place> places)
        {
            try
            {
                var supportedTypes = Types.Select(n => n.TypeID).Distinct().ToList();
                var zips = Zips.Where(n => n.CityID == cityID).Where(n => n.Latitude != null).Where(n => n.Longitude != null).ToList();
                foreach (var place in places)
                {
                    place.Zip = DetermineNearestZipForPlace(place, cityID, zips);
                    if (SavePlace(place))
                    {
                        if (place.TypesList.Count > 0)
                        {
                            // clear previous types list in case anything is outdated
                            var oldTypes = PlaceTypes.Where(n => n.PlaceID == place.PlaceID).ToList();
                            if (oldTypes.Count > 0)
                            {
                                db.PlaceTypes.RemoveRange(oldTypes);
                            }
                            foreach (string typeID in place.TypesList)
                            {
                                if (supportedTypes.Contains(typeID))
                                {
                                    db.PlaceTypes.Add(new PlaceType
                                    {
                                        PlaceID = place.PlaceID,
                                        TypeID = typeID,
                                        DateCreated = DateTime.Now,
                                        DateModified = DateTime.Now
                                    });
                                }
                            }
                            db.SaveChanges();
                        }
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        private string DetermineNearestZipForPlace(Place place, int cityID, List<Zip> zips = null)
        {
            try
            {
                if(zips == null)
                {
                    zips = Zips.Where(n => n.CityID == cityID).Where(n => n.Latitude != null).Where(n => n.Longitude != null).ToList();
                }
                return zips.OrderBy(n => new GeoCoordinate(n.Latitude.Value, n.Longitude.Value).GetDistanceTo(new GeoCoordinate(place.Latitude, place.Longitude))).Select(n => n.Zip1).FirstOrDefault();
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public Place GetPlace(string placeID)
        {
            try
            {
                // maybe in future, if place not found in DB get it from Google Places
                var place = Places.Where(n => n.PlaceID == placeID).FirstOrDefault();
                if(place != null)
                {
                    place.City = DetermineNearestCity(place.Location);
                    return place;
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
