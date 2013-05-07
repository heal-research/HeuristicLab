#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Zhu {
  [Item("ZhuPermutationManipulator", "An operator which manipulates a VRP representation by using a standard permutation manipulator. It is implemented as described in Zhu, K.Q. (2000). A New Genetic Algorithm For VRPTW. Proceedings of the International Conference on Artificial Intelligence.")]
  [StorableClass]
  public sealed class ZhuPermutationManipulator : ZhuManipulator {
    public IValueLookupParameter<IPermutationManipulator> InnerManipulatorParameter {
      get { return (IValueLookupParameter<IPermutationManipulator>)Parameters["InnerManipulator"]; }
    }

    [StorableConstructor]
    private ZhuPermutationManipulator(bool deserializing) : base(deserializing) { }
    private ZhuPermutationManipulator(ZhuPermutationManipulator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZhuPermutationManipulator(this, cloner);
    }
    public ZhuPermutationManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IPermutationManipulator>("InnerManipulator", "The permutation manipulator.", new TranslocationManipulator()));
    }

    protected override void Manipulate(IRandom random, ZhuEncoding individual) {
      InnerManipulatorParameter.ActualValue.PermutationParameter.ActualName = VRPToursParameter.ActualName;

      IAtomicOperation op = this.ExecutionContext.CreateOperation(
        InnerManipulatorParameter.ActualValue, this.ExecutionContext.Scope);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);
    }
  }
}
