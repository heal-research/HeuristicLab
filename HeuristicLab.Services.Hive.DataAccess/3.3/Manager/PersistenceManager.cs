#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Transactions;
using HeuristicLab.Services.Hive.DataAccess.Daos;
using HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics;
using HeuristicLab.Services.Hive.DataAccess.Data;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;

namespace HeuristicLab.Services.Hive.DataAccess.Manager {
  public class PersistenceManager : IPersistenceManager {
    private readonly DataContext dataContext;
    public DataContext DataContext {
      get { return dataContext; }
    }

    #region Hive daos

    private AssignedResourceDao assignedResourceDao;
    public AssignedResourceDao AssignedResourceDao {
      get { return assignedResourceDao ?? (assignedResourceDao = new AssignedResourceDao(dataContext)); }
    }

    private DowntimeDao downtimeDao;
    public DowntimeDao DowntimeDao {
      get { return downtimeDao ?? (downtimeDao = new DowntimeDao(dataContext)); }
    }

    private JobDao jobDao;
    public JobDao JobDao {
      get { return jobDao ?? (jobDao = new JobDao(dataContext)); }
    }

    private JobPermissionDao jobPermissionDao;
    public JobPermissionDao JobPermissionDao {
      get { return jobPermissionDao ?? (jobPermissionDao = new JobPermissionDao(dataContext)); }
    }

    private LifecycleDao lifecycleDao;
    public LifecycleDao LifecycleDao {
      get { return lifecycleDao ?? (lifecycleDao = new LifecycleDao(dataContext)); }
    }

    private PluginDao pluginDao;
    public PluginDao PluginDao {
      get { return pluginDao ?? (pluginDao = new PluginDao(dataContext)); }
    }

    private PluginDataDao pluginDataDao;
    public PluginDataDao PluginDataDao {
      get { return pluginDataDao ?? (pluginDataDao = new PluginDataDao(dataContext)); }
    }

    private RequiredPluginDao requiredPluginDao;
    public RequiredPluginDao RequiredPluginDao {
      get { return requiredPluginDao ?? (requiredPluginDao = new RequiredPluginDao(dataContext)); }
    }

    private ResourceDao resourceDao;
    public ResourceDao ResourceDao {
      get { return resourceDao ?? (resourceDao = new ResourceDao(dataContext)); }
    }

    private ResourcePermissionDao resourcePermissionDao;
    public ResourcePermissionDao ResourcePermissionDao {
      get { return resourcePermissionDao ?? (resourcePermissionDao = new ResourcePermissionDao(dataContext)); }
    }

    private SlaveDao slaveDao;
    public SlaveDao SlaveDao {
      get { return slaveDao ?? (slaveDao = new SlaveDao(dataContext)); }
    }

    private SlaveGroupDao slaveGroupDao;
    public SlaveGroupDao SlaveGroupDao {
      get { return slaveGroupDao ?? (slaveGroupDao = new SlaveGroupDao(dataContext)); }
    }

    private StateLogDao stateLogDao;
    public StateLogDao StateLogDao {
      get { return stateLogDao ?? (stateLogDao = new StateLogDao(dataContext)); }
    }

    private TaskDao taskDao;
    public TaskDao TaskDao {
      get { return taskDao ?? (taskDao = new TaskDao(dataContext)); }
    }

    private TaskDataDao taskDataDao;
    public TaskDataDao TaskDataDao {
      get { return taskDataDao ?? (taskDataDao = new TaskDataDao(dataContext)); }
    }

    private UserPriorityDao userPriorityDao;
    public UserPriorityDao UserPriorityDao {
      get { return userPriorityDao ?? (userPriorityDao = new UserPriorityDao(dataContext)); }
    }
    #endregion

    #region HiveStatistics daos

    private DimClientDao dimClientDao;
    public DimClientDao DimClientDao {
      get { return dimClientDao ?? (dimClientDao = new DimClientDao(dataContext)); }
    }

    private DimJobDao dimJobDao;
    public DimJobDao DimJobDao {
      get { return dimJobDao ?? (dimJobDao = new DimJobDao(dataContext)); }
    }

