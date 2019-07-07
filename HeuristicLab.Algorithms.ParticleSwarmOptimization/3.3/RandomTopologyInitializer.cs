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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Random Distinct Topology Initializer", "Each particle is informed by exactly k+1 distinct other particles (including itself).")]
  [StorableType("50A4DB55-5C5C-4C0E-A438-6C7DE14EA8A6")]
  public sealed class RandomTopologyInitializer : TopologyInitializer, IStochasticOperator {
    #region Parameters
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> NrOfConnectionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NrOfConnections"]; }
    }
    #endregion
    
    #region Construction & Cloning
    [StorableConstructor]
    private RandomTopologyInitializer(StorableConstructorFlag _) : base(_) { }
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
      var random = RandomParameter.ActualValue;
      var swarmSize = SwarmSizeParameter.ActualValue.Value;
      var nrOfConnections = NrOfConnectionsParameter.ActualValue.Value;

      ItemArray<IntArray> neighbors = new ItemArray<IntArray>(swarmSize);
      for (int i = 0; i < swarmSize; i++) {
        var numbers = Enumerable.Range(0, swarmSize).ToList();
        numbers.RemoveAt(i);
        var selectedNumbers = new List<int>(nrOfConnections + 1);
        selectedNumbers.Add(i);
        for (int j = 0; j < nrOfConnections && numbers.Count > 0; j++) {
          int index = random.Next(numbers.Count);
          selectedNumbers.Add(numbers[index]);
          numbers.RemoveAt(index);
        }
        neighbors[i] = new IntArray(selectedNumbers.ToArray());
      }
      NeighborsParameter.ActualValue = neighbors;
      return base.Apply();
    }
  }
}
