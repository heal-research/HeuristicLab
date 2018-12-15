#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using HeuristicLab.Services.Hive.DataAccess.Daos;
using HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics;
using HeuristicLab.Services.Hive.DataAccess.Data;

namespace HeuristicLab.Services.Hive.DataAccess.Interfaces {
  public interface IPersistenceManager : IDisposable {
    DataContext DataContext { get; }

    #region Hive daos
    AssignedJobResourceDao AssignedJobResourceDao { get; }
    AssignedProjectResourceDao AssignedProjectResourceDao { get; }
    DowntimeDao DowntimeDao { get; }
    JobDao JobDao { get; }
    JobPermissionDao JobPermissionDao { get; }
    LifecycleDao LifecycleDao { get; }
    PluginDao PluginDao { get; }
    PluginDataDao PluginDataDao { get; }
    ProjectDao ProjectDao { get; }
    ProjectPermissionDao ProjectPermissionDao { get; }
    RequiredPluginDao RequiredPluginDao { get; }
    ResourceDao ResourceDao { get; }
    SlaveDao SlaveDao { get; }
    SlaveGroupDao SlaveGroupDao { get; }
    StateLogDao StateLogDao { get; }
    TaskDao TaskDao { get; }
    TaskDataDao TaskDataDao { get; }
    UserPriorityDao UserPriorityDao { get; }
    #endregion

    #region HiveStatistics daos
    DimClientDao DimClientDao { get; }
    DimJobDao DimJobDao { get; }
    DimTimeDao DimTimeDao { get; }
    DimUserDao DimUserDao { get; }
    FactClientInfoDao FactClientInfoDao { get; }
    FactTaskDao FactTaskDao { get; }
    DimProjectDao DimProjectDao { get; }
    FactProjectInfoDao FactProjectInfoDao { get; }
    #endregion

    #region Transaction management
    void UseTransaction(Action call, bool repeatableRead = false, bool longRunning = false);
    T UseTransaction<T>(Func<T> call, bool repeatableRead = false, bool longRunning = false);
    #endregion

    TableInformation GetTableInformation(string table);
    void SubmitChanges();
  }
}
