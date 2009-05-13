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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.Diagnostics;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification {
  /// <summary>
  /// Evaluates FunctionTrees recursively by interpretation of the function symbols in each node.
  /// Not thread-safe!
  /// </summary>
  public class BakedTreeEvaluator {
    private const double EPSILON = 1.0e-7;
    private double estimatedValueMax;
    private double estimatedValueMin;

    private class Instr {
      public double d_arg0;
      public short i_arg0;
      public short i_arg1;
      public byte arity;
      public byte symbol;
      public ushort exprLength;
      public IFunction function;
    }

    private Instr[] codeArr;
    private int PC;
    private Dataset dataset;
    private int sampleIndex;

    public void ResetEvaluator(BakedFunctionTree functionTree, Dataset dataset, int targetVariable, int start, int end, double punishmentFactor) {
      this.dataset = dataset;
      double maximumPunishment = punishmentFactor * dataset.GetRange(targetVariable, start, end);

      // get the mean of the values of the target variable to determine the max and min bounds of the estimated value
      double targetMean = dataset.GetMean(targetVariable, start, end);
      estimatedValueMin = targetMean - maximumPunishment;
      estimatedValueMax = targetMean + maximumPunishment;

      List<LightWeightFunction> linearRepresentation = functionTree.LinearRepresentation;
      codeArr = new Instr[linearRepresentation.Count];
      int i = 0;
      foreach (LightWeightFunction f in linearRepresentation) {
        codeArr[i++] = TranslateToInstr(f);
      }
      exprIndex = 0;
      ushort exprLength;
      bool constExpr;
      PatchExpressionLengthsAndConstants(0, out constExpr, out exprLength);
    }

    ushort exprIndex;
    private void PatchExpressionLengthsAndConstants(ushort index, out bool constExpr, out ushort exprLength) {
      exprLength = 1;
      if (codeArr[index].arity == 0) {
        // when no children then it's a constant expression only if the terminal is a constant
        constExpr = codeArr[index].symbol == EvaluatorSymbolTable.CONSTANT;
      } else {
        constExpr = true; // when there are children it's a constant expression if all children are constant;
      }
      for (int i = 0; i < codeArr[index].arity; i++) {
        exprIndex++;
        ushort branchLength;
        bool branchConstExpr;
        PatchExpressionLengthsAndConstants(exprIndex, out branchConstExpr, out branchLength);
        exprLength += branchLength;
        constExpr &= branchConstExpr;
      }

      if (constExpr) {
        PC = index;
        codeArr[index].d_arg0 = EvaluateBakedCode();
        codeArr[index].symbol = EvaluatorSymbolTable.CONSTANT;
      }
      codeArr[index].exprLength = exprLength;
    }

    private Instr TranslateToInstr(LightWeightFunction f) {
      Instr instr = new Instr();
      instr.arity = f.arity;
      instr.symbol = EvaluatorSymbolTable.MapFunction(f.functionType);
      switch (instr.symbol) {
        case EvaluatorSymbolTable.DIFFERENTIAL:
        case EvaluatorSymbolTable.VARIABLE: {
            instr.i_arg0 = (short)f.data[0]; // var
            instr.d_arg0 = f.data[1]; // weight
            instr.i_arg1 = (short)f.data[2]; // sample-offset
            instr.exprLength = 1;
            break;
          }
        case EvaluatorSymbolTable.CONSTANT: {
            instr.d_arg0 = f.data[0]; // value
            instr.exprLength = 1;
            break;
          }
        case EvaluatorSymbolTable.UNKNOWN: {
            instr.function = f.functionType;
            instr.exprLength = 1;
            break;
          }
      }
      return instr;
    }

    public double Evaluate(int sampleIndex) {
      PC = 0;
      this.sampleIndex = sampleIndex;

      double estimated = EvaluateBakedCode();
      if (double.IsNaN(estimated) || double.IsInfinity(estimated)) {
        estimated = estimatedValueMax;
      } else if (estimated > estimatedValueMax) {
        estimated = estimatedValueMax;
      } else if (estimated < estimatedValueMin) {
        estimated = estimatedValueMin;
      }
      return estimated;
    }

    // skips a whole branch
    private void SkipBakedCode() {
      PC += codeArr[PC].exprLength;
    }

    private double EvaluateBakedCode() {
      Instr currInstr = codeArr[PC++];
      switch (currInstr.symbol) {
        case EvaluatorSymbolTable.VARIABLE: {
            int row = sampleIndex + currInstr.i_arg1;
            if (row < 0 || row >= dataset.Rows) return double.NaN;
            else return currInstr.d_arg0 * dataset.GetValue(row, currInstr.i_arg0);
          }
        case EvaluatorSymbolTable.CONSTANT: {
            PC += currInstr.exprLength - 1;
            return currInstr.d_arg0;
          }
        case EvaluatorSymbolTable.DIFFERENTIAL: {
            int row = sampleIndex + currInstr.i_arg1;
            if (row < 0 || row >= dataset.Rows) return double.NaN;
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
            if (currInstr.arity == 1) {
              return -EvaluateBakedCode();
            } else {
              double result = EvaluateBakedCode();
              for (int i = 1; i < currInstr.arity; i++) {
                result -= EvaluateBakedCode();
              }
              return result;
            }
          }
        case EvaluatorSymbolTable.DIVISION: {
            double result;
            if (currInstr.arity == 1) {
              result = 1.0 / EvaluateBakedCode();
            } else {
              result = EvaluateBakedCode();
              for (int i = 1; i < currInstr.arity; i++) {
                result /= EvaluateBakedCode();
              }
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
        case EvaluatorSymbolTable.AND: { // only defined for inputs 1 and 0
            double result = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              if (result == 0.0) SkipBakedCode();
              else {
                result = EvaluateBakedCode();
              }
              Debug.Assert(result == 0.0 || result == 1.0);
            }
            return result;
          }
        case EvaluatorSymbolTable.EQU: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (Math.Abs(x - y) < EPSILON) return 1.0; else return 0.0;
          }
        case EvaluatorSymbolTable.GT: {
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            if (x > y) return 1.0;
            else return 0.0;
          }
        case EvaluatorSymbolTable.IFTE: { // only defined for condition 0 or 1
            double condition = EvaluateBakedCode();
            Debug.Assert(condition == 0.0 || condition == 1.0);
            double result;
            if (condition == 0.0) {
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
            else return 0.0;
          }
        case EvaluatorSymbolTable.NOT: { // only defined for inputs 0 or 1
            double result = EvaluateBakedCode();
            Debug.Assert(result == 0.0 || result == 1.0);
            return Math.Abs(result - 1.0);
          }
        case EvaluatorSymbolTable.OR: { // only defined for inputs 0 or 1
            double result = EvaluateBakedCode();
            for (int i = 1; i < currInstr.arity; i++) {
              if (result > 0.0) SkipBakedCode();
              else {
                result = EvaluateBakedCode();
                Debug.Assert(result == 0.0 || result == 1.0);
              }
            }
            return result;
          }
        case EvaluatorSymbolTable.XOR: { // only defined for inputs 0 or 1
            double x = EvaluateBakedCode();
            double y = EvaluateBakedCode();
            return Math.Abs(x - y);
          }
        case EvaluatorSymbolTable.UNKNOWN: { // evaluate functions which are not statically defined directly
            return currInstr.function.Apply();
          }
        default: {
            throw new NotImplementedException();
          }
      }
    }
  }
}
