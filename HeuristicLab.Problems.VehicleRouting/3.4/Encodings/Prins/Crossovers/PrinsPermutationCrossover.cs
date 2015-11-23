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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsPermutationCrossover", "An operator which crosses two VRP representations using a standard permutation operator.  It is implemented as described in Prins, C. (2004). A simple and effective evolutionary algorithm for the vehicle routing problem. Computers & Operations Research, 12:1985-2002.")]
  [StorableClass]
  public sealed class PrinsPermutationCrossover : PrinsCrossover, IPrinsOperator {
    public IValueLookupParameter<IPermutationCrossover> InnerCrossoverParameter {
      get { return (IValueLookupParameter<IPermutationCrossover>)Parameters["InnerCrossover"]; }
    }

    [StorableConstructor]
    private PrinsPermutationCrossover(bool deserializing) : base(deserializing) { }

    public PrinsPermutationCrossover()
      : base() {
      Parameters.Add(new ValueLookupParameter<IPermutationCrossover>("InnerCrossover", "The permutation crossover.", new OrderCrossover()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PrinsPermutationCrossover(this, cloner);
    }

    private PrinsPermutationCrossover(PrinsPermutationCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PrinsEncoding Crossover(IRandom random, PrinsEncoding parent1, PrinsEncoding parent2) {
      if (parent1.Length != parent2.Length)
        return parent1.Clone() as PrinsEncoding;

      //note - the inner crossover is called here and the result is converted to a prins representation
      //some refactoring should be done here in the future - the crossover operation should be called directly

      InnerCrossoverParameter.ActualValue.ParentsParameter.ActualName = ParentsParameter.ActualName;
      IAtomicOperation op = this.ExecutionContext.CreateOperation(
        InnerCrossoverParameter.ActualValue, this.ExecutionContext.Scope);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);

      string childName = InnerCrossoverParameter.ActualValue.ChildParameter.ActualName;
      if (ExecutionContext.Scope.Variables.ContainsKey(childName)) {
        Permutation permutation = ExecutionContext.Scope.Variables[childName].Value as Permutation;
        ExecutionContext.Scope.Variables.Remove(childName);

        return new PrinsEncoding(permutation, ProblemInstance);
      } else
        return null;
    }
  }
}
