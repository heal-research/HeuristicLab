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

using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using HeuristicLab.Services.Hive.DataAccess.Manager;
using HeuristicLab.Services.Hive.Manager;

namespace HeuristicLab.Services.Hive {
  public class ServiceLocator : IServiceLocator {
    private static IServiceLocator instance;
    public static IServiceLocator Instance {
      get {
        if (instance == null) instance = new ServiceLocator();
        return instance;
      }
      set { instance = value; }
    }

    public IPersistenceManager PersistenceManager {
      get {
        var dataContext = HiveOperationContext.Current != null
                            ? HiveOperationContext.Current.DataContext
                            : new HiveDataContext(Settings.Default.HeuristicLab_Hive_LinqConnectionString);
        return new PersistenceManager(dataContext);
      }
    }

    private Access.IRoleVerifier roleVerifier;
    public Access.IRoleVerifier RoleVerifier {
      get {
        if (roleVerifier == null) roleVerifier = new Access.RoleVerifier();
        return roleVerifier;
      }
    }

    private IAuthorizationManager authorizationManager;
    public IAuthorizationManager AuthorizationManager {
      get {
        if (authorizationManager == null) authorizationManager = new AuthorizationManager();
        return authorizationManager;
      }
    }

    private IEventManager eventManager;
    public IEventManager EventManager {
      get {
        if (eventManager == null) eventManager = new EventManager();
        return eventManager;
      }
    }

    private IStatisticsGenerator statisticsGenerator;
    public IStatisticsGenerator StatisticsGenerator {
      get { return statisticsGenerator ?? (statisticsGenerator = new HiveStatisticsGenerator()); }
    }

    private Access.IUserManager userManager;
    public Access.IUserManager UserManager {
      get {
        if (userManager == null) userManager = new Access.UserManager();
        return userManager;
      }
    }

    private HeartbeatManager heartbeatManager;
    public HeartbeatManager HeartbeatManager {
      get {
        if (heartbeatManager == null) heartbeatManager = new HeartbeatManager();
        return heartbeatManager;
      }
    }

    private ITaskScheduler taskScheduler;
    public ITaskScheduler TaskScheduler {
      get {
        if (taskScheduler == null) taskScheduler = new RoundRobinTaskScheduler();
        return taskScheduler;
      }
    }
  }
}
