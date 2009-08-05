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
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using HeuristicLab.GP.Interfaces;

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

      codeArr = new Instr[functionTree.GetSize()];
      int i = 0;
      foreach (IFunctionTree tree in IteratePrefix(functionTree)) {
        codeArr[i++] = TranslateToInstr(tree);
      }
    }

    private IEnumerable<IFunctionTree> IteratePrefix(IFunctionTree functionTree) {
      List<IFunctionTree> prefixForm = new List<IFunctionTree>();
      prefixForm.Add(functionTree);
      foreach (IFunctionTree subTree in functionTree.SubTrees) {
        prefixForm.AddRange(IteratePrefix(subTree));
      }
      return prefixForm;
    }

    private Instr TranslateToInstr(IFunctionTree tree) {
      Instr instr = new Instr();
      instr.arity = (byte)tree.SubTrees.Count;
      instr.symbol = EvaluatorSymbolTable.MapFunction(tree.Function);
      switch (instr.symbol) {
        case EvaluatorSymbolTable.DIFFERENTIAL:
        case EvaluatorSymbolTable.VARIABLE: {
            VariableFunctionTree varTree = (VariableFunctionTree)tree;
            instr.i_arg0 = (short)dataset.GetVariableIndex(varTree.VariableName);
            instr.d_arg0 = varTree.Weight;
            instr.i_arg1 = (short)varTree.SampleOffset;
            break;
          }
        case EvaluatorSymbolTable.CONSTANT: {
            ConstantFunctionTree constTree = (ConstantFunctionTree)tree;
            instr.d_arg0 = constTree.Value;
            break;
          }
        case EvaluatorSymbolTable.UNKNOWN: {
            throw new NotSupportedException("Unknown function symbol: " + instr.symbol);
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
