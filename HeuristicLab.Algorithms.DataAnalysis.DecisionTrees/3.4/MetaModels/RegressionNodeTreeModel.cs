#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("FAF1F955-82F3-4824-9759-9D2846E831AE")]
  public class RegressionNodeTreeModel : RegressionModel, IDecisionTreeModel {
    public const string NumCurrentLeafsResultName = "Number of current leafs";
    public const string RootVariableName = "Root";
    #region Properties
    [Storable]
    internal RegressionNodeModel Root { get; private set; }
    #endregion

    #region HLConstructors & Cloning
    [StorableConstructor]
    protected RegressionNodeTreeModel(StorableConstructorFlag _) : base(_) { }
    protected RegressionNodeTreeModel(RegressionNodeTreeModel original, Cloner cloner) : base(original, cloner) {
      Root = cloner.Clone(original.Root);
    }
    protected RegressionNodeTreeModel(string targetVariable) : base(targetVariable) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionNodeTreeModel(this, cloner);
    }
    #endregion

    internal static RegressionNodeTreeModel CreateTreeModel(string targetAttr, RegressionTreeParameters regressionTreeParams) {
      return regressionTreeParams.LeafModel.ProvidesConfidence ? new ConfidenceRegressionNodeTreeModel(targetAttr) : new RegressionNodeTreeModel(targetAttr);
    }

    #region RegressionModel
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return Root.VariablesUsedForPrediction ?? new List<string>(); }
    }
    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      if (Root == null) throw new NotSupportedException("The model has not been built yet");
      return Root.GetEstimatedValues(dataset, rows);
    }
    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, problemData);
    }
    #endregion

    #region IDecisionTreeModel
    public void Build(IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, IScope statescope, ResultCollection results, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)statescope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      //start with one node
      if (Root == null)
        Root = RegressionNodeModel.CreateNode(regressionTreeParams.TargetVariable, regressionTreeParams);

      //split into (overfitted tree)
      regressionTreeParams.Splitter.Split(this, trainingRows, statescope, cancellationToken);

      //prune
      regressionTreeParams.Pruning.Prune(this, trainingRows, pruningRows, statescope, cancellationToken);

      //build final leaf models
      regressionTreeParams.LeafModel.Build(this, trainingRows.Union(pruningRows).ToArray(), statescope, cancellationToken);
    }

    public void Update(IReadOnlyList<int> rows, IScope statescope, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)statescope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      regressionTreeParams.LeafModel.Build(this, rows, statescope, cancellationToken);
    }

    public static void Initialize(IScope stateScope) {
      var param = (RegressionTreeParameters)stateScope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      stateScope.Variables.Add(new Variable(RootVariableName, RegressionNodeModel.CreateNode(param.TargetVariable, param)));
    }
    #endregion

    public void BuildModel(IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, IScope statescope, ResultCollection results, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)statescope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      //start with one node
      Root = RegressionNodeModel.CreateNode(regressionTreeParams.TargetVariable, regressionTreeParams);

      //split into (overfitted tree)
      regressionTreeParams.Splitter.Split(this, trainingRows, statescope, cancellationToken);

      //prune
      regressionTreeParams.Pruning.Prune(this, trainingRows, pruningRows, statescope, cancellationToken);
    }

    [StorableType("E84ACC40-5694-4E40-A947-190673643206")]
    private sealed class ConfidenceRegressionNodeTreeModel : RegressionNodeTreeModel, IConfidenceRegressionModel {
      #region HLConstructors & Cloning
      [StorableConstructor]
      private ConfidenceRegressionNodeTreeModel(StorableConstructorFlag _) : base(_) { }
      private ConfidenceRegressionNodeTreeModel(ConfidenceRegressionNodeTreeModel original, Cloner cloner) : base(original, cloner) { }
      public ConfidenceRegressionNodeTreeModel(string targetVariable) : base(targetVariable) { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new ConfidenceRegressionNodeTreeModel(this, cloner);
      }
      #endregion

      public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
        if (Root == null) throw new NotSupportedException("The model has not been built yet");
        return ((IConfidenceRegressionModel)Root).GetEstimatedVariances(dataset, rows);
      }
      public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
        return new ConfidenceRegressionSolution(this, problemData);
      }
    }
  }
}