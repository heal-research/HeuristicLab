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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaManipulator", "An operator which manipulates an Alba VRP representation.")]
  [StorableClass]
  public abstract class AlbaManipulator : VRPManipulator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected AlbaManipulator(bool deserializing) : base(deserializing) { }

    public AlbaManipulator()
      : base() {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));

        AlbaEncoding.RemoveUnusedParameters(Parameters);
    }

    protected abstract void Manipulate(IRandom random, AlbaEncoding individual);

    protected int FindCustomerLocation(int customer, AlbaEncoding individual) {
      int pos = -1;
      for (int i = 0; i < individual.Length; i++) {
        if (individual[i] == customer) {
          pos = i;
          break;
        }
      }

      return pos;
    }

    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is AlbaEncoding)) {
        VRPToursParameter.ActualValue = AlbaEncoding.ConvertFrom(solution, VehiclesParameter.ActualValue.Value);
      }

      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as AlbaEncoding);

      return base.Apply();
    }
  }
}
