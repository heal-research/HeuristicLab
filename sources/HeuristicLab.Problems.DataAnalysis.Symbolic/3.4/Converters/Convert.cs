#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using AutoDiff;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {

  ///  static class which provides methods for all converters for trees for convenience 
  public static class Convert {
    public static void ConvertToAutoDiff(ISymbolicExpressionTree tree, bool makeVariableWeightsVariable,
      out string[] variableNames, out int[] lags, out double[] initialConstants,
      out TreeToAutoDiffTermConverter.ParametricFunction func,
      out TreeToAutoDiffTermConverter.ParametricFunctionGradient func_grad) {
      var success = TreeToAutoDiffTermConverter.TryTransformToAutoDiff(tree, makeVariableWeightsVariable, out variableNames,
        out lags, out initialConstants, out func, out func_grad);
      if (!success) throw new ArgumentException("Cannot convert tree to AutoDiff term.");
    }

    public static ISymbolicExpressionTree Simplify(ISymbolicExpressionTree tree) {
      var simplifier = new TreeSimplifier();
      return simplifier.Simplify(tree);
    }

    public static ISymbolicExpressionTree CreateLinearModel(string[] variableNames, double[] coefficients,
      double @const = 0) {
      return LinearModelToTreeConverter.CreateTree(variableNames, coefficients, @const);
    }
  }
}
