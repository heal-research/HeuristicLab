#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class TaskDataDao : GenericDao<Guid, TaskData> {
    public TaskDataDao(DataContext dataContext) : base(dataContext) { }

    public override TaskData GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public int DeleteObsolete(int batchSize) {
      return DataContext.ExecuteCommand(DeleteObsoleteQueryString, batchSize);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, TaskData> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid taskId) =>
        (from taskData in db.GetTable<TaskData>()
         where taskData.TaskId == taskId
         select taskData).SingleOrDefault());
    #endregion

    #region String queries
    private const string DeleteObsoleteQueryString = @"
delete top ({0}) td
from taskdata td
  join task t on t.taskid = td.taskid
  join job j on j.jobid = t.jobid
where j.jobstate = 'deletionpending'
    ";
    #endregion
  }
}
