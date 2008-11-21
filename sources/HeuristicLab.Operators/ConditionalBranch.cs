#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Branch of (one or)two operators whose executions depend on a specific condition.
  /// </summary>
  public class ConditionalBranch : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"ConditionalBranch expects to have 1 or 2 sub-operators.
It will return the 1st sub-operator if ""Condition"" is true and the 2nd sub-operator if ""Condition"" equals to false.

In case a 2nd sub-operator does not exist and ""Condition"" would equal to false, Conditional Branch will not return a new operation."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConditionalBranch"/> with one variable info 
    /// (<c>Condition</c>).
    /// </summary>
    public ConditionalBranch()
      : base() {
      AddVariableInfo(new VariableInfo("Condition", "A boolean variable that decides the branch", typeof(BoolData), VariableKind.In));
    }

    /// <summary>
    /// Applies the operator of branch one under a specific condition on the given 
    /// <paramref name="scope"/>, or - if existent - operator of branch two if the condition could not
    /// be fulfilled. 
    /// </summary>
    /// <param name="scope">The scope to apply the operators on.</param>
    /// <returns>A new <see cref="AtomicOperation"/> with either operator 1 or operator 2 applied
    /// to the given <paramref name="scope"/> or <c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      BoolData resultData = GetVariableValue<BoolData>("Condition", scope, true);
      bool result = resultData.Data;

      if ((result) && (SubOperators.Count > 0) && (SubOperators[0] != null))
        return new AtomicOperation(SubOperators[0], scope);
      else if ((!result) && (SubOperators.Count > 1) && (SubOperators[1] != null))
        return new AtomicOperation(SubOperators[1], scope);
      return null;
    }
  }
}
