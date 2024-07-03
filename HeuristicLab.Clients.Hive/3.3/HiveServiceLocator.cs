#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.ServiceModel;
using HeuristicLab.Clients.Common;
using Microsoft.Extensions.Configuration;

namespace HeuristicLab.Clients.Hive {
  public class HiveServiceLocator : IHiveServiceLocator {
    private static IHiveServiceLocator instance = null;
    public static IHiveServiceLocator Instance {
      get {
        if (instance == null) {
          instance = new HiveServiceLocator();
        }
        return instance;
      }
    }

    private HiveServiceLocator() { }

    private string username;
    public string Username {
      get { return username; }
      set { username = value; }
    }

    private string password;
    public string Password {
      get { return password; }
      set { password = value; }
    }

    public int EndpointRetries { get; private set; }

    public WCFClientConfiguration WorkingEndpoint { get; private set; }

    private static readonly Lazy<IConfigurationRoot> configurationRoot = new Lazy<IConfigurationRoot>(() => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

    private static List<WCFClientConfiguration> GetConfigurations() {
      configurationRoot.Value.Reload();
      return configurationRoot.Value.GetRequiredSection("HiveService").Get<List<WCFClientConfiguration>>();
    }

    public string GetEndpointInformation() {
      string message = "Configured endpoints: " + Environment.NewLine;

      foreach (var config in GetConfigurations()) {
        message += "HiveService: " + config.Address + Environment.NewLine;
      }

      if (WorkingEndpoint is null) {
        message += "No working endpoint found, check you configuration.";
      } else {
        message += "Used endpoint: " + WorkingEndpoint;
      }

      return message;
    }

    private HiveServiceClient NewServiceClient() {
      if (EndpointRetries >= Settings.Default.MaxEndpointRetries) {
        return CreateClient(WorkingEndpoint);
      }

      Exception exception = null;
      foreach (var config in GetConfigurations()) {
        try {
          var cl = CreateClient(config);
          cl.Open();
          WorkingEndpoint = config;
          return cl;
        } catch (EndpointNotFoundException exc) {
          exception = exc;
          EndpointRetries++;
        }
      }

      throw exception ?? new Exception("No endpoint for Hive service found.");
    }

    private HiveServiceClient CreateClient(WCFClientConfiguration config) {
      HiveServiceClient cl = null;

      if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        cl = ClientFactory.CreateCoreClient<HiveServiceClient, IHiveService>(config);
      else
        cl = ClientFactory.CreateCoreClient<HiveServiceClient, IHiveService>(config, username, password);

      return cl;
    }

    public T CallHiveService<T>(Func<IHiveService, T> call) {
      HiveServiceClient client = NewServiceClient();
      HandleAnonymousUser(client);

      try {
        return call(client);
      } finally {
        try {
          client.Close();
        } catch (Exception) {
          client.Abort();
        }
      }
    }

    public void CallHiveService(Action<IHiveService> call) {
      HiveServiceClient client = NewServiceClient();
      HandleAnonymousUser(client);

      try {
        call(client);
      } finally {
        try {
          client.Close();
        } catch (Exception) {
          client.Abort();
        }
      }
    }

    private void HandleAnonymousUser(HiveServiceClient client) {
      if (client.ClientCredentials.UserName.UserName == Settings.Default.AnonymousUserName) {
        try {
          client.Close();
        } catch (Exception) {
          client.Abort();
        }
        throw new AnonymousUserException();
      }
    }
  }
}
