#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.PluginInfrastructure;
using System.Runtime.CompilerServices;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Server.Core;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using HeuristicLab.DataAccess.Interfaces;
using System.Data.SqlClient;

/// <summary>
/// The service locator for the server core
/// </summary>
public class ServiceLocator {
  private static DiscoveryService discoveryService =
    new DiscoveryService();

  private static IClientManager clientManager = null;

  private static IJobManager jobManager = null;

  private static IClientCommunicator clientCommunicator = null;

  private static ILifecycleManager lifecycleManager = null;

  private static ISessionFactory sessionFactory = null;

  private static IScheduler scheduler = null;

  /// <summary>
  /// Gets the client manager
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IClientManager GetClientManager() {
    if (clientManager == null)
      clientManager = new ClientManager();

    return clientManager;
  }

  /// <summary>
  /// Gets the job manager
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IJobManager GetJobManager() {
    if (jobManager == null)
      jobManager = new JobManager();

    return jobManager;
  }

  /// <summary>
  /// Gets the client Communicator
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IClientCommunicator GetClientCommunicator() {
    if (clientCommunicator == null)
      clientCommunicator = new ClientCommunicator();

    return clientCommunicator;
  }

  /// <summary>
  /// Gets the lifecycle manager
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static ILifecycleManager GetLifecycleManager() {
    if (lifecycleManager == null) {
      lifecycleManager = new LifecycleManager();
    }

    return lifecycleManager;
  }

  /// <summary>
  /// Gets the db session factory
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static ISessionFactory GetSessionFactory() {
    if (sessionFactory == null) {
      sessionFactory = 
        discoveryService.GetInstances<ISessionFactory>()[0];

      sessionFactory.DbConnectionType =
        typeof(SqlConnection);
      
      sessionFactory.DbConnectionString =
        HeuristicLab.Hive.Server.Core.Properties.Settings.Default.HiveServerConnectionString;
    }

    return sessionFactory;
  }

  /// <summary>
  /// Gets the scheduler
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IScheduler GetScheduler() {
    if (scheduler == null) {
      scheduler = discoveryService.GetInstances<IScheduler>()[0];
    }

    return scheduler;
  }
}