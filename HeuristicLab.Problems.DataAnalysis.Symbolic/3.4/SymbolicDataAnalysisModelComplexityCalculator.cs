#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class SymbolicDataAnalysisModelComplexityCalculator {
    public static double CalculateComplexity(ISymbolicExpressionTree tree) {
      return CalculateComplexity(tree.Root);
    }
    public static double CalculateComplexity(ISymbolicExpressionTreeNode treeNode) {
      var node = treeNode;
      if (node.Symbol is ProgramRootSymbol) node = node.GetSubtree(0);
      if (node.Symbol is StartSymbol) node = node.GetSubtree(0);

      switch (OpCodes.MapSymbolToOpCode(node)) {
        case OpCodes.Constant: {
            return 1;
          }
        case OpCodes.Variable:
        case OpCodes.BinaryFactorVariable:
        case OpCodes.FactorVariable: {
            return 2;
          }
        case OpCodes.Add:
        case OpCodes.Sub: {
            double complexity = 0;
            for (int i = 0; i < node.SubtreeCount; i++) {
              complexity += CalculateComplexity(node.GetSubtree(i));
            }
            return complexity;
          }
        case OpCodes.Mul:
        case OpCodes.Div: {
            double complexity = 1;
            for (int i = 0; i < node.SubtreeCount; i++) {
              var nodeComplexity = CalculateComplexity(node.GetSubtree(i));
              complexity *= nodeComplexity + 1;
            }
            return complexity;
          }
        case OpCodes.Sin:
        case OpCodes.Cos:
        case OpCodes.Tan:
        case OpCodes.Exp:
        case OpCodes.Log: {
            double complexity = CalculateComplexity(node.GetSubtree(0));
            return Math.Pow(2.0, complexity);
          }
        case OpCodes.Square: {
            double complexity = CalculateComplexity(node.GetSubtree(0));
            return complexity * complexity;
          }
        case OpCodes.SquareRoot: {
            double complexity = CalculateComplexity(node.GetSubtree(0));
            return complexity * complexity * complexity;
          }
        case OpCodes.Power:
        case OpCodes.Root: {
            double complexity = CalculateComplexity(node.GetSubtree(0));
            var exponent = node.GetSubtree(1) as ConstantTreeNode;
            if (exponent != null) {
              double expVal = exponent.Value;
              if (expVal < 0) expVal = Math.Abs(expVal);
              if (expVal < 1) expVal = 1 / expVal;
              return Math.Pow(complexity, Math.Round(expVal));
            }

            double expComplexity = CalculateComplexity(node.GetSubtree(1));
            return Math.Pow(complexity, 2 * expComplexity);
          }

        default:
          throw new NotSupportedException();
      }
    }
  }
}
