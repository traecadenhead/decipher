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
        #region NotificationTargets

        public IQueryable<NotificationTarget> NotificationTargets
        {
            get
            {
                return db.NotificationTargets.AsQueryable();
            }
        }

        public bool ValidateNotificationTarget(NotificationTarget entity)
        {
            bool valid = true;
            if (entity.NotificationID == 0)
            {
                HttpContext.Current.Trace.Warn("NotificationID is a required field.");
                valid = false;
            }
            if (entity.TargetType == null || entity.TargetType.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("TargetType is a required field.");
                valid = false;
            }
            return valid;
        }

        public bool SaveNotificationTarget(NotificationTarget entity)
        {
            try
            {
                if (ValidateNotificationTarget(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = NotificationTargets.Where(n => n.NotificationTargetID == entity.NotificationTargetID).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.NotificationTargets.Add(entity);
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

        public bool DeleteNotificationTarget(int id)
        {
            try
            {
                var entity = db.NotificationTargets.Find(id);
                if (entity != null)
                {
                    db.NotificationTargets.Remove(entity);
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
