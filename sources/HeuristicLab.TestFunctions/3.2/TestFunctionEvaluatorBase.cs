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
using HeuristicLab.Operators;

namespace HeuristicLab.TestFunctions {
  /// <summary>
  /// Base class for a test function evaluator.
  /// </summary>
  public abstract class TestFunctionEvaluatorBase : SingleObjectiveEvaluatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="TestFunctionEvaluatorBase"/> with one variable info
    /// (<c>Point</c>).
    /// </summary>
    public TestFunctionEvaluatorBase() {
      AddVariableInfo(new VariableInfo("Point", "n-dimensional point for which the test function should be evaluated", typeof(DoubleArrayData), VariableKind.In));
    }

    /// <summary>
    /// Evaluates the test function for a specific point.
    /// </summary>
    /// <remarks>Calls <see cref="EvaluateFunction(double[])"/>.</remarks>
    /// <param name="scope">The current scope with the point for which to evaluate.</param>
    /// <returns>The result value of the function at the given point.</returns>
    protected sealed override double Evaluate(IScope scope) {
      return EvaluateFunction(GetVariableValue<DoubleArrayData>("Point", scope, false).Data);
    }

    /// <summary>
    /// Evaluates the test function for a specific <paramref name="point"/>.
    /// </summary>
    /// <param name="point">N-dimensional point for which the test function should be evaluated.</param>
    /// <returns>The result value of the function at the given point.</returns>
    protected abstract double EvaluateFunction(double[] point);
  }
}
