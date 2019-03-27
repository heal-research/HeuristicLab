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

using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {

  public partial class StateLog {

    public StateLog() { }

    protected StateLog(StateLog original, Cloner cloner)
      : base(original, cloner) {
      this.DateTime = original.DateTime;
      this.UserId = original.UserId;
      this.SlaveId = original.SlaveId;
      this.Exception = original.Exception;
      this.State = original.State;
      this.TaskId = original.TaskId;
    }

    public string TaskName { get; set; }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StateLog(this, cloner);
    }

    public override string ToString() {
      return State.ToString();
    }
  }
}
