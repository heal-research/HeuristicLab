#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class StateLogDao : GenericDao<Guid, StateLog> {
    private Table<Task> TaskTable {
      get { return DataContext.GetTable<Task>(); }
    }

    public StateLogDao(DataContext dataContext) : base(dataContext) { }

    public override StateLog GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public StateLog GetLastStateLogFromTask(Task task) {
      return GetLastStateLogFromTaskQuery(DataContext, task);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, StateLog> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid stateLogId) =>
        (from stateLog in db.GetTable<StateLog>()
         where stateLog.StateLogId == stateLogId
         select stateLog).SingleOrDefault());

    private static readonly Func<DataContext, Task, StateLog> GetLastStateLogFromTaskQuery =
      CompiledQuery.Compile((DataContext db, Task task) =>
        (from stateLog in db.GetTable<StateLog>()
         where stateLog.TaskId == task.TaskId
         orderby stateLog.DateTime descending
         select stateLog).First(x => x.SlaveId != null));
    #endregion
  }
}
