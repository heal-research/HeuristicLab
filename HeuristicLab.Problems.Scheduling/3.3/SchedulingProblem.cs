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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.Scheduling {
  [Item("SchedulingProblem", "Abstract class that represents a Scheduling Problem")]
  [StorableType("D3EFE88B-7725-40DF-861F-37B17314D3F5")]
  public abstract class SchedulingProblem : SingleObjectiveHeuristicOptimizationProblem<ISchedulingEvaluator, IScheduleCreator> {
    [StorableConstructor]
    protected SchedulingProblem(StorableConstructorFlag _) : base(_) { }
    protected SchedulingProblem(SchedulingProblem original, Cloner cloner) : base(original, cloner) { }
    protected SchedulingProblem(ISchedulingEvaluator evaluator, IScheduleCreator creator) : base(evaluator, creator) { }
  }
}
