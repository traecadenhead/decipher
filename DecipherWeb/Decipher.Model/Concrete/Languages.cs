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
        #region Languages

        public IQueryable<Language> Languages
        {
            get
            {
                return db.Languages.AsQueryable();
            }
        }

        public bool ValidateLanguage(Language entity)
        {
            if (entity.LanguageID == null || entity.LanguageID.Trim().Length == 0)
            {
                ModelState.AddModelError("LanguageID", "LanguageID is a required field.");
            }
            if (entity.Name == null || entity.Name.Trim().Length == 0)
            {
                ModelState.AddModelError("Name", "Name is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveLanguage(Language entity)
        {
            try
            {
                if (ValidateLanguage(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Languages.Find(entity.LanguageID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Languages.Add(entity);
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

        public bool DeleteLanguage(int id)
        {
            try
            {
                var entity = db.Languages.Find(id);
                if (entity != null)
                {
                    db.Languages.Remove(entity);
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

        public List<SelectListItem> ListLanguages(string defaultValue = "", string emptyText = "")
        {
            var languages = Languages.OrderBy(n => n.Name).ToList();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            languages.ForEach(n => dict.Add(n.LanguageID, n.Name));
            return CreateList(dict, defaultValue, emptyText);
        }

        # endregion
    }
}
