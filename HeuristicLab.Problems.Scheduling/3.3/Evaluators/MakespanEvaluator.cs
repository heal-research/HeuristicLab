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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Scheduling {
  [Item("Makespan Evaluator", "Represents an evaluator using the maximum makespan of a schedule.")]
  [StorableClass]
  public class MakespanEvaluator : ScheduleEvaluator {

    [StorableConstructor]
    protected MakespanEvaluator(bool deserializing) : base(deserializing) { }
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
