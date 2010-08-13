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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("PermutationCrossover", "An operator which crosses two VRP representations using a standard permutation operator.")]
  [StorableClass]
  public sealed class PermutationCrossover : AlbaCrossover {    
    public IValueLookupParameter<IPermutationCrossover> InnerCrossoverParameter {
      get { return (IValueLookupParameter<IPermutationCrossover>)Parameters["InnerCrossover"]; }
    }

    [StorableConstructor]
    private PermutationCrossover(bool deserializing) : base(deserializing) { }

    public PermutationCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IPermutationCrossover>("InnerCrossover", "The permutation crossover.", new EdgeRecombinationCrossover()));
    }

    protected override AlbaEncoding Crossover(IRandom random, AlbaEncoding parent1, AlbaEncoding parent2) {
      //note - the inner crossover is called here and the result is converted to an alba representation
      //some refactoring should be done here in the future - the crossover operation should be called directly

      InnerCrossoverParameter.ActualValue.ParentsParameter.ActualName = ParentsParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateOperation(
        InnerCrossoverParameter.ActualValue, this.ExecutionContext.Scope);
      op.Operator.Execute((IExecutionContext)op);

      string childName = InnerCrossoverParameter.ActualValue.ChildParameter.ActualName;
      if (ExecutionContext.Scope.Variables.ContainsKey(childName)) {
        Permutation permutation = ExecutionContext.Scope.Variables[childName].Value as Permutation;
        ExecutionContext.Scope.Variables.Remove(childName);

        return new AlbaEncoding(permutation, Cities);
      } else
        return null;
    }
  }
}
