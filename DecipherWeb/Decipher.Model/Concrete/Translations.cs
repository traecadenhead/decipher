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
            bool valid = true;
            if (entity.TranslationID == null || entity.TranslationID.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("TranslationID is a required field.");
                valid = false;
            }
            if (entity.LanguageID == null || entity.LanguageID.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("LanguageID is a required field.");
                valid = false;
            }
            if (entity.Text == null || entity.Text.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("Text is a required field.");
                valid = false;
            }
            return valid;
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
                    else
                    {
                        HttpContext.Current.Trace.Warn("no changes saved");
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

        public string TranslateString(string translationID, string text, string language = "en")
        {
            try
            {
                string[] arr = translationID.Split('.');
                return TranslateString(arr[0], arr[1], arr[2], text, language);
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return text;
        }

        public string TranslateString(string table, string id, string field, string text, string language = "en", List<Translation> translations = null)
        {
            try
            {
                Translation trans = null;
                if (translations != null && translations.Count > 0)
                {
                    trans = translations.Where(n => n.TranslationID == table + "." + id + "." + field + "." + language).FirstOrDefault();                    
                }
                else
                {
                    string transID = table + "." + id + "." + field + "." + language;
                    trans = Translations.Where(n => n.TranslationID == transID).FirstOrDefault();
                }
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

        public string GetOriginalTranslation(string translationID)
        {
            string str = String.Empty;
            try
            {
                string[] arr = translationID.Split('.');
                string type = arr[0];
                string id = arr[1];
                string field = arr[2];
                switch (type)
                {
                    case "Questions":
                        var q = Questions.Where(n => n.QuestionID.ToString() == id).FirstOrDefault();
                        str = q.Text;
                        break;
                    case "Descriptors":
                        var d = Descriptors.Where(n => n.DescriptorID.ToString() == id).FirstOrDefault();
                        str = d.Name;
                        break;
                    case "Pages":
                        var p = Pages.Where(n => n.PageID.ToString() == id).FirstOrDefault();
                        if(field == "Title")
                        {
                            str = p.Title;
                        }
                        else if(field == "Content")
                        {
                            str = p.Content;
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return str;
        }

        public bool SaveOriginalTranslation(string translationID, string text)
        {
            try
            {
                string[] arr = translationID.Split('.');
                string type = arr[0];
                string id = arr[1];
                string field = arr[2];
                switch (type)
                {
                    case "Questions":
                        var q = Questions.Where(n => n.QuestionID.ToString() == id).FirstOrDefault();
                        q.Text = text;
                        return SaveQuestion(q);
                    case "Descriptors":
                        var d = Descriptors.Where(n => n.DescriptorID.ToString() == id).FirstOrDefault();
                        d.Name = text;
                        return SaveDescriptor(d);
                    case "Pages":
                        var p = Pages.Where(n => n.PageID.ToString() == id).FirstOrDefault();
                        if (field == "Title")
                        {
                            p.Title = text;
                        }
                        else if (field == "Content")
                        {
                            p.Content = text;
                        }
                        return SavePage(p);
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        #endregion
    }
}
