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
    protected double estimatedValueMax;
    protected double estimatedValueMin;

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

    public void ResetEvaluator(Dataset dataset, int targetVariable, int start, int end, double punishmentFactor) {
      this.dataset = dataset;
      double maximumPunishment = punishmentFactor * dataset.GetRange(targetVariable, start, end);

      // get the mean of the values of the target variable to determine the max and min bounds of the estimated value
      double targetMean = dataset.GetMean(targetVariable, start, end);
      estimatedValueMin = targetMean - maximumPunishment;
      estimatedValueMax = targetMean + maximumPunishment;
    }

    public void PrepareForEvaluation(IFunctionTree functionTree) {
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
    protected void SkipBakedCode() {
      int i = 1;
      while (i > 0) {
        i += codeArr[PC++].arity;
        i--;
      }
    }

    protected abstract double EvaluateBakedCode();

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      TreeEvaluatorBase clone = (TreeEvaluatorBase)base.Clone(clonedObjects);
      if (!clonedObjects.ContainsKey(dataset.Guid)) {
        clone.dataset = (Dataset)dataset.Clone(clonedObjects);
      } else {
        clone.dataset = (Dataset)clonedObjects[dataset.Guid];
      }
      clone.estimatedValueMax = estimatedValueMax;
      clone.estimatedValueMin = estimatedValueMin;
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute minEstimatedValueAttr = document.CreateAttribute("MinEstimatedValue");
      minEstimatedValueAttr.Value = XmlConvert.ToString(estimatedValueMin);
      node.Attributes.Append(minEstimatedValueAttr);

      XmlAttribute maxEstimatedValueAttr = document.CreateAttribute("MaxEstimatedValue");
      maxEstimatedValueAttr.Value = XmlConvert.ToString(estimatedValueMax);
      node.Attributes.Append(maxEstimatedValueAttr);

      node.AppendChild(PersistenceManager.Persist("Dataset", dataset, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      estimatedValueMax = XmlConvert.ToDouble(node.Attributes["MaxEstimatedValue"].Value);
      estimatedValueMin = XmlConvert.ToDouble(node.Attributes["MinEstimatedValue"].Value);

      dataset = (Dataset)PersistenceManager.Restore(node.SelectSingleNode("Dataset"), restoredObjects);
    }
  }
}
