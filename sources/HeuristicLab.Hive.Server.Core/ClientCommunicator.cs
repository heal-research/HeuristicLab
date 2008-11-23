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
    List<ClientInfo> clients;
    LinkedList<long> jobs;
    int nrOfJobs = 10;

    public ClientCommunicator() {
      jobs = new LinkedList<long>();
      for (long i = 1; i < nrOfJobs; i++) {
        jobs.AddFirst(i);
      }
    }

    #region IClientCommunicator Members

    public Response Login(ClientInfo clientInfo) {
      if (clients == null)
        clients = new List<ClientInfo>();

      clients.Add(clientInfo);

      Response response = new Response();
      response.Success = true;
      response.StatusMessage = "Client with GUID " + clientInfo.ClientId + " successuflly logged in";

      return response;
    }

    public ResponseHB SendHeartBeat(HeartBeatData hbData) {
      ResponseHB response = new ResponseHB();

      response.Success = true;
      response.StatusMessage = "HeartBeat received";
      response.ActionRequest = new List<MessageContainer>();
      if (jobs.Count > 0) 
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.FetchJob));
      else
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.NoMessage));

      return response;
    }

    public ResponseJob PullJob(Guid clientId) {
      ResponseJob response = new ResponseJob();
      lock (this) {
        response.JobId = jobs.Last.Value;
        jobs.RemoveLast(); 
      }
      
      response.Success = true;
      response.StatusMessage = "Job with id " + jobs.Count + " sent";     
      return response;
    }

    public Response SendJobResult(JobResult Result, bool finished) {
      Response response = new Response();
      response.Success = true;
      response.StatusMessage = "Thanks for calculating";

      return response;
    }
                           
    public Response Logout(Guid clientId) {
      bool clientRemoved = false;
      foreach (ClientInfo client in clients) {
        if (client.ClientId.Equals(clientId)) {
          clients.Remove(client);
          clientRemoved = true;
        }
      }
      Response response = new Response();
      
      if (clientRemoved) {
        response.Success = true;
        response.StatusMessage = "Successfully logged out. Good bye";
      } else {
        response.Success = false;
        response.StatusMessage = "Sorry, but you weren't logged in";
      }
      return response;
    }

    #endregion
  }
}
