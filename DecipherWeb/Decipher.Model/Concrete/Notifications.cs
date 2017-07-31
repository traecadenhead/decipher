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
        #region Notifications

        public IQueryable<Notification> Notifications
        {
            get
            {
                return db.Notifications.AsQueryable();
            }
        }

        public bool ValidateNotification(Notification entity)
        {
            bool valid = true;
            if (entity.Message == null || entity.Message.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("Message is a required field.");
                valid = false;
            }
            if (entity.AuthorID == null || entity.AuthorID.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("AuthorID is a required field.");
                valid = false;
            }
            return valid;
        }

        public bool SaveNotification(Notification entity)
        {
            try
            {
                if (ValidateNotification(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = Notifications.Where(n => n.NotificationID == entity.NotificationID).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Notifications.Add(entity);
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

        public bool DeleteNotification(int id)
        {
            try
            {
                var entity = db.Notifications.Find(id);
                if (entity != null)
                {
                    db.Notifications.Remove(entity);
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
