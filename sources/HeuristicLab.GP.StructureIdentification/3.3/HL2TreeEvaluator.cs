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
  public class HL2TreeEvaluator : ItemBase, ITreeEvaluator {
    private const double EPSILON = 1.0e-10;
    private double estimatedValueMax;
    private double estimatedValueMin;

    private class Instr {
      public double d_arg0;
      public short i_arg0;
      public short i_arg1;
      public byte arity;
      public byte symbol;
      public IFunction function;
    }

    private Instr[] codeArr;
    private int PC;
    private Dataset dataset;
    private int sampleIndex;

    public void ResetEvaluator(Dataset dataset, int targetVariable, int start, int end, double punishmentFactor) {
      this.dataset = dataset;
      double maximumPunishment = punishmentFactor * dataset.GetRange(targetVariable, start, end);

      // get the mean of the values of the target variable to determine the max and min bounds of the estimated value
      double targetMean = dataset.GetMean(targetVariable, start, end);
      estimatedValueMin = targetMean - maximumPunishment;
      estimatedValueMax = targetMean + maximumPunishment;
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
            break;
          }
        case EvaluatorSymbolTable.CONSTANT: {
            instr.d_arg0 = f.data[0]; // value
            break;
          }
        case EvaluatorSymbolTable.UNKNOWN: {
            instr.function = f.functionType;
            break;
          }
      }
      return instr;
    }

    public double Evaluate(IFunctionTree functionTree, int sampleIndex) {
      BakedFunctionTree bakedTree = functionTree as BakedFunctionTree;
      if (bakedTree == null) throw new ArgumentException("HL2Evaluator can only evaluate BakedFunctionTrees");

      List<LightWeightFunction> linearRepresentation = bakedTree.LinearRepresentation;
      codeArr = new Instr[linearRepresentation.Count];
      int i = 0;
      foreach (LightWeightFunction f in linearRepresentation) {
        codeArr[i++] = TranslateToInstr(f);
      }

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
      int i = 1;
      while (i > 0) {
        i += codeArr[PC++].arity;
        i--;
      }
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
