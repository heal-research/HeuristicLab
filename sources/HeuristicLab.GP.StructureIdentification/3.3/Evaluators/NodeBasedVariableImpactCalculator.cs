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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Linq;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Modeling;

namespace HeuristicLab.GP.StructureIdentification {
  public class NodeBasedVariableImpactCalculator : OperatorBase {

    public NodeBasedVariableImpactCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The GP model", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "TargetVariable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("InputVariableNames", "Names of used variables in the model (optional)", typeof(ItemList<StringData>), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "SamplesStart", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "SamplesEnd", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeEvaluator", "Evaluator that should be used for impact calculation", typeof(ITreeEvaluator), VariableKind.In));
      AddVariableInfo(new VariableInfo(ModelingResult.VariableNodeImpact.ToString(), "Variable impacts", typeof(ItemList), VariableKind.New | VariableKind.Out));
    }

    public override string Description {
      get { return @"Calculates the impact of all allowed input variables on the quality of the model based on node impacts."; }
    }

    public override IOperation Apply(IScope scope) {
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      string targetVariableName = GetVariableValue<StringData>("TargetVariable", scope, true).Data;
      int targetVariable = dataset.GetVariableIndex(targetVariableName);
      ItemList<StringData> inputVariableNames = GetVariableValue<ItemList<StringData>>("InputVariableNames", scope, true, false);
      ITreeEvaluator evaluator = GetVariableValue<ITreeEvaluator>("TreeEvaluator", scope, true);
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;

      Dictionary<string, double> qualityImpacts;
      if (inputVariableNames == null)
        qualityImpacts = Calculate(dataset, evaluator, gpModel.FunctionTree, targetVariableName, start, end);
      else
        qualityImpacts = Calculate(dataset, evaluator, gpModel.FunctionTree, targetVariableName, inputVariableNames.Select(iv => iv.Data), start, end);

      ItemList varImpacts = GetVariableValue<ItemList>(ModelingResult.VariableNodeImpact.ToString(), scope, true, false);
      if (varImpacts == null) {
        varImpacts = new ItemList();
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(ModelingResult.VariableNodeImpact.ToString()), varImpacts));
      }

      varImpacts.Clear();
      foreach (KeyValuePair<string, double> p in qualityImpacts) {
        if (p.Key != targetVariableName) {
          ItemList row = new ItemList();
          row.Add(new StringData(p.Key));
          row.Add(new DoubleData(p.Value));
          varImpacts.Add(row);
        }
      }

      return null;
    }

    public static Dictionary<string, double> Calculate(Dataset dataset, ITreeEvaluator evaluator,
      IFunctionTree tree, string targetVariableName, int start, int end) {
      return Calculate(dataset, evaluator, tree, targetVariableName, null, start, end);
    }

    public static Dictionary<string, double> Calculate(Dataset dataset, ITreeEvaluator evaluator, IFunctionTree tree, string targetVariableName, IEnumerable<string> inputVariableNames, int start, int end) {
      Dictionary<string, double> impacts = new Dictionary<string, double>();
      Dictionary<IFunctionTree, double> nodeImpacts = new Dictionary<IFunctionTree, double>();
      Dictionary<IFunctionTree, double> nodeReplacementValues = new Dictionary<IFunctionTree, double>();
      Dictionary<IFunctionTree, IFunctionTree> parent = new Dictionary<IFunctionTree, IFunctionTree>();
      int targetVariable = dataset.GetVariableIndex(targetVariableName);
      IEnumerable<string> variables;
      if (inputVariableNames != null)
        variables = inputVariableNames;
      else
        variables = dataset.VariableNames;

      parent[tree] = null;
      foreach (var node in FunctionTreeIterator.IteratePostfix(tree)) {
        foreach (var subTree in node.SubTrees) {
          parent[subTree] = node;
        }
        nodeReplacementValues[node] = CalculateReplacementValue(dataset, evaluator, node, targetVariable, start, end);
      }

      double originalMse = CalculateMSE(dataset, evaluator, tree, targetVariable, start, end);
      foreach (var node in FunctionTreeIterator.IteratePostfix(tree)) {
        IFunctionTree newTree = ReplaceBranchInTree(tree, node, nodeReplacementValues[node]);
        double newMse = CalculateMSE(dataset, evaluator, newTree, targetVariable, start, end);
        nodeImpacts[node] = newMse / originalMse;
      }


      foreach (string variableName in variables) {
        var matchingNodes = from node in nodeImpacts.Keys
                            where node is VariableFunctionTree && ((VariableFunctionTree)node).VariableName == variableName
                            select node;
        double maxImpact;
        if (matchingNodes.Count() > 0) {
          maxImpact = (from matchingNode in matchingNodes
                       select (from n in AncestorList(matchingNode, parent)
                               select nodeImpacts[n]).Min()).Max();
        } else {
          maxImpact = 1.0;
        }

        impacts[variableName] = maxImpact;
      }

      return impacts;
    }

    private static double CalculateMSE(Dataset dataset, ITreeEvaluator evaluator, IFunctionTree tree, int targetVariable, int start, int end) {
      double[,] values = new double[end - start, 2];
      evaluator.PrepareForEvaluation(dataset, tree);
      for (int i = start; i < end; i++) {
        values[i - start, 0] = dataset.GetValue(i, targetVariable);
        values[i - start, 1] = evaluator.Evaluate(i);
      }
      return SimpleMSEEvaluator.Calculate(values);
    }

    private static IEnumerable<IFunctionTree> AncestorList(IFunctionTree node, Dictionary<IFunctionTree, IFunctionTree> parent) {
      while (node != null) {
        yield return node;
        node = parent[node];
      }
    }

    private static double CalculateReplacementValue(Dataset dataset, ITreeEvaluator evaluator, IFunctionTree tree, int targetVariable, int start, int end) {
      double[] values = new double[end - start];
      evaluator.PrepareForEvaluation(dataset, tree);
      for (int i = start; i < end; i++) {
        values[i - start] = evaluator.Evaluate(i);
      }
      return Statistics.Median(values);
    }

    private static IFunctionTree ReplaceBranchInTree(IFunctionTree tree, IFunctionTree node, double p) {
      if (tree == node) return CreateConstantNode(p);
      List<IFunctionTree> originalSubTrees = new List<IFunctionTree>(tree.SubTrees);
      while (tree.SubTrees.Count > 0) tree.RemoveSubTree(0);
      IFunctionTree clonedNode = (IFunctionTree)tree.Clone();
      for (int i = 0; i < originalSubTrees.Count; i++) {
        tree.AddSubTree(originalSubTrees[i]);
        clonedNode.AddSubTree(ReplaceBranchInTree(originalSubTrees[i], node, p));
      }
      return clonedNode;
    }

    private static IFunctionTree CreateConstantNode(double value) {
      ConstantFunctionTree constantTree = (ConstantFunctionTree)(new Constant().GetTreeNode());
      constantTree.Value = value;
      return (IFunctionTree)constantTree;
    }
  }
}
