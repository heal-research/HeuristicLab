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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("SPSO Random Topology Initializer", "Each particle informs k+1 other particles (including itself). The same particle (including itself) can be informed multiple times.")]
  [StorableType("3A589247-0629-44E4-8A49-610DE0FEC642")]
  public sealed class SPSORandomTopologyInitializer : TopologyInitializer, IStochasticOperator {
    #region Parameters
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> KParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["K"]; }
    }
    #endregion
    
    #region Construction & Cloning
    [StorableConstructor]
    private SPSORandomTopologyInitializer(StorableConstructorFlag _) : base(_) { }
    private SPSORandomTopologyInitializer(SPSORandomTopologyInitializer original, Cloner cloner) : base(original, cloner) { }
    public SPSORandomTopologyInitializer() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A random number generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("K", "The number of informed particles (excluding itself).", new IntValue(3)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SPSORandomTopologyInitializer(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var swarmSize = SwarmSizeParameter.ActualValue.Value;
      var k = KParameter.ActualValue.Value;

      // SPSO: Each particle informs at most K+1 particles (at least itself and K others)
      //       it is by design that we draw from the particles with repetition
      var particlesInform = new List<HashSet<int>>(swarmSize);
      for (var i = 0; i < swarmSize; i++) {
        var informs = new HashSet<int>() { i };
        for (var j = 0; j < k; j++) {
          informs.Add(random.Next(swarmSize));
        }
        particlesInform.Add(informs);
      }

      var neighbors = new ItemArray<IntArray>(swarmSize);
      for (int i = 0; i < swarmSize; i++) {
        // calculate the informants for each particle
        var informants = particlesInform.Select((val, idx) => val.Contains(i) ? idx : -1).Where(x => x >= 0).ToArray();
        neighbors[i] = new IntArray(informants);
      }
      NeighborsParameter.ActualValue = neighbors;
      return base.Apply();
    }
  }
}
