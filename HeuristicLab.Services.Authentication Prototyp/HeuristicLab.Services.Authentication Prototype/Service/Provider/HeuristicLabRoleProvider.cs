using System;
using System.Web.Security;
using System.Linq;
using Persistence;
using System.Text;
using System.Collections.Generic;

namespace Service.Provider {
  class HeuristicLabRoleProvider : RoleProvider {
    protected string applicationName;

    public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
      if (usernames != null && roleNames != null) {
        Persistence.DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext();
        List<HeuristicLabUser> users = db.HeuristicLabUsers.Where(u => usernames.Contains(u.UserName)).ToList<HeuristicLabUser>();
        List<HeuristicLabRole> roles = db.HeuristicLabRole.Where(r => roleNames.Contains(r.RoleName)).ToList<HeuristicLabRole>();
        foreach (HeuristicLabUser user in users) {
          foreach (HeuristicLabRole role in roles) {
            HeuristicLabUserRole userRole = new HeuristicLabUserRole();
            userRole.HeuristicLabRole = role;
            userRole.HeuristicLabUser = user;
            db.HeuristicLabUserRole.InsertOnSubmit(userRole);
          }
        }
        db.SubmitChanges();
        db.Dispose();
      }
    }

    public override string ApplicationName {
      get {
        return this.applicationName;
      }
      set {
        this.applicationName = value;
      }
    }

    public override void CreateRole(string roleName) {
      CreateRole(roleName, false);

    }

    public virtual void CreateRole(String roleName, bool isPermission) {
      if (roleName != null) {
        Persistence.DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext();
        HeuristicLabRole role = new HeuristicLabRole();
        role.RoleName = roleName;
        role.IsPermission = isPermission;
        db.HeuristicLabRole.InsertOnSubmit(role);
        db.SubmitChanges();
        db.Dispose();
      }
    }

    //
    // RoleProvider.DeleteRole
    //
    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
      String[] roleArray = new String[1];
      roleArray[0] = roleName;
      Persistence.DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext();

      if (!RoleExists(roleName)) {
        return false;
        //throw new Exception("Role does not exist.");
      }

      if (throwOnPopulatedRole && GetUsersInRole(roleName).Length > 0) {
        return false;
        //throw new ProviderException("Cannot delete a populated role.");
      }

      RemoveUsersFromRoles(GetUsersInRole(roleName), roleArray);

      Persistence.HeuristicLabRole role = db.HeuristicLabRole.Single(r => r.RoleName == roleName);
      db.HeuristicLabRole.DeleteOnSubmit(role);
      db.Dispose();

      return true;
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
      List<string> returnValue = new List<string>();
      Persistence.DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext();
      if (roleName != null && db.HeuristicLabRole.Count(r => r.RoleName == roleName) == 1) {
        HeuristicLabRole role = db.HeuristicLabRole.Single(r => r.RoleName == roleName);
        foreach (HeuristicLabUserRole userRole in role.HeuristicLabUserRoles) {
          if (userRole.HeuristicLabUser.UserName.Contains(usernameToMatch)) {
            returnValue.Add(userRole.HeuristicLabUser.UserName);
          }
        }
      }
      db.Dispose();
      return returnValue.ToArray();
    }

    public override string[] GetAllRoles() {
      List<string> roleList = new List<string>();

      Persistence.DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext();
      List<HeuristicLabRole> roles = new List<HeuristicLabRole>(db.HeuristicLabRole.Select(r => r));
      foreach (HeuristicLabRole r in roles) {
        roleList.Add(r.RoleName);
      }
      db.Dispose();

      return roleList.ToArray();
    }

    public override string[] GetRolesForUser(string username) {
      List<string> roleList = new List<string>();
      DataClassesDataContext context = DatabaseUtil.createDataClassesDataContext();
      if (username != null && context.HeuristicLabUsers.Count(u => u.UserName == username) > 0) {

        Persistence.HeuristicLabUser user = context.HeuristicLabUsers.Single(u => u.UserName == username);
        foreach (Persistence.HeuristicLabUserRole userRole in user.HeuristicLabUserRoles) {
          roleList.Add(userRole.HeuristicLabRole.RoleName);
        }
      }
      context.Dispose();
      return roleList.ToArray();
    }

    public override string[] GetUsersInRole(string roleName) {
      List<string> userList = new List<string>();
      DataClassesDataContext context = DatabaseUtil.createDataClassesDataContext();
      if (roleName != null && context.HeuristicLabRole.Count(r => r.RoleName == roleName) > 0) {

        Persistence.HeuristicLabRole role = context.HeuristicLabRole.Single(r => r.RoleName == roleName);
        foreach (Persistence.HeuristicLabUserRole roleUser in role.HeuristicLabUserRoles) {
          if (!userList.Contains(roleUser.HeuristicLabUser.UserName))
            userList.Add(roleUser.HeuristicLabUser.UserName);
        }
      }
      context.Dispose();
      return userList.ToArray();
    }

    public override bool IsUserInRole(string username, string roleName) {
      bool returnValue = false;
      DataClassesDataContext context = DatabaseUtil.createDataClassesDataContext();
      if (username != null && roleName != null) {
        returnValue = (context.HeuristicLabUserRole.Count(ur => ur.HeuristicLabRole.RoleName == roleName && ur.HeuristicLabUser.UserName == username) > 0);
      }
      context.Dispose();
      return returnValue;
    }

    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {

      DataClassesDataContext context = DatabaseUtil.createDataClassesDataContext();

      foreach (string userName in usernames) {
        if (userName != null && context.HeuristicLabUsers.Count(u => u.UserName == userName) > 0) {
          Persistence.HeuristicLabUser user = context.HeuristicLabUsers.Single(u => u.UserName == userName);
          foreach (Persistence.HeuristicLabUserRole userRole in user.HeuristicLabUserRoles) {
            if (roleNames.Contains(userRole.HeuristicLabRole.RoleName)) {
              context.HeuristicLabUserRole.DeleteOnSubmit(userRole);
              //deleteList.Add(userRole);
            }//if
          }//foreach
        }//if
      }//foreach
      context.SubmitChanges();
      context.Dispose();
    }

    public override bool RoleExists(string roleName) {

      Persistence.DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext();
      bool returnValue = (db.HeuristicLabRole.Count(r => r.RoleName == roleName) == 1);
      db.Dispose();
      return returnValue;

    }
  }
}//HeuristicLabRoleProvider
