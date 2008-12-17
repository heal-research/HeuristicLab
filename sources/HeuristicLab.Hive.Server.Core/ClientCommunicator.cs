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

namespace HeuristicLab.Hive.Server.Core {
  /// <summary>
  /// The ClientCommunicator manages the whole communication with the client
  /// </summary>
  public class ClientCommunicator: IClientCommunicator {
    int nrOfJobs = 10;

    IClientAdapter clientAdapter;
    IJobAdapter jobAdapter;
    IJobResultsAdapter jobResultAdapter;

    public ClientCommunicator() {
      clientAdapter = ServiceLocator.GetClientAdapter();
      jobAdapter = ServiceLocator.GetJobAdapter();

      for (int i = 0; i < nrOfJobs; i++) {
        Job job = new Job();
        job.Id = i;
        job.State = State.offline;
        jobAdapter.Update(job);
      }

    }

    #region IClientCommunicator Members

    public Response Login(ClientInfo clientInfo) {
      Response response = new Response();

      ICollection<ClientInfo> allClients = clientAdapter.GetAll();
      ClientInfo client = clientAdapter.GetById(clientInfo.ClientId);
      if (client != null && client.State != State.offline) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGIN_USER_ALLREADY_ONLINE;
        return response;
      }
      clientAdapter.Update(clientInfo);
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_LOGIN_SUCCESS;

      return response;
    }

    public ResponseHB SendHeartBeat(HeartBeatData hbData) {
      ResponseHB response = new ResponseHB();

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_HARDBEAT_RECEIVED;
      response.ActionRequest = new List<MessageContainer>();
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
          response.JobId = job2Calculate.Id;
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
      Job job = jobAdapter.GetById(result.JobId);
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
      response.JobId = result.JobId;

      return response;
    }
                           
    public Response Logout(Guid clientId) {
      Response response = new Response();
      
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
