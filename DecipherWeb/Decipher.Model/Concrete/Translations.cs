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
        #region Translations

        public IQueryable<Translation> Translations
        {
            get
            {
                return db.Translations.AsQueryable();
            }
        }

        public bool ValidateTranslation(Translation entity)
        {
            if (entity.TranslationID == null || entity.TranslationID.Trim().Length == 0)
            {
                ModelState.AddModelError("TranslationID", "TranslationID is a required field.");
            }
            if (entity.LanguageID == null || entity.LanguageID.Trim().Length == 0)
            {
                ModelState.AddModelError("LanguageID", "LanguageID is a required field.");
            }
            if (entity.Text == null || entity.Text.Trim().Length == 0)
            {
                ModelState.AddModelError("Text", "Text is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveTranslation(Translation entity)
        {
            try
            {
                if (ValidateTranslation(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Translations.Where(n => n.TranslationID == entity.TranslationID).FirstOrDefault();
                    if (original != null)
                    {
                        entity.DateCreated = original.DateCreated;
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Translations.Add(entity);
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

        public bool DeleteTranslation(int id)
        {
            try
            {
                var entity = db.Translations.Find(id);
                if (entity != null)
                {
                    db.Translations.Remove(entity);
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

        public string TranslateString(string table, string id, string field, string text, string language = "en", List<Translation> translations = null)
        {
            try
            {
                var trans = translations.Where(n => n.TranslationID == table + "." + id + "." + field + "." + language).FirstOrDefault();
                if (trans == null)
                {
                    string translated = GoogleTranslateString(text, language);
                    if (!String.IsNullOrEmpty(translated))
                    {
                        trans = new Translation
                        {
                            TranslationID = table + "." + id + "." + field + "." + language,
                            LanguageID = language,
                            Text = translated,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now
                        };
                        db.Translations.Add(trans);
                        db.SaveChanges();
                    }
                }
                if (trans != null)
                {
                    return trans.Text;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return text;
        }

        #endregion
    }
}
