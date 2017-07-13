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
        #region Pages

        public IQueryable<Page> Pages
        {
            get
            {
                return db.Pages.AsQueryable();
            }
        }

        public bool ValidatePage(Page entity)
        {
            if (entity.Title == null || entity.Title.Trim().Length == 0)
            {
                ModelState.AddModelError("Title", "Title is a required field.");
            }
            if (entity.Content == null || entity.Content.Trim().Length == 0)
            {
                ModelState.AddModelError("Content", "Content is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SavePage(Page entity)
        {
            try
            {
                if (ValidatePage(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = Pages.Where(n => n.PageID == entity.PageID).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.Ordinal = Pages.OrderByDescending(n => n.Ordinal).Select(n => n.Ordinal).FirstOrDefault() + 1;
                        entity.DateCreated = DateTime.Now;
                        db.Pages.Add(entity);
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

        public bool DeletePage(int id)
        {
            try
            {
                var entity = db.Pages.Find(id);
                if (entity != null)
                {
                    db.Pages.Remove(entity);
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

        public bool OrderPages(string data)
        {
            try
            {
                string[] arr = data.Split('~');
                var list = Pages.ToList();
                foreach (string item in arr)
                {
                    string[] itemarr = item.Split('-');
                    int ID = Convert.ToInt32(itemarr[0]);
                    int ordinal = Convert.ToInt32(itemarr[1]);
                    var p = list.Where(n => n.PageID == ID).FirstOrDefault();
                    p.Ordinal = ordinal;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        public List<Page> GetPages(string language = "en")
        {
            List<Page> list = new List<Page>();
            try
            {
                var pages = Pages.Where(n => n.Active == true).OrderBy(n => n.Ordinal).ToList();
                if (language != GetConfig("DefaultLanguage"))
                {
                    var translations = Translations.Where(n => n.TranslationID.IndexOf("Pages.") == 0).Where(n => n.LanguageID == language).ToList();
                    foreach (var page in pages)
                    {
                        list.Add(TranslatePage(page, language, translations));
                    }
                }
                else
                {
                    list.AddRange(pages);
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        public Page GetPage(int pageID, string language = "en")
        {
            try
            {
                var page = Pages.Where(n => n.PageID == pageID).FirstOrDefault();
                if (language != GetConfig("DefaultLanguage"))
                {
                    var translations = Translations.Where(n => n.TranslationID.IndexOf("Pages." + page.PageID) == 0).Where(n => n.LanguageID == language).ToList();
                    return TranslatePage(page, language, translations);
                }
                else
                {
                    return page;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public Page TranslatePage(Page page, string language = "en", List<Translation> translations = null)
        {
            try
            {
                if(translations == null)
                {
                    translations = Translations.Where(n => n.TranslationID.IndexOf("Pages." + page.PageID) == 0).Where(n => n.LanguageID == language).ToList();
                }
                return new Page
                {
                    PageID = page.PageID,
                    Title = TranslateString("Pages", page.PageID.ToString(), "Title", page.Title, language, translations),
                    Content = TranslateString("Pages", page.PageID.ToString(), "Content", page.Content, language, translations),
                    Ordinal = page.Ordinal,
                    DateCreated = page.DateCreated,
                    DateModified = page.DateModified
                };
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return page;
        }

        # endregion
    }
}
