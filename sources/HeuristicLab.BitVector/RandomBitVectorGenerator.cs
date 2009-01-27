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

namespace HeuristicLab.BitVector {
  /// <summary>
  /// Generates a new random bit vector.
  /// </summary>
  public class RandomBitVectorGenerator : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Operator generating a new random bit vector."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RandomBitVectorGenerator"/> with three variable infos
    /// (<c>Random</c>, <c>Length</c> and <c>BitVector</c>).
    /// </summary>
    public RandomBitVectorGenerator() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Length", "Vector length", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("BitVector", "Created random bit vector", typeof(BoolArrayData), VariableKind.New));
    }

    /// <summary>
    /// Generates a new random bit vector with the given <paramref name="length"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="length">The length of the bit vector.</param>
    /// <returns>The newly created bit vector.</returns>
    public static bool[] Apply(IRandom random, int length) {
      bool[] result = new bool[length];
      for (int i = 0; i < length; i++)
        result[i] = random.Next() < 0.5;
      return result;
    }

    /// <summary>
    /// Generates a new random bit vector and injects it in the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to get the values from and where to inject the newly 
    /// created bit vector.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      int length = GetVariableValue<IntData>("Length", scope, true).Data;

      bool[] vector = Apply(random, length);
      scope.AddVariable(new Variable(scope.TranslateName("BitVector"), new BoolArrayData(vector)));

      return null;
    }
  }
}
