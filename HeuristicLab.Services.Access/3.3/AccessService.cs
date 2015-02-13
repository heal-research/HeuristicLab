#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Security;
using HeuristicLab.GeoIP;
using DA = HeuristicLab.Services.Access.DataAccess;
using DT = HeuristicLab.Services.Access.DataTransfer;

namespace HeuristicLab.Services.Access {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  public class AccessService : IAccessService {
    private IUserManager userManager;
    private IUserManager UserManager {
      get {
        if (userManager == null) userManager = AccessServiceLocator.Instance.UserManager;
        return userManager;
      }
    }

    private IRoleVerifier roleVerifier;
    private IRoleVerifier RoleVerifier {
      get {
        if (roleVerifier == null) roleVerifier = AccessServiceLocator.Instance.RoleVerifier;
        return roleVerifier;
      }
    }

    #region Client Members
    public bool ClientExists(Guid id) {
      if (id != Guid.Empty) {
        using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
          return (context.Resources.Where(x => x.Id == id).Count() != 0);
        }
      }
      return false;
    }

    public DT.Client GetClient(Guid id) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.GetTable<DA.Resource>().OfType<DA.Client>()
                    where c.Id == id
                    select c;
        if (query.Count() > 0) {
          return Convert.ToDto(query.FirstOrDefault());
        } else {
          return null;
        }
      }
    }

    public IEnumerable<DT.Client> GetClients(IEnumerable<Guid> ids) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.GetTable<DA.Resource>().OfType<DA.Client>()
                    where ids.Contains(c.Id)
                    select Convert.ToDto(c);
        return query.ToList();
      }
    }

    public IEnumerable<DT.Client> GetAllClients() {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.GetTable<DA.Resource>().OfType<DA.Client>()
                    select Convert.ToDto(c);
        return query.ToList();
      }
    }

    public void AddClient(DT.Client client) {
      string country = string.Empty;

      OperationContext opContext = OperationContext.Current;

      if (opContext != null) {
        MessageProperties properties = opContext.IncomingMessageProperties;
        RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
        string ipAdr = endpoint.Address;
        country = GeoIPLookupService.Instance.GetCountryName(ipAdr);
      }

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        DA.Client entity = Convert.ToEntity(client);

        if (country != string.Empty) {
          var query = from c in context.GetTable<DA.Country>()
                      where c.Name == country
                      select c;
          if (query.Count() > 0) {
            entity.CountryId = query.First().Id;
          }
        }

        if (entity.OperatingSystem != null) {
          string osversion = entity.OperatingSystem.Name;
          var query = from os in context.GetTable<DA.OperatingSystem>()
                      where os.Name == osversion
                      select os;
          if (query.Count() > 0) {
            entity.OperatingSystem = query.First();
          }
        }

        if (entity.ClientType != null) {
          string cType = entity.ClientType.Name;
          var query = from t in context.GetTable<DA.ClientType>()
                      where t.Name == cType
                      select t;
          if (query.Count() > 0) {
            entity.ClientType = query.First();
          }
        }

        context.Resources.InsertOnSubmit(entity);
        context.SubmitChanges();
      }
    }

    public void UpdateClient(DT.Client client) {
      string country = string.Empty;

      OperationContext opContext = OperationContext.Current;

      if (opContext != null) {
        MessageProperties properties = opContext.IncomingMessageProperties;
        RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
        string ipAdr = endpoint.Address;
        country = GeoIPLookupService.Instance.GetCountryName(ipAdr);
      }

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.Resources.OfType<DA.Client>()
                    where c.Id == client.Id
                    select c;

        if (query.Count() > 0) {
          var entity = query.First();

          if (country != string.Empty) {
            var countryQuery = from c in context.GetTable<DA.Country>()
                               where c.Name == country
                               select c;
            if (countryQuery.Count() > 0) {
              entity.CountryId = countryQuery.First().Id;
            }
          }

          entity.Name = client.Name;
          entity.Description = client.Description;
          entity.HeuristicLabVersion = client.HeuristicLabVersion;
          entity.Timestamp = DateTime.Now;

          context.SubmitChanges();
        }
      }
    }

    public void DeleteClient(DT.Client client) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        //load client because we could get a detached object
        var query = from c in context.GetTable<DA.Resource>().OfType<DA.Client>()
                    where c.Id == client.Id
                    select c;
        if (query.Count() > 0) {

          //delete affiliation first
          var queryMapping = context.ResourceResourceGroups.Where(x => x.ResourceId == client.Id);
          if (queryMapping.Count() > 0) {
            context.ResourceResourceGroups.DeleteAllOnSubmit(queryMapping);
          }

          context.Resources.DeleteOnSubmit(query.First());
          context.SubmitChanges();
        }
      }
    }
    #endregion

    #region ClientGroup
    public IEnumerable<DT.ClientGroup> GetAllClientGroups() {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.GetTable<DA.Resource>().OfType<DA.ClientGroup>()
                    select Convert.ToDto(c);
        return query.ToList();
      }
    }

    public IEnumerable<DT.ClientGroup> GetClientGroups(IEnumerable<Guid> ids) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.GetTable<DA.Resource>().OfType<DA.ClientGroup>()
                    where ids.Contains(c.Id)
                    select Convert.ToDto(c);
        return query.ToList();
      }
    }

    public Guid AddClientGroup(DT.ClientGroup group) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        if (group.Id == Guid.Empty)
          group.Id = Guid.NewGuid();

        var entity = Convert.ToEntity(group);
        context.Resources.InsertOnSubmit(entity);
        context.SubmitChanges();
        return entity.Id;
      }
    }

    public void UpdateClientGroup(DT.ClientGroup clientGroup) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from g in context.Resources.OfType<DA.ClientGroup>()
                    where g.Id == clientGroup.Id
                    select g;

        if (query.Count() > 0) {
          var entity = query.First();
          entity.Name = clientGroup.Name;
          entity.Description = clientGroup.Description;
          context.SubmitChanges();
        }
      }
    }

    public void DeleteClientGroup(DT.ClientGroup clientGroup) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        //load clientGroup because we could get a detached object
        var query = from c in context.GetTable<DA.Resource>().OfType<DA.ClientGroup>()
                    where c.Id == clientGroup.Id
                    select c;
        if (query.Count() > 0) {
          context.Resources.DeleteOnSubmit(query.First());
          context.SubmitChanges();
        }
      }
    }

    public void AddResourceToGroup(DT.Resource resource, DT.ClientGroup group) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        DA.ResourceResourceGroup rrg = new DA.ResourceResourceGroup() {
          ResourceId = resource.Id,
          ResourceGroupId = group.Id
        };

        context.ResourceResourceGroups.InsertOnSubmit(rrg);
        context.SubmitChanges();
      }
    }

    public void RemoveResourceFromGroup(DT.Resource resource, DT.ClientGroup group) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = context.ResourceResourceGroups.Where(x => x.ResourceId == resource.Id && x.ResourceGroupId == group.Id);
        if (query.Count() > 0) {
          context.ResourceResourceGroups.DeleteOnSubmit(query.First());
          context.SubmitChanges();
        }
      }
    }
    #endregion

    #region ClientGroupMapping
    public IEnumerable<DT.ClientGroupMapping> GetClientGroupMapping() {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.GetTable<DA.ResourceResourceGroup>()
                    select Convert.ToDto(c);
        return query.ToList();
      }
    }
    #endregion

    #region Resource
    public IEnumerable<DT.Resource> GetResources() {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from r in context.Resources
                    select Convert.ToDto(r);
        return query.ToList();
      }
    }
    #endregion

    #region ClientLog
    public DT.ClientLog GetLastClientLog(Guid clientId) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from r in context.ClientLogs
                    where r.ResourceId == clientId
                    select r;
        return Convert.ToDto(query.OrderBy(x => x.Timestamp).LastOrDefault());
      }
    }

    public IEnumerable<DT.ClientLog> GetClientLogs(Guid clientId) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from r in context.ClientLogs
                    where r.ResourceId == clientId
                    select Convert.ToDto(r);
        return query.ToList();
      }
    }

    public IEnumerable<DT.ClientLog> GetClientLogsSince(DateTime startDate) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from r in context.ClientLogs
                    where r.Timestamp >= startDate
                    select Convert.ToDto(r);
        return query.ToList();
      }
    }

    public void AddClientLog(DT.ClientLog log) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        context.ClientLogs.InsertOnSubmit(Convert.ToEntity(log));
        context.SubmitChanges();
      }
    }

    public void DeleteClientLog(DT.ClientLog log) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        context.ClientLogs.DeleteOnSubmit(Convert.ToEntity(log));
        context.SubmitChanges();
      }
    }
    #endregion

    #region User
    private DT.User BuildUserDto(Guid userId) {
      DA.aspnet_User aspUser = null;
      DA.aspnet_Membership aspMembership = null;
      DA.User accessUser = null;

      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var userQuery = from u in context.aspnet_Users
                        where u.UserId == userId
                        select u;
        if (userQuery.Count() == 1) {
          aspUser = userQuery.First();
        }

        var memQuery = from u in context.aspnet_Memberships
                       where u.UserId == userId
                       select u;
        if (memQuery.Count() == 1) {
          aspMembership = memQuery.First();
        }
      }

      if (aspUser != null || aspMembership != null) {
        using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
          var query = from u in context.UserGroupBases.OfType<DA.User>()
                      where u.Id == userId
                      select u;
          if (query.Count() == 1) {
            accessUser = query.First();
          } else {
            //if the user is not in the access db add it (this makes it easy to upgrade with an existing asp.net authentication db)
            DA.User user = new DA.User();
            user.Id = userId;
            user.FullName = "Not set";
            context.UserGroupBases.InsertOnSubmit(user);
            context.SubmitChanges();
            accessUser = user;
          }
        }
      }

      if (aspUser == null || aspMembership == null || accessUser == null) {
        throw new Exception("User with id " + userId + " not found.");
      } else {
        return Convert.ToDto(accessUser, aspUser, aspMembership);
      }
    }

    private DT.LightweightUser BuildLightweightUserDto(Guid userId) {
      DA.aspnet_User aspUser = null;
      DA.aspnet_Membership aspMembership = null;
      DA.User accessUser = null;
      List<DA.aspnet_Role> roles = new List<DA.aspnet_Role>();
      List<DA.UserGroup> groups = new List<DA.UserGroup>();


      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var userQuery = from u in context.aspnet_Users
                        where u.UserId == userId
                        select u;

        var memQuery = from u in context.aspnet_Memberships
                       where u.UserId == userId
                       select u;
        if (memQuery.Count() == 1) {
          aspMembership = memQuery.First();
        }

        if (userQuery.Count() == 1) {
          aspUser = userQuery.First();
          roles = (from ur in context.aspnet_UsersInRoles
                   where ur.UserId == aspUser.UserId
                   join r in context.aspnet_Roles on ur.RoleId equals r.RoleId
                   select r).ToList();
        }
      }

      if (aspUser != null || aspMembership != null) {
        using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
          var query = from u in context.UserGroupBases.OfType<DA.User>()
                      where u.Id == userId
                      select u;
          if (query.Count() == 1) {
            accessUser = query.First();
            groups = (from ug in context.UserGroupUserGroups
                      where ug.UserGroupId == accessUser.Id
                      join g in context.UserGroupBases.OfType<DA.UserGroup>() on ug.UserGroupUserGroupId equals g.Id
                      select g).ToList();
          } else {
            //if the user is not in the access db add it (this makes it easy to upgrade with an existing asp.net authentication db)
            DA.User user = new DA.User();
            user.Id = userId;
            user.FullName = "Not set";
            context.UserGroupBases.InsertOnSubmit(user);
            context.SubmitChanges();
            accessUser = user;
          }
        }
      }

      if (aspUser == null || accessUser == null || aspMembership == null) {
        throw new Exception("User with id " + userId + " not found.");
      } else {
        return Convert.ToDto(accessUser, aspUser, aspMembership, roles, groups);
      }
    }

    public DT.LightweightUser Login() {
      Guid userId = UserManager.CurrentUserId;
      return BuildLightweightUserDto(userId);
    }

    public void UpdateLightweightUser(DT.LightweightUser user) {
      DT.User u = BuildUserDto(user.Id);

      u.Email = user.EMail;
      u.FullName = user.FullName;

      UpdateUser(u);
    }

    public IEnumerable<DT.UserGroup> GetGroupsOfCurrentUser() {
      Guid userId = UserManager.CurrentUserId;

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from g in context.UserGroupUserGroups
                    from ug in context.UserGroupBases.OfType<DA.UserGroup>()
                    where g.UserGroupId == userId && g.UserGroupUserGroupId == ug.Id
                    select Convert.ToDto(ug);
        return query.ToList();
      }
    }

    public IEnumerable<DT.Role> GetRolesOfCurrentUser() {
      Guid userId = UserManager.CurrentUserId;

      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var query = from ur in context.aspnet_UsersInRoles
                    from r in context.aspnet_Roles
                    where ur.UserId == userId && ur.RoleId == r.RoleId
                    select Convert.ToDto(r);
        return query.ToList();
      }
    }


    public IEnumerable<DT.LightweightUser> GetAllLightweightUsers() {
      List<Guid> accessUserGuids = null;

      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var query = from u in context.aspnet_Users
                    select u.UserId;
        accessUserGuids = query.ToList();
      }
      return accessUserGuids.Select(x => BuildLightweightUserDto(x));
    }

    public IEnumerable<DT.User> GetAllUsers() {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      List<Guid> accessUserGuids = null;

      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var query = from u in context.aspnet_Users
                    select u.UserId;
        accessUserGuids = query.ToList();
      }

      return accessUserGuids.Select(x => BuildUserDto(x));
    }

    public IEnumerable<DT.User> GetUsers(IEnumerable<Guid> ids) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      List<Guid> accessUserGuids = null;

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases.OfType<DA.User>()
                    where ids.Contains(u.Id)
                    select u.Id;
        accessUserGuids = query.ToList();
      }

      if (accessUserGuids.Count() != ids.Count()) {
        throw new Exception("Couldn't find one or more users for the given user ids.");
      }

      return accessUserGuids.Select(x => BuildUserDto(x));
    }

    public IEnumerable<DT.LightweightUser> GetLightweightUsers(IEnumerable<Guid> ids) {
      List<Guid> accessUserGuids = null;

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases.OfType<DA.User>()
                    where ids.Contains(u.Id)
                    select u.Id;
        accessUserGuids = query.ToList();
      }

      if (accessUserGuids.Count() != ids.Count()) {
        throw new Exception("Couldn't find one or more users for the given user ids.");
      }

      return accessUserGuids.Select(x => BuildLightweightUserDto(x));
    }

    public DT.User AddUser(DT.User user) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      DA.User accessUser;
      DA.aspnet_User aspUser;
      DA.aspnet_Membership aspMembership;
      bool userExistsInASP;

      Convert.ToEntity(user, out accessUser, out aspUser, out aspMembership, out userExistsInASP);

      if (userExistsInASP) {
        if (accessUser.Id == null || accessUser.Id == Guid.Empty) {
          accessUser.Id = aspMembership.UserId;
        }
        using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
          context.UserGroupBases.InsertOnSubmit(accessUser);
          context.SubmitChanges();
        }
        MembershipUser membershipUser = Membership.GetUser((object)accessUser.Id);
        if (membershipUser != null) {
          membershipUser.Email = aspMembership.Email;
          membershipUser.IsApproved = aspMembership.IsApproved;
          membershipUser.Comment = aspMembership.Comment;
          Membership.UpdateUser(membershipUser);
        }
      } else {
        MembershipUser membershipUser = Membership.CreateUser(aspUser.UserName, aspUser.UserName, aspMembership.Email);
        membershipUser.IsApproved = aspMembership.IsApproved;
        membershipUser.Comment = aspMembership.Comment;
        Membership.UpdateUser(membershipUser);

        Guid userId = (Guid)membershipUser.ProviderUserKey;
        accessUser.Id = userId;

        using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
          context.UserGroupBases.InsertOnSubmit(accessUser);
          context.SubmitChanges();
        }
      }

      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var newAspUser = context.aspnet_Users.Where(x => x.UserId == accessUser.Id).FirstOrDefault();
        var newAspMembership = context.aspnet_Memberships.Where(x => x.UserId == accessUser.Id).FirstOrDefault();
        return Convert.ToDto(accessUser, newAspUser, newAspMembership);
      }
    }

    public void DeleteUser(DT.User user) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      if (user.Id != null && user.Id != Guid.Empty) {
        //delete asp.net user
        Membership.DeleteUser(user.UserName);
        using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
          var query = context.UserGroupBases.OfType<DA.User>().Where(x => x.Id == user.Id);
          if (query.Count() > 0) {

            //delete affiliation first
            var queryMapping = context.UserGroupUserGroups.Where(x => x.UserGroupId == user.Id);
            if (queryMapping.Count() > 0) {
              context.UserGroupUserGroups.DeleteAllOnSubmit(queryMapping);
            }

            //delete user from access db
            context.UserGroupBases.DeleteOnSubmit(query.First());
            context.SubmitChanges();
          }
        }
      }
    }

    public void UpdateUser(DT.User user) {
      if (user.Id != UserManager.CurrentUserId) {
        RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);
      }

      MembershipUser membershipUser = Membership.GetUser((object)user.Id);
      if (membershipUser != null) {
        membershipUser.Email = user.Email;
        membershipUser.IsApproved = user.IsApproved;
        membershipUser.Comment = user.Comment;
        Membership.UpdateUser(membershipUser);
      }

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases.OfType<DA.User>()
                    where u.Id == user.Id
                    select u;
        if (query.Count() > 0) {
          DA.User u = query.First();
          u.FullName = user.FullName;
          context.SubmitChanges();
        }
      }
    }

    public void AddUserToRole(DT.Role role, DT.User user) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      MembershipUser msUser = Membership.GetUser((object)user.Id);
      if (msUser != null) {
        Roles.AddUserToRole(msUser.UserName, role.Name);
      }
    }

    public void RemoveUserFromRole(DT.Role role, DT.User user) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      MembershipUser msUser = Membership.GetUser((object)user.Id);
      if (msUser != null) {
        Roles.RemoveUserFromRole(msUser.UserName, role.Name);
      }
    }

    public bool ChangePassword(Guid userId, string oldPassword, string newPassword) {
      MembershipUser msUser = Membership.GetUser(userId);
      if (msUser != null) {
        return msUser.ChangePassword(oldPassword, newPassword);
      }
      return false;
    }

    public string ResetPassword(Guid userId) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      MembershipUser msUser = Membership.GetUser(userId);
      if (msUser != null) {
        return msUser.ResetPassword();
      } else {
        throw new Exception("Password reset failed.");
      }
    }
    #endregion

    #region UserGroup
    public IEnumerable<DT.UserGroup> GetAllUserGroups() {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases.OfType<DA.UserGroup>()
                    select Convert.ToDto(u);
        return query.ToList();
      }
    }

    public IEnumerable<DT.UserGroup> GetUserGroupsOfUser(Guid userId) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var groupIds = from g in context.UserGroupUserGroups
                       where g.UserGroupId == userId
                       select g.UserGroupUserGroupId;

        var query = from g in context.UserGroupBases.OfType<DA.UserGroup>()
                    where groupIds.Contains(g.Id)
                    select Convert.ToDto(g);

        return query.ToList();
      }
    }

    public IEnumerable<DT.UserGroup> GetUserGroups(IEnumerable<Guid> ids) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases.OfType<DA.UserGroup>()
                    where ids.Contains(u.Id)
                    select Convert.ToDto(u);
        return query.ToList();
      }
    }

    public Guid AddUserGroup(DT.UserGroup group) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        //because id is not automatically set because of user, we have to do it here manually for group   
        group.Id = Guid.NewGuid();

        context.UserGroupBases.InsertOnSubmit(Convert.ToEntity(group));
        context.SubmitChanges();
        return group.Id;
      }
    }

    public void UpdateUserGroup(DT.UserGroup group) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        DA.UserGroup entity = context.UserGroupBases.OfType<DA.UserGroup>().FirstOrDefault(x => x.Id == group.Id);
        Convert.ToEntity(group, entity);
        context.SubmitChanges();
      }
    }

    public void DeleteUserGroup(DT.UserGroup group) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);
      var g = group; //linq does not like vars called group

      if (g.Id != null && g.Id != Guid.Empty) {
        using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
          var query = from ug in context.UserGroupBases.OfType<DA.UserGroup>()
                      where ug.Id == g.Id
                      select ug;
          if (query.Count() > 0) {
            context.UserGroupBases.DeleteOnSubmit(query.First());
            context.SubmitChanges();
          } else {
            throw new Exception("UserGroup with id " + g.Id + " does not exist.");
          }
        }
      }
    }

    public void AddUserGroupBaseToGroup(DT.UserGroupBase resource, DT.UserGroup group) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        DA.UserGroupUserGroup ugug = new DA.UserGroupUserGroup();
        ugug.UserGroupId = resource.Id;
        ugug.UserGroupUserGroupId = group.Id;
        context.UserGroupUserGroups.InsertOnSubmit(ugug);
        context.SubmitChanges();
      }
    }

    public void RemoveUserGroupBaseFromGroup(DT.UserGroupBase resource, DT.UserGroup userGroup) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupUserGroups
                    where u.UserGroupId == resource.Id && u.UserGroupUserGroupId == userGroup.Id
                    select u;

        if (query.Count() == 1) {
          context.UserGroupUserGroups.DeleteOnSubmit(query.First());
          context.SubmitChanges();
        }
      }
    }

    public IEnumerable<DT.UserGroupBase> GetUsersAndGroups() {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases
                    select Convert.ToDto(u);
        return query.ToList();
      }
    }

    public IEnumerable<DT.UserGroupMapping> GetUserGroupMapping() {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupUserGroups
                    select Convert.ToDto(u);
        return query.ToList();
      }
    }

    public IEnumerable<Guid> GetUserGroupIdsOfGroup(Guid groupId) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupUserGroups
                    where u.UserGroupUserGroupId == groupId
                    select u.UserGroupId;
        return query.ToList();
      }
    }
    #endregion

    #region UserGroupBase
    public IEnumerable<DT.UserGroupBase> GetAllLeightweightUsersAndGroups() {
      List<DT.UserGroup> userGroups = new List<DT.UserGroup>();
      List<DT.UserGroupBase> result = new List<DT.UserGroupBase>();

      List<Guid> accessUserGuids = null;
      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var query = from u in context.aspnet_Users
                    select u.UserId;
        accessUserGuids = query.ToList();
      }
      var lightweightUsers = accessUserGuids.Select(x => BuildLightweightUserDto(x));

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases.OfType<DA.UserGroup>()
                    select Convert.ToDto(u);
        userGroups = query.ToList();
      }

      result.AddRange(lightweightUsers);
      result.AddRange(userGroups);

      return result;
    }

    public IEnumerable<DT.UserGroupBase> GetLeightweightUsersAndGroups(IEnumerable<Guid> ids) {
      List<DA.UserGroupBase> dbUserGroupsBases = new List<DA.UserGroupBase>();
      List<DT.UserGroupBase> result = new List<DT.UserGroupBase>();

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from u in context.UserGroupBases
                    where ids.Contains(u.Id)
                    select u;
        dbUserGroupsBases = query.ToList();
      }

      foreach (var ugb in dbUserGroupsBases) {
        if (ugb.GetType() == typeof(DA.User)) {
          var user = BuildLightweightUserDto(ugb.Id);
          result.Add(user);
        } else if (ugb.GetType() == typeof(DA.UserGroup)) {
          var group = Convert.ToDto(ugb as DA.UserGroup);
          result.Add(group);
        }
      }
      return result;
    }
    #endregion

    #region Roles
    public IEnumerable<DT.Role> GetRoles() {
      using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
        var query = from u in context.aspnet_Roles
                    select Convert.ToDto(u);
        return query.ToList();
      }
    }

    public DT.Role AddRole(DT.Role role) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      Roles.CreateRole(role.Name);
      return role;
    }

    public void DeleteRole(DT.Role role) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      Roles.DeleteRole(role.Name);
    }

    public IEnumerable<DT.Role> GetUserRoles(DT.User user) {
      var roles = Roles.GetRolesForUser(user.UserName);
      return roles.Select(x => new DT.Role() { Name = x });
    }

    public void AddRoleToGroup(DT.UserGroup userGroup, DT.Role role) {
      Guid[] userIds;
      string[] aspUsers;

      using (DA.AccessServiceDataContext accessContext = new DA.AccessServiceDataContext()) {
        userIds = (from u in accessContext.UserGroupUserGroups
                   where u.UserGroupUserGroupId == userGroup.Id
                   select u.UserGroupId).ToArray();
      }

      using (DA.ASPNETAuthenticationDataContext aspContext = new DA.ASPNETAuthenticationDataContext()) {
        aspUsers = (from u in aspContext.aspnet_Users
                    where userIds.Contains(u.UserId)
                    select u.UserName).ToArray();
      }

      Roles.AddUsersToRole(aspUsers, role.Name);

    }

    public void RemoveRoleFromGroup(DT.UserGroup userGroup, DT.Role role) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      Guid[] userIds;
      string[] aspUsers;

      using (DA.AccessServiceDataContext accessContext = new DA.AccessServiceDataContext()) {
        userIds = (from u in accessContext.UserGroupUserGroups
                   where u.UserGroupUserGroupId == userGroup.Id
                   select u.UserGroupId).ToArray();
      }

      using (DA.ASPNETAuthenticationDataContext aspContext = new DA.ASPNETAuthenticationDataContext()) {
        aspUsers = (from u in aspContext.aspnet_Users
                    where userIds.Contains(u.UserId)
                    select u.UserName).ToArray();
      }

      Roles.RemoveUsersFromRole(aspUsers.ToArray(), role.Name);
    }
    #endregion

    #region Error Reporting
    public void ReportError(DT.ClientError error) {
      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        context.ClientErrors.InsertOnSubmit(Convert.ToEntity(error));
        context.SubmitChanges();
      }
    }

    public IEnumerable<DT.ClientError> GetClientErrors() {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.ClientErrors
                    select Convert.ToDto(c);
        return query.ToList();
      }
    }

    public IEnumerable<DT.ClientError> GetLastClientErrors(DateTime startDate) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = from c in context.ClientErrors
                    where c.Timestamp >= startDate
                    select Convert.ToDto(c);
        return query.ToList();
      }
    }

    public void DeleteError(DT.ClientError error) {
      RoleVerifier.AuthenticateForAllRoles(AccessServiceRoles.Administrator);

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        var query = context.ClientErrors.Where(x => x.Id == error.Id);
        if (query.Count() > 0) {
          context.ClientErrors.DeleteOnSubmit(query.First());
          context.SubmitChanges();
        }
      }
    }
    #endregion
  }
}
