using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Server.Core {
  /// <summary>
  /// The ClientCommunicator manages the whole communication with the client
  /// </summary>
  public class ClientCommunicator: IClientCommunicator {
    #region IClientCommunicator Members

    public Response Login(Client clientInfo) {
      Response response = new Response();
      response.Success = true;
      response.StatusMessage = "Logged in...";

      return response;
    }

    public ResponseHB SendHeartBeat(HeartBeatData hbData) {
      throw new NotImplementedException();
    }

    public ResponseJob PullJob(Guid clientId) {
      throw new NotImplementedException();
    }

    public Response SendJobResult(JobResult Result, bool finished) {
      throw new NotImplementedException();
    }

    public Response Logout(Guid clientId) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
