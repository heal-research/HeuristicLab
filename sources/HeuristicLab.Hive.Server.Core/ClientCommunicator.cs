﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.Interfaces;

namespace HeuristicLab.Hive.Server.Core {
  class ClientCommunicator: IClientCommunicator {
    #region IClientCommunicator Members

    public Response Login(HeuristicLab.Hive.Server.BusinessObjects.Client clientInfo) {
      Response response = new Response();
      response.Success = true;
      response.StatusMessage = "Logged in...";

      return response;
    }

    #endregion
  }
}
