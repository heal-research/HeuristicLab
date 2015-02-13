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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive {
  [Item("Hive Administrator", "Hive Administrator")]
  public sealed class HiveAdminClient : IContent {
    private static HiveAdminClient instance;
    public static HiveAdminClient Instance {
      get {
        if (instance == null) instance = new HiveAdminClient();
        return instance;
      }
    }

    private IItemList<Resource> resources;
    public IItemList<Resource> Resources {
      get { return resources; }
    }

    private IItemList<Downtime> downtimes;
    public IItemList<Downtime> Downtimes {
      get { return downtimes; }
    }

    private Guid downtimeForResourceId;
    public Guid DowntimeForResourceId {
      get { return downtimeForResourceId; }
      set {
        downtimeForResourceId = value;
        if (downtimes != null) {
          downtimes.Clear();
        }
      }
    }

    #region Events
    public event EventHandler Refreshing;
    private void OnRefreshing() {
      EventHandler handler = Refreshing;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Refreshed;
    private void OnRefreshed() {
      var handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    private HiveAdminClient() { }

    #region Refresh
    public void Refresh() {
      OnRefreshing();

      try {
        resources = new ItemList<Resource>();

        HiveServiceLocator.Instance.CallHiveService(service => {
          service.GetSlaveGroups().ForEach(g => resources.Add(g));
          service.GetSlaves().ForEach(s => resources.Add(s));
        });
      }
      catch {
        throw;
      }
      finally {
        OnRefreshed();
      }
    }
    #endregion

    #region Refresh downtime calendar
    public void RefreshCalendar() {
      if (downtimeForResourceId != null && downtimeForResourceId != Guid.Empty) {
        OnRefreshing();

        try {
          downtimes = new ItemList<Downtime>();

          HiveServiceLocator.Instance.CallHiveService(service => {
            service.GetDowntimesForResource(downtimeForResourceId).ForEach(d => downtimes.Add(d));
          });
        }
        catch {
          throw;
        }
        finally {
          OnRefreshed();
        }
      }
    }
    #endregion

    #region Store
    public static void Store(IHiveItem item, CancellationToken cancellationToken) {
      if (item.Id == Guid.Empty) {
        if (item is SlaveGroup) {
          item.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddSlaveGroup((SlaveGroup)item));
        }
        if (item is Slave) {
          item.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddSlave((Slave)item));
        }
        if (item is Downtime) {
          item.Id = HiveServiceLocator.Instance.CallHiveService((s) => s.AddDowntime((Downtime)item));
        }
      } else {
        if (item is SlaveGroup) {
          HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateSlaveGroup((SlaveGroup)item));
        }
        if (item is Slave) {
          HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateSlave((Slave)item));
        }
        if (item is Downtime) {
          HiveServiceLocator.Instance.CallHiveService((s) => s.UpdateDowntime((Downtime)item));
        }
      }
    }
    #endregion

    #region Delete
    public static void Delete(IHiveItem item) {
      if (item is SlaveGroup) {
        HiveServiceLocator.Instance.CallHiveService((s) => s.DeleteSlaveGroup(item.Id));
      } else if (item is Slave) {
        HiveServiceLocator.Instance.CallHiveService((s) => s.DeleteSlave(item.Id));
      } else if (item is Downtime) {
        HiveServiceLocator.Instance.CallHiveService((s) => s.DeleteDowntime(item.Id));
      }
    }
    #endregion

    public void ResetDowntime() {
      downtimeForResourceId = Guid.Empty;
      if (downtimes != null) {
        downtimes.Clear();
      }
    }
  }
}
