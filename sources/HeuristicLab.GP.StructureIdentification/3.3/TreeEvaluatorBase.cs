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
  /// Base class for tree evaluators
  /// </summary>
  public abstract class TreeEvaluatorBase : ItemBase, ITreeEvaluator {
    protected const double EPSILON = 1.0e-7;
    protected double maxValue;
    protected double minValue;

    protected class Instr {
      public double d_arg0;
      public short i_arg0;
      public short i_arg1;
      public byte arity;
      public byte symbol;
      public IFunction function;
    }

    protected Instr[] codeArr;
    protected int PC;
    protected Dataset dataset;
    protected int sampleIndex;

    public void PrepareForEvaluation(Dataset dataset, int targetVariable, int start, int end, double punishmentFactor, IFunctionTree functionTree) {
      this.dataset = dataset;
      // calculate upper and lower bounds for the estimated value (mean +/- punishmentFactor * range)
      double mean = dataset.GetMean(targetVariable, start, end);
      double range = dataset.GetRange(targetVariable, start, end);
      maxValue = mean + punishmentFactor * range;
      minValue = mean - punishmentFactor * range;

      BakedFunctionTree bakedTree = functionTree as BakedFunctionTree;
      if (bakedTree == null) throw new ArgumentException("TreeEvaluators can only evaluate BakedFunctionTrees");

      List<LightWeightFunction> linearRepresentation = bakedTree.LinearRepresentation;
      codeArr = new Instr[linearRepresentation.Count];
      int i = 0;
      foreach (LightWeightFunction f in linearRepresentation) {
        codeArr[i++] = TranslateToInstr(f);
      }
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

    public double Evaluate(int sampleIndex) {
      PC = 0;
      this.sampleIndex = sampleIndex;

      double estimated = EvaluateBakedCode();
      if (double.IsNaN(estimated) || double.IsInfinity(estimated)) estimated = maxValue;
      else if (estimated < minValue) estimated = minValue;
      else if (estimated > maxValue) estimated = maxValue;
      return estimated;
    }

    // skips a whole branch
    protected void SkipBakedCode() {
      int i = 1;
      while (i > 0) {
        i += codeArr[PC++].arity;
        i--;
      }
    }

    protected abstract double EvaluateBakedCode();
  }
}
