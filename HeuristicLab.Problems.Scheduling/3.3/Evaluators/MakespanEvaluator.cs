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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.Scheduling {
  [Item("Makespan Evaluator", "Represents an evaluator using the maximum makespan of a schedule.")]
  [StorableType("DCD90872-1FF9-482F-8FCF-AD34AC6DF051")]
  public class MakespanEvaluator : ScheduleEvaluator {

    [StorableConstructor]
    protected MakespanEvaluator(StorableConstructorFlag _) : base(_) { }
    protected MakespanEvaluator(MakespanEvaluator original, Cloner cloner) : base(original, cloner) {}
    public MakespanEvaluator()
      : base() {
      QualityParameter.ActualName = "Makespan";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MakespanEvaluator(this, cloner);
    }

    public static double GetMakespan(Schedule schedule) {
      return schedule.Resources.Select(r => r.TotalDuration).Max();
    }

    protected override double Evaluate(Schedule schedule) {
      return GetMakespan(schedule);
    }
  }
}
