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
using System.Diagnostics;
using HeuristicLab.Common; // double.IsAlmost extension

namespace HeuristicLab.GP.StructureIdentification {
  /// <summary>
  /// Evaluates FunctionTrees recursively by interpretation of the function symbols in each node.
  /// Not thread-safe!
  /// </summary>
  public class HL3TreeEvaluator : TreeEvaluatorBase {
    protected override double EvaluateBakedCode() {
      Instr currInstr = codeArr[PC++];
      switch (currInstr.symbol) {
        case EvaluatorSymbolTable.VARIABLE: {
            int row = sampleIndex + currInstr.i_arg1;
            if (row < 0 || row >= dataset.Rows) throw new InvalidOperationException("Out of range access to dataset row: " + row);
            else return currInstr.d_arg0 * dataset.GetValue(row, currInstr.i_arg0);
          }
        case EvaluatorSymbolTable.CONSTANT: {
            return currInstr.d_arg0;
          }
        case EvaluatorSymbolTable.DIFFERENTIAL: {
            int row = sampleIndex + currInstr.i_arg1;
            if (row < 0 || row >= dataset.Rows) throw new InvalidOperationException("Out of range access to dataset row: " + row);
            else if (row < 1) return 0.0;
            else {
              double prevValue = dataset.GetValue(row - 1, currInstr.i_arg0);
              if (double.IsNaN(prevValue) || double.IsInfinity(prevValue)) return 0.0;
              else return currInstr.d_arg0 * (dataset.GetValue(row, currInstr.i_arg0) - prevValue);
            }
          }
        case EvaluatorSymbolTable.MULTIPLICATION: {
            double result = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              result *= EvaluateBakedCode();
            }
            return result;
          }
        case EvaluatorSymbolTable.ADDITION: {
            double sum = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              sum += EvaluateBakedCode();
            }
            return sum;
          }
        case EvaluatorSymbolTable.SUBTRACTION: {
            double result = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              result -= EvaluateBakedCode();
            }
            return result;
          }
        case EvaluatorSymbolTable.DIVISION: {
            double result;
            result = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              result /= EvaluateBakedCode();
            }
            if (double.IsInfinity(result)) return 0.0;
            else return result;
          }
        case EvaluatorSymbolTable.AVERAGE: {
            double sum = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              sum += EvaluateBakedCode();
            }
            return sum / currInstr.arity;
          }
        case EvaluatorSymbolTable.COSINUS: {
            return Math.Cos(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.SINUS: {
            return Math.Sin(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.EXP: {
            return Math.Exp(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.LOG: {
            return Math.Log(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.POWER: {
            double x = EvaluateBakedCode();
            double p = EvaluateBakedCode();
            return Math.Pow(x, p);
          }
        case EvaluatorSymbolTable.SIGNUM: {
            double value = EvaluateBakedCode();
            if (double.IsNaN(value)) return double.NaN;
            else return Math.Sign(value);
          }
        case EvaluatorSymbolTable.SQRT: {
            return Math.Sqrt(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.TANGENS: {
            return Math.Tan(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.AND: {
            double result = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              if (result < 0.0) SkipBakedCode();
              else {
                result = EvaluateBakedCode();
              }
            }
            return Math.Sign(result);
          }
        case EvaluatorSymbolTable.EQU: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (x.IsAlmost(y)) return 1.0; else return -1.0;
          }
        case EvaluatorSymbolTable.GT: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (x > y) return 1.0;
            else return -1.0;
          }
        case EvaluatorSymbolTable.IFTE: {
            double condition = EvaluateBakedCode();
            double result;
            if (condition < 0.0) {
              result = EvaluateBakedCode(); SkipBakedCode();
            } else {
              SkipBakedCode(); result = EvaluateBakedCode();
            }
            return result;
          }
        case EvaluatorSymbolTable.LT: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (x < y) return 1.0;
            else return -1.0;
          }
        case EvaluatorSymbolTable.NOT: {
            return -EvaluateBakedCode();
          }
        case EvaluatorSymbolTable.OR: {
            double result = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              if (result >= 0.0) SkipBakedCode();
              else {
                result = EvaluateBakedCode();
              }
            }
            return Math.Sign(result);
          }
        case EvaluatorSymbolTable.XOR: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (x > y) {
              double tmp = x;
              x = y;
              y = tmp;
            }
            // invariant y >= x 
            if (y < 0.0 || x > 0.0) return -1.0;
            else return 1.0;
          }
        default: {
            throw new NotImplementedException();
          }
      }
    }
  }
}
