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
        #region Zips

        public IQueryable<Zip> Zips
        {
            get
            {
                return db.Zips.AsQueryable();
            }
        }

        public bool ValidateZip(Zip entity)
        {
            return ModelState.IsValid;
        }

        public bool SaveZip(Zip entity)
        {
            try
            {
                if (ValidateZip(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Zips.Where(n => n.Zip1 == entity.Zip1).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Zips.Add(entity);
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

        public bool DeleteZip(int id)
        {
            try
            {
                var entity = db.Zips.Find(id);
                if (entity != null)
                {
                    db.Zips.Remove(entity);
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

        public List<SelectListItem> ListDiversityIndexes(string defaultValue = "", string emptyText = "Any")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add(".50", "At least 50%");
            dict.Add(".40", "At least 40%");
            dict.Add(".30", "At Least 30%");
            dict.Add(".20", "At Least 20%");
            dict.Add(".00", "Under 20%");             
            return CreateList(dict, defaultValue, emptyText);
        }
        # endregion
    }
}
