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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.ES {
  /// <summary>
  /// Applies its sub operator a number of times depending on the <c>ShakingFactor</c>.
  /// </summary>
  public class VariableStrengthRepeatingManipulator : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"Applies its suboperator a number of times depending on the ShakingFactor"; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableStrengthRepeatingManipulator"/> with two variable
    /// infos (<c>Random</c> and <c>ShakingFactor</c>).
    /// </summary>
    public VariableStrengthRepeatingManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Determines the strength of the mutation (repeated application of the operator)", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Applies its suboperator a number of times depending on the <c>ShakingFactor</c>.
    /// </summary>
    /// <param name="scope">The current scope where to apply the operator.</param>
    /// <returns>A new <see cref="CompositeOperation"/>, containing a specified number of 
    /// the same operations with the same operator, the number depending on the <c>ShakingFactor</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      double shakingFactor = scope.GetVariableValue<DoubleData>("ShakingFactor", true).Data;
      NormalDistributedRandom N = new NormalDistributedRandom(random, 1.0, shakingFactor);
      int strength = (int)Math.Ceiling(Math.Abs(N.NextDouble()));
      if (strength == 0) strength = 1;

      CompositeOperation co = new CompositeOperation();
      co.ExecuteInParallel = false;
      for (int i = 0; i < strength; i++) {
        co.AddOperation(new AtomicOperation(SubOperators[0], scope));
      }

      return co;
    }
  }
}
