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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("B643D965-D13F-415D-8589-3F3527460347")]
  public class ComplexityPruning : ParameterizedNamedItem, IPruning {
    public const string PruningStateVariableName = "PruningState";

    public const string PruningStrengthParameterName = "PruningStrength";
    public const string PruningDecayParameterName = "PruningDecay";
    public const string FastPruningParameterName = "FastPruning";

    public IFixedValueParameter<DoubleValue> PruningStrengthParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[PruningStrengthParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> PruningDecayParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[PruningDecayParameterName]; }
    }
    public IFixedValueParameter<BoolValue> FastPruningParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[FastPruningParameterName]; }
    }

    public double PruningStrength {
      get { return PruningStrengthParameter.Value.Value; }
      set { PruningStrengthParameter.Value.Value = value; }
    }
    public double PruningDecay {
      get { return PruningDecayParameter.Value.Value; }
      set { PruningDecayParameter.Value.Value = value; }
    }
    public bool FastPruning {
      get { return FastPruningParameter.Value.Value; }
      set { FastPruningParameter.Value.Value = value; }
    }

    #region Constructors & Cloning
    [StorableConstructor]
    protected ComplexityPruning(StorableConstructorFlag _) : base(_) { }
    protected ComplexityPruning(ComplexityPruning original, Cloner cloner) : base(original, cloner) { }
    public ComplexityPruning() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(PruningStrengthParameterName, "The strength of the pruning. Higher values force the algorithm to create simpler models (default=2.0).", new DoubleValue(2.0)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(PruningDecayParameterName, "Pruning decay allows nodes higher up in the tree to be more stable (default=1.0).", new DoubleValue(1.0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(FastPruningParameterName, "Accelerate pruning by using linear models instead of leaf models (default=true).", new BoolValue(true)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ComplexityPruning(this, cloner);
    }
    #endregion

    #region IPruning
    public int MinLeafSize(IRegressionProblemData pd, ILeafModel leafModel) {
      return (FastPruning ? new LinearLeaf() : leafModel).MinLeafSize(pd);
    }
    public void Initialize(IScope states) {
      states.Variables.Add(new Variable(PruningStateVariableName, new PruningState()));
    }

    public void Prune(RegressionNodeTreeModel treeModel, IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, IScope statescope, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)statescope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      var state = (PruningState)statescope.Variables[PruningStateVariableName].Value;

      var leaf = FastPruning ? new LinearLeaf() : regressionTreeParams.LeafModel;
      if (state.Code <= 1) {
        InstallModels(treeModel, state, trainingRows, pruningRows, leaf, regressionTreeParams, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
      }
      if (state.Code <= 2) {
        AssignPruningThresholds(treeModel, state, PruningDecay);
        cancellationToken.ThrowIfCancellationRequested();
      }
      if (state.Code <= 3) {
        UpdateThreshold(treeModel, state);
        cancellationToken.ThrowIfCancellationRequested();
      }
      if (state.Code <= 4) {
        Prune(treeModel, state, PruningStrength);
        cancellationToken.ThrowIfCancellationRequested();
      }

      state.Code = 5;
    }
    #endregion

    private static void InstallModels(RegressionNodeTreeModel tree, PruningState state, IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, ILeafModel leaf, RegressionTreeParameters regressionTreeParams, CancellationToken cancellationToken) {
      if (state.Code == 0) {
        state.FillBottomUp(tree, trainingRows, pruningRows, regressionTreeParams.Data);
        state.Code = 1;
      }
      while (state.nodeQueue.Count != 0) {
        cancellationToken.ThrowIfCancellationRequested();
        var n = state.nodeQueue.Peek();
        var training = state.trainingRowsQueue.Peek();
        var pruning = state.pruningRowsQueue.Peek();
        BuildPruningModel(n, leaf, training, pruning, state, regressionTreeParams, cancellationToken);
        state.nodeQueue.Dequeue();
        state.trainingRowsQueue.Dequeue();
        state.pruningRowsQueue.Dequeue();
      }
    }

    private static void AssignPruningThresholds(RegressionNodeTreeModel tree, PruningState state, double pruningDecay) {
      if (state.Code == 1) {
        state.FillBottomUp(tree);
        state.Code = 2;
      }
      while (state.nodeQueue.Count != 0) {
        var n = state.nodeQueue.Dequeue();
        if (n.IsLeaf) continue;
        n.PruningStrength = PruningThreshold(state.pruningSizes[n], state.modelComplexities[n], state.nodeComplexities[n], state.modelErrors[n], SubtreeError(n, state.pruningSizes, state.modelErrors), pruningDecay);
      }
    }

    private static void UpdateThreshold(RegressionNodeTreeModel tree, PruningState state) {
      if (state.Code == 2) {
        state.FillTopDown(tree);
        state.Code = 3;
      }
      while (state.nodeQueue.Count != 0) {
        var n = state.nodeQueue.Dequeue();
        if (n.IsLeaf || n.Parent == null || double.IsNaN(n.Parent.PruningStrength)) continue;
        n.PruningStrength = Math.Min(n.PruningStrength, n.Parent.PruningStrength);
      }
    }

    private static void Prune(RegressionNodeTreeModel tree, PruningState state, double pruningStrength) {
      if (state.Code == 3) {
        state.FillTopDown(tree);
        state.Code = 4;
      }
      while (state.nodeQueue.Count != 0) {
        var n = state.nodeQueue.Dequeue();
        if (n.IsLeaf || pruningStrength <= n.PruningStrength) continue;
        n.ToLeaf();
      }
    }

    private static void BuildPruningModel(RegressionNodeModel regressionNode, ILeafModel leaf, IReadOnlyList<int> trainingRows, IReadOnlyList<int> pruningRows, PruningState state, RegressionTreeParameters regressionTreeParams, CancellationToken cancellationToken) {
      //create regressionProblemdata from pruning data 
      var vars = regressionTreeParams.AllowedInputVariables.Concat(new[] { regressionTreeParams.TargetVariable }).ToArray();
      var reducedData = new Dataset(vars, vars.Select(x => regressionTreeParams.Data.GetDoubleValues(x, pruningRows).ToList()));
      var pd = new RegressionProblemData(reducedData, regressionTreeParams.AllowedInputVariables, regressionTreeParams.TargetVariable);
      pd.TrainingPartition.Start = pd.TrainingPartition.End = pd.TestPartition.Start = 0;
      pd.TestPartition.End = reducedData.Rows;

      //build pruning model
      int numModelParams;
      var model = leaf.BuildModel(trainingRows, regressionTreeParams, cancellationToken, out numModelParams);

      //record error and complexities
      var rmsModel = model.CreateRegressionSolution(pd).TestRootMeanSquaredError;
      state.pruningSizes.Add(regressionNode, pruningRows.Count);
      state.modelErrors.Add(regressionNode, rmsModel);
      state.modelComplexities.Add(regressionNode, numModelParams);
      if (regressionNode.IsLeaf) { state.nodeComplexities[regressionNode] = state.modelComplexities[regressionNode]; } else { state.nodeComplexities.Add(regressionNode, state.nodeComplexities[regressionNode.Left] + state.nodeComplexities[regressionNode.Right] + 1); }
    }

    private static double PruningThreshold(double noIntances, double modelParams, double nodeParams, double modelError, double nodeError, double w) {
      var res = modelError / nodeError;
      if (modelError.IsAlmost(nodeError)) res = 1.0;
      res /= Math.Pow((nodeParams + noIntances) / (2 * (modelParams + noIntances)), w);
      return res;
    }

    private static double SubtreeError(RegressionNodeModel regressionNode, IDictionary<RegressionNodeModel, int> pruningSizes,
      IDictionary<RegressionNodeModel, double> modelErrors) {
      if (regressionNode.IsLeaf) return modelErrors[regressionNode];
      var errorL = SubtreeError(regressionNode.Left, pruningSizes, modelErrors);
      var errorR = SubtreeError(regressionNode.Right, pruningSizes, modelErrors);
      errorL = errorL * errorL * pruningSizes[regressionNode.Left];
      errorR = errorR * errorR * pruningSizes[regressionNode.Right];
      return Math.Sqrt((errorR + errorL) / pruningSizes[regressionNode]);
    }

    [StorableType("EAD60C7E-2C58-45C4-9697-6F735F518CFD")]
    public class PruningState : Item {
      [Storable]
      public IDictionary<RegressionNodeModel, int> modelComplexities;
      [Storable]
      public IDictionary<RegressionNodeModel, int> nodeComplexities;
      [Storable]
      public IDictionary<RegressionNodeModel, int> pruningSizes;
      [Storable]
      public IDictionary<RegressionNodeModel, double> modelErrors;

      [Storable]
      private RegressionNodeModel[] storableNodeQueue { get { return nodeQueue.ToArray(); } set { nodeQueue = new Queue<RegressionNodeModel>(value); } }
      public Queue<RegressionNodeModel> nodeQueue;
      [Storable]
      private IReadOnlyList<int>[] storabletrainingRowsQueue { get { return trainingRowsQueue.ToArray(); } set { trainingRowsQueue = new Queue<IReadOnlyList<int>>(value); } }
      public Queue<IReadOnlyList<int>> trainingRowsQueue;
      [Storable]
      private IReadOnlyList<int>[] storablepruningRowsQueue { get { return pruningRowsQueue.ToArray(); } set { pruningRowsQueue = new Queue<IReadOnlyList<int>>(value); } }
      public Queue<IReadOnlyList<int>> pruningRowsQueue;

      //State.Code values denote the current action (for pausing)
      //0...nothing has been done;
      //1...building Models;
      //2...assigning threshold
      //3...adjusting threshold
      //4...pruning
      //5...finished
      [Storable]
      public int Code = 0;

      #region HLConstructors & Cloning
      [StorableConstructor]
      protected PruningState(StorableConstructorFlag _) : base(_) { }
      protected PruningState(PruningState original, Cloner cloner) : base(original, cloner) {
        modelComplexities = original.modelComplexities.ToDictionary(x => cloner.Clone(x.Key), x => x.Value);
        nodeComplexities = original.nodeComplexities.ToDictionary(x => cloner.Clone(x.Key), x => x.Value);
        pruningSizes = original.pruningSizes.ToDictionary(x => cloner.Clone(x.Key), x => x.Value);
        modelErrors = original.modelErrors.ToDictionary(x => cloner.Clone(x.Key), x => x.Value);
        nodeQueue = new Queue<RegressionNodeModel>(original.nodeQueue.Select(cloner.Clone));
        trainingRowsQueue = new Queue<IReadOnlyList<int>>(original.trainingRowsQueue.Select(x => (IReadOnlyList<int>)x.ToArray()));
        pruningRowsQueue = new Queue<IReadOnlyList<int>>(original.pruningRowsQueue.Select(x => (IReadOnlyList<int>)x.ToArray()));
        Code = original.Code;
      }
      public PruningState() {
        modelComplexities = new Dictionary<RegressionNodeModel, int>();
        nodeComplexities = new Dictionary<RegressionNodeModel, int>();
        pruningSizes = new Dictionary<RegressionNodeModel, int>();
        modelErrors = new Dictionary<RegressionNodeModel, double>();
        nodeQueue = new Queue<RegressionNodeModel>();
        trainingRowsQueue = new Queue<IReadOnlyList<int>>();
        pruningRowsQueue = new Queue<IReadOnlyList<int>>();
      }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new PruningState(this, cloner);
      }
      #endregion

      public void FillTopDown(RegressionNodeTreeModel tree) {
        var helperQueue = new Queue<RegressionNodeModel>();
        nodeQueue.Clear();

        helperQueue.Enqueue(tree.Root);
        nodeQueue.Enqueue(tree.Root);

        while (helperQueue.Count != 0) {
          var n = helperQueue.Dequeue();
          if (n.IsLeaf) continue;
          helperQueue.Enqueue(n.Left);
          helperQueue.Enqueue(n.Right);
          nodeQueue.Enqueue(n.Left);
          nodeQueue.Enqueue(n.Right);
        }
      }

      public void FillTopDown(RegressionNodeTreeModel tree, IReadOnlyList<int> pruningRows, IReadOnlyList<int> trainingRows, IDataset data) {
        var helperQueue = new Queue<RegressionNodeModel>();
        var trainingHelperQueue = new Queue<IReadOnlyList<int>>();
        var pruningHelperQueue = new Queue<IReadOnlyList<int>>();
        nodeQueue.Clear();
        trainingRowsQueue.Clear();
        pruningRowsQueue.Clear();

        helperQueue.Enqueue(tree.Root);

        trainingHelperQueue.Enqueue(trainingRows);
        pruningHelperQueue.Enqueue(pruningRows);

        nodeQueue.Enqueue(tree.Root);
        trainingRowsQueue.Enqueue(trainingRows);
        pruningRowsQueue.Enqueue(pruningRows);


        while (helperQueue.Count != 0) {
          var n = helperQueue.Dequeue();
          var p = pruningHelperQueue.Dequeue();
          var t = trainingHelperQueue.Dequeue();
          if (n.IsLeaf) continue;

          IReadOnlyList<int> leftPruning, rightPruning;
          RegressionTreeUtilities.SplitRows(p, data, n.SplitAttribute, n.SplitValue, out leftPruning, out rightPruning);
          IReadOnlyList<int> leftTraining, rightTraining;
          RegressionTreeUtilities.SplitRows(t, data, n.SplitAttribute, n.SplitValue, out leftTraining, out rightTraining);

          helperQueue.Enqueue(n.Left);
          helperQueue.Enqueue(n.Right);
          trainingHelperQueue.Enqueue(leftTraining);
          trainingHelperQueue.Enqueue(rightTraining);
          pruningHelperQueue.Enqueue(leftPruning);
          pruningHelperQueue.Enqueue(rightPruning);

          nodeQueue.Enqueue(n.Left);
          nodeQueue.Enqueue(n.Right);
          trainingRowsQueue.Enqueue(leftTraining);
          trainingRowsQueue.Enqueue(rightTraining);
          pruningRowsQueue.Enqueue(leftPruning);
          pruningRowsQueue.Enqueue(rightPruning);
        }
      }

      public void FillBottomUp(RegressionNodeTreeModel tree) {
        FillTopDown(tree);
        nodeQueue = new Queue<RegressionNodeModel>(nodeQueue.Reverse());
      }

      public void FillBottomUp(RegressionNodeTreeModel tree, IReadOnlyList<int> pruningRows, IReadOnlyList<int> trainingRows, IDataset data) {
        FillTopDown(tree, pruningRows, trainingRows, data);
        nodeQueue = new Queue<RegressionNodeModel>(nodeQueue.Reverse());
        pruningRowsQueue = new Queue<IReadOnlyList<int>>(pruningRowsQueue.Reverse());
        trainingRowsQueue = new Queue<IReadOnlyList<int>>(trainingRowsQueue.Reverse());
      }
    }
  }
}