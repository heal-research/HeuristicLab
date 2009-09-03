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

namespace HeuristicLab.GP.StructureIdentification {
  /// <summary>
  /// Evaluates FunctionTrees recursively by interpretation of the function symbols in each node with HL2 semantics.
  /// Not thread-safe!
  /// </summary>
  public class HL2TreeEvaluator : TreeEvaluatorBase {
    public HL2TreeEvaluator() : base() { } // for persistence
    public HL2TreeEvaluator(double minValue, double maxValue) : base(minValue, maxValue) { }

    protected override double EvaluateBakedCode() {
      Instr currInstr = codeArr[PC++];
      switch (currInstr.symbol) {
        case EvaluatorSymbolTable.VARIABLE: {
            int row = sampleIndex + currInstr.i_arg1;
            if (row < 0 || row >= dataset.Rows) return double.NaN;
            else return currInstr.d_arg0 * dataset.GetValue(row, currInstr.i_arg0);
          }
        case EvaluatorSymbolTable.CONSTANT: {
            return currInstr.d_arg0;
          }
        case EvaluatorSymbolTable.DIFFERENTIAL: {
            int row = sampleIndex + currInstr.i_arg1;
            if (row < 1 || row >= dataset.Rows) return double.NaN;
            else {
              double prevValue = dataset.GetValue(row - 1, currInstr.i_arg0);
              return currInstr.d_arg0 * (dataset.GetValue(row, currInstr.i_arg0) - prevValue);
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
            return EvaluateBakedCode() - EvaluateBakedCode();
          }
        case EvaluatorSymbolTable.DIVISION: {
            double arg0 = EvaluateBakedCode();
            double arg1 = EvaluateBakedCode();
            if (double.IsNaN(arg0) || double.IsNaN(arg1)) return double.NaN;
            if (Math.Abs(arg1) < (10e-20)) return 0.0; else return (arg0 / arg1);
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
            if (value < 0.0) return -1.0;
            if (value > 0.0) return 1.0;
            return 0.0;
          }
        case EvaluatorSymbolTable.SQRT: {
            return Math.Sqrt(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.TANGENS: {
            return Math.Tan(EvaluateBakedCode());
          }
        case EvaluatorSymbolTable.AND: { // only defined for inputs 1 and 0
            double result = EvaluateBakedCode();
            bool hasNaNBranch = false;
            for (int i = 1; i < currInstr.arity; i++) {
              if (result < 0.5 || double.IsNaN(result)) hasNaNBranch |= double.IsNaN(EvaluateBakedCode());
              else {
                result = EvaluateBakedCode();
              }
            }
            if (hasNaNBranch || double.IsNaN(result)) return double.NaN;
            if (result < 0.5) return 0.0;
            return 1.0;
          }
        case EvaluatorSymbolTable.EQU: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (double.IsNaN(x) || double.IsNaN(y)) return double.NaN;
            // direct comparison of double values is most likely incorrect but
            // that's the way how it is implemented in the standard HL2 function library
            if (x == y) return 1.0; else return 0.0;
          }
        case EvaluatorSymbolTable.GT: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (double.IsNaN(x) || double.IsNaN(y)) return double.NaN;
            if (x > y) return 1.0;
            return 0.0;
          }
        case EvaluatorSymbolTable.IFTE: { // only defined for condition 0 or 1
            double condition = EvaluateBakedCode();
            double result;
            bool hasNaNBranch = false;
            if (double.IsNaN(condition)) return double.NaN;
            if (condition > 0.5) {
              result = EvaluateBakedCode(); hasNaNBranch = double.IsNaN(EvaluateBakedCode());
            } else {
              hasNaNBranch = double.IsNaN(EvaluateBakedCode()); result = EvaluateBakedCode();
            }
            if (hasNaNBranch) return double.NaN;
            return result;
          }
        case EvaluatorSymbolTable.LT: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (double.IsNaN(x) || double.IsNaN(y)) return double.NaN;
            if (x < y) return 1.0;
            return 0.0;
          }
        case EvaluatorSymbolTable.NOT: { // only defined for inputs 0 or 1
            double result = EvaluateBakedCode();
            if (double.IsNaN(result)) return double.NaN;
            if (result < 0.5) return 1.0;
            return 0.0;
          }
        case EvaluatorSymbolTable.OR: { // only defined for inputs 0 or 1
            double result = EvaluateBakedCode();
            bool hasNaNBranch = false;
            for (int i = 1; i < currInstr.arity; i++) {
              if (double.IsNaN(result) || result > 0.5) hasNaNBranch |= double.IsNaN(EvaluateBakedCode());
              else
                result = EvaluateBakedCode();
            }
            if (hasNaNBranch || double.IsNaN(result)) return double.NaN;
            if (result > 0.5) return 1.0;
            return 0.0;
          }
        default: {
            throw new NotImplementedException();
          }
      }
    }
  }
}
