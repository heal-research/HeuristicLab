#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaCrossover", "An operator which crosses two VRP representations.")]
  [StorableClass]
  public abstract class AlbaCrossover : VRPCrossover, IAlbaOperator, IVRPCrossover, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected AlbaCrossover(bool deserializing)
      : base(deserializing) {
    }

    public AlbaCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    protected AlbaCrossover(AlbaCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract AlbaEncoding Crossover(IRandom random, AlbaEncoding parent1, AlbaEncoding parent2);

    public override IOperation InstrumentedApply() {
      ItemArray<IVRPEncoding> parents = new ItemArray<IVRPEncoding>(ParentsParameter.ActualValue.Length);
      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];

        if (!(solution is AlbaEncoding)) {
          parents[i] = AlbaEncoding.ConvertFrom(solution, ProblemInstance);
        } else {
          parents[i] = solution;
        }
      }
      ParentsParameter.ActualValue = parents;

      ChildParameter.ActualValue =
        Crossover(RandomParameter.ActualValue, parents[0] as AlbaEncoding, parents[1] as AlbaEncoding);
      (ChildParameter.ActualValue as AlbaEncoding).Repair();

      return base.InstrumentedApply();
    }
  }
}
