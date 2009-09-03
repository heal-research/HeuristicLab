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
using System.Xml;

namespace HeuristicLab.GP.StructureIdentification {
  /// <summary>
  /// Base class for tree evaluators
  /// </summary>
  public abstract class TreeEvaluatorBase : ItemBase, ITreeEvaluator {
    protected const double EPSILON = 1.0e-7;

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

    public double UpperEvaluationLimit { get; set; }
    public double LowerEvaluationLimit { get; set; }

    public TreeEvaluatorBase() // for persistence
      : this(double.MinValue, double.MaxValue) {
    }

    public TreeEvaluatorBase(double minEstimatedValue, double maxEstimatedValue)
      : base() {
      UpperEvaluationLimit = maxEstimatedValue;
      LowerEvaluationLimit = minEstimatedValue;
    }

    public void PrepareForEvaluation(Dataset dataset, IFunctionTree functionTree) {
      this.dataset = dataset;
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

      double estimated = Math.Min(Math.Max(EvaluateBakedCode(), LowerEvaluationLimit), UpperEvaluationLimit);
      if (double.IsNaN(estimated)) estimated = UpperEvaluationLimit;
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
      clone.UpperEvaluationLimit = UpperEvaluationLimit;
      clone.LowerEvaluationLimit = LowerEvaluationLimit;
      return clone;
    }

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute maxValueAttribute = document.CreateAttribute("UpperEvaluationLimit");
      XmlAttribute minValueAttribute = document.CreateAttribute("LowerEvaluationLimit");
      maxValueAttribute.Value = XmlConvert.ToString(UpperEvaluationLimit);
      minValueAttribute.Value = XmlConvert.ToString(LowerEvaluationLimit);
      node.Attributes.Append(minValueAttribute);
      node.Attributes.Append(maxValueAttribute);
      return node;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      LowerEvaluationLimit = XmlConvert.ToDouble(node.Attributes["LowerEvaluationLimit"].Value);
      UpperEvaluationLimit = XmlConvert.ToDouble(node.Attributes["UpperEvaluationLimit"].Value);
    }
  }
}
