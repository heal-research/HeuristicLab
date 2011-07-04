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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Random Topology Initializer", "Randomly connectes every particle with k other particles.")]
  [StorableClass]
  public sealed class RandomTopologyInitializer : TopologyInitializer {
    #region Parameters
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> NrOfConnectionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NrOfConnections"]; }
    }
    #endregion

    #region Parameter Values
    private IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    private int NrOfConnections {
      get { return NrOfConnectionsParameter.ActualValue.Value; }
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    private RandomTopologyInitializer(bool deserializing) : base(deserializing) { }
    private RandomTopologyInitializer(RandomTopologyInitializer original, Cloner cloner) : base(original, cloner) { }
    public RandomTopologyInitializer() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A random number generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NrOfConnections", "Nr of connected neighbors.", new IntValue(3)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomTopologyInitializer(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      ItemArray<IntArray> neighbors = new ItemArray<IntArray>(SwarmSize);
      for (int i = 0; i < SwarmSize; i++) {
        var numbers = Enumerable.Range(0, SwarmSize).ToList();
        numbers.RemoveAt(i);
        var selectedNumbers = new List<int>(NrOfConnections);
        for (int j = 0; j < NrOfConnections && numbers.Count > 0; j++) {
          int index = Random.Next(numbers.Count);
          selectedNumbers.Add(numbers[index]);
          numbers.RemoveAt(index);
        }
        neighbors[i] = new IntArray(selectedNumbers.ToArray());
      }
      Neighbors = neighbors;
      return base.Apply();
    }
  }
}
