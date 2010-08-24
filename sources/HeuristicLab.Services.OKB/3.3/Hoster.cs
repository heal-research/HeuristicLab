#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using log4net;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// Simple main class for self hosting the OKB service with all necessary
  /// services for full operation.
  /// </summary>
  public class Hoster {

    // you might have to run
    // netsh http add urlacl url=http://+:8000/OKB/ user=DOMAIN\username
    // as administrator
    /// <summary>
    /// Main method to start the services.
    /// </summary>
    /// <param name="args">The args.</param>
    public static void Main(string[] args) {
      InitializeLogging();
      StartService(typeof(QueryService));
      StartService(typeof(RunnerService));
      StartService(typeof(AdminService));
      StartService(typeof(TableService));
      StartService(typeof(DataService));
      AppDomain.CurrentDomain.UnhandledException += UnhandledException;
      Console.ReadLine();
    }

    static ILog logger = log4net.LogManager.GetLogger("HeuristicLab.Services.OKB.Hoster");

    static void UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      logger.Warn("unhandled exception, e");
    }

    private static void InitializeLogging() {
      log4net.Config.BasicConfigurator.Configure();
    }

    private static void StartService(Type type) {
      ServiceHost host = new ServiceHost(type);
      try {
        host.Open();
        PrintConfig(host);
      }
      catch (AddressAccessDeniedException x) {
        Console.WriteLine(
          "Could not process listerner request. Try running the following as Administrator:\n\n" +
          "netsh http add urlacl url=http://+port/OKB/ user=DOMAIN\\username\n\n" +
          x.ToString());
      }
      catch (Exception x) {
        Console.WriteLine("Aborting host of type {0} due to {1}",
          host.GetType(),
          x.ToString());
        host.Abort();
      }
    }

    static void PrintConfig(ServiceHost host) {
      Console.WriteLine("{0}", host.Description.ServiceType.Name);
      Console.WriteLine("\n  endpoints:");
      foreach (ServiceEndpoint se in host.Description.Endpoints) {
        Console.WriteLine("    {0}", se.Address);
      }
      Console.WriteLine("\n  methods:");
      foreach (MethodInfo mi in host.Description.ServiceType.GetMethods(
          BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)) {
        StringBuilder sb = new StringBuilder();
        foreach (ParameterInfo pi in mi.GetParameters()) {
          sb.Append(pi.ParameterType.Name).Append(' ').Append(pi.Name).Append(", ");
        }
        if (sb.Length > 2)
          sb.Remove(sb.Length - 2, 2);
        Console.WriteLine("    {0}({1}) : {2}", mi.Name, sb, mi.ReturnType.Name);
      }
      Console.WriteLine();
    }

  }
}
