using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Decipher.Model.Entities;
using System.Device.Location;

namespace Decipher.UI.Controllers.API
{
    public class ReviewController : BaseController
    {
        [HttpPut]
        public int Save(Review entity)
        {
            return db.SaveReviewResponses(entity);
        }

        [HttpGet]
        public Review Get(int id)
        {
            return db.GetReview(id);
        }

        [HttpPost]
        public ReviewSummary Summary(ReviewFilter entity)
        {
            return db.GetReviewSummary(entity);
        }

        [HttpPost]
        public ReviewFilter GetFilters(ReviewFilter entity)
        {
            return db.GetReviewFilters(entity);
        }
    }
}
