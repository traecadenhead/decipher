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
        #region UserNotifications

        public IQueryable<UserNotification> UserNotifications
        {
            get
            {
                return db.UserNotifications.AsQueryable();
            }
        }

        public bool ValidateUserNotification(UserNotification entity)
        {
            bool valid = true;
            if (entity.NotificationID == 0)
            {
                HttpContext.Current.Trace.Warn("NotificationID is a required field.");
                valid = false;
            }
            if (entity.UserID == 0)
            {
                HttpContext.Current.Trace.Warn("UserID is a required field.");
                valid = false;
            }
            return valid;
        }

        public bool SaveUserNotification(UserNotification entity)
        {
            try
            {
                if (ValidateUserNotification(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = UserNotifications.Where(n => n.UserNotificationID == entity.UserNotificationID).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.UserNotifications.Add(entity);
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

        public bool DeleteUserNotification(int id)
        {
            try
            {
                var entity = db.UserNotifications.Find(id);
                if (entity != null)
                {
                    db.UserNotifications.Remove(entity);
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