    private DimTimeDao dimTimeDao;
    public DimTimeDao DimTimeDao {
      get { return dimTimeDao ?? (dimTimeDao = new DimTimeDao(dataContext)); }
    }

    private DimUserDao dimUserDao;
    public DimUserDao DimUserDao {
      get { return dimUserDao ?? (dimUserDao = new DimUserDao(dataContext)); }
    }

    private FactClientInfoDao factClientInfoDao;
    public FactClientInfoDao FactClientInfoDao {
      get { return factClientInfoDao ?? (factClientInfoDao = new FactClientInfoDao(dataContext)); }
    }

    private FactTaskDao factTaskDao;
    public FactTaskDao FactTaskDao {
      get { return factTaskDao ?? (factTaskDao = new FactTaskDao(dataContext)); }
    }
    #endregion

    public PersistenceManager(bool longRunning = false) {
      var context = new HiveDataContext(Settings.Default.HeuristicLab_Hive_LinqConnectionString);
      if (longRunning) context.CommandTimeout = (int)Settings.Default.LongRunningDatabaseCommandTimeout.TotalSeconds;
      dataContext = context;
    }

    public PersistenceManager(DataContext dataContext) {
      this.dataContext = dataContext;
    }

    #region Transaction management
    public void UseTransaction(Action call, bool repeatableRead = false, bool longRunning = false) {
      UseTransaction<object>(() => {
        call();
        return null;
      });
    }

    public T UseTransaction<T>(Func<T> call, bool repeatableRead = false, bool longRunning = false) {
      int n = 10;
      while (n > 0) {
        TransactionScope transaction = CreateTransaction(repeatableRead, longRunning);
        try {
          T result = call();
          transaction.Complete();
          return result;
        }
        catch (System.Data.SqlClient.SqlException e) {
          n--; // probably deadlock situation, let it roll back and repeat the transaction n times
          LogFactory.GetLogger(typeof(TransactionManager).Namespace).Log(string.Format("Exception occured, repeating transaction {0} more times. Details: {1}", n, e.ToString()));
          if (n <= 0) throw;
        }
        finally {
          transaction.Dispose();
        }
      }
      throw new Exception("Transaction couldn't be completed.");
    }

    private static TransactionScope CreateTransaction(bool repeatableRead, bool longRunning) {
      var options = new TransactionOptions {
        IsolationLevel = repeatableRead ? IsolationLevel.RepeatableRead : IsolationLevel.ReadUncommitted
      };
      if (longRunning) {
        options.Timeout = Settings.Default.LongRunningDatabaseCommandTimeout;
      }
      return new TransactionScope(TransactionScopeOption.Required, options);
    }
    #endregion

    public TableInformation GetTableInformation(string table) {
      string query = string.Format("sp_spaceused '{0}'", table);
      var result = dataContext.ExecuteQuery<SqlServerTableInformation>(query).FirstOrDefault();
      if (result == null) return null;
      return new TableInformation {
        Name = result.Name,
        Rows = int.Parse(result.Rows.Remove(result.Rows.IndexOf(' '))),
        Reserved = int.Parse(result.Reserved.Remove(result.Reserved.IndexOf(' '))),
        Data = int.Parse(result.Data.Remove(result.Data.IndexOf(' '))),
        IndexSize = int.Parse(result.Index_Size.Remove(result.Index_Size.IndexOf(' '))),
        Unused = int.Parse(result.Unused.Remove(result.Unused.IndexOf(' ')))
      };
    }

    public void SubmitChanges() {
      if (dataContext != null) {
        dataContext.SubmitChanges();
      }
    }

    public void Dispose() {
      if (dataContext != null) {
        dataContext.Dispose();
      }
    }

    private class SqlServerTableInformation {
      public string Name { get; set; }
      public string Rows { get; set; }
      public string Reserved { get; set; }
      public string Data { get; set; }
      public string Index_Size { get; set; } // naming of sp_spaceused...
      public string Unused { get; set; }
    }
  }
}
