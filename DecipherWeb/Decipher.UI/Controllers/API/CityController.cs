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
        public City Determine(Coords entity)
        {
            return db.DetermineNearestCity(entity.Coordinates, entity.Language);
        }

        [HttpGet]
        public City GetDefault(string language = "en")
        {
            return db.GetDefaultCity(language);
        }
    }
}
