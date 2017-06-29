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
        #region ZipDemographics

        public IQueryable<ZipDemographic> ZipDemographics
        {
            get
            {
                return db.ZipDemographics.AsQueryable();
            }
        }

        public bool ValidateZipDemographic(ZipDemographic entity)
        {
            if (entity.Zip == null || entity.Zip.Trim().Length == 0)
            {
                ModelState.AddModelError("Zip", "Zip is a required field.");
            }
            if (entity.DemographicID == 0)
            {
                ModelState.AddModelError("DemographicID", "Demographic is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveZipDemographic(ZipDemographic entity)
        {
            try
            {
                if (ValidateZipDemographic(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.ZipDemographics.Find(entity.ZipDemographicID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.ZipDemographics.Add(entity);
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

        public bool DeleteZipDemographic(int id)
        {
            try
            {
                var entity = db.ZipDemographics.Find(id);
                if (entity != null)
                {
                    db.ZipDemographics.Remove(entity);
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
