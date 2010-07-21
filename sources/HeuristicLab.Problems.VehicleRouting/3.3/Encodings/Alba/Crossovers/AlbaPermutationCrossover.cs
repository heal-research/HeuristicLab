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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaPermutationCrossover", "An operator which crosses two alba VRP representations.")]
  [StorableClass]
  public sealed class AlbaPermutationCrossover : VRPCrossover {
    public IValueLookupParameter<IPermutationCrossover> PermutationCrossoverParameter {
      get { return (IValueLookupParameter<IPermutationCrossover>)Parameters["PermutationCrossover"]; }
    }

    public AlbaPermutationCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IPermutationCrossover>("PermutationCrossover", "The permutation crossover.", new EdgeRecombinationCrossover()));
    }

    public override IOperation Apply() {
      int cities = 0;

      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];
        cities = solution.Cities;
        if (!(solution is AlbaEncoding)) {
          ParentsParameter.ActualValue[i] = AlbaEncoding.ConvertFrom(solution);
        }
      }

      PermutationCrossoverParameter.ActualValue.ParentsParameter.ActualName = ParentsParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateOperation(PermutationCrossoverParameter.ActualValue);
      op.Operator.Execute((IExecutionContext)op);

      if (ExecutionContext.Scope.Variables.ContainsKey("Permutation")) {
        Permutation permutation = ExecutionContext.Scope.Variables["Permutation"].Value as Permutation;
        ExecutionContext.Scope.Variables.Remove("Permutation");

        ChildParameter.ActualValue = new AlbaEncoding(permutation, cities);
      } else
        ChildParameter.ActualValue = null;

      return base.Apply();
    }
  }
}
