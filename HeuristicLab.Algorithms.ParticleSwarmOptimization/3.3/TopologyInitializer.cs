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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization; 
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("TopologyInitializer", "Groups the particles into neighborhoods according to a certain strategy.")]
  [StorableClass]
  public abstract class TopologyInitializer : SingleSuccessorOperator, ITopologyInitializer {
    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameters
    public IScopeTreeLookupParameter<IntArray> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntArray>)Parameters["Neighbors"]; }
    }

    public ILookupParameter<IntValue> SwarmSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["SwarmSize"]; }
    }

    #endregion

    #region Parameter Values
    protected ItemArray<IntArray> Neighbors {
      get { return NeighborsParameter.ActualValue; }
      set { NeighborsParameter.ActualValue = value; }
    }
    protected int SwarmSize {
      get { return SwarmSizeParameter.ActualValue.Value; }
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected TopologyInitializer(bool deserializing) : base(deserializing) { }
    protected TopologyInitializer(TopologyInitializer original, Cloner cloner) : base(original, cloner) { }
    
    public TopologyInitializer() {
      Parameters.Add(new ScopeTreeLookupParameter<IntArray>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new LookupParameter<IntValue>("SwarmSize", "Number of particles in the swarm."));
    }
    #endregion
  }

}
