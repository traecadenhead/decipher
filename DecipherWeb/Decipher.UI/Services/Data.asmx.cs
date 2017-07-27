using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Decipher.Model.Abstract;
using Decipher.Model.Concrete;
using Decipher.Model.Entities;

namespace Decipher.UI.Services
{
    /// <summary>
    /// Summary description for Data
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Data : System.Web.Services.WebService
    {

        //[WebMethod]
        //public bool AssociateZips()
        //{
        //    IDataRepository db = new Repository();
        //    return db.AssociateAllCityZips();
        //}

        //[WebMethod]
        //public bool AssociateCityZips(int cityID)
        //{
        //    IDataRepository db = new Repository();
        //    return db.AssociateCityZips(cityID);
        //}

        [WebMethod]
        public bool ImportCityPlaces(int cityID, string type = null)
        {
            IDataRepository db = new Repository();
            return db.ImportPlacesInCity(cityID, type);
        }

        [WebMethod]
        public bool ImportPlace(string placeID)
        {
            IDataRepository db = new Repository();
            return db.ImportPlace(placeID);
        }

        [WebMethod]
        public bool RecalculateReviewScores()
        {
            IDataRepository db = new Repository();
            return db.RecalculateReviewScores();
        }

        [WebMethod]
        public List<CustomString> GetCustomStrings()
        {
            IDataRepository db = new Repository();
            return db.GetCustomStrings();
        }
    }
}
