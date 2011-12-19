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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Multi PSO Topology Updater", "Splits swarm into NrOfSwarms non-overlapping sub-swarms. Swarms are re-grouped every regroupingPeriod iteration. The operator is implemented as described in Liang, J.J. and Suganthan, P.N 2005. Dynamic multi-swarm particle swarm optimizer. IEEE Swarm Intelligence Symposium, pp. 124-129.")]
  [StorableClass]
  public sealed class MultiPSOTopologyUpdater : SingleSuccessorOperator, ITopologyUpdater {

    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameters
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> NrOfSwarmsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NrOfSwarms"]; }
    }
    public ILookupParameter<IntValue> SwarmSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["SwarmSize"]; }
    }
    public IScopeTreeLookupParameter<IntArray> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntArray>)Parameters["Neighbors"]; }
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
    private int NrOfSwarms {
      get { return NrOfSwarmsParameter.ActualValue.Value; }
    }
    private int SwarmSize {
      get { return SwarmSizeParameter.ActualValue.Value; }
    }
    private ItemArray<IntArray> Neighbors {
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

    #region Construction & Cloning
    [StorableConstructor]
    private MultiPSOTopologyUpdater(bool deserializing) : base(deserializing) { }
    private MultiPSOTopologyUpdater(MultiPSOTopologyUpdater original, Cloner cloner) : base(original, cloner) { }
    public MultiPSOTopologyUpdater()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NrOfSwarms", "Nr of connected sub-swarms.", new IntValue(3)));
      Parameters.Add(new LookupParameter<IntValue>("SwarmSize", "Number of particles in the swarm."));
      Parameters.Add(new ScopeTreeLookupParameter<IntArray>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new LookupParameter<IntValue>("CurrentIteration", "The current iteration of the algorithm."));
      Parameters.Add(new ValueLookupParameter<IntValue>("RegroupingPeriod", "Update interval (=iterations) for regrouping of neighborhoods.", new IntValue(5)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiPSOTopologyUpdater(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      if (CurrentIteration > 0 && CurrentIteration % RegroupingPeriod == 0) {
        ItemArray<IntArray> neighbors = new ItemArray<IntArray>(SwarmSize);

        var particles = Enumerable.Range(0, SwarmSize).ToList();
        for (int i = SwarmSize-1; i>0; i--) {
          int j = Random.Next(i+1);
          int t = particles[j];
          particles[j] = particles[i];
          particles[i] = t;
        }

        for (int partitionNr = 0; partitionNr<NrOfSwarms; partitionNr++) {
          int start = partitionNr*SwarmSize/NrOfSwarms;
          int end = (partitionNr+1)*SwarmSize/NrOfSwarms;
          for (int i = start; i<end; i++)
            neighbors[particles[i]] = GetSegment(particles, start, end, i);
        }

        Neighbors = neighbors;
      }
      return base.Apply();
    }

    public static IntArray GetSegment(IEnumerable<int> list, int start, int end, int excludedIndex) {
      return new IntArray(list
        .Skip(start)
        .Take(end-start)
        .Where((p, j) => start+j != excludedIndex)
        .ToArray());
    }
  }
}
