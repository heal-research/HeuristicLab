#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PWRDecoder", "An item used to convert a PWR-individual into a generalized schedule.")]
  [StorableType("60D171BE-9704-40E1-9C63-0E56D95403CD")]
  public class PWRDecoder : ScheduleDecoder<PWREncoding> {
    [StorableConstructor]
    protected PWRDecoder(StorableConstructorFlag _) : base(_) { }
    protected PWRDecoder(PWRDecoder original, Cloner cloner) : base(original, cloner) { }
    public PWRDecoder() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWRDecoder(this, cloner);
    }

    public override Schedule DecodeSchedule(PWREncoding solution, ItemList<Job> jobData) {
      return Decode(solution, jobData);
    }

    public static Schedule Decode(PWREncoding solution, ItemList<Job> jobData) {
      var jobs = (ItemList<Job>)jobData.Clone();
      var resultingSchedule = new Schedule(jobs[0].Tasks.Count);
      foreach (int jobNr in solution.PermutationWithRepetition) {
        int i = 0;
        while (jobs[jobNr].Tasks[i].IsScheduled) i++;
        Task currentTask = jobs[jobNr].Tasks[i];
        double startTime = GTAlgorithmUtils.ComputeEarliestStartTime(currentTask, resultingSchedule);
        currentTask.IsScheduled = true;
        resultingSchedule.ScheduleTask(currentTask.ResourceNr, startTime, currentTask.Duration, currentTask.JobNr);
      }
      return resultingSchedule;
    }
  }
}
