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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsPermutationManipulator", "An operator which manipulates a VRP representation by using a standard permutation manipulator. It is implemented as described in Prins, C. (2004). A simple and effective evolutionary algorithm for the vehicle routing problem. Computers & Operations Research, 12:1985-2002.")]
  [StorableType("D5455C8E-B245-4706-AC26-86B4085B6339")]
  public sealed class PrinsPermutationManipulator : PrinsManipulator {
    public IValueLookupParameter<IPermutationManipulator> InnerManipulatorParameter {
      get { return (IValueLookupParameter<IPermutationManipulator>)Parameters["InnerManipulator"]; }
    }

    [StorableConstructor]
    private PrinsPermutationManipulator(StorableConstructorFlag _) : base(_) { }

    public PrinsPermutationManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IPermutationManipulator>("InnerManipulator", "The permutation manipulator.", new TranslocationManipulator()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PrinsPermutationManipulator(this, cloner);
    }

    private PrinsPermutationManipulator(PrinsPermutationManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Manipulate(IRandom random, PrinsEncoding individual) {
      InnerManipulatorParameter.ActualValue.PermutationParameter.ActualName = VRPToursParameter.ActualName;

      IAtomicOperation op = this.ExecutionContext.CreateOperation(
        InnerManipulatorParameter.ActualValue, this.ExecutionContext.Scope);
      op.Operator.Execute((IExecutionContext)op, CancellationToken);
    }
  }
}
