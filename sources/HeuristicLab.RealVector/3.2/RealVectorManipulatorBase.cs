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

namespace HeuristicLab.RealVector {
  /// <summary>
  /// Base class for all real vector manipulators.
  /// </summary>
  public abstract class RealVectorManipulatorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="RealVectorManipulatorBase"/> with two variable infos
    /// (<c>Random</c> and <c>RealVector</c>).
    /// </summary>
    public RealVectorManipulatorBase() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("RealVector", "Real vector to manipulate", typeof(DoubleArrayData), VariableKind.In | VariableKind.Out));
    }

    /// <summary>
    /// Manipulates the real vector.
    /// </summary>
    /// <param name="scope">The current scope whose real vector to manipulate.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleArrayData vector = GetVariableValue<DoubleArrayData>("RealVector", scope, false);
      vector.Data = Manipulate(scope, random, vector.Data);
      return null;
    }

    /// <summary>
    /// Manipulates the given real <paramref name="vector"/> with the given random number generator.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="vector">The real vector to manipulate.</param>
    /// <returns>The manipulated real vector.</returns>
    protected abstract double[] Manipulate(IScope scope, IRandom random, double[] vector);
  }
}
