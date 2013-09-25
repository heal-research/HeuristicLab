#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Common;

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

    private HiveServiceLocator() {
    }

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

    public string WorkingEndpoint { get; private set; }

    private HiveServiceClient NewServiceClient() {
      if (EndpointRetries >= Settings.Default.MaxEndpointRetries) {
        return CreateClient(WorkingEndpoint);
      }

      var configurations = Settings.Default.EndpointConfigurationPriorities;

      Exception exception = null;
      foreach (var endpointConfigurationName in configurations) {
        try {
          var cl = CreateClient(endpointConfigurationName);
          cl.Open();
          WorkingEndpoint = endpointConfigurationName;
          return cl;
        }
        catch (Exception exc) {
          exception = exc;
          EndpointRetries++;
        }
      }

      throw exception ?? new Exception("No endpoint for Hive service found.");
    }

    private HiveServiceClient CreateClient(string endpointConfigurationName) {
      HiveServiceClient cl = null;

      if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        cl = ClientFactory.CreateClient<HiveServiceClient, IHiveService>(endpointConfigurationName);
      else
        cl = ClientFactory.CreateClient<HiveServiceClient, IHiveService>(endpointConfigurationName, null, username, password);

      return cl;
    }

    public T CallHiveService<T>(Func<IHiveService, T> call) {
      HiveServiceClient client = NewServiceClient();
      HandleAnonymousUser(client);

      try {
        return call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }

    public void CallHiveService(Action<IHiveService> call) {
      HiveServiceClient client = NewServiceClient();
      HandleAnonymousUser(client);

      try {
        call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }

    private void HandleAnonymousUser(HiveServiceClient client) {
      if (client.ClientCredentials.UserName.UserName == Settings.Default.AnonymousUserName) {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
        throw new AnonymousUserException();
      }
    }
  }
}
