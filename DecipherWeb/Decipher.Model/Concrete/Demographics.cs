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
        #region Demographics

        public IQueryable<Demographic> Demographics
        {
            get
            {
                return db.Demographics.AsQueryable();
            }
        }

        public bool ValidateDemographic(Demographic entity)
        {
            if (entity.Name == null || entity.Name.Trim().Length == 0)
            {
                ModelState.AddModelError("Name", "Name is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveDemographic(Demographic entity)
        {
            try
            {
                if (ValidateDemographic(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Demographics.Find(entity.DemographicID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Demographics.Add(entity);
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

        public bool DeleteDemographic(int id)
        {
            try
            {
                var entity = db.Demographics.Find(id);
                if (entity != null)
                {
                    db.Demographics.Remove(entity);
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
