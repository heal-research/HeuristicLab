#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("SymbolicClassificationPruningOperator", "An operator which prunes symbolic classificaton trees.")]
  public class SymbolicClassificationPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    private const string ModelCreatorParameterName = "ModelCreator";

    #region parameter properties
    public ILookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (ILookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    #endregion

    protected SymbolicClassificationPruningOperator(SymbolicClassificationPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationPruningOperator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicClassificationPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicClassificationPruningOperator(ISymbolicDataAnalysisSolutionImpactValuesCalculator impactValuesCalculator)
      : base(impactValuesCalculator) {
      Parameters.Add(new LookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName));
    }

    protected override ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, IDataAnalysisProblemData problemData, DoubleLimit estimationLimits) {
      var model = ModelCreatorParameter.ActualValue.CreateSymbolicClassificationModel(tree, interpreter, estimationLimits.Lower, estimationLimits.Upper);
      var classificationProblemData = (IClassificationProblemData)problemData;
      var rows = classificationProblemData.TrainingIndices;
      model.RecalculateModelParameters(classificationProblemData, rows);
      return model;
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var classificationModel = (IClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)ProblemData;
      var trainingIndices = Enumerable.Range(FitnessCalculationPartition.Start, FitnessCalculationPartition.Size);

      return Evaluate(classificationModel, classificationProblemData, trainingIndices);
    }

    private static double Evaluate(IClassificationModel model, IClassificationProblemData problemData, IEnumerable<int> rows) {
      var estimatedValues = model.GetEstimatedClassValues(problemData.Dataset, rows);
      var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      OnlineCalculatorError errorState;
      var quality = OnlineAccuracyCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }

    public static ISymbolicExpressionTree Prune(ISymbolicExpressionTree tree, ISymbolicClassificationModelCreator modelCreator,
      SymbolicClassificationSolutionImpactValuesCalculator impactValuesCalculator, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      IClassificationProblemData problemData, DoubleLimit estimationLimits, IEnumerable<int> rows,
      double nodeImpactThreshold = 0.0, bool pruneOnlyZeroImpactNodes = false) {
      var clonedTree = (ISymbolicExpressionTree)tree.Clone();
      var model = modelCreator.CreateSymbolicClassificationModel(clonedTree, interpreter, estimationLimits.Lower, estimationLimits.Upper);

      var nodes = clonedTree.IterateNodesPrefix().ToList();
      double quality = Evaluate(model, problemData, rows);

      for (int i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        if (node is ConstantTreeNode) continue;

        double impactValue, replacementValue;
        impactValuesCalculator.CalculateImpactAndReplacementValues(model, node, problemData, rows, out impactValue, out replacementValue, quality);

        if (pruneOnlyZeroImpactNodes) {
          if (!impactValue.IsAlmost(0.0)) continue;
        } else if (nodeImpactThreshold < impactValue) {
          continue;
        }

        var constantNode = (ConstantTreeNode)node.Grammar.GetSymbol("Constant").CreateTreeNode();
        constantNode.Value = replacementValue;

        ReplaceWithConstant(node, constantNode);
        i += node.GetLength() - 1; // skip subtrees under the node that was folded

        quality -= impactValue;
      }
      return model.SymbolicExpressionTree;
    }
  }
}
