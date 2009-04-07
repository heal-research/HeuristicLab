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
  /// Branch of (one or) two operators that have different probabilities to get executed.
  /// </summary>
  public class StochasticBranch : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StochasticBranch"/> with two variable infos
    /// (<c>Random</c> and <c>Probability</c>).
    /// </summary>
    public StochasticBranch()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Probability", "Probability to choose first branch", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Applies the operator of branch one with a specific probability on the given 
    /// <paramref name="scope"/>, or - if existent - with another probability operator of branch two.  
    /// </summary>
    /// <param name="scope">The scope to apply the operators on.</param>
    /// <returns>A new <see cref="AtomicOperation"/> with either operator 1 or operator 2 applied
    /// to the given <paramref name="scope"/> or <c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleData probability = GetVariableValue<DoubleData>("Probability", scope, true);

      bool result = random.NextDouble() < probability.Data;
      if ((result) && (SubOperators.Count > 0) && (SubOperators[0] != null))
        return new AtomicOperation(SubOperators[0], scope);
      else if ((!result) && (SubOperators.Count > 1) && (SubOperators[1] != null))
        return new AtomicOperation(SubOperators[1], scope);
      return null;
    }
  }
}
