#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {

  [StorableClass]
  public abstract class TopologyInitializer : SingleSuccessorOperator, ITopologyInitializer {

    #region Parameters
    public IScopeTreeLookupParameter<IntegerVector> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntegerVector>)Parameters["Neighbors"]; }
    }
    public ILookupParameter<IntValue> SwarmSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["SwarmSize"]; }
    }

    #endregion

    #region Parameter Values
    public ItemArray<IntegerVector> Neighbors {
      get { return NeighborsParameter.ActualValue; }
      set { NeighborsParameter.ActualValue = value; }
    }
    public int SwarmSize {
      get { return SwarmSizeParameter.ActualValue.Value; }
    }
    #endregion

    #region Construction & Cloning
    protected TopologyInitializer() {
      Parameters.Add(new ScopeTreeLookupParameter<IntegerVector>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new LookupParameter<IntValue>("SwarmSize", "Number of particles in the swarm."));
    }
    [StorableConstructor]
    protected TopologyInitializer(bool deserializing) : base(deserializing) { }
    protected TopologyInitializer(TopologyInitializer original, Cloner cloner)
      : base(original, cloner) {
    }
    #endregion

  }

}
