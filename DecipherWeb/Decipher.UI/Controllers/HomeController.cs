using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Decipher.Model.Entities;

namespace Decipher.UI.Controllers
{
    public class HomeController : Controller
    {
        private Decipher.Model.Abstract.IDataRepository db;

        public HomeController()
        {
            db = new Decipher.Model.Concrete.Repository(ModelState);
        }

        public ActionResult App()
        {
            return View();
        }

        #region jQuery-driven app to be replaced

        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("App");
        }

        public ActionResult Identify(int id)
        {
            return View(db.GetIdentify(id));
        }

        [HttpPost]
        public ContentResult SaveIdentify(User entity)
        {
            return Content(db.SaveIdentify(entity).ToString());
        }

        public ActionResult Find()
        {
            ViewBag.Diversity = db.ListDiversityIndexes();
            ViewBag.TypeID = db.ListTypes();
            ViewBag.Distance = db.ListPlaceDistances();
            return View();
        }

        public ActionResult FindResults(Search entity)
        {
            // legacy find
            var results = db.SearchPlaces(entity);
            return View(results.Results);
        }

        public ActionResult Review(string id)
        {
            return View(db.GetPlaceForReview(id));
        }

        [HttpPost]
        public ActionResult SaveReviewResponses(Review entity)
        {
            return Content(db.SaveReviewResponses(entity).ToString());
        }

        public ActionResult Thanks()
        {
            return View();
        }

        public ActionResult Intro()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        #endregion
    }
}