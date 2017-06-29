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
        #region UserDescriptors

        public IQueryable<UserDescriptor> UserDescriptors
        {
            get
            {
                return db.UserDescriptors.AsQueryable();
            }
        }

        public bool ValidateUserDescriptor(UserDescriptor entity)
        {
            if (entity.UserID == 0)
            {
                ModelState.AddModelError("UserID", "User is a required field.");
            }
            if (entity.DescriptorID == 0)
            {
                ModelState.AddModelError("DescriptorID", "Descriptor is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveUserDescriptor(UserDescriptor entity)
        {
            try
            {
                if (ValidateUserDescriptor(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.UserDescriptors.Find(entity.UserDescriptorID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.UserDescriptors.Add(entity);
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

        public bool DeleteUserDescriptor(int id)
        {
            try
            {
                var entity = db.UserDescriptors.Find(id);
                if (entity != null)
                {
                    db.UserDescriptors.Remove(entity);
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
