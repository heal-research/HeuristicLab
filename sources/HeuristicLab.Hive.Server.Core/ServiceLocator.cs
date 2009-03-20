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

/// <summary>
/// The service locator for the server core
/// </summary>
public class ServiceLocator {
  private static DiscoveryService discoveryService =
    new DiscoveryService();

  private static IDBSynchronizer transManager = null;

  private static IClientManager clientManager = null;

  private static IJobManager jobManager = null;

  private static IClientCommunicator clientCommunicator = null;

  private static ILifecycleManager lifecycleManager = null;

  private static IClientAdapter clientAdapter = null;

  private static IClientGroupAdapter clientGroupAdapter = null;

  private static IResourceAdapter resourceAdapter = null;

  private static IJobAdapter jobAdapter = null;

  private static IJobResultsAdapter jobResultsAdapter = null;

  private static IScheduler scheduler = null;


  /// <summary>
  /// Gets the db transaction manager
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IDBSynchronizer GetDBSynchronizer() {
    if (transManager == null) {
      transManager = discoveryService.GetInstances<IDBSynchronizer>()[0];
    }

    return transManager;
  }

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
  /// Gets the client database adapter
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IClientAdapter GetClientAdapter() {
    if (clientAdapter == null) {
      clientAdapter = discoveryService.GetInstances<IClientAdapter>()[0];
    }

    return clientAdapter;
  }

  /// <summary>
  /// Gets the client group database adapter
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IClientGroupAdapter GetClientGroupAdapter() {
    if (clientGroupAdapter == null) {
      clientGroupAdapter = discoveryService.GetInstances<IClientGroupAdapter>()[0];
    }

    return clientGroupAdapter;
  }

  /// <summary>
  /// Gets the resource database adapter
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IResourceAdapter GetResourceAdapter() {
    if (resourceAdapter == null) {
      resourceAdapter = discoveryService.GetInstances<IResourceAdapter>()[0];
    }

    return resourceAdapter;
  }

  /// <summary>
  /// Gets the job database adapter
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IJobAdapter GetJobAdapter() {
    if (jobAdapter == null) {
      jobAdapter = discoveryService.GetInstances<IJobAdapter>()[0];
    }

    return jobAdapter;
  }

  /// <summary>
  /// Gets the job results database adapter
  /// </summary>
  /// <returns></returns>
  [MethodImpl(MethodImplOptions.Synchronized)]
  public static IJobResultsAdapter GetJobResultsAdapter() {
    if (jobResultsAdapter == null) {
      jobResultsAdapter = discoveryService.GetInstances<IJobResultsAdapter>()[0];
    }

    return jobResultsAdapter;
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