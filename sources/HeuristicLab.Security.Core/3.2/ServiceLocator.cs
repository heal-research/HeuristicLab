using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.DataAccess.Interfaces;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;

namespace HeuristicLab.Security.Core {
  class ServiceLocator {
    private static ISessionFactory sessionFactory = null;

    /// <summary>
    /// Gets the db session factory
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static ISessionFactory GetSessionFactory() {
      if (sessionFactory == null) {
        sessionFactory =
          ApplicationManager.Manager.GetInstances<ISessionFactory>().First();

        sessionFactory.DbConnectionType =
          typeof(SqlConnection);

        sessionFactory.DbConnectionString =
          HeuristicLab.Security.Core.Properties.Settings.Default.SecurityServerConnectionString;
      }

      return sessionFactory;
    }
  }
}
