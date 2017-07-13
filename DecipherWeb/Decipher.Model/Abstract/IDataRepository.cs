using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Device.Location;
using Decipher.Model.Entities;

namespace Decipher.Model.Abstract
{
    public interface IDataRepository
    {
        #region Admins
        Admin AuthenticateAdmin(Admin login);
        List<string> GetRolesForAdmin(string username);
        #endregion

        #region Cities
        IQueryable<City> Cities { get; }
        bool SaveCity(City entity);
        bool DeleteCity(int id);
        bool AssociateAllCityZips();
        bool AssociateCityZips(int cityID);
        #endregion

        #region CustomStrings
        List<CustomString> GetCustomStrings(string language = "en");
        CustomString GetCustomString(string id);
        #endregion

        #region Demographics
        IQueryable<Demographic> Demographics { get; }
        bool SaveDemographic(Demographic entity);
        bool DeleteDemographic(int id);
        #endregion

        #region Descriptors
        IQueryable<Descriptor> Descriptors { get; }
        bool SaveDescriptor(Descriptor entity);
        bool DeleteDescriptor(int id);
        bool OrderDescriptors(string data, string descriptorType, int? associatedID = null);
        #endregion

        #region Helpers
        string InfoMessage(string message);
        string WarningMessage(string message);
        string ErrorMessage(string message);
        GeoCoordinate Geocode(string address);
        string CreateSHAHash(string phrase);
        #endregion

        #region Languages
        IQueryable<Language> Languages { get; }
        bool SaveLanguage(Language entity);
        bool DeleteLanguage(int id);
        #endregion

        #region Lists
        ListHolder GetListHolder();
        #endregion

        #region Pages
        IQueryable<Page> Pages { get; }
        bool SavePage(Page entity);
        bool DeletePage(int id);
        bool OrderPages(string data);
        List<Page> GetPages(string language = "en");
        Page GetPage(int pageID, string language = "en");
        #endregion

        #region Places
        IQueryable<Place> Places { get; }
        bool SavePlace(Place entity);
        bool DeletePlace(int id);
        List<SelectListItem> ListPlaceDistances(string defaultValue = "", string emptyText = "");
        PlaceResult SearchPlaces(Search entity);
        PlaceResult NearbyPlaces(Search entity);
        Place GetPlaceForReview(string placeID);
        bool ImportPlace(string placeID);
        bool ImportPlacesInCity(int cityID, string type = null);
        Place GetPlace(string placeID);
        #endregion

        #region PlaceTypes
        IQueryable<PlaceType> PlaceTypes { get; }
        bool SavePlaceType(PlaceType entity);
        bool DeletePlaceType(int id);
        #endregion

        #region Questions
        IQueryable<Question> Questions { get; }
        bool SaveQuestion(Question entity);
        bool DeleteQuestion(int id);
        bool OrderQuestions(string data);
        #endregion

        #region ReviewResponses
        IQueryable<ReviewResponse> ReviewResponses { get; }
        bool SaveReviewResponse(ReviewResponse entity);
        bool DeleteReviewResponse(int id);
        int SaveReviewResponses(Review entity);
        #endregion

        #region Reviews
        IQueryable<Review> Reviews { get; }
        bool SaveReview(Review entity);
        bool DeleteReview(int id);
        Review GetReview(int reviewID);
        ReviewSummary GetReviewSummary(ReviewFilter filters);
        ReviewFilter GetReviewFilters(ReviewFilter entity);
        bool RecalculateReviewScores();
        #endregion

        #region Translations
        IQueryable<Translation> Translations { get; }
        bool SaveTranslation(Translation entity);
        bool DeleteTranslation(int id);
        #endregion

        #region Types
        IQueryable<Decipher.Model.Entities.Type> Types { get; }
        bool SaveType(Decipher.Model.Entities.Type entity);
        bool DeleteType(int id);
        List<SelectListItem> ListTypes(string defaultValue = "", string emptyText = "");
        bool SaveTypes(List<Entities.Type> list);
        #endregion

        #region UserDescriptors
        IQueryable<UserDescriptor> UserDescriptors { get; }
        bool SaveUserDescriptor(UserDescriptor entity);
        bool DeleteUserDescriptor(int id);
        #endregion

        #region Users
        IQueryable<User> Users { get; }
        bool SaveUser(User entity);
        bool DeleteUser(int id);
        User GetIdentify(int userID, string language = "en");
        int SaveIdentify(User entity);
        User GetUser(int userID);
        #endregion

        #region ZipDemographics
        IQueryable<ZipDemographic> ZipDemographics { get; }
        bool SaveZipDemographic(ZipDemographic entity);
        bool DeleteZipDemographic(int id);
        #endregion

        #region Zips
        IQueryable<Zip> Zips { get; }
        bool SaveZip(Zip entity);
        bool DeleteZip(int id);
        List<SelectListItem> ListDiversityIndexes(string defaultValue = "", string emptyText = "Any");
        #endregion

        #region ZipTypes
        IQueryable<ZipType> ZipTypes { get; }
        bool SaveZipType(ZipType entity);
        bool DeleteZipType(int id);
        #endregion
    }
}
