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
using System.ServiceModel;
using HeuristicLab.Clients.Common.Properties;

namespace HeuristicLab.Clients.Common {
  public static class ClientFactory {
    public static I CreateClient<T, I>()
      where T : ClientBase<I>, I
      where I : class {
      return CreateClient<T, I>(null, null);
    }
    public static I CreateClient<T, I>(string endpointConfigurationName)
      where T : ClientBase<I>, I
      where I : class {
      return CreateClient<T, I>(endpointConfigurationName, null);
    }
    public static I CreateClient<T, I>(string endpointConfigurationName, string remoteAddress)
      where T : ClientBase<I>, I
      where I : class {
      T client;
      if (string.IsNullOrEmpty(endpointConfigurationName)) {
        client = Activator.CreateInstance<T>();
      } else {
        client = (T)Activator.CreateInstance(typeof(T), endpointConfigurationName);
      }

      if (!string.IsNullOrEmpty(remoteAddress)) {
        client.Endpoint.Address = new EndpointAddress(remoteAddress);
      }

      client.ClientCredentials.UserName.UserName = Settings.Default.UserName;
      client.ClientCredentials.UserName.Password = Settings.Default.Password;
      client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
      return (I)client;
    }
  }
}
