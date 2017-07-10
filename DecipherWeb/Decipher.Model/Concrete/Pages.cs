﻿using System;
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
                    var original = db.Pages.Find(entity.PageID);
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

        # endregion
    }
}