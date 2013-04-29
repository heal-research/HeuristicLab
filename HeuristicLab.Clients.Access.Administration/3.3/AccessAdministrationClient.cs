#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Common;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Access.Administration {
  [Item("AccessAdministrationClient", "AccessAdministration client.")]
  public class AccessAdministrationClient : IContent {
    private static AccessAdministrationClient instance;
    public static AccessAdministrationClient Instance {
      get {
        if (instance == null) instance = new AccessAdministrationClient();
        return instance;
      }
    }

    #region Properties
    private ItemList<User> users;
    public ItemList<User> Users {
      get {
        return users;
      }
    }

    public ItemList<UserGroup> Groups { get; set; }
    public ItemList<Role> Roles { get; set; }
    #endregion

    private AccessAdministrationClient() { }

    #region Refresh
    public void RefreshUsers() {
      users = new ItemList<User>();
      users.AddRange(CallAccessService<ItemList<User>>(s => new ItemList<User>(s.GetAllUsers())));
    }

    public void RefreshUsersAsync(Action<Exception> exceptionCallback) {
      ExecuteActionAsync(RefreshUsers, exceptionCallback);
    }

    public void RefreshUserGroups() {
      Groups = new ItemList<UserGroup>();
      Groups.AddRange(CallAccessService<ItemList<UserGroup>>(s => new ItemList<UserGroup>(s.GetAllUserGroups())));
    }

    public void RefreshUserGroupsAsync(Action<Exception> exceptionCallback) {
      ExecuteActionAsync(RefreshUserGroups, exceptionCallback);
    }

    public void RefreshRoles() {
      Roles = new ItemList<Role>();
      Roles.AddRange(CallAccessService<ItemList<Role>>(s => new ItemList<Role>(s.GetRoles())));
    }

    public void RefreshRolesAsync(Action<Exception> exceptionCallback) {
      ExecuteActionAsync(RefreshRoles, exceptionCallback);
    }

    #endregion

    #region Store
    public void StoreUsers() {
      foreach (User u in users) {
        if (u.Modified) {
          if (u.Id == Guid.Empty) {
            CallAccessService(s => u.Id = s.AddUser(u).Id);
          } else {
            CallAccessService(s => s.UpdateUser(u));
          }
          u.SetUnmodified();
        }
      }
    }

    public void StoreUsersAsync(Action<Exception> exceptionCallback) {
      ExecuteActionAsync(StoreUsers, exceptionCallback);
    }

    public void StoreUserGroups() {
      foreach (UserGroup g in Groups) {
        if (g.Modified) {
          if (g.Id == Guid.Empty) {
            CallAccessService(s => g.Id = s.AddUserGroup(g));
          } else {
            CallAccessService(s => s.UpdateUserGroup(g));
          }
          g.SetUnmodified();
        }
      }
    }

    public void StoreUserGroupsAsync(Action<Exception> exceptionCallback) {
      ExecuteActionAsync(StoreUserGroups, exceptionCallback);
    }

    public void StoreRoles() {
      foreach (Role g in Roles) {
        if (g.Modified) {
          CallAccessService(s => s.AddRole(g));
          g.SetUnmodified();
        }
      }
    }

    public void StoreRolesAsync(Action<Exception> exceptionCallback) {
      ExecuteActionAsync(StoreRoles, exceptionCallback);
    }
    #endregion

    #region Delete
    public void DeleteUser(User u) {
      CallAccessService(s => s.DeleteUser(u));
    }

    public void DeleteUserAsync(User u, Action<Exception> exceptionCallback) {
      Action deleteUserAction = new Action(delegate { DeleteUser(u); });
      ExecuteActionAsync(deleteUserAction, exceptionCallback);
    }

    public void DeleteUserGroup(UserGroup u) {
      CallAccessService(s => s.DeleteUserGroup(u));
    }

    public void DeleteUserGroupAsync(UserGroup u, Action<Exception> exceptionCallback) {
      Action deleteUserGroupAction = new Action(delegate { DeleteUserGroup(u); });
      ExecuteActionAsync(deleteUserGroupAction, exceptionCallback);
    }

    public void DeleteRole(Role u) {
      CallAccessService(s => s.DeleteRole(u));
    }

    public void DeleteRoleAsync(Role u, Action<Exception> exceptionCallback) {
      Action deleteRoleAction = new Action(delegate { DeleteRole(u); });
      ExecuteActionAsync(deleteRoleAction, exceptionCallback);
    }
    #endregion

    public void ExecuteActionAsync(Action action, Action<Exception> exceptionCallback) {
      var call = new Func<Exception>(delegate() {
        try {
          OnRefreshing();
          action();
        }
        catch (Exception ex) {
          return ex;
        }
        finally {
          OnRefreshed();
        }
        return null;
      });
      call.BeginInvoke(delegate(IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) exceptionCallback(ex);
      }, null);
    }

    #region Events
    public event EventHandler Refreshing;
    private void OnRefreshing() {
      EventHandler handler = Refreshing;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Refreshed;
    private void OnRefreshed() {
      EventHandler handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region Helpers
    public static void CallAccessService(Action<IAccessService> call) {
      AccessServiceClient client = ClientFactory.CreateClient<AccessServiceClient, IAccessService>();
      try {
        call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }
    public static T CallAccessService<T>(Func<IAccessService, T> call) {
      AccessServiceClient client = ClientFactory.CreateClient<AccessServiceClient, IAccessService>();
      try {
        return call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }
    #endregion
  }
}
