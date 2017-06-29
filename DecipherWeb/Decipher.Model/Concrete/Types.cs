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
        #region Types

        public IQueryable<Decipher.Model.Entities.Type> Types
        {
            get
            {
                return db.Types.AsQueryable();
            }
        }

        public bool ValidateType(Decipher.Model.Entities.Type entity)
        {
            if (entity.TypeID == null || entity.TypeID.Trim().Length == 0)
            {
                ModelState.AddModelError("TypeID", "Type is a required field.");
            }
            if (entity.Name == null || entity.Name.Trim().Length == 0)
            {
                ModelState.AddModelError("Name", "Name is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveType(Decipher.Model.Entities.Type entity)
        {
            try
            {
                if (ValidateType(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Types.Find(entity.TypeID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Types.Add(entity);
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

        public bool DeleteType(int id)
        {
            try
            {
                var entity = db.Types.Find(id);
                if (entity != null)
                {
                    db.Types.Remove(entity);
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

        public List<SelectListItem> ListTypes(string defaultValue = "", string emptyText = "Select One")
        {
            var types = Types.Where(n => n.Active == true).OrderBy(n => n.Name).ToList();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            types.ForEach(n => dict.Add(n.TypeID, n.Name));
            return CreateList(dict, defaultValue, emptyText);
        }

        public bool SaveTypes(List<Entities.Type> list)
        {
            try
            {
                var types = Types.ToList();
                foreach(var item in list)
                {
                    var type = types.Where(n => n.TypeID == item.TypeID).FirstOrDefault();
                    if(type != null)
                    {
                        type.Active = item.Active;
                        type.Name = item.Name;
                        type.DateModified = DateTime.Now;
                    }                    
                }
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        # endregion
    }
}
