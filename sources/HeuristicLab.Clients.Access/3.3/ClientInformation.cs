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

using System;
using System.ServiceModel.Security;

namespace HeuristicLab.Clients.Access {
  public sealed class ClientInformation {
    private static ClientInformation instance;
    public static ClientInformation Instance {
      get {
        if (instance == null) instance = new ClientInformation();
        return instance;
      }
    }

    private Client clientInfo;
    public Client ClientInfo {
      get { return clientInfo; }
    }

    private bool clientExists = false;
    public bool ClientExists {
      get { return clientExists; }
    }

    private bool errorOccured = false;
    public bool ErrorOccured {
      get { return errorOccured; }
    }

    private Exception occuredException;
    public Exception OccuredException {
      get { return occuredException; }
    }

    private ClientInformation() {
      if (ClientInformationUtils.IsClientHeuristicLab()) {
        FetchClientInformationFromServer();
      } else {
        // this means we are executed by an Hive slave, therefore we just get our machine id (e.g. for OKB Algs)
        // because the slave has already done the registration process
        GenerateLocalClientConfig();
      }
    }

    private void GenerateLocalClientConfig() {
      clientExists = true;
      errorOccured = false;
      occuredException = null;
      clientInfo = new Client();
      clientInfo.Id = ClientInformationUtils.GetUniqueMachineId();
    }

    private void FetchClientInformationFromServer() {
      Guid clientId = ClientInformationUtils.GetUniqueMachineId();

      try {
        AccessClient.CallAccessService(x => clientInfo = x.GetClient(clientId));
        if (clientInfo != null) {
          clientExists = true;
          clientInfo.HeuristicLabVersion = ClientInformationUtils.GetHLVersion();
          AccessClient.CallAccessService(x => x.UpdateClient(clientInfo));
        }
        errorOccured = false;
        occuredException = null;
      }
      catch (MessageSecurityException e) {
        //wrong username or password
        clientExists = false;
        errorOccured = true;
        occuredException = e;
      }
      catch (Exception e) {
        clientExists = false;
        errorOccured = true;
        occuredException = e;
      }
    }

    public void Refresh() {
      FetchClientInformationFromServer();
    }
  }
}
