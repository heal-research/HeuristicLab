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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.Scheduling {
  [Item("PWRDecoder", "An item used to convert a PWR-individual into a generalized schedule.")]
  [StorableType("FE66781A-93CB-4312-8AC0-AA049220FEB8")]
  public class PWRDecoder : ScheduleDecoder, IStochasticOperator, IJSSPOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemList<Job>> JobDataParameter {
      get { return (LookupParameter<ItemList<Job>>)Parameters["JobData"]; }
    }

    [StorableConstructor]
    protected PWRDecoder(StorableConstructorFlag _) : base(_) { }
    protected PWRDecoder(PWRDecoder original, Cloner cloner) : base(original, cloner) { }
    public PWRDecoder()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<ItemList<Job>>("JobData", "Job data taken from the JSSP - Instance."));
      ScheduleEncodingParameter.ActualName = "PermutationWithRepetition";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWRDecoder(this, cloner);
    }

    public override Schedule CreateScheduleFromEncoding(IScheduleEncoding encoding) {
      var solution = encoding as PWREncoding;
      if (solution == null) throw new InvalidOperationException("Encoding is not of type PWREncoding");

      var jobs = (ItemList<Job>)JobDataParameter.ActualValue.Clone();
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
