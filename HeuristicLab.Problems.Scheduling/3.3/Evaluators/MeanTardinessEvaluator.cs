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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.Scheduling {
  [Item("Mean tardiness Evaluator", "Represents an evaluator using the mean tardiness of a schedule.")]
  [StorableType("75C00D86-4C81-43EC-A2D4-60736509C97E")]
  public class MeanTardinessEvaluator : ScheduleEvaluator, IJSSPOperator {

    [StorableConstructor]
    protected MeanTardinessEvaluator(StorableConstructorFlag _) : base(_) { }
    protected MeanTardinessEvaluator(MeanTardinessEvaluator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanTardinessEvaluator(this, cloner);
    }

    #region Parameter Properties
    public ILookupParameter<ItemList<Job>> JobDataParameter {
      get { return (ILookupParameter<ItemList<Job>>)Parameters["JobData"]; }
    }
    #endregion

    public MeanTardinessEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<ItemList<Job>>("JobData", "Jobdata defining the precedence relationships and the duration of the tasks in this JSSP-Instance."));
      QualityParameter.ActualName = "MeanTardiness";
    }

    public static double GetMeanTardiness(Schedule schedule, ItemList<Job> jobData) {
      return schedule.Resources
        .Select(r => Math.Max(0, r.Tasks.Last().EndTime - jobData[r.Tasks.Last().JobNr].DueDate))
        .Average();
    }

    protected override double Evaluate(Schedule schedule) {
      return GetMeanTardiness(schedule, JobDataParameter.ActualValue);
    }
  }
}
