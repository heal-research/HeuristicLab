using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Security.Contracts.Interfaces;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.Security.Core {
  public class SecurityManager : ISecurityManager {

    private static ISessionFactory factory = (new DiscoveryService()).GetInstances<ISessionFactory>()[0];

    private ISession session;

   /// <summary>
    /// Add new user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public User AddNewUser(User user) {
      try {
        session = factory.GetSessionForCurrentThread();

        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();
        if (user != null)
          userAdapter.Update(user);

        return user;
      }
      finally {
        if (session != null)
          session.EndSession();
      } 
    }

    /// <summary>
    /// Update user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public User UpdateUser(User user) {
      try {
        session = factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();

        if (user != null)
          userAdapter.Update(user);
        return user;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

   /// <summary>
    /// Remove user.
   /// </summary>
   /// <param name="userId"></param>
   /// <returns></returns>
    public bool RemoveUser(Guid userId) {
      try {
        session = factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();
        User user = userAdapter.GetById(userId);

        if (user != null) 
          return userAdapter.Delete(user);
        return false;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Gets all Users.
    /// </summary>
    /// <returns></returns>
    public ICollection<User> GetAllUsers() {
      try {
        session = factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();

        return userAdapter.GetAll();
      }
      finally {
        if (session != null)
          session.EndSession();
      }         
    }

    /// <summary>
    /// Gets user by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public User GetUserByName(string name) {
      try {
        session = factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();

        return userAdapter.GetByName(name);
      }
      finally {
        if (session != null)
          session.EndSession();
      }  
    }

    /// <summary>
    /// Add new user group.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public UserGroup AddNewUserGroup(UserGroup userGroup) {
      try {
        session = factory.GetSessionForCurrentThread();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        if (userGroup != null)
          userGroupAdapter.Update(userGroup);

        return userGroup;
      }
      finally {
        if (session != null)
          session.EndSession();
      } 
    }

    /// <summary>
    /// Update user group.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public UserGroup UpdateUserGroup(UserGroup userGroup) {
      try {
        session = factory.GetSessionForCurrentThread();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        if (userGroup != null)
          userGroupAdapter.Update(userGroup);

        return userGroup;
      }
      finally {
        if (session != null)
          session.EndSession();
      } 
    }

    /// <summary>
    /// Remove user group.
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public bool RemoveUserGroup(Guid userGroupId) {
      try {
        session = factory.GetSessionForCurrentThread();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        UserGroup userGroup = userGroupAdapter.GetById(userGroupId);   

        if (userGroup != null) 
          return userGroupAdapter.Delete(userGroup);
        return false;
      }
      finally {
        if (session != null)
          session.EndSession();
      } 
    }

    /// <summary>
    /// Gets all UserGroups.
    /// </summary>
    /// <returns></returns>
    public ICollection<UserGroup> GetAllUserGroups() {
      try {
        session = factory.GetSessionForCurrentThread();
        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();

        return userGroupAdapter.GetAll();
       }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Gets UserGroup by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UserGroup GetUserGroupByName(string name) {
      try {
        session = factory.GetSessionForCurrentThread();
        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();

        return userGroupAdapter.GetByName(name);
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Updates a PermissionOwner.
    /// </summary>
    /// <param name="permissionOwner"></param>
    /// <returns></returns>
    public PermissionOwner UpdatePermissionOwner(PermissionOwner permissionOwner) {
      try {
        session = factory.GetSessionForCurrentThread();

        IPermissionOwnerAdapter permOwnerAdapter = session.GetDataAdapter<PermissionOwner, IPermissionOwnerAdapter>();
        if (permissionOwner != null)
          permOwnerAdapter.Update(permissionOwner);

        return permissionOwner;
      }
      finally {
        if (session != null)
          session.EndSession();
      } 
    }

    /// <summary>
    /// Add permission owner to group.
    /// </summary>
    /// <param name="userGroupId"></param>
    /// <param name="permissionOwnerId"></param>
    /// <returns></returns>
    public bool AddPermissionOwnerToGroup(Guid userGroupId, Guid permissionOwnerId) {
      try {
        session = factory.GetSessionForCurrentThread();
        ITransaction transaction = session.BeginTransaction();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        UserGroup userGroup = userGroupAdapter.GetById(userGroupId);

        IPermissionOwnerAdapter permOwnerAdapter = session.GetDataAdapter<PermissionOwner, IPermissionOwnerAdapter>();
        PermissionOwner permissionOwner = permOwnerAdapter.GetById(permissionOwnerId);
        
        if ((userGroup != null) && (permissionOwner != null)) {
          userGroup.Members.Add(permissionOwner);
          userGroupAdapter.Update(userGroup);
          transaction.Commit();
          return true;
        }
        return false;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }


    /// <summary>
    /// Remove permission owner from group.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="permissionOwnerId"></param>
    /// <returns></returns>
    public bool RemovePermissionOwnerFromGroup(Guid userGroupId, Guid permissionOwnerId) {
      try {
        session = factory.GetSessionForCurrentThread();
        ITransaction transaction = session.BeginTransaction();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        UserGroup userGroup = userGroupAdapter.GetById(userGroupId);

        IPermissionOwnerAdapter permOwnerAdapter = session.GetDataAdapter<PermissionOwner, IPermissionOwnerAdapter>();
        PermissionOwner permissionOwner = permOwnerAdapter.GetById(permissionOwnerId);

        if ((userGroup != null) && (permissionOwner != null)) {
          userGroup.Members.Add(permissionOwner);
          userGroupAdapter.Delete(userGroup);
          transaction.Commit();
          return true;
        }
        return false;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Grant permission.
    /// </summary>
    /// <param name="permissionOwnerId"></param>
    /// <param name="permissionId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public bool GrantPermission(Guid permissionOwnerId, Guid permissionId, Guid entityId) {
      try {
        session = factory.GetSessionForCurrentThread();                         
        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();

        return permissionAdapter.grantPermission(permissionOwnerId, permissionId, entityId);
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Gets Permission by ID.
    /// </summary>
    /// <param name="permissionId"></param>
    /// <returns></returns>
    public Permission GetPermissionById(Guid permissionId) {
      try {
        session = factory.GetSessionForCurrentThread();

        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();
        return permissionAdapter.GetById(permissionId);
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Revoke permission.
    /// </summary>
    /// <param name="permissionOwnerId"></param>
    /// <param name="permissionId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public bool RevokePermission(Guid permissionOwnerId, Guid permissionId, Guid entityId) {
      try {
        session = factory.GetSessionForCurrentThread();     
        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();
 
        return permissionAdapter.revokePermission(permissionOwnerId, permissionId, entityId);
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }
 
  }
}
