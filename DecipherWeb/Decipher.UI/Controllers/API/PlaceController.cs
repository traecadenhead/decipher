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
    public class PlaceController : BaseController
    {
        [HttpPost]
        public PlaceResult Search(Search entity)
        {
            return db.SearchPlaces(entity);
        }

        [HttpGet]
        public GeoCoordinate GetLocation(string id)
        {
            return db.Geocode(id);
        }

        [HttpPut]
        public bool Save(Place entity)
        {
            return db.SavePlace(entity);
        }

        [HttpGet]
        public Place GetForReview(string id)
        {
            return db.GetPlaceForReview(id);
        }
    }
}
