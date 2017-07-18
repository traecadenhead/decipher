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
        #region Users

        public IQueryable<User> Users
        {
            get
            {
                return db.Users.AsQueryable();
            }
        }

        public bool ValidateUser(User entity)
        {
            return ModelState.IsValid;
        }

        public bool SaveUser(User entity)
        {
            try
            {
                if (ValidateUser(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Users.Where(n => n.UserID == entity.UserID).FirstOrDefault();
                    if (original != null)
                    {
                        entity.DateCreated = original.DateCreated;
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Users.Add(entity);
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

        public bool DeleteUser(int id)
        {
            try
            {
                var entity = db.Users.Find(id);
                if (entity != null)
                {
                    db.Users.Remove(entity);
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

        public User GetIdentify(int userID, string language = "en")
        {
            try
            {
                var user = Users.Where(n => n.UserID == userID).FirstOrDefault();
                if (user == null)
                {
                    user = new User { UserID = 0 };
                }
                var existingDescriptors = UserDescriptors.Where(n => n.UserID == userID).ToList();
                user.Descriptors = TranslateDescriptors(Descriptors.Where(n => n.DescriptorType == "Profile").OrderBy(n => n.Name).ToList(), language);
                foreach(var d in user.Descriptors)
                {
                    if(existingDescriptors.Where(n => n.DescriptorID == d.DescriptorID).FirstOrDefault() != null)
                    {
                        d.Selected = true;
                    }
                    else
                    {
                        d.Selected = false;
                    }
                }
                return user;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public int SaveIdentify(User entity)
        {
            try
            {
                List<Descriptor> descriptors = entity.Descriptors;
                int userID = entity.UserID;
                if(userID == 0)
                {
                    // get a new user
                    var user = new User { UserID = 0 };
                    if (SaveUser(user))
                    {
                        userID = user.UserID;
                    }
                }
                HttpContext.Current.Trace.Warn("UserID: " + userID);
                if(userID > 0)
                {
                    var existingDescriptors = UserDescriptors.Where(n => n.UserID == userID).ToList();
                    // save the selected descriptors
                    foreach(var sel in descriptors.Where(n => n.Selected == true).ToList())
                    {
                        HttpContext.Current.Trace.Warn("looking to save " + sel.DescriptorID);
                        if(existingDescriptors.Where(n => n.DescriptorID == sel.DescriptorID).FirstOrDefault() == null)
                        {
                            HttpContext.Current.Trace.Warn("doing save " + sel.DescriptorID);
                            // this is a newly selected descriptor so it needs to save
                            SaveUserDescriptor(new UserDescriptor
                            {
                                DescriptorID = sel.DescriptorID,
                                UserID = userID
                            });
                        }
                    }
                    // check the existing descriptors to see if any previously selected are no longer selected
                    foreach(var exist in existingDescriptors)
                    {
                        HttpContext.Current.Trace.Warn("descriptors in list: " + descriptors.Count);
                        if(descriptors.Where(n => n.DescriptorID == exist.DescriptorID).Where(n => n.Selected == false).FirstOrDefault() == null)
                        {
                            HttpContext.Current.Trace.Warn("deleting " + exist.DescriptorID);
                            // delete this user descriptor
                            DeleteUserDescriptor(exist.UserDescriptorID);
                        }
                    }
                }
                return userID;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return 0;
        }

        public User GetUser(int userID)
        {
            try
            {
                var entity = Users.Where(n => n.UserID == userID).Select(n => new { Descriptors = n.UserDescriptors, User = n }).FirstOrDefault();
                var profileDescriptors = Descriptors.Where(n => n.DescriptorType == "Profile").ToList();
                User user = entity.User;
                user.Descriptors = new List<Descriptor>();
                foreach(var d in entity.Descriptors)
                {
                    var desc = profileDescriptors.Where(n => n.DescriptorID == d.DescriptorID).FirstOrDefault();
                    if(desc != null)
                    {
                        user.Descriptors.Add(desc);
                    }
                }
                return user;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        # endregion
    }
}
