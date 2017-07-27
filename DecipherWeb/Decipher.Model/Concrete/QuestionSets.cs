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
        #region QuestionSets

        public IQueryable<QuestionSet> QuestionSets
        {
            get
            {
                return db.QuestionSets.AsQueryable();
            }
        }

        public bool ValidateQuestionSet(QuestionSet entity)
        {
            if (entity.Name == null || entity.Name.Trim().Length == 0)
            {
                ModelState.AddModelError("Name", "Name is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveQuestionSet(QuestionSet entity)
        {
            try
            {
                if (ValidateQuestionSet(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = QuestionSets.Where(n => n.QuestionSetID == entity.QuestionSetID).FirstOrDefault();
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.QuestionSets.Add(entity);
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

        public bool DeleteQuestionSet(int id)
        {
            try
            {
                var entity = db.QuestionSets.Find(id);
                if (entity != null)
                {
                    db.QuestionSets.Remove(entity);
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

        public List<SelectListItem> ListQuestionSets(string defaultValue = "", string emptyTitle = null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var items = QuestionSets.ToList();
            items.ForEach(n => dict.Add(n.QuestionSetID.ToString(), n.Name));
            return CreateList(dict, defaultValue, emptyTitle);
        }

        # endregion
    }
}
