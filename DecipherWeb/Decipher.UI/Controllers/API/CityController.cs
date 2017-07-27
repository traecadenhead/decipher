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
    public class CityController : BaseController
    {
        [HttpPost]
        public City Determine(GeoCoordinate entity)
        {
            return db.DetermineNearestCity(entity);
        }

        [HttpGet]
        public City GetDefault()
        {
            return db.GetDefaultCity();
        }
    }
}
