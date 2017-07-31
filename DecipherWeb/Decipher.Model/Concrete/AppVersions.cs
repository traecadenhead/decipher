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
        #region AppVersions

        public IQueryable<AppVersion> AppVersions
        {
            get
            {
                return db.AppVersions.AsQueryable();
            }
        }

        public bool ValidateAppVersion(AppVersion entity)
        {
            bool valid = true;
            if (entity.VersionOrdinal == 0)
            {
                HttpContext.Current.Trace.Warn("VersionOrdinal is a required field.");
                valid = false;
            }
            if (entity.PlatformType == null || entity.PlatformType.Trim().Length == 0)
            {
                HttpContext.Current.Trace.Warn("Name is a required field.");
                valid = false;
            }
            return valid;
        }

        public bool SaveAppVersion(AppVersion entity)
        {
            try
            {
                if (ValidateAppVersion(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = AppVersions.Where(n => n.VersionNum == entity.VersionNum).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.AppVersions.Add(entity);
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

        public bool DeleteAppVersion(string versionNum)
        {
            try
            {
                var entity = AppVersions.Where(n => n.VersionNum == versionNum).FirstOrDefault();
                if (entity != null)
                {
                    db.AppVersions.Remove(entity);
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
