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
        #region Descriptors

        public IQueryable<Descriptor> Descriptors
        {
            get
            {
                return db.Descriptors.AsQueryable();
            }
        }

        public bool ValidateDescriptor(Descriptor entity)
        {
            if (entity.Name == null || entity.Name.Trim().Length == 0)
            {
                ModelState.AddModelError("Name", "Name is a required field.");
            }
            if (entity.DescriptorType == null || entity.DescriptorType.Trim().Length == 0)
            {
                ModelState.AddModelError("DescriptorType", "DescriptorType is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveDescriptor(Descriptor entity)
        {
            try
            {
                if (ValidateDescriptor(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Descriptors.Find(entity.DescriptorID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.Ordinal = Descriptors.Where(n => n.DescriptorType == entity.DescriptorType).Where(n => n.AssociatedID == entity.AssociatedID).OrderByDescending(n => n.Ordinal).Select(n => n.Ordinal).FirstOrDefault() + 1;
                        entity.DateCreated = DateTime.Now;
                        db.Descriptors.Add(entity);
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

        public bool DeleteDescriptor(int id)
        {
            try
            {
                var entity = db.Descriptors.Find(id);
                if (entity != null)
                {
                    var userDescriptors = UserDescriptors.Where(n => n.DescriptorID == id).ToList();
                    db.UserDescriptors.RemoveRange(userDescriptors);
                    var responses = ReviewResponses.Where(n => n.DescriptorID == id).ToList();
                    db.ReviewResponses.RemoveRange(responses);
                    db.Descriptors.Remove(entity);
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

        public bool OrderDescriptors(string data, string descriptorType, int? associatedID = null)
        {
            try
            {
                string[] arr = data.Split('~');
                var list = Descriptors.Where(n => n.DescriptorType == descriptorType).Where(n => n.AssociatedID == associatedID).ToList();
                foreach (string item in arr)
                {
                    string[] itemarr = item.Split('-');
                    int ID = Convert.ToInt32(itemarr[0]);
                    int ordinal = Convert.ToInt32(itemarr[1]);
                    var desc = list.Where(n => n.DescriptorID == ID).FirstOrDefault();
                    desc.Ordinal = ordinal;
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

        public List<Descriptor> TranslateDescriptors(List<Descriptor> descriptors, string language = "en")
        {
            List<Descriptor> list = new List<Descriptor>();
            try
            {
                if (language != GetConfig("DefaultLanguage"))
                {
                    var translations = Translations.Where(n => n.TranslationID.IndexOf("Descriptors.") == 0).ToList();
                    foreach (var desc in descriptors)
                    {
                        var trans = TranslateDescriptor(desc, language, translations);
                        if (trans != null)
                        {
                            list.Add(trans);
                        }
                    }
                }
                else
                {
                    return descriptors;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        public Descriptor TranslateDescriptor(Descriptor descriptor, string language = "en", List<Translation> translations = null)
        {
            try
            {
                if (translations == null)
                {
                    translations = Translations.Where(n => n.TranslationID == "Descriptors." + descriptor.DescriptorID + ".Name." + language).ToList();
                }
                var desc = new Descriptor { DescriptorID = descriptor.DescriptorID };
                desc = (Descriptor)UpdateObject(desc, descriptor, "DescriptorID");
                desc.Name = TranslateString("Descriptors", descriptor.DescriptorID.ToString(), "Name", descriptor.Name, language, translations);
                return desc;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return descriptor;
        }

        # endregion
    }
}
