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
  /// Calculates the quality of a solution candidate.
  /// </summary>
  public abstract class SingleObjectiveEvaluatorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="SingleObjectiveEvaluatorBase"/> with one variable info
    /// (<c>Quality</c>).
    /// </summary>
    public SingleObjectiveEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.New));
    }

    /// <summary>
    /// Evaluates the quality of a solution candidate.
    /// </summary>
    /// <remarks>Creates a new variable with the calculated quality if it not already exists, and 
    /// injects it in the given <paramref name="scope"/> if it is not a local one.</remarks>
    /// <param name="scope">The scope where to evaluate the quality.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      double qualityValue = Evaluate(scope);
      DoubleData quality = GetVariableValue<DoubleData>("Quality", scope, false, false);
      if (quality == null) {
        quality = new DoubleData(qualityValue);
        IVariableInfo qualityInfo = GetVariableInfo("Quality");
        if (qualityInfo.Local)
          AddVariable(new Variable(qualityInfo.ActualName, quality));
        else
          scope.AddVariable(new Variable(scope.TranslateName(qualityInfo.FormalName), quality));
      } else {
        quality.Data = qualityValue;
      }
      return null;
    }

    /// <summary>
    /// Calculates the quality of a solution candidate in the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to calculate the quality.</param>
    /// <returns>A double value that indicates the quality.</returns>
    protected abstract double Evaluate(IScope scope);
  }
}
