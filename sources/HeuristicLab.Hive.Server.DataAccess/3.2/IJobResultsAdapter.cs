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
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.DataAccess.Interfaces;
using System.IO;

namespace HeuristicLab.Hive.Server.DataAccess {
  public interface IJobResultsAdapter: IDataAdapter<JobResult> {
    /// <summary>
    /// Gets all results for the specified job
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    ICollection<JobResult> GetResultsOf(Guid jobId);

    /// <summary>
    /// Gets the last result for the specified job
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    JobResult GetLastResultOf(Guid jobId);

    /// <summary>
    /// Gets the computable job result with the secified jobResultId
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    SerializedJobResult GetSerializedJobResult(Guid jobResultId);

    /// <summary>
    /// Gets a stream object for the large serialized job result data
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    Stream GetSerializedJobResultStream(Guid jobResultId, bool useExistingConnection);

    /// <summary>
    /// Saves or update the computable job result
    /// </summary>
    /// <param name="jobId"></param>
    void UpdateSerializedJobResult(SerializedJobResult jobResult);
  }
}
