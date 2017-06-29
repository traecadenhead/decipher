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
        #region ZipTypes

        public IQueryable<ZipType> ZipTypes
        {
            get
            {
                return db.ZipTypes.AsQueryable();
            }
        }

        public bool ValidateZipType(ZipType entity)
        {
            if (entity.Zip == null || entity.Zip.Trim().Length == 0)
            {
                ModelState.AddModelError("Zip", "Zip is a required field.");
            }
            if (entity.TypeID == null || entity.TypeID.Trim().Length == 0)
            {
                ModelState.AddModelError("PlaceID", "TypeID is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveZipType(ZipType entity)
        {
            try
            {
                if (ValidateZipType(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.ZipTypes.Find(entity.ZipTypeID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.ZipTypes.Add(entity);
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

        public bool DeleteZipType(int id)
        {
            try
            {
                var entity = db.ZipTypes.Find(id);
                if (entity != null)
                {
                    db.ZipTypes.Remove(entity);
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
