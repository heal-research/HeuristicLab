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
  [Item("AlbaManipulator", "An operator which manipulates an alba VRP representation.")]
  [StorableClass]
  public sealed class AlbaManipulator : VRPManipulator {
    public IValueLookupParameter<IPermutationManipulator> InnerManipulatorParameter {
      get { return (IValueLookupParameter<IPermutationManipulator>)Parameters["InnerManipulator"]; }
    }

    [StorableConstructor]
    private AlbaManipulator(bool deserializing) : base(deserializing) { }

    public AlbaManipulator()
      : base() {
        Parameters.Add(new ValueLookupParameter<IPermutationManipulator>("InnerManipulator", "The permutation manipulator.", new TranslocationManipulator()));

        AlbaEncoding.RemoveUnusedParameters(Parameters);
    }

    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is AlbaEncoding)) {
        VRPToursParameter.ActualValue = AlbaEncoding.ConvertFrom(solution, VehiclesParameter.ActualValue.Value);
      }
      
      OperationCollection next = new OperationCollection(base.Apply());

      IPermutationManipulator op = InnerManipulatorParameter.ActualValue;
      if (op != null) {
        op.PermutationParameter.ActualName = VRPToursParameter.ActualName;
        next.Insert(0, ExecutionContext.CreateOperation(op));
      }

      return next;
    }
  }
}
