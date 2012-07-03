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

using HeuristicLab.Services.Hive.DataAccess;

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

    private IHiveDao hiveDao;
    public IHiveDao HiveDao {
      get {
        if (hiveDao == null) hiveDao = new HiveDao();
        return hiveDao;
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

    private ITransactionManager transactionManager;
    public ITransactionManager TransactionManager {
      get {
        if (transactionManager == null) transactionManager = new TransactionManager();
        return transactionManager;
      }
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
  }
}
