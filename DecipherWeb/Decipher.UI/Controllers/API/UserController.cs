using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using Decipher.Model.Entities;

namespace Decipher.UI.Controllers.API
{
    public class UserController : BaseController
    {
        [HttpGet]
        public User Get(int? id = null, string language = "en")
        {
            return db.GetIdentify(id.GetValueOrDefault(), language);
        }

        [HttpPut]
        public int Save(User entity)
        {
            return db.SaveIdentify(entity);
        }

        [HttpPut]
        public bool SaveLanguage(User entity)
        {
            return db.SaveUser(entity);
        }

        [HttpPut]
        public bool SaveDevice(UserDevice entity)
        {
            var version = db.AppVersions.Where(n => n.VersionNum == entity.AppVersion).FirstOrDefault();
            if (version == null)
            {
                entity.AppVersion = db.AppVersions.Where(n => n.PlatformType == entity.DeviceType).OrderByDescending(n => n.VersionOrdinal).Select(n => n.VersionNum).FirstOrDefault();
            }
            return db.SaveUserDevice(entity);
        }
    }
}
