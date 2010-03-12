using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Server.LINQDataAccess;

namespace HeuristicLab.Hive.Server.Core {
  public class DaoLocator {
    
    [ThreadStatic] private static IClientDao clientDao;
    [ThreadStatic] private static IClientConfigDao clientConfigDao;
    [ThreadStatic] private static IClientGroupDao clientGroupDao;
    [ThreadStatic] private static IJobDao jobDao;
    [ThreadStatic] private static IPluginInfoDao pluginInfoDao;

    public static IClientDao ClientDao {
      get {
        if (clientDao == null)
          clientDao = new ClientDao();
        return clientDao;
      }
    }

    public static IClientConfigDao ClientConfigDao {
      get {
        if(clientConfigDao == null)
          clientConfigDao = new ClientConfigDao();
        return clientConfigDao;
      }
    }

    public static IClientGroupDao ClientGroupDao {
      get {
        if(clientGroupDao == null)
          clientGroupDao = new ClientGroupDao();
        return clientGroupDao;
      }
    }

    public static IJobDao JobDao {
      get {
        if(jobDao == null)
          jobDao = new JobDao();
        return jobDao;
      }
    }

    public static IPluginInfoDao PluginInfoDao {
      get {
        if(pluginInfoDao == null)
          pluginInfoDao = new PluginInfoDao();
        return pluginInfoDao;
      }
    }

    public static void DestroyContext() {
      if (ContextFactory.Context != null) {
        ContextFactory.Context.Dispose();
        ContextFactory.Context = null;
      }
    }

  }
}
