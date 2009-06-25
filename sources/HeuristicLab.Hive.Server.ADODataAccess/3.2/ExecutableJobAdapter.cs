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
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Linq.Expressions;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ExecutableJobAdapter :
    DataAdapterBase<dsHiveServerTableAdapters.JobTableAdapter,
                      ExecutableJob, 
                      dsHiveServer.JobRow>, 
    IExecutableJobAdapter {
    #region Fields
    private IJobAdapter jobAdapter = null;

    private IJobAdapter JobAdapter {
      get {
        if (jobAdapter == null)
          jobAdapter = 
            this.Session.GetDataAdapter<Job, IJobAdapter>();

        return jobAdapter;
      }
    }
    #endregion

    public ExecutableJobAdapter(): base(new ExecutableJobAdapterWrapper()) {
    }

    #region Overrides
    protected override ExecutableJob ConvertRow(dsHiveServer.JobRow row,
      ExecutableJob job) {
      if (row != null && job != null) {
        JobAdapter.GetById(job);

        if (!row.IsSerializedJobNull())
          job.SerializedJob = row.SerializedJob;
        else
          job.SerializedJob = null;

        return job;
      } else
        return null;
    }

    protected override dsHiveServer.JobRow ConvertObj(ExecutableJob job,
      dsHiveServer.JobRow row) {
      if (job != null && row != null) {
        row.SerializedJob = job.SerializedJob;
      }

      return row;
    }

    protected override void doUpdate(ExecutableJob obj) {
      if (obj != null) {
        JobAdapter.Update(obj);

        base.doUpdate(obj);
      }
    }
    #endregion

    public ICollection<ExecutableJob> FindJobs(State state, int cores, int memory,
      Guid resourceId) {
      return
         base.FindMultiple(
           delegate() {
             return Adapter.GetDataByStateCoresMemoryResource(
               state.ToString(),
               cores,
               memory,
               resourceId);
           });
    }
  }
}
