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
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;

namespace HeuristicLab.Hive.Server.Core {
  class JobManager: IJobManager {

    IJobAdapter jobAdapter;

    #region IJobManager Members

    public JobManager() {
      jobAdapter = ServiceLocator.GetJobAdapter();
    }

    public ResponseList<Job> GetAllJobs() {
      ResponseList<Job> response = new ResponseList<Job>();

      response.List = new List<Job>(jobAdapter.GetAllJobs());
      response.Success = true;
      response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ALL_JOBS;
      
      return response;
    }

    public ResponseObject<Job> AddNewJob(Job job) {
      ResponseObject<Job> response = new ResponseObject<Job>();

      if (job != null) {
        if (job.JobId != 0) {
          response.Success = false;
          response.StatusMessage = ApplicationConstants.RESPONSE_JOB_ID_MUST_NOT_BE_SET;
          return response;
        }
        jobAdapter.UpdateJob(job);
        response.Success = true;
        response.Obj = job;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_ADDED;
        return response;
      }
      response.Success = false;
      response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_NULL;

      return response;
    }

    public Response RemoveJob(long jobId) {
      Response response = new Response();

      Job job = jobAdapter.GetJobById(jobId);
      if (job == null) {
        response.Success = false;
        response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_DOESNT_EXIST;
        return response;
      }
      jobAdapter.DeleteJob(job);
      response.Success = false;
      response.StatusMessage = ApplicationConstants.RESPONSE_JOB_JOB_REMOVED;

      return response;
    }

    #endregion
  }
}
