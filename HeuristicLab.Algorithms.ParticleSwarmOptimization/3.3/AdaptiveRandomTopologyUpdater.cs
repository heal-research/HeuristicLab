#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("SPSO Adaptive Random Topology Updater", "Each unsuccessful iteration the topology initializer is applied again.")]
  [StorableType("F0FC17DF-44B3-4005-9CC3-EDBA7945544A")]
  public sealed class SPSOAdaptiveRandomTopologyUpdater : SingleSuccessorOperator, ITopologyUpdater, ISingleObjectiveOperator {

    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameters
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> SwarmBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["SwarmBestQuality"]; }
    }
    public ILookupParameter<DoubleValue> PreviousBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["PreviousBestQuality"]; }
    }
    public ILookupParameter<ITopologyInitializer> TopologyInitializerParameter {
      get { return (ILookupParameter<ITopologyInitializer>)Parameters["TopologyInitializer"]; }
    }
    public IScopeTreeLookupParameter<IntArray> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntArray>)Parameters["Neighbors"]; }
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    private SPSOAdaptiveRandomTopologyUpdater(StorableConstructorFlag _) : base(_) { }
    private SPSOAdaptiveRandomTopologyUpdater(SPSOAdaptiveRandomTopologyUpdater original, Cloner cloner) : base(original, cloner) { }
    public SPSOAdaptiveRandomTopologyUpdater()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "Whether the problem is to be maximized or not."));
      Parameters.Add(new LookupParameter<DoubleValue>("SwarmBestQuality", "The swarm's best quality."));
      Parameters.Add(new LookupParameter<DoubleValue>("PreviousBestQuality", "The best quality of the previous iteration."));
      Parameters.Add(new LookupParameter<ITopologyInitializer>("TopologyInitializer", "The topology initializer is called again in case no improvement is made."));
      Parameters.Add(new ScopeTreeLookupParameter<IntArray>("Neighbors", "The list of neighbors for each particle."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SPSOAdaptiveRandomTopologyUpdater(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      var swarmBest = SwarmBestQualityParameter.ActualValue;
      if (swarmBest == null) return base.Apply();

      var previousBest = PreviousBestQualityParameter.ActualValue;
      if (previousBest == null) {
        PreviousBestQualityParameter.ActualValue = new DoubleValue(swarmBest.Value);
        return base.Apply();
      };

      var successor = new OperationCollection(new[] { base.Apply() });
      var max = MaximizationParameter.ActualValue.Value;
      if (max && swarmBest.Value <= previousBest.Value
        || !max && swarmBest.Value >= previousBest.Value)
        successor.Insert(0, ExecutionContext.CreateOperation(TopologyInitializerParameter.ActualValue));

      previousBest.Value = swarmBest.Value;
      return successor;
    }
  }
}
