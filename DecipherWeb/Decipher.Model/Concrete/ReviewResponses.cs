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
        #region ReviewResponses

        public IQueryable<ReviewResponse> ReviewResponses
        {
            get
            {
                return db.ReviewResponses.AsQueryable();
            }
        }

        public bool ValidateReviewResponse(ReviewResponse entity)
        {
            if (entity.QuestionID == 0)
            {
                ModelState.AddModelError("QuestionID", "Question is a required field.");
            }
            if (entity.ReviewID == 0)
            {
                ModelState.AddModelError("ReviewID", "Review is a required field.");
            }
            if (entity.DescriptorID == 0)
            {
                ModelState.AddModelError("DescriptorID", "Descriptor is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveReviewResponse(ReviewResponse entity)
        {
            try
            {
                if (ValidateReviewResponse(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.ReviewResponses.Find(entity.ReviewResponseID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.ReviewResponses.Add(entity);
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

        public bool DeleteReviewResponse(int id)
        {
            try
            {
                var entity = db.ReviewResponses.Find(id);
                if (entity != null)
                {
                    db.ReviewResponses.Remove(entity);
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

        public int SaveReviewResponses(Review entity)
        {
            try
            {
                if(entity.ReviewID == 0)
                {
                    // create a review first
                    entity.VisitDate = DateTime.Now;
                    SaveReview(entity);
                }
                if (entity.ReviewID > 0)
                {
                    var origReview = Reviews.Where(n => n.ReviewID == entity.ReviewID).FirstOrDefault();
                    entity.Score = origReview.Score;
                    entity.DateCreated = origReview.DateCreated;
                    entity.VisitDate = origReview.VisitDate;
                    var descs = Descriptors.Where(n => n.DescriptorType == "Question").ToList();
                    foreach (var response in entity.Responses)
                    {
                        response.ReviewID = entity.ReviewID;
                        SaveReviewResponse(response);
                        var desc = descs.Where(n => n.DescriptorID == response.DescriptorID).FirstOrDefault();
                        HttpContext.Current.Trace.Warn("adding points");
                        entity.Score += desc.PointsSafe;
                        HttpContext.Current.Trace.Warn("added " + desc.PointsSafe);
                    }
                    // save the score
                    SaveReview(entity);
                    if (entity.Report && !entity.Reported)
                    {
                        var place = GetPlace(entity.PlaceID);
                        if (!String.IsNullOrEmpty(place.City.ReportEmail))
                        {
                            // send email 
                            if (SendReviewToCity(entity, place.City))
                            {
                                // mark as reported
                                entity.Reported = true;
                                SaveReview(entity);
                            }
                        }
                    }
                    return entity.ReviewID;
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return 0;
        }

        # endregion
    }
}
