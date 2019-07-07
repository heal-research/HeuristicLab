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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("ScheduledTask", "Represents a task that has been scheduled already.")]
  [StorableType("8E947B77-37DF-4451-ACF4-03251516FC52")]
  public class ScheduledTask : Item {

    #region Properties
    [Storable]
    public int TaskNr { get; set; }
    [Storable]
    public int ResourceNr { get; set; }
    [Storable]
    public double Duration { get; set; }
    [Storable]
    public double StartTime { get; set; }
    public double EndTime {
      get {
        return Duration + StartTime;
      }
    }
    [Storable]
    public int JobNr { get; set; }
    #endregion

    [StorableConstructor]
    protected ScheduledTask(StorableConstructorFlag _) : base(_) { }
    protected ScheduledTask(ScheduledTask original, Cloner cloner)
      : base(original, cloner) {
      this.TaskNr = original.TaskNr;
      this.ResourceNr = original.ResourceNr;
      this.Duration = original.Duration;
      this.StartTime = original.StartTime;
      this.JobNr = original.JobNr;
    }
    public ScheduledTask(int resNr, double startTime, double duration, int jobNr)
      : base() {
      Duration = duration;
      ResourceNr = resNr;
      StartTime = startTime;
      JobNr = jobNr;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScheduledTask(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[" + JobNr + "," + ResourceNr + "]");
      return sb.ToString();
    }
  }
}
