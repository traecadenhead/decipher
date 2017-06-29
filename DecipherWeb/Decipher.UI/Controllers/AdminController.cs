using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Decipher.Model.Entities;

namespace Decipher.UI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private Decipher.Model.Abstract.IDataRepository db;

        public AdminController()
        {
            db = new Decipher.Model.Concrete.Repository(ModelState);
        }

        [OverrideAuthorization]
        public ActionResult SignIn()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OverrideAuthorization]
        public ActionResult SignIn(Admin login)
        {
            if(Membership.ValidateUser(login.Username, login.Password))
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Messages"] = db.WarningMessage("Sorry, we couldn't log you in with the info you provided.");
                return RedirectToAction("SignIn");
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cities()
        {
            return View(db.Cities.OrderBy(n => n.Name).ToList());
        }

        public ActionResult CityEdit(int id)
        {
            var entity = db.Cities.Where(n => n.CityID == id).FirstOrDefault();
            if(entity == null)
            {
                entity = new City { Radius = 30 };
            }
            return View(entity);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CityEdit(City entity)
        {
            if (db.SaveCity(entity))
            {
                TempData["Messages"] = db.InfoMessage("The city has been saved successfully.");
                return RedirectToAction("Cities");
            }
            else
            {
                ViewBag.Messages = db.WarningMessage("Sorry, your changes to the city could not be saved.");
                return View(entity);
            }
        }

        public ActionResult Questions()
        {
            return View(db.Questions.OrderBy(n => n.Ordinal).ToList());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuestionsOrder(string data)
        {
            return Content(db.OrderQuestions(data).ToString());
        }
        
        public ActionResult QuestionEdit(int id)
        {
            var question = db.Questions.Where(n => n.QuestionID == id).FirstOrDefault();
            if(question == null)
            {
                question = new Question { QuestionID = 0 };
            }
            else
            {
                question.Descriptors = db.Descriptors.Where(n => n.DescriptorType == "Question").Where(n => n.AssociatedID == id).OrderBy(n => n.Ordinal).ThenBy(n => n.Name).ToList();
            }
            return View(question);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuestionEdit(Question question)
        {
            if (db.SaveQuestion(question))
            {
                TempData["Messages"] = db.InfoMessage("The question was saved successfully.");
                return RedirectToAction("QuestionEdit", new { id = question.QuestionID });
            }
            else
            {
                ViewBag.Messages = db.WarningMessage("A problem occurred while saving the question.");
                return View(question);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuestionRemove(int questionID)
        {
            if (db.DeleteQuestion(questionID))
            {
                TempData["Messages"] = db.InfoMessage("The question was removed.");
            }
            else
            {
                TempData["Messages"] = db.WarningMessage("A problem occured while removing the question.");
            }
            return RedirectToAction("Questions");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DescriptorAdd(Descriptor entity, string returnAction = "QuestionEdit")
        {
            if (db.SaveDescriptor(entity))
            {
                TempData["Messages"] = db.InfoMessage("The item was added.");
            }
            else
            {
                TempData["Messages"] = db.WarningMessage("A problem occurred adding the item.");
            }
            return RedirectToAction(returnAction, new { id = entity.AssociatedID });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DescriptorRemove(int descriptorID, int? associatedID = null, string returnAction = "QuestionEdit")
        {
            if (db.DeleteDescriptor(descriptorID))
            {
                TempData["Messages"] = db.InfoMessage("The response was removed.");
            }
            else
            {
                TempData["Messages"] = db.WarningMessage("A problem occurred removing the response.");
            }
            return RedirectToAction(returnAction, new { id = associatedID });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DescriptorsOrder(string data, string type, int? associatedID = null)
        {
            return Content(db.OrderDescriptors(data, type, associatedID).ToString());
        }

        public ActionResult Identifiers()
        {
            return View(db.Descriptors.Where(n => n.DescriptorType == "Profile").OrderBy(n => n.Ordinal).ThenBy(n => n.Name).ToList());
        }

        public ActionResult Pages()
        {
            return View(db.Pages.OrderBy(n => n.Ordinal).ToList());
        }

        public ActionResult PageEdit(int id)
        {
            Page entity = db.Pages.Where(n => n.PageID == id).FirstOrDefault();
            if(entity == null)
            {
                entity = new Page { Title = String.Empty };
            }
            return View(entity);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult PageEdit(Page entity)
        {
            if (db.SavePage(entity))
            {
                TempData["Messages"] = db.InfoMessage("The page was saved successfully.");
                return RedirectToAction("Pages");
            }
            else
            {
                ViewBag.Messages = db.WarningMessage("A problem occurred while saving the page.");
                return View(entity);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PageRemove(int pageID)
        {
            if (db.DeletePage(pageID))
            {
                TempData["Messages"] = db.InfoMessage("The page was removed.");
            }
            else
            {
                TempData["Messages"] = db.WarningMessage("A problem occured while removing the page.");
            }
            return RedirectToAction("Pages");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PagesOrder(string data)
        {
            return Content(db.OrderPages(data).ToString());
        }

        public ActionResult Types()
        {
            return View(db.Types.OrderBy(n => n.Name).ToList());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveTypes(List<Decipher.Model.Entities.Type> types)
        {
            return Content(db.SaveTypes(types).ToString());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveDescriptorPoints(Descriptor entity)
        {
            var original = db.Descriptors.Where(n => n.DescriptorID == entity.DescriptorID).FirstOrDefault();
            if(original != null)
            {
                original.Points = entity.Points;
                return Content(db.SaveDescriptor(original).ToString());
            }
            return Content(false.ToString());
        }

        public ActionResult Hash(string id)
        {
            return Content(db.CreateSHAHash(id));
        }
    }
}