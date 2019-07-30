#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("F3A9CCD4-975F-4F55-BE24-3A3E932591F6")]
  public abstract class LeafBase : ParameterizedNamedItem, ILeafModel {
    public const string LeafBuildingStateVariableName = "LeafBuildingState";
    public const string UseDampeningParameterName = "UseDampening";
    public const string DampeningParameterName = "DampeningStrength";

    public IFixedValueParameter<DoubleValue> DampeningParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[DampeningParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UseDampeningParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UseDampeningParameterName]; }
    }

    public bool UseDampening {
      get { return UseDampeningParameter.Value.Value; }
      set { UseDampeningParameter.Value.Value = value; }
    }
    public double Dampening {
      get { return DampeningParameter.Value.Value; }
      set { DampeningParameter.Value.Value = value; }
    }

    #region Constructors & Cloning
    [StorableConstructor]
    protected LeafBase(StorableConstructorFlag _) : base(_) { }
    protected LeafBase(LeafBase original, Cloner cloner) : base(original, cloner) { }
    protected LeafBase() {
      Parameters.Add(new FixedValueParameter<BoolValue>(UseDampeningParameterName, "Whether logistic dampening should be used to prevent extreme extrapolation (default=false)", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(DampeningParameterName, "Determines the strength of logistic dampening. Must be > 0.0. Larger numbers lead to more conservative predictions. (default=1.5)", new DoubleValue(1.5)));
    }
    #endregion

    #region IModelType
    public abstract bool ProvidesConfidence { get; }
    public abstract int MinLeafSize(IRegressionProblemData pd);

    public void Initialize(IScope states) {
      states.Variables.Add(new Variable(LeafBuildingStateVariableName, new LeafBuildingState()));
    }

    public void Build(RegressionNodeTreeModel tree, IReadOnlyList<int> trainingRows, IScope stateScope, CancellationToken cancellationToken) {
      var parameters = (RegressionTreeParameters)stateScope.Variables[DecisionTreeRegression.RegressionTreeParameterVariableName].Value;
      var state = (LeafBuildingState)stateScope.Variables[LeafBuildingStateVariableName].Value;

      if (state.Code == 0) {
        state.FillLeafs(tree, trainingRows, parameters.Data);
        state.Code = 1;
      }
      while (state.nodeQueue.Count != 0) {
        var n = state.nodeQueue.Peek();
        var t = state.trainingRowsQueue.Peek();
        int numP;
        n.SetLeafModel(BuildModel(t, parameters, cancellationToken, out numP));
        state.nodeQueue.Dequeue();
        state.trainingRowsQueue.Dequeue();
      }
    }

    public IRegressionModel BuildModel(IReadOnlyList<int> rows, RegressionTreeParameters parameters, CancellationToken cancellation, out int numberOfParameters) {
      var reducedData = RegressionTreeUtilities.ReduceDataset(parameters.Data, rows, parameters.AllowedInputVariables.ToArray(), parameters.TargetVariable);
      var pd = new RegressionProblemData(reducedData, parameters.AllowedInputVariables.ToArray(), parameters.TargetVariable);
      pd.TrainingPartition.Start = 0;
      pd.TrainingPartition.End = pd.TestPartition.Start = pd.TestPartition.End = reducedData.Rows;

      int numP;
      var model = Build(pd, parameters.Random, cancellation, out numP);
      if (UseDampening && Dampening > 0.0) {
        model = DampenedModel.DampenModel(model, pd, Dampening);
      }

      numberOfParameters = numP;
      cancellation.ThrowIfCancellationRequested();
      return model;
    }

    public abstract IRegressionModel Build(IRegressionProblemData pd, IRandom random, CancellationToken cancellationToken, out int numberOfParameters);
    #endregion

    [StorableType("495243C0-6C15-4328-B30D-FFBFA0F54DCB")]
    public class LeafBuildingState : Item {
      [Storable]
      private RegressionNodeModel[] storableNodeQueue { get { return nodeQueue.ToArray(); } set { nodeQueue = new Queue<RegressionNodeModel>(value); } }
      public Queue<RegressionNodeModel> nodeQueue;
      [Storable]
      private IReadOnlyList<int>[] storabletrainingRowsQueue { get { return trainingRowsQueue.ToArray(); } set { trainingRowsQueue = new Queue<IReadOnlyList<int>>(value); } }
      public Queue<IReadOnlyList<int>> trainingRowsQueue;

      //State.Code values denote the current action (for pausing)
      //0...nothing has been done;
      //1...building models;
      [Storable]
      public int Code = 0;

      #region HLConstructors & Cloning
      [StorableConstructor]
      protected LeafBuildingState(StorableConstructorFlag _) : base(_) { }
      protected LeafBuildingState(LeafBuildingState original, Cloner cloner) : base(original, cloner) {
        nodeQueue = new Queue<RegressionNodeModel>(original.nodeQueue.Select(cloner.Clone));
        trainingRowsQueue = new Queue<IReadOnlyList<int>>(original.trainingRowsQueue.Select(x => (IReadOnlyList<int>)x.ToArray()));
        Code = original.Code;
      }
      public LeafBuildingState() {
        nodeQueue = new Queue<RegressionNodeModel>();
        trainingRowsQueue = new Queue<IReadOnlyList<int>>();
      }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new LeafBuildingState(this, cloner);
      }
      #endregion

      public void FillLeafs(RegressionNodeTreeModel tree, IReadOnlyList<int> trainingRows, IDataset data) {
        var helperQueue = new Queue<RegressionNodeModel>();
        var trainingHelperQueue = new Queue<IReadOnlyList<int>>();
        nodeQueue.Clear();
        trainingRowsQueue.Clear();

        helperQueue.Enqueue(tree.Root);
        trainingHelperQueue.Enqueue(trainingRows);

        while (helperQueue.Count != 0) {
          var n = helperQueue.Dequeue();
          var t = trainingHelperQueue.Dequeue();
          if (n.IsLeaf) {
            nodeQueue.Enqueue(n);
            trainingRowsQueue.Enqueue(t);
            continue;
          }

          IReadOnlyList<int> leftTraining, rightTraining;
          RegressionTreeUtilities.SplitRows(t, data, n.SplitAttribute, n.SplitValue, out leftTraining, out rightTraining);

          helperQueue.Enqueue(n.Left);
          helperQueue.Enqueue(n.Right);
          trainingHelperQueue.Enqueue(leftTraining);
          trainingHelperQueue.Enqueue(rightTraining);
        }
      }
    }
  }
}