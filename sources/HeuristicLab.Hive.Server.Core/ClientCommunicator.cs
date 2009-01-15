#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    Dictionary<Guid, DateTime> lastHeartbeats = 
      new Dictionary<Guid,DateTime>();

    IClientAdapter clientAdapter;
    IJobAdapter jobAdapter;
    IJobResultsAdapter jobResultAdapter;
    ILifecycleManager lifecycleManager;

    /// <summary>
    /// Initialization of the Adapters to the database
    /// Initialization of Eventhandler for the lifecycle management
    /// Initialization of lastHearbeats Dictionary
    /// </summary>
    public ClientCommunicator() {
      clientAdapter = ServiceLocator.GetClientAdapter();
      jobAdapter = ServiceLocator.GetJobAdapter();
      jobResultAdapter = ServiceLocator.GetJobResultsAdapter();
      lifecycleManager = ServiceLocator.GetLifecycleManager();

      lifecycleManager.RegisterHeartbeat( 
        new EventHandler(lifecycleManager_OnServerHeartbeat));

      lastHeartbeats = new Dictionary<Guid, DateTime>();
    }

    /// <summary>
    /// Check if online clients send their hearbeats
    /// if not -> set them offline and check if they where calculating a job
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
            TimeSpan dif = DateTime.Now.Subtract(lastHbOfClient);
            // check if time between last hearbeat and now is greather than HEARTBEAT_MAX_DIF
            if (dif.Seconds > ApplicationConstants.HEARTBEAT_MAX_DIF) {
              // if client calculated jobs, the job must be reset
              if (client.State == State.calculating) {
                // check wich job the client was calculating and reset it
                foreach (Job job in allJobs) {
                  if (job.Client.ClientId == client.ClientId) {
                    resetJobsDependingOnResults(job);
                  }
                }
              }
              
              // client must be set offline
              client.State = State.offline;
              clientAdapter.Update(client);
              lastHeartbeats.Remove(client.ClientId);
            }
          }
        } else {
          if (lastHeartbeats.ContainsKey(client.ClientId))
            lastHeartbeats.Remove(client.ClientId);
        }
      }
    }

    private void resetJobsDependingOnResults(Job job) {
      List<JobResult> allJobResults = new List<JobResult>(jobResultAdapter.GetAll());
      JobResult lastJobResult = null;
      foreach (JobResult jR in allJobResults) {
        if (jR.Job != null && jR.Job.Id == job.Id) {
          if (lastJobResult != null) {
            // if lastJobResult was before the current jobResult the lastJobResult must be updated
            if ((jR.timestamp.Subtract(lastJobResult.timestamp)).Seconds > 0)
              lastJobResult = jR;
          }
        }
      }
      if (lastJobResult != null) {
        job.Client = null;
        job.Percentage = lastJobResult.Percentage;
        job.State = State.idle;
        job.SerializedJob = lastJobResult.Result;
      } else {
        job.Client = null;
        job.Percentage = 0;
        job.State = State.idle;
      }
      jobAdapter.Update(job);
    }

    #region IClientCommunicator Members

    /// <summary>
    /// Login process for the client
    /// A hearbeat entry is created as well (login is the first hearbeat)
    /// </summary>
    /// <param name="clientInfo"></param>
    /// <returns></returns>
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

    /// <summary>
    /// The client has to send regulary heartbeats
    /// this hearbeats will be stored in the heartbeats dictionary
    /// check if there is work for the client and send the client a response if he should pull a job
    /// </summary>
    /// <param name="hbData"></param>
    /// <returns></returns>
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

      if (hbData.jobProgress != null) {
        foreach (KeyValuePair<long, double> jobProgress in hbData.jobProgress) {
          Job curJob = jobAdapter.GetById(jobProgress.Key);
          curJob.Percentage = jobProgress.Value;
          jobAdapter.Update(curJob);
        }
      }

      return response;
    }
   
    /// <summary>
    /// if the client asked to pull a job he calls this method
    /// the server selects a job and sends it to the client
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ResponseJob PullJob(Guid clientId) {
      ResponseJob response = new ResponseJob();
      lock (this) {
        LinkedList<Job> allOfflineJobs = new LinkedList<Job>(jobAdapter.GetJobsByState(State.offline));
        if (allOfflineJobs != null && allOfflineJobs.Count > 0) {
          Job job2Calculate = allOfflineJobs.First.Value;
          job2Calculate.State = State.calculating;
          job2Calculate.Client = clientAdapter.GetById(clientId);
          response.Job = job2Calculate;
          jobAdapter.Update(job2Calculate);
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_PULLED;
          return response;
        } 
      }
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT;
      return response;
    }

    /// <summary>
    /// the client can send job results during calculating 
    /// and will send a final job result when he finished calculating
    /// these job results will be stored in the database
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="jobId"></param>
    /// <param name="result"></param>
    /// <param name="exception"></param>
    /// <param name="finished"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ResponseResultReceived SendJobResult(Guid clientId, 
      long jobId, 
      byte[] result, 
      double percentage,
      Exception exception,  
      bool finished) {
      ResponseResultReceived response = new ResponseResultReceived();
      ClientInfo client =
        clientAdapter.GetById(clientId);

      Job job = 
        jobAdapter.GetById(jobId);

      if (job.Client == null)    {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED;
        return response;
      }
      if (job.Client.ClientId != clientId) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_WRONG_CLIENT_FOR_JOB;
        return response;
      }
      if (job == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOB_WITH_THIS_ID;
        return response;
      }
      if (job.State != State.calculating) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_WRONG_JOB_STATE;
        return response;
      }
      job.SerializedJob = result;
      job.Percentage = percentage;

      if (finished) {
        job.State = State.finished;
        jobAdapter.Update(job);

        List<JobResult> jobResults = new List<JobResult>(jobResultAdapter.GetResultsOf(job));
        foreach (JobResult currentResult in jobResults) 
          jobResultAdapter.Delete(currentResult);
      }

      JobResult jobResult =
        new JobResult();
      jobResult.Client = client;
      jobResult.Job = job;
      jobResult.Result = result;
      jobResult.Percentage = percentage;
      jobResult.Exception = exception;

      jobResultAdapter.Update(jobResult);
      jobAdapter.Update(job);

      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED;
      response.JobId = jobId;
      response.finished = finished;

      return response;
    }

    /// <summary>
    /// when a client logs out the state will be set
    /// and the entry in the last hearbeats dictionary will be removed
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
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
      List<Job> allJobs = new List<Job>(jobAdapter.GetAll());
      if (client.State == State.calculating) {
        // check wich job the client was calculating and reset it
        foreach (Job job in allJobs) {
          if (job.Client.ClientId == client.ClientId) {
            resetJobsDependingOnResults(job);
          }
        }
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
