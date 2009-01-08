using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Core;
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using System.Resources;
using System.Reflection;
using HeuristicLab.Hive.JobBase;
using System.Runtime.CompilerServices;

namespace HeuristicLab.Hive.Server.Core {
  /// <summary>
  /// The ClientCommunicator manages the whole communication with the client
  /// </summary>
  public class ClientCommunicator: IClientCommunicator {
    int nrOfJobs = 0;
    Dictionary<Guid, DateTime> lastHeartbeats;

    IClientAdapter clientAdapter;
    IJobAdapter jobAdapter;
    IJobResultsAdapter jobResultAdapter;
    ILifecycleManager lifecycleManager;

    public ClientCommunicator() {
      clientAdapter = ServiceLocator.GetClientAdapter();
      jobAdapter = ServiceLocator.GetJobAdapter();
      jobResultAdapter = ServiceLocator.GetJobResultsAdapter();
      lifecycleManager = ServiceLocator.GetLifecycleManager();

      lifecycleManager.OnServerHeartbeat += 
        new EventHandler(lifecycleManager_OnServerHeartbeat);

      for (int i = 0; i < nrOfJobs; i++) {
        Job job = new Job();
        job.Id = i;
        job.State = State.offline;
        jobAdapter.Update(job);
      }
      lastHeartbeats = new Dictionary<Guid, DateTime>();

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    void lifecycleManager_OnServerHeartbeat(object sender, EventArgs e) {
      List<ClientInfo> allClients = new List<ClientInfo>(clientAdapter.GetAll());
      List<Job> allJobs = new List<Job>(jobAdapter.GetAll());

      foreach (ClientInfo client in allClients) {
        if (client.State != State.offline && client.State != State.nullState) {
          if (!lastHeartbeats.ContainsKey(client.ClientId)) {
            client.State = State.offline;
            clientAdapter.Update(client);
          } else {
            DateTime lastHbOfClient = lastHeartbeats[client.ClientId];
            int diff = lastHbOfClient.CompareTo(DateTime.Now);
            Console.WriteLine(diff);
          }
        } else {
          if (lastHeartbeats.ContainsKey(client.ClientId))
            lastHeartbeats.Remove(client.ClientId);
        }
      }
    }

    #region IClientCommunicator Members

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Response Login(ClientInfo clientInfo) {
      Response response = new Response();

      if (lastHeartbeats.ContainsKey(clientInfo.ClientId)) {
        lastHeartbeats[clientInfo.ClientId] = DateTime.Now;
      } else {
        lastHeartbeats.Add(clientInfo.ClientId, DateTime.Now);
      }

      ICollection<ClientInfo> allClients = clientAdapter.GetAll();
      ClientInfo client = clientAdapter.GetById(clientInfo.ClientId);
      if (client != null && client.State != State.offline && client.State != State.nullState) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGIN_USER_ALLREADY_ONLINE;
        return response;
      }
      clientInfo.State = State.idle;
      clientAdapter.Update(clientInfo);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGIN_SUCCESS;

      return response;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public ResponseHB SendHeartBeat(HeartBeatData hbData) {
      ResponseHB response = new ResponseHB();

      response.ActionRequest = new List<MessageContainer>();
      if (clientAdapter.GetById(hbData.ClientId).State == State.offline ||
          clientAdapter.GetById(hbData.ClientId).State == State.nullState) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_USER_NOT_LOGGED_IN;
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.NoMessage));
        return response;
      }

      if (lastHeartbeats.ContainsKey(hbData.ClientId)) {
        lastHeartbeats[hbData.ClientId] = DateTime.Now;
      } else {
        lastHeartbeats.Add(hbData.ClientId, DateTime.Now);
      }

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_HARDBEAT_RECEIVED;
      List<Job> allOfflineJobs = new List<Job>(jobAdapter.GetJobsByState(State.offline));
      if (allOfflineJobs.Count > 0 && hbData.freeCores > 0) 
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.FetchJob));
      else
        response.ActionRequest.Add(new MessageContainer(MessageContainer.MessageType.NoMessage));

      return response;
    }

    public ResponseJob PullJob(Guid clientId) {
      ResponseJob response = new ResponseJob();
      lock (this) {
        LinkedList<Job> allOfflineJobs = new LinkedList<Job>(jobAdapter.GetJobsByState(State.offline));
        if (allOfflineJobs != null && allOfflineJobs.Count > 0) {
          Job job2Calculate = allOfflineJobs.First.Value;
          job2Calculate.State = State.calculating;
          response.Job = job2Calculate;
          jobAdapter.Update(job2Calculate);          
          response.SerializedJob = PersistenceManager.SaveToGZip(new TestJob());
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_PULLED;
          return response;
        } 
      }
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT;
      return response;
    }

    public ResponseResultReceived SendJobResult(JobResult result, bool finished) {
      ResponseResultReceived response = new ResponseResultReceived();
      if (result.Id != 0) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_ID_MUST_NOT_BE_SET;
        return response;
      }
      Job job = result.Job;
      if (job == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JO_WITH_THIS_ID;
        return response;
      }
      if (job.State != State.calculating) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_WRONG_JOB_STATE;
        return response;
      }
      if (finished) {
        job.State = State.finished;
        jobAdapter.Update(job);

        List<JobResult> jobResults = new List<JobResult>(jobResultAdapter.GetResultsOf(job));
        foreach (JobResult currentResult in jobResults) 
          jobResultAdapter.Delete(currentResult);
      }
      jobResultAdapter.Update(result);    

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED;
      response.Job = result.Job;

      return response;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]                       
    public Response Logout(Guid clientId) {
      Response response = new Response();

      if (lastHeartbeats.ContainsKey(clientId))
        lastHeartbeats.Remove(clientId);

      ClientInfo client = clientAdapter.GetById(clientId);
      if (client == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGOUT_CLIENT_NOT_REGISTERED;
        return response;
      }
      client.State = State.offline;
      clientAdapter.Update(client);

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGOUT_SUCCESS;
      
      return response;
    }

    #endregion
  }
}
