﻿using System;
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

        [HttpPut]
        public bool SaveSubmit(Review entity)
        {
            return db.SubmitReview(entity, entity.Language);
        }

        [HttpGet]
        public Review Get(int id, string language = "en")
        {
            return db.GetReview(id, language);
        }

        [HttpGet]
        public Review GetForSubmit(int id, string language = "en")
        {
            return db.GetReviewForSubmission(id, language);
        }

        [HttpPost]
        public ReviewSummary Summary(ReviewFilter entity)
        {
            return db.GetReviewSummary(entity, entity.Language);
        }

        [HttpPost]
        public ReviewFilter GetFilters(ReviewFilter entity)
        {
            return db.GetReviewFilters(entity, entity.Language);
        }
    }
}
