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
        #region UserDevices

        public IQueryable<UserDevice> UserDevices
        {
            get
            {
                return db.UserDevices.AsQueryable();
            }
        }

        public bool ValidateUserDevice(UserDevice entity)
        {
            bool valid = true;
            if (entity.UserID == 0)
            {
                HttpContext.Current.Trace.Warn("UserID is a required field.");
                valid = false;
            }
            if (entity.DeviceType == null || entity.DeviceType.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("DeviceType is a required field.");
                valid = false;
            }
            if (entity.DeviceID == null || entity.DeviceID.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("DeviceID is a required field.");
                valid = false;
            }
            if (entity.AppVersion == null || entity.AppVersion.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("AppVersion is a required field.");
                valid = false;
            }
            return valid;
        }

        public bool SaveUserDevice(UserDevice entity)
        {
            try
            {
                if (ValidateUserDevice(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = UserDevices.Where(n => n.UserID == entity.UserID).Where(n => n.DeviceID == entity.DeviceID).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.UserDevices.Add(entity);
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

        public bool DeleteUserDevice(int id)
        {
            try
            {
                var entity = db.UserDevices.Find(id);
                if (entity != null)
                {
                    db.UserDevices.Remove(entity);
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
