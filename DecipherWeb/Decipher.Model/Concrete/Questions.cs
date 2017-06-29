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
        #region Questions

        public IQueryable<Question> Questions
        {
            get
            {
                return db.Questions.AsQueryable();
            }
        }

        public bool ValidateQuestion(Question entity)
        {
            if (entity.Text == null || entity.Text.Trim().Length == 0)
            {
                ModelState.AddModelError("Text", "Text is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveQuestion(Question entity)
        {
            try
            {
                if (ValidateQuestion(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Questions.Find(entity.QuestionID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        entity.Ordinal = Questions.OrderByDescending(n => n.Ordinal).Select(n => n.Ordinal).FirstOrDefault() + 1;
                        db.Questions.Add(entity);
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

        public bool DeleteQuestion(int id)
        {
            try
            {
                var entity = db.Questions.Find(id);
                if (entity != null)
                {
                    db.Questions.Remove(entity);
                    var descriptors = Descriptors.Where(n => n.DescriptorType == "Question").Where(n => n.AssociatedID == id).ToList();
                    descriptors.ForEach(n => DeleteDescriptor(n.DescriptorID));
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

        public bool OrderQuestions(string data)
        {
            try
            {
                string[] arr = data.Split('~');
                var list = Questions.ToList();
                foreach (string item in arr)
                {
                    string[] itemarr = item.Split('-');
                    int ID = Convert.ToInt32(itemarr[0]);
                    int ordinal = Convert.ToInt32(itemarr[1]);
                    var question = list.Where(n => n.QuestionID == ID).FirstOrDefault();
                    question.Ordinal = ordinal;
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

        # endregion
    }
}
