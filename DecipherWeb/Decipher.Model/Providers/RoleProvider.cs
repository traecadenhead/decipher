using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Mvc;
using System.Web;
using Decipher.Model.Entities;
using Decipher.Model.Abstract;
using Decipher.Model.Concrete;

namespace Decipher.Model.Providers
{
    public class Role : RoleProvider
    {
        #region Non-Implemented Methods

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implemented Methods

        public override string[] GetRolesForUser(string username)
        {
            IDataRepository db = new Repository();
            return db.GetRolesForAdmin(username).ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] userRoles = GetRolesForUser(username);
            foreach (string userRole in userRoles)
            {
                if (userRole.ToLower() == roleName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
