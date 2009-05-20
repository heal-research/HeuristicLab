#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Selection.OffspringSelection {
  /// <summary>
  /// Analyzes the offspring on whether it is successful or not based on its quality in comparison to its best and worst parents' qualities.
  /// </summary>
  public class WeightedOffspringFitnessComparer : OperatorBase  {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"Compares the quality values of the child with a weighted average of the best and worst parents' qualities.
Adds a variable SuccessfulChild into the current scope with the result of the comparison.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="WeightedOffspringFitnessComparer"/> with four variable infos 
    /// (<c>Maximization</c>, <c>Quality</c>, <c>SuccessfulChild</c>, and <c>ComparisonFactor</c>).
    /// </summary>
    public WeightedOffspringFitnessComparer()
      : base() {
      AddVariableInfo(new VariableInfo("Maximization", "True if the problem is a maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SuccessfulChild", "True if the child is successful", typeof(BoolData), VariableKind.New));
      AddVariableInfo(new VariableInfo("ComparisonFactor", "Factor for comparing the quality of a child with the qualities of its parents (0 = better than worst parent, 1 = better than best parent)", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Weighs the worst and best parent quality with a given factor and decides whether the child is better than this threshold.
    /// The result of this decision is added as variable "SuccessfulChild" into the scope.
    /// </summary>
    /// <param name="scope">The current scope which represents a new child.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      bool maximize = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      double compFactor = GetVariableValue<DoubleData>("ComparisonFactor", scope, true).Data;
      double child = GetVariableValue<DoubleData>("Quality", scope, false).Data;

      double lowParent = double.MaxValue; // lowest quality parent
      double highParent = double.MinValue; // highest quality parent
      for (int i = 0; i < scope.SubScopes.Count; i++) {
        double parentQuality = scope.SubScopes[i].GetVariableValue<DoubleData>("Quality", false).Data;
        if (parentQuality < lowParent) lowParent = parentQuality;
        if (parentQuality > highParent) highParent = parentQuality;
      }

      double threshold;
      if (!maximize)
        threshold = highParent + (lowParent - highParent) * compFactor;
      else
        threshold = lowParent + (highParent - lowParent) * compFactor;

      BoolData successful;
      if (((!maximize) && (child < threshold)) ||
          ((maximize) && (child > threshold)))
        successful = new BoolData(true);
      else
        successful = new BoolData(false);

      scope.AddVariable(new Variable(scope.TranslateName("SuccessfulChild"), successful));
      return null;
    }
  }
}
