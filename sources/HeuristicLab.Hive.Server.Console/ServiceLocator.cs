using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.Hive.Contracts.Interfaces;
using System.ServiceModel;

namespace HeuristicLab.Hive.Server.Console {
  internal class ServiceLocator {
    private static IClientManager clientManager = null;
    private static IJobManager jobManager = null;
    private static IUserRoleManager userManager = null;

    internal static string Address { get; set; }
    internal static string Port { get; set; }

    internal static IClientManager GetClientManager() {
      if (clientManager == null && 
        Address != String.Empty &&
        Port != String.Empty) {
        ChannelFactory<IClientManager> factory =
          new ChannelFactory<IClientManager>(
            new NetTcpBinding(),
            new EndpointAddress("net.tcp://" + Address + ":" + Port + "/HiveServerConsole/ClientManager"));
        
        clientManager = factory.CreateChannel();
      }

      return clientManager;
    }

    internal static IJobManager GetJobManager() {
      if (jobManager == null &&
        Address != String.Empty &&
        Port != String.Empty) {
        ChannelFactory<IJobManager> factory =
          new ChannelFactory<IJobManager>(
            new NetTcpBinding(),
            new EndpointAddress("net.tcp://" + Address + ":" + Port + "/HiveServerConsole/JobManager"));

        jobManager = factory.CreateChannel();
      }

      return jobManager;
    }

    internal static IUserRoleManager GetUserRoleManager() {
      if (userManager == null &&
        Address != String.Empty &&
        Port != String.Empty) {
        ChannelFactory<IUserRoleManager> factory =
          new ChannelFactory<IUserRoleManager>(
            new NetTcpBinding(),
            new EndpointAddress("net.tcp://" + Address + ":" + Port + "/HiveServerConsole/UserRoleManager"));

        userManager = factory.CreateChannel();
      }

      return userManager;
    }
  }
}
