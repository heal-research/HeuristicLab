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

namespace HeuristicLab.Clients.Access {
  [Item("AccessClient", "Access client.")]
  public sealed class AccessClient : IContent {
    private static AccessClient instance;
    public static AccessClient Instance {
      get {
        if (instance == null) instance = new AccessClient();
        return instance;
      }
    }

    #region Properties
    private ItemList<UserGroupBase> usersAndGroups;
    public ItemList<UserGroupBase> UsersAndGroups {
      get { return usersAndGroups; }
    }
    #endregion

    private AccessClient() { }

    #region Refresh
    public void Refresh() {
      usersAndGroups = new ItemList<UserGroupBase>();
      usersAndGroups.AddRange(CallAccessService<ItemList<UserGroupBase>>(s => new ItemList<UserGroupBase>(s.GetAllLeightweightUsersAndGroups())));
    }
    public void RefreshAsync(Action<Exception> exceptionCallback) {
      var call = new Func<Exception>(delegate() {
        try {
          OnRefreshing();
          Refresh();
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
    #endregion

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
