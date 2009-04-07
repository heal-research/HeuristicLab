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
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.Hive.Server.Core {
  class JobManager: IJobManager, IInternalJobManager {

    ISessionFactory factory;
    ILifecycleManager lifecycleManager;

    #region IJobManager Members

    public JobManager() {
      factory = ServiceLocator.GetSessionFactory();
      lifecycleManager = ServiceLocator.GetLifecycleManager();

      lifecycleManager.RegisterStartup(new EventHandler(lifecycleManager_OnStartup));
      lifecycleManager.RegisterStartup(new EventHandler(lifecycleManager_OnShutdown));
    }

    private JobResult GetLastJobResult(Job job) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobResultsAdapter jobResultAdapter =
            session.GetDataAdapter<JobResult, IJobResultsAdapter>();

        List<JobResult> allJobResults = new List<JobResult>(jobResultAdapter.GetResultsOf(job));
        JobResult lastJobResult = null;
        foreach (JobResult jR in allJobResults) {
          // if lastJobResult was before the current jobResult the lastJobResult must be updated
          if (lastJobResult == null ||
              (jR.timestamp > lastJobResult.timestamp))
            lastJobResult = jR;
        }
        return lastJobResult;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public void ResetJobsDependingOnResults(Job job) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();

        JobResult lastJobResult = GetLastJobResult(job);
        if (lastJobResult != null) {
          job.Percentage = lastJobResult.Percentage;
          job.SerializedJob = lastJobResult.Result;
        } else {
          job.Percentage = 0;
        }

        job.Client = null;
        job.State = State.offline;

        jobAdapter.Update(job);
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    void checkForDeadJobs() {
       ISession session = factory.GetSessionForCurrentThread();

       try {
         IJobAdapter jobAdapter =
             session.GetDataAdapter<Job, IJobAdapter>();

         List<Job> allJobs = new List<Job>(jobAdapter.GetAll());
         foreach (Job curJob in allJobs) {
           if (curJob.State == State.calculating) {
             ResetJobsDependingOnResults(curJob);
           }
         }
       }
       finally {
         if (session != null)
           session.EndSession();
       }
    }

    void lifecycleManager_OnStartup(object sender, EventArgs e) {
      checkForDeadJobs();
    }

    void lifecycleManager_OnShutdown(object sender, EventArgs e) {
      checkForDeadJobs();
    }

    /// <summary>
    /// returns all jobs stored in the database
    /// </summary>
    /// <returns></returns>
    public ResponseList<Job> GetAllJobs() {
       ISession session = factory.GetSessionForCurrentThread();

       try {
         IJobAdapter jobAdapter =
             session.GetDataAdapter<Job, IJobAdapter>();

         ResponseList<Job> response = new ResponseList<Job>();

         response.List = new List<Job>(jobAdapter.GetAll());
         response.Success = true;
         response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ALL_JOBS;

         return response;
       }
       finally {
         if (session != null)
           session.EndSession();
       }
    }

    /// <summary>
    /// Adds a new job into the database
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public ResponseObject<Job> AddNewJob(Job job) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();

        ResponseObject<Job> response = new ResponseObject<Job>();

        if (job != null) {
          if (job.State != State.offline) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOBSTATE_MUST_BE_OFFLINE;
            return response;
          }
          if (job.Id != Guid.Empty) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ID_MUST_NOT_BE_SET;
            return response;
          }
          if (job.SerializedJob == null) {
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_NULL;
            response.Success = false;
            return response;
          }

          job.DateCreated = DateTime.Now;
          jobAdapter.Update(job);
          response.Success = true;
          response.Obj = job;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_ADDED;
        } else {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_NULL;
        }

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Removes a job from the database
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public Response RemoveJob(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();
        Response response = new Response();

        Job job = jobAdapter.GetById(jobId);
        if (job == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
          return response;
        }
        jobAdapter.Delete(job);
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_REMOVED;

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public ResponseObject<JobResult> GetLastJobResultOf(Guid jobId, bool requested) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();

        ResponseObject<JobResult> response = new ResponseObject<JobResult>();
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_RESULT_SENT;
        response.Obj = GetLastJobResult(jobAdapter.GetById(jobId));

        return response;
      }
      finally {
        if(session != null)
          session.EndSession();
      }
    }

    public Response RequestSnapshot(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();
      Response response = new Response();
      
      try {
        

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public Response AbortJob(Guid jobId) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
