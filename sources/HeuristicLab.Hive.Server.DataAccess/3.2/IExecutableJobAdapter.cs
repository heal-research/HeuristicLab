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

namespace HeuristicLab.Hive.Server.DataAccess {
  public interface IExecutableJobAdapter: IDataAdapter<ExecutableJob> {
    /// <summary>
    /// Finds a job with the specified criterias 
    /// </summary>
    /// <param name="state">all jobs with the specified state</param>
    /// <param name="cores">all jobs which require less or equal cores</param>
    /// <param name="memory">all jobs which require less or equal memory</param>
    /// <param name="resourceId">all jobs that can be calculated by that resource</param>
    /// <returns></returns>
    ICollection<ExecutableJob> FindJobs(State state,
      int cores,
      int memory,
      Guid resourceId);
  }
}
