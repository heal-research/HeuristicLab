using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Security.Contracts.Interfaces;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.DataAccess.Interfaces;
using System.ServiceModel;

namespace HeuristicLab.Security.Core {
  public class SecurityManager : ISecurityManager {

    private static ISessionFactory factory;
    private static ISessionFactory Factory {
      get {
        // lazy initialization
        if(factory==null)
          factory = ServiceLocator.GetSessionFactory();
        return factory;
      }
    }

    private ISession session;

   /// <summary>
    /// Add new user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public User AddNewUser(User user) {
      try {
        session = Factory.GetSessionForCurrentThread();

        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();
        if (user != null)
          userAdapter.Update(user);

        return user;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();

        if (user != null)
          userAdapter.Update(user);
        return user;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();
        User user = userAdapter.GetById(userId);

        if (user != null) 
          return userAdapter.Delete(user);
        return false;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();

        return userAdapter.GetAll();
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();

        return userAdapter.GetByName(name);
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
      finally {
        if (session != null)
          session.EndSession();
      }  
    }

    /// <summary>
    /// Gets user by his login.
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    public User GetUserByLogin(string login) {
      try {
        session = Factory.GetSessionForCurrentThread();
        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();

        return userAdapter.GetByLogin(login);
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        if (userGroup != null)
          userGroupAdapter.Update(userGroup);

        return userGroup;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        if (userGroup != null)
          userGroupAdapter.Update(userGroup);

        return userGroup;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();

        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();
        UserGroup userGroup = userGroupAdapter.GetById(userGroupId);   

        if (userGroup != null) 
          return userGroupAdapter.Delete(userGroup);
        return false;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();

        return userGroupAdapter.GetAll();
       }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
        IUserGroupAdapter userGroupAdapter = session.GetDataAdapter<UserGroup, IUserGroupAdapter>();

        return userGroupAdapter.GetByName(name);
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();

        IPermissionOwnerAdapter permOwnerAdapter = session.GetDataAdapter<PermissionOwner, IPermissionOwnerAdapter>();
        if (permissionOwner != null)
          permOwnerAdapter.Update(permissionOwner);

        return permissionOwner;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
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
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();
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
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();                         
        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();

        return permissionAdapter.grantPermission(permissionOwnerId, permissionId, entityId);
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();

        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();
        return permissionAdapter.GetById(permissionId);
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
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
        session = Factory.GetSessionForCurrentThread();     
        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();
 
        return permissionAdapter.revokePermission(permissionOwnerId, permissionId, entityId);
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public Permission AddPermission(Permission permission) {
      try {
        session = Factory.GetSessionForCurrentThread();
        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();

        if (permission != null) {
          permissionAdapter.Update(permission);

          return permission;
        } else
          return null;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public bool RemovePermission(Guid permissionId) {
      try {
        session = Factory.GetSessionForCurrentThread();
        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();

        Permission permission = permissionAdapter.GetById(permissionId);
        if (permission != null)
          return permissionAdapter.Delete(permission);
        else
          return false;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public Permission UpdatePermission(Permission permission) {
      try {
        session = Factory.GetSessionForCurrentThread();
        IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();

        if(permission != null) {
          permissionAdapter.Update(permission);
          return permission;
        }
        else
          return null;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
      finally {
        if (session != null)
          session.EndSession();
      }
    }
  }
}
