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
  public sealed class AlbaPermutationCrossover : AlbaCrossover {
    public IValueLookupParameter<IPermutationCrossover> PermutationCrossoverParameter {
      get { return (IValueLookupParameter<IPermutationCrossover>)Parameters["PermutationCrossover"]; }
    }

    public AlbaPermutationCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IPermutationCrossover>("PermutationCrossover", "The permutation crossover.", new EdgeRecombinationCrossover()));
    }

    protected override void Crossover() {
      int cities = ParentsParameter.ActualValue[0].Cities;

      PermutationCrossoverParameter.ActualValue.ParentsParameter.ActualName = ParentsParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateOperation(
        PermutationCrossoverParameter.ActualValue, this.ExecutionContext.Scope);
      op.Operator.Execute((IExecutionContext)op);

      string childName = PermutationCrossoverParameter.ActualValue.ChildParameter.ActualName;
      if (ExecutionContext.Scope.Variables.ContainsKey(childName)) {
        Permutation permutation = ExecutionContext.Scope.Variables[childName].Value as Permutation;
        ExecutionContext.Scope.Variables.Remove(childName);

        ChildParameter.ActualValue = new AlbaEncoding(permutation, cities);
      } else
        ChildParameter.ActualValue = null;
    }
  }
}
