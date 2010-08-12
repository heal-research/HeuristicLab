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
  [Item("PermutationManipulator", "An operator which manipulates an Alba VRP representation by using a standard permutation manipulator.")]
  [StorableClass]
  public sealed class PermutationManipualtor : AlbaManipulator {
    public IValueLookupParameter<IPermutationManipulator> InnerManipulatorParameter {
      get { return (IValueLookupParameter<IPermutationManipulator>)Parameters["InnerManipulator"]; }
    }

    [StorableConstructor]
    private PermutationManipualtor(bool deserializing) : base(deserializing) { }

    public PermutationManipualtor()
      : base() {
        Parameters.Add(new ValueLookupParameter<IPermutationManipulator>("InnerManipulator", "The permutation manipulator.", new TranslocationManipulator()));
    }

    protected override void Manipulate(IRandom random, AlbaEncoding individual) {
      InnerManipulatorParameter.ActualValue.PermutationParameter.ActualName = VRPToursParameter.ActualName;

      IAtomicOperation op = this.ExecutionContext.CreateOperation(
        InnerManipulatorParameter.ActualValue, this.ExecutionContext.Scope);
      op.Operator.Execute((IExecutionContext)op);
    }
  }
}
