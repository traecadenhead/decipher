using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        #region Reviews

        public IQueryable<Review> Reviews
        {
            get
            {
                return db.Reviews.AsQueryable();
            }
        }

        public bool ValidateReview(Review entity)
        {
            if (entity.UserID == 0)
            {
                ModelState.AddModelError("UserID", "User is a required field.");
            }
            if (entity.PlaceID == null || entity.PlaceID.Trim().Length == 0)
            {
                ModelState.AddModelError("PlaceID", "Place is a required field.");
            }
            return ModelState.IsValid;
        }

        public bool SaveReview(Review entity)
        {
            try
            {
                if (ValidateReview(entity))
                {
                    entity.DateModified = DateTime.Now;
                    var original = db.Reviews.Find(entity.ReviewID);
                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entity.DateCreated = DateTime.Now;
                        db.Reviews.Add(entity);
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

        public bool DeleteReview(int id)
        {
            try
            {
                var entity = db.Reviews.Find(id);
                if (entity != null)
                {
                    db.Reviews.Remove(entity);
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

        public Review GetReview(int reviewID)
        {
            try
            {
                var entity = Reviews.Where(n => n.ReviewID == reviewID).Select(n => new { Review = n, Responses = n.ReviewResponses }).FirstOrDefault();
                if (entity != null)
                {
                    var questions = Questions.OrderBy(n => n.Ordinal).ToList();
                    HttpContext.Current.Trace.Warn(questions.Count + " questions");
                    var quesDescriptors = Descriptors.Where(n => n.DescriptorType == "Question").ToList();
                    HttpContext.Current.Trace.Warn(quesDescriptors.Count + " descriptors");
                    var review = entity.Review;
                    review.CurrentUser = GetUser(review.UserID);
                    HttpContext.Current.Trace.Warn("got current user");
                    review.CurrentPlace = GetPlace(review.PlaceID);
                    HttpContext.Current.Trace.Warn("got current place");
                    review.Questions = new List<Question>();
                    foreach (var q in questions)
                    {
                        var quesResponses = entity.Responses.Where(n => n.QuestionID == q.QuestionID).ToList();
                        HttpContext.Current.Trace.Warn(quesResponses.Count + " responses to question");
                        if (quesResponses.Count > 0)
                        {
                            q.Descriptors = new List<Descriptor>();
                            HttpContext.Current.Trace.Warn("make descriptors empty");
                            foreach (var r in quesResponses)
                            {
                                var desc = quesDescriptors.Where(n => n.DescriptorID == r.DescriptorID).FirstOrDefault();
                                if (desc != null)
                                {
                                    q.Descriptors.Add(desc);
                                }
                            }
                            q.Detail = quesResponses.Select(n => n.Detail).FirstOrDefault();
                            review.Questions.Add(q);
                        }
                    }
                    return review;
                }
                else
                {
                    HttpContext.Current.Trace.Warn("no entity");
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public bool SendReviewToCity(Review review, City city)
        {
            try
            {
                NameValueCollection r = new NameValueCollection();
                r.Add("ReportName", city.ReportName);
                r.Add("CityName", city.Name);
                r.Add("PlaceName", review.Place.Name);
                r.Add("Url", SiteAddress + "/app/review/detail/" + review.ReviewID);
                SendTemplateEmail(r, "Report.xml", city.ReportEmail, "An experience at " + review.Place.Name);
                return true;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return false;
        }

        public ReviewSummary GetReviewSummary(ReviewFilter filters)
        {
            try
            {
                // get all the reviews with their data;
                List<ReviewContainer> allReviews = new List<ReviewContainer>();
                if(filters.Places != null && filters.Places.Count > 0)
                {
                    // specific places were selected. Add each selected place
                    foreach(var placeItem in filters.Places)
                    {
                        allReviews.AddRange(Reviews.Where(n => n.PlaceID == placeItem.PlaceID).Select(n => new ReviewContainer { Review = n, Responses = n.ReviewResponses, User = n.User, UserDescriptors = n.User.UserDescriptors }).ToList());
                    }
                }
                else if(filters.Zips != null && filters.Zips.Count > 0)
                {
                    // zips were selected. Add reviews for those zips
                    foreach(var zipItem in filters.Zips)
                    {
                        allReviews.AddRange(Reviews.Where(n => n.Place.Zip == zipItem.Zip1).Select(n => new ReviewContainer { Review = n, Responses = n.ReviewResponses, User = n.User, UserDescriptors = n.User.UserDescriptors }).ToList());
                    }
                }
                else if (filters.Cities != null && filters.Cities.Count > 0)
                {
                    // city was selected. Add review for selected cities
                    foreach(var cityItem in filters.Cities)
                    {
                        allReviews.AddRange(Reviews.Where(n => n.Place.Zip1.CityID == cityItem.CityID).Select(n => new ReviewContainer { Review = n, Responses = n.ReviewResponses, User = n.User, UserDescriptors = n.User.UserDescriptors }).ToList());
                    }
                }
                var allDescriptors = Descriptors.Where(n => n.DescriptorType == "Question").ToList();
                var allQuestions = Questions.OrderBy(n => n.Ordinal).ToList();
                List<int> requiredDescs = new List<int>();
                filters.Descriptors.Where(n => n.Selected == true).ToList().ForEach(n => requiredDescs.Add(n.DescriptorID));
                // first get reviews that apply to these filters
                List<ReviewContainer> applyReviews = new List<ReviewContainer>();
                if (requiredDescs.Count == 0)
                {
                    // all reviews apply
                    applyReviews.AddRange(allReviews);
                }
                else
                {
                    // only reviews that have users containing selected descriptors apply
                    foreach (var review in allReviews)
                    {
                        bool isApply = true;
                        foreach (int descID in requiredDescs)
                        {
                            if (review.UserDescriptors.Where(n => n.DescriptorID == descID).FirstOrDefault() == null)
                            {
                                isApply = false;
                            }
                        }
                        if (isApply)
                        {
                            applyReviews.Add(review);
                        }
                    }
                }
                
                var summary = new ReviewSummary
                {
                    Name = "Review Summary",
                    Description = String.Empty,
                    Questions = new List<Question>(),
                    UserDescriptors = new List<Descriptor>(),
                    Reviews = applyReviews.Select(n => n.Review).OrderByDescending(n => n.DateCreated).ToList()
                };
                // specific place requested so give the name of the place
                if(filters.Places != null && filters.Places.Count == 1)
                {
                    var place = GetPlace(filters.Places.Select(n => n.PlaceID).FirstOrDefault());
                    summary.Name = place.Name;
                    summary.Description = place.Address;
                }
                foreach (var desc in Descriptors.Where(n => n.DescriptorType == "Profile").OrderBy(n => n.Ordinal).ToList())
                {
                    if (filters.Descriptors.Where(n => n.DescriptorID == desc.DescriptorID).Where(n => n.Selected).FirstOrDefault() != null)
                    {
                        desc.Selected = true;
                    }
                    summary.UserDescriptors.Add(desc);
                }
                foreach (var question in allQuestions)
                {
                    question.Details = new List<string>();
                    question.Descriptors = new List<Descriptor>();
                    foreach (var desc in allDescriptors.Where(n => n.AssociatedID.Value == question.QuestionID).ToList())
                    {
                        desc.NumSelected = 0;
                        desc.TotalSelected = 0;
                        foreach (var review in applyReviews)
                        {
                            if (review.Responses.Where(n => n.DescriptorID == desc.DescriptorID).FirstOrDefault() != null)
                            {
                                desc.NumSelected++;
                            }
                            var selections = review.Responses.Where(n => n.QuestionID == question.QuestionID).ToList();
                            if (selections.Count > 0)
                            {
                                var selection = selections.FirstOrDefault();
                                if (!String.IsNullOrEmpty(selection.Detail) && !question.Details.Contains(selection.Detail))
                                {
                                    question.Details.Add(selection.Detail);
                                }
                                desc.TotalSelected += selections.Count;
                            }
                        }
                        if (desc.NumSelected > 0)
                        {
                            question.Descriptors.Add(desc);
                        }
                    }
                    if (question.Descriptors.Count > 0)
                    {
                        summary.Questions.Add(question);
                    }
                }
                return summary;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public ReviewFilter GetReviewFilters(ReviewFilter entity)
        {
            try
            {
                if(entity.Zips != null && entity.Zips.Where(n => n.Selected == true).Count() > 0)
                {
                    // get the places for the zip
                    List<Place> chosenPlaces = new List<Place>();
                    if(entity.Places != null)
                    {
                        chosenPlaces.AddRange(entity.Places.Where(n => n.Selected == true).ToList());
                    }
                    entity.Places = new List<Place>();
                    foreach(var zip in entity.Zips.Where(n => n.Selected == true).ToList())
                    {
                        entity.Places.AddRange(Places.Where(n => n.Zip == zip.Zip1).ToList());
                    }
                    foreach(var chosenPlace in chosenPlaces)
                    {
                        var selPlace = entity.Places.Where(n => n.PlaceID == chosenPlace.PlaceID).FirstOrDefault();
                        if(selPlace != null)
                        {
                            selPlace.Selected = true;
                        }
                    }
                }
                if(entity.Cities != null && entity.Cities.Where(n => n.Selected == true).Count() > 0)
                {
                    // get the zips for the city
                    List<Zip> chosenZips = new List<Zip>();
                    if (entity.Zips != null)
                    {
                        chosenZips.AddRange(entity.Zips.Where(n => n.Selected == true).ToList());
                    }
                    entity.Zips = new List<Zip>();
                    foreach(var city in entity.Cities.Where(n => n.Selected == true).ToList())
                    {
                        entity.Zips.AddRange(Zips.Where(n => n.CityID == city.CityID).ToList());
                    }
                    foreach (var chosenZip in chosenZips)
                    {
                        var selZip = entity.Zips.Where(n => n.Zip1 == chosenZip.Zip1).FirstOrDefault();
                        if (selZip != null)
                        {
                            selZip.Selected = true;
                        }
                    }
                }
                // Load the cities
                List<City> chosenCities = new List<City>();
                if(entity.Cities != null)
                {
                    chosenCities.AddRange(entity.Cities.Where(n => n.Selected == true).ToList());
                }
                entity.Cities = Cities.Where(n => n.ZipsAssociated).OrderBy(n => n.Name).ToList();
                foreach (var chosenCity in chosenCities)
                {
                    var selCity = entity.Cities.Where(n => n.CityID == chosenCity.CityID).FirstOrDefault();
                    if (selCity != null)
                    {
                        selCity.Selected = true;
                    }
                }
                // Load the identifiers
                List<Descriptor> chosenDescs = new List<Descriptor>();
                if(entity.Descriptors != null)
                {
                    chosenDescs.AddRange(entity.Descriptors.Where(n => n.Selected == true).ToList());
                }
                entity.Descriptors = Descriptors.Where(n => n.DescriptorType == "Profile").OrderBy(n => n.Ordinal).ToList();
                foreach (var chosenDesc in chosenDescs)
                {
                    var selDesc = entity.Descriptors.Where(n => n.DescriptorID == chosenDesc.DescriptorID).FirstOrDefault();
                    if (selDesc != null)
                    {
                        selDesc.Selected = true;
                    }
                }
                return entity;
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        public bool RecalculateReviewScores()
        {
            try
            {
                var reviews = Reviews.Select(n => new { Review = n, Responses = n.ReviewResponses }).ToList();
                var descs = Descriptors.Where(n => n.DescriptorType == "Question").ToList();
                foreach(var review in reviews)
                {
                    var entity = review.Review;
                    entity.Score = 0;
                    foreach(var resp in review.Responses)
                    {
                        var desc = descs.Where(n => n.DescriptorID == resp.DescriptorID).FirstOrDefault();
                        entity.Score += desc.PointsSafe;
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
