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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Decipher.Model.Abstract;
using Decipher.Model.Entities;

namespace Decipher.Model.Concrete
{
    public partial class Repository : IDataRepository
    {
        #region Admins    

        public List<Admin> GetAdmins()
        {
            List<Admin> list = new List<Admin>();
            try
            {
                var xml = XElement.Load(HttpContext.Current.Server.MapPath(AdminXMLPath));
                foreach(XElement el in xml.Elements("Admin"))
                {
                    list.Add(ParseAdmin(el));
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        private Admin ParseAdmin(XElement el)
        {
            var admin = new Admin
            {
                Username = SafeXMLString(el.Element("Username")),
                Password = SafeXMLString(el.Element("Password")),
                Roles = new List<string>()
            };
            foreach (XElement r in el.Element("Roles").Elements("Role"))
            {
                admin.Roles.Add(SafeXMLString(r));
            }
            return admin;
        }

        public Admin AuthenticateAdmin(Admin login)
        {
            try
            {
                string hashPW = CreateSHAHash(login.Password);
                HttpContext.Current.Trace.Warn("username: " + login.Username);
                HttpContext.Current.Trace.Warn("hasPw: " + hashPW);
                var admins = GetAdmins();
                HttpContext.Current.Trace.Warn(admins.Count + " admins found");
                var admin = admins.Where(n => n.Username == login.Username).Where(n => n.Password == hashPW).FirstOrDefault();
                if(admin != null)
                {
                    return admin;
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return null;
        }

        private string AdminXMLPath
        {
            get
            {
                return "~/App_Data/Admins.xml";
            }
        }

        public List<string> GetRolesForAdmin(string username)
        {
            List<string> list = new List<string>();
            try
            {
                var admin = GetAdmins().Where(n => n.Username == username).FirstOrDefault();
                if(admin != null)
                {
                    list.AddRange(admin.Roles);
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());
            }
            return list;
        }

        #endregion
    }
}
