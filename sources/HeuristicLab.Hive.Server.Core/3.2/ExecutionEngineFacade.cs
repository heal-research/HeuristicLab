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
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  public class ExecutionEngineFacade: IExecutionEngineFacade {

    private IJobManager jobManager =
      ServiceLocator.GetJobManager();

    public ExecutionEngineFacade() {
      
    }

    #region IExecutionEngineFacade Members

    public ResponseObject<Job> AddJob(SerializedJob job) {
      return jobManager.AddNewJob(job);
    }

    public Response RequestSnapshot(Guid jobId) {
      return jobManager.RequestSnapshot(jobId);
    }

    public ResponseObject<SerializedJobResult> 
      GetLastSerializedResult(Guid jobId, bool requested) {
      return jobManager.GetLastSerializedJobResultOf(jobId, requested);
    }

    public Response AbortJob(Guid jobId) {
      return jobManager.AbortJob(jobId);
    }

    #endregion
  }
}
