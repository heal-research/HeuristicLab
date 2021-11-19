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
using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class IntervalUtil {
    public static IEnumerable<double> GetConstraintViolations(
      IEnumerable<ShapeConstraint> constraints, IBoundsEstimator estimator, IntervalCollection intervalCollection,
      ISymbolicExpressionTree solution) {
      return constraints.Select(constraint => GetConstraintViolation(constraint, estimator, intervalCollection, solution)).ToList();
    }

    public static double GetConstraintViolation(
      ShapeConstraint constraint, IBoundsEstimator estimator, IntervalCollection variableRanges,
      ISymbolicExpressionTree tree) {
      var varRanges = variableRanges.GetReadonlyDictionary();

      if (!string.IsNullOrEmpty(constraint.Variable) && !varRanges.ContainsKey(constraint.Variable)) {
        throw new ArgumentException(
          $"No variable range found for variable {constraint.Variable} used in the constraints.",
          nameof(constraint));
      }

      // Create new variable ranges for defined regions
      var regionRanges = new IntervalCollection();
      foreach (var kvp in varRanges) {
        if (constraint.Regions.GetReadonlyDictionary().TryGetValue(kvp.Key, out var val)) {
          regionRanges.AddInterval(kvp.Key, val);
        } else {
          regionRanges.AddInterval(kvp.Key, kvp.Value);
        }
      }

      if (!constraint.IsDerivative) {
        return estimator.GetConstraintViolation(tree, regionRanges, constraint);
      } else {
        for (var i = 0; i < constraint.NumberOfDerivations; ++i) {
          if (!estimator.IsCompatible(tree) || !DerivativeCalculator.IsCompatible(tree)) {
            throw new ArgumentException("The tree contains an unsupported symbol.");
          }

          tree = DerivativeCalculator.Derive(tree, constraint.Variable);
        }

        return estimator.GetConstraintViolation(tree, regionRanges, constraint);
      }
    }
  }
}