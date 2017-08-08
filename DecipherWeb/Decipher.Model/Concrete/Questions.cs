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

        public bool OrderQuestions(int questionSetID, string data)
        {
            try
            {
                string[] arr = data.Split('~');
                var list = Questions.Where(n => n.QuestionSetID == questionSetID).ToList();
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

        public List<Question> TranslateQuestions(List<Question> questions, string language = "en", List<Translation> translations = null)
        {
            try
            {
                List<Question> list = new List<Question>();
                if (translations == null)
                {
                    translations = Translations.Where(n => n.TranslationID.IndexOf("Questions.") == 0 || n.TranslationID.IndexOf("Descriptors.") == 0).Where(n => n.LanguageID == language).ToList();
                }
                foreach(var q in questions)
                {
                    var question = new Question { QuestionID = q.QuestionID };
                    question = (Question)UpdateObject(question, q, "QuestionID");
                    question.Text = TranslateString("Questions", q.QuestionID.ToString(), "Text", q.Text, language, translations);
                    question.Descriptors = new List<Descriptor>();
                    if (q.Descriptors != null && q.Descriptors.Count > 0)
                    {
                        foreach (var d in q.Descriptors)
                        {
                            var desc = new Descriptor { DescriptorID = d.DescriptorID };
                            desc = (Descriptor)UpdateObject(desc, d, "DescriptorID");
                            desc.Name = TranslateString("Descriptors", d.DescriptorID.ToString(), "Name", d.Name, language, translations);
                            question.Descriptors.Add(desc);
                        }
                    }
                    list.Add(question);
                }
                return list;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return questions;
        }

        # endregion
    }
}
