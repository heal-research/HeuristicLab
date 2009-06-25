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
using System.Data;

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

    private JobResult GetLastJobResult(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobResultsAdapter jobResultAdapter =
            session.GetDataAdapter<JobResult, IJobResultsAdapter>();

        return jobResultAdapter.GetLastResultOf(jobId);
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public void ResetJobsDependingOnResults(Job job) {
      ISession session = factory.GetSessionForCurrentThread();
      ITransaction tx = null;

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();

        IJobResultsAdapter jobResultsAdapter =
          session.GetDataAdapter<JobResult, IJobResultsAdapter>();

        tx = session.BeginTransaction();

        if (job != null) {
          SerializedJob computableJob =
              new SerializedJob();
          computableJob.JobInfo =
            job;

          JobResult lastResult = 
            GetLastJobResult(job.Id);

          if (lastResult != null) {
            SerializedJobResult lastJobResult =
              jobResultsAdapter.GetSerializedJobResult(lastResult.Id);

            if (lastJobResult != null) {
              computableJob.JobInfo.Percentage = lastJobResult.JobResult.Percentage;
              computableJob.SerializedJobData = lastJobResult.SerializedJobResultData;

              jobAdapter.UpdateSerializedJob(computableJob);
            } else {
              computableJob.JobInfo.Percentage = 0;
            }
          } else {
            computableJob.JobInfo.Percentage = 0;
          }

          computableJob.JobInfo.Client = null;
          computableJob.JobInfo.State = State.offline;

          jobAdapter.Update(computableJob.JobInfo);
        }

        tx.Commit();
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
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
    /// returns the job with the specified id
    /// </summary>
    /// <returns></returns>
    public ResponseObject<Job> GetJobById(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();

        ResponseObject<Job> response = new ResponseObject<Job>();

        response.Obj = jobAdapter.GetById(jobId);
        if (response.Obj != null) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_GET_JOB_BY_ID;
        } else {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
        }

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
    public ResponseObject<Job> AddNewJob(SerializedJob job) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();

        ResponseObject<Job> response = new ResponseObject<Job>();

        if (job != null && job.JobInfo != null) {
          if (job.JobInfo.State != State.offline) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOBSTATE_MUST_BE_OFFLINE;
            return response;
          }
          if (job.JobInfo.Id != Guid.Empty) {
            response.Success = false;
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ID_MUST_NOT_BE_SET;
            return response;
          }
          if (job.SerializedJobData == null) {
            response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_NULL;
            response.Success = false;
            return response;
          }

          job.JobInfo.DateCreated = DateTime.Now;
          jobAdapter.UpdateSerializedJob(job);
          response.Success = true;
          response.Obj = job.JobInfo;
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

    public ResponseObject<JobResult> GetLastJobResultOf(Guid jobId) {
       ResponseObject<JobResult> result = 
        new ResponseObject<JobResult>();

       result.Obj =
         GetLastJobResult(jobId);
       result.Success =
         result.Obj != null;

       return result;
    }

    public ResponseObject<SerializedJobResult>
      GetLastSerializedJobResultOf(Guid jobId, bool requested) {
      ISession session = factory.GetSessionForCurrentThread();

      ITransaction tx = null;

      try {
        IJobAdapter jobAdapter =
            session.GetDataAdapter<Job, IJobAdapter>();

        IJobResultsAdapter jobResultsAdapter =
          session.GetDataAdapter<JobResult, IJobResultsAdapter>();

        tx = session.BeginTransaction();

        ResponseObject<SerializedJobResult> response =
          new ResponseObject<SerializedJobResult>();

        Job job = jobAdapter.GetById(jobId);
        if (requested && (job.State == State.requestSnapshot || job.State == State.requestSnapshotSent)) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_RESULT_NOT_YET_HERE;

          tx.Commit();
          
          return response;
        }

        JobResult lastResult =
          jobResultsAdapter.GetLastResultOf(job.Id);

        if (lastResult != null) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_RESULT_SENT;
          response.Obj =
            jobResultsAdapter.GetSerializedJobResult(
              lastResult.Id);          
        } else {
          response.Success = false;
        }

        tx.Commit();
        return response;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }


    public Response RequestSnapshot(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();
      Response response = new Response();
      
      try {
        IJobAdapter jobAdapter = session.GetDataAdapter<Job, IJobAdapter>();

        Job job = jobAdapter.GetById(jobId);
        if (job.State == State.requestSnapshot || job.State == State.requestSnapshotSent) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_REQUEST_ALLREADY_SET;
          return response; // no commit needed
        }
        if (job.State != State.calculating) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_IS_NOT_BEEING_CALCULATED;
          return response; // no commit needed
        }
        // job is in correct state
        job.State = State.requestSnapshot;
        jobAdapter.Update(job);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_REQUEST_SET;

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public Response AbortJob(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();
      Response response = new Response();

      try {
        IJobAdapter jobAdapter = session.GetDataAdapter<Job, IJobAdapter>();

        Job job = jobAdapter.GetById(jobId);
        if (job == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
          return response; // no commit needed
        }
        if (job.State == State.abort) {
          response.Success = true;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ABORT_REQUEST_ALLREADY_SET;
          return response; // no commit needed
        }
        if (job.State != State.calculating && job.State != State.requestSnapshot && job.State != State.requestSnapshotSent) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_IS_NOT_BEEING_CALCULATED;
          return response; // no commit needed
        }
        // job is in correct state
        job.State = State.abort;
        jobAdapter.Update(job);

        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ABORT_REQUEST_SET;

        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public ResponseList<JobResult> GetAllJobResults(Guid jobId) {
      ISession session = factory.GetSessionForCurrentThread();
      ResponseList<JobResult> response = new ResponseList<JobResult>();

      try {
        IJobResultsAdapter jobResultAdapter =
            session.GetDataAdapter<JobResult, IJobResultsAdapter>();
        IJobAdapter jobAdapter = session.GetDataAdapter<Job, IJobAdapter>();

        Job job = jobAdapter.GetById(jobId);
        if (job == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
          return response;
        }
        response.List = new List<JobResult>(jobResultAdapter.GetResultsOf(job.Id));
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_RESULT_SENT;

        return response;
      }
      finally {
        if(session != null)
          session.EndSession();
      }
    }

    public ResponseList<Project> GetAllProjects() {
      ISession session = factory.GetSessionForCurrentThread();
      ResponseList<Project> response = new ResponseList<Project>();

      try {
        IProjectAdapter projectAdapter =
          session.GetDataAdapter<Project, IProjectAdapter>();

        List<Project> allProjects = new List<Project>(projectAdapter.GetAll());
        response.List = allProjects;
        response.Success = true;
        return response;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    private Response createUpdateProject(Project project) {
      ISession session = factory.GetSessionForCurrentThread();
      Response response = new Response();
      ITransaction tx = null;

      try {
        IProjectAdapter projectAdapter =
          session.GetDataAdapter<Project, IProjectAdapter>();

        if (project.Name == null || project.Name == "") {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_PROJECT_NAME_EMPTY;
          return response;
        }
        tx = session.BeginTransaction();
        projectAdapter.Update(project);

        tx.Commit();
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_PROJECT_ADDED;
      } catch (ConstraintException ce) {
        if (tx != null)
          tx.Rollback();
        response.Success = false;
        response.StatusMessage = ce.Message;
      }
      catch (Exception ex) {
        if (tx != null)
          tx.Rollback();
        throw ex;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
      return response;
    }

    public Response CreateProject(Project project) {
      return createUpdateProject(project);
    }

    public Response ChangeProject(Project project) {
      return createUpdateProject(project);
    }

    public Response DeleteProject(Guid projectId) {
      ISession session = factory.GetSessionForCurrentThread();
      Response response = new Response();
      ITransaction tx = null;

      try {
        IProjectAdapter projectAdapter =
          session.GetDataAdapter<Project, IProjectAdapter>();

        Project project = projectAdapter.GetById(projectId);
        if (project == null) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_PROJECT_DOESNT_EXIST;
          return response;
        }
        projectAdapter.Delete(project);
        tx.Commit();
        response.Success = true;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_PROJECT_DELETED;
      }
      catch (Exception e) {
        if (tx != null)
          tx.Rollback();
        response.Success = false;
        response.StatusMessage = e.Message;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
      return response;
    }

    public ResponseList<Job> GetJobsByProject(Guid projectId) {
      ISession session = factory.GetSessionForCurrentThread();
      ResponseList<Job> response = new ResponseList<Job>();

      try {
        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();
        List<Job> jobsByProject = new List<Job>(jobAdapter.GetJobsByProject(projectId));
        response.List = jobsByProject;
        response.Success = true;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
      return response;
    }

    #endregion
  }
}
