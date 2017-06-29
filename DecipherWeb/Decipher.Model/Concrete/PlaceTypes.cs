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
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region PlaceTypes

        public IQueryable<PlaceType> PlaceTypes
        {
            get
            {
                return db.PlaceTypes.AsQueryable();
            }
        }

        public bool ValidatePlaceType(PlaceType entity)
        {
            if (entity.PlaceID == null || entity.PlaceID.Trim().Length == 0)
            {
                ModelState.AddModelError("PlaceID", "PlaceID is a required field.");
            }
            if (entity.TypeID == null || entity.TypeID.Trim().Length == 0)
            {
                ModelState.AddModelError("TypeID", "TypeID is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SavePlaceType(PlaceType entity)
        {
            try
            {
                if (ValidatePlaceType(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.PlaceTypes.Where(n => n.PlaceID == entity.PlaceID).Where(n => n.TypeID == entity.TypeID).FirstOrDefault();
                    if (original != null)
                    {
                        entity.PlaceTypeID = original.PlaceTypeID;
                        entity.DateCreated = original.DateCreated;
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.PlaceTypes.Add(entity);
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

        public bool DeletePlaceType(int id)
        {
            try
            {
                var entity = db.PlaceTypes.Find(id);
                if (entity != null)
                {
                    db.PlaceTypes.Remove(entity);
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

        # endregion
    }
}
