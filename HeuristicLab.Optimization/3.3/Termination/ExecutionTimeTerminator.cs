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
using HeuristicLab.Data;
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [Item("ExecutionTimeTerminator", "A termination criterion based on execution time of an algorithm.")]
  [StorableType("7BBF7D6F-C41C-4826-963B-5ED9544F8391")]
  public class ExecutionTimeTerminator : ThresholdTerminator<TimeSpanValue> {

    [Storable]
    private readonly IExecutable executable;

    [StorableConstructor]
    protected ExecutionTimeTerminator(StorableConstructorFlag _) : base(_) { }
    protected ExecutionTimeTerminator(ExecutionTimeTerminator original, Cloner cloner)
      : base(original, cloner) {
      executable = cloner.Clone(original.executable);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExecutionTimeTerminator(this, cloner);
    }
    public ExecutionTimeTerminator(IExecutable executable)
      : this(executable, new TimeSpanValue(new TimeSpan(0, 10, 0))) { }
    public ExecutionTimeTerminator(IExecutable executable, TimeSpanValue maximumExecutionTime)
      : base(maximumExecutionTime) {
      this.executable = executable;
      Name = "Execution Time";
    }

    protected override bool CheckContinueCriterion() {
      var max = ThresholdParameter.Value.Value;
      return executable.ExecutionTime < max;
    }

    public override string ToString() {
      if (Threshold == null) return Name;
      else return string.Format("{0} {1} {2}", Name, "<", ThresholdParameter.Value);
    }
  }
}