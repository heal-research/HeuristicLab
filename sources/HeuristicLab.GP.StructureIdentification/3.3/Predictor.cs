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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP;
using HeuristicLab.Modeling;
using System;
using System.Xml;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification {
  public class Predictor : PredictorBase {
    public Predictor() : base() { } // for persistence
    public Predictor(ITreeEvaluator evaluator, IGeneticProgrammingModel tree, double lowerPredictionLimit, double upperPredictionLimit)
      : base(lowerPredictionLimit, upperPredictionLimit) {
      this.treeEvaluator = evaluator;
      this.functionTree = tree;
    }

    private ITreeEvaluator treeEvaluator;
    public ITreeEvaluator TreeEvaluator {
      get { return (ITreeEvaluator) this.treeEvaluator.Clone(); }
    }

    private IGeneticProgrammingModel functionTree;
    public IGeneticProgrammingModel FunctionTree {
      get { return functionTree; }
      set { this.functionTree = value; }
    }

    public override double[] Predict(Dataset input, int start, int end) {
      treeEvaluator.UpperEvaluationLimit = UpperPredictionLimit;
      treeEvaluator.LowerEvaluationLimit = LowerPredictionLimit;

      if (start < 0 || end <= start) throw new ArgumentException("start must be larger than zero and strictly smaller than end");
      if (end > input.Rows) throw new ArgumentOutOfRangeException("number of rows in input is smaller then end");
      treeEvaluator.PrepareForEvaluation(input, functionTree.FunctionTree);
      double[] result = new double[end - start];
      for (int i = 0; i < result.Length; i++) {
        try {
          result[i] = treeEvaluator.Evaluate(i + start);
        }
        catch (ArgumentException) {
          result[i] = double.NaN;
        }
      }
      return result;
    }

    public override IEnumerable<string> GetInputVariables() {
      HashSet<string> inputVariables = new HashSet<string>();
      foreach (IFunctionTree ft in FunctionTreeIterator.IteratePrefix(functionTree.FunctionTree)) {
        if (ft is VariableFunctionTree) {
          VariableFunctionTree variable = (VariableFunctionTree)ft;
          inputVariables.Add(variable.VariableName);
        }
      }
      return inputVariables;
    }

    public override IView CreateView() {
      return new PredictorView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Predictor clone = (Predictor)base.Clone(clonedObjects);
      clone.treeEvaluator = (ITreeEvaluator)Auxiliary.Clone(treeEvaluator, clonedObjects);
      clone.functionTree = (IGeneticProgrammingModel)Auxiliary.Clone(functionTree, clonedObjects);
      return clone;
    }

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Evaluator", treeEvaluator, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("FunctionTree", functionTree, document, persistedObjects));
      return node;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      treeEvaluator = (ITreeEvaluator)PersistenceManager.Restore(node.SelectSingleNode("Evaluator"), restoredObjects);
      functionTree = (IGeneticProgrammingModel)PersistenceManager.Restore(node.SelectSingleNode("FunctionTree"), restoredObjects);
    }
  }
}
