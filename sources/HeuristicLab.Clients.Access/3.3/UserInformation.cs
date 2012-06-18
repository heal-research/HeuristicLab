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
  public sealed class UserInformation {
    public const string AdministratorRoleName = "AccessService Administrator";

    private static UserInformation instance;
    public static UserInformation Instance {
      get {
        if (instance == null) instance = new UserInformation();
        return instance;
      }
    }

    private string userName;
    public string UserName {
      get { return userName; }
    }

    private bool userExists = false;
    public bool UserExists {
      get { return userExists; }
    }

    private LightweightUser user;
    public LightweightUser User {
      get { return user; }
    }

    private bool errorOccured;
    public bool ErrorOccured {
      get { return errorOccured; }
    }

    private Exception occuredException;
    public Exception OccuredException {
      get { return occuredException; }
    }

    private UserInformation() {
      //this blocks, so there should be anywhere in the Optimizer startup process
      //a call to FetchUserInformationFromServerAsync which is non-blocking
      FetchUserInformationFromServer();
    }


    private void FetchUserInformationFromServer() {
      userName = HeuristicLab.Clients.Common.Properties.Settings.Default.UserName;

      try {
        AccessClient.CallAccessService(x => user = x.Login());
        errorOccured = false;
        userExists = true;
        occuredException = null;
      }
      catch (MessageSecurityException e) {
        //wrong username or password
        errorOccured = false;
        userExists = false;
        occuredException = e;
      }
      catch (Exception e) {
        errorOccured = true;
        userExists = false;
        occuredException = e;
      }
    }

    public void Refresh() {
      FetchUserInformationFromServer();
    }
  }
}
