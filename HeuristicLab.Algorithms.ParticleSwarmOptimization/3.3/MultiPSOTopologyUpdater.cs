#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Multi PSO Topology Initializer/Updater", "Splits swarm into swarmsize / (nrOfConnections + 1) non-overlapping sub-swarms. Swarms are re-grouped every regroupingPeriod iteration. The operator is implemented as described in Liang, J.J. and Suganthan, P.N 2005. Dynamic multi-swarm particle swarm optimizer. IEEE Swarm Intelligence Symposium, pp. 124-129.")]
  [StorableClass]
  public sealed class MultiPSOTopologyUpdater : SingleSuccessorOperator, ITopologyUpdater, ITopologyInitializer {
    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameters
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> NrOfConnectionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NrOfConnections"]; }
    }
    public ILookupParameter<IntValue> SwarmSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["SwarmSize"]; }
    }
    public IScopeTreeLookupParameter<IntegerVector> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntegerVector>)Parameters["Neighbors"]; }
    }
    public ILookupParameter<IntValue> CurrentIterationParameter {
      get { return (ILookupParameter<IntValue>)Parameters["CurrentIteration"]; }
    }
    public IValueLookupParameter<IntValue> RegroupingPeriodParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["RegroupingPeriod"]; }
    }
    #endregion

    #region Parameter Values
    private IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    private int NrOfConnections {
      get { return NrOfConnectionsParameter.ActualValue.Value; }
    }
    private int SwarmSize {
      get { return SwarmSizeParameter.ActualValue.Value; }
    }
    private ItemArray<IntegerVector> Neighbors {
      get { return NeighborsParameter.ActualValue; }
      set { NeighborsParameter.ActualValue = value; }
    }
    private int CurrentIteration {
      get { return CurrentIterationParameter.ActualValue.Value; }
    }
    private int RegroupingPeriod {
      get { return RegroupingPeriodParameter.ActualValue.Value; }
    }
    #endregion

    [StorableConstructor]
    private MultiPSOTopologyUpdater(bool deserializing) : base(deserializing) { }
    private MultiPSOTopologyUpdater(MultiPSOTopologyUpdater original, Cloner cloner) : base(original, cloner) { }
    public MultiPSOTopologyUpdater()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NrOfConnections", "Nr of connected neighbors.", new IntValue(3)));
      Parameters.Add(new LookupParameter<IntValue>("SwarmSize", "Number of particles in the swarm."));
      Parameters.Add(new ScopeTreeLookupParameter<IntegerVector>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new LookupParameter<IntValue>("CurrentIteration", "The current iteration of the algorithm."));
      Parameters.Add(new ValueLookupParameter<IntValue>("RegroupingPeriod", "Update interval (=iterations) for regrouping of neighborhoods.", new IntValue(5)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiPSOTopologyUpdater(this, cloner);
    }

    // Splits the swarm into non-overlapping sub swarms
    public override IOperation Apply() {
      if (CurrentIteration % RegroupingPeriod == 0) {
        ItemArray<IntegerVector> neighbors = new ItemArray<IntegerVector>(SwarmSize);
        Dictionary<int, List<int>> neighborsPerParticle = new Dictionary<int, List<int>>();
        for (int i = 0; i < SwarmSize; i++) {
          neighborsPerParticle.Add(i, new List<int>());
        }

        // partition swarm into groups
        Dictionary<int, List<int>> groups = new Dictionary<int, List<int>>();
        int groupId = 0;
        var numbers = Enumerable.Range(0, SwarmSize).ToList();
        for (int i = 0; i < SwarmSize; i++) {
          int nextParticle = numbers[Random.Next(0, numbers.Count)];
          if (!groups.ContainsKey(groupId)) {
            groups.Add(groupId, new List<int>());
          }
          groups[groupId].Add(nextParticle);
          if (groups[groupId].Count - 1 == NrOfConnections) {
            groupId++;
          }
          numbers.Remove(nextParticle);
        }

        // add neighbors to each particle
        foreach (List<int> group in groups.Values) {
          foreach (int sib1 in group) {
            foreach (int sib2 in group) {
              if (sib1 != sib2 && !neighborsPerParticle[sib1].Contains(sib2)) {
                neighborsPerParticle[sib1].Add(sib2);
              }
            }
          }
        }

        for (int particle = 0; particle < neighborsPerParticle.Count; particle++) {
          neighbors[particle] = new IntegerVector(neighborsPerParticle[particle].ToArray());
        }
        Neighbors = neighbors;
      }
      return base.Apply();
    }
  }
}
