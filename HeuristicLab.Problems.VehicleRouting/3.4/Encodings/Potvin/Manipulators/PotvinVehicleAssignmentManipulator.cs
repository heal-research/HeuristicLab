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
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentMainpulator", "A manipulator that changes the vehicle assignment")]
  [StorableType("57755DA2-B9CB-4A9F-B32C-988BFC06365A")]
  public sealed class PotvinVehicleAssignmentMainpulator : VRPManipulator, ITimeWindowedOperator,
    IMultiDepotOperator, IHeterogenousCapacitatedOperator {
    public IValueParameter<IPermutationManipulator> VehicleAssignmentManipuator {
      get { return (IValueParameter<IPermutationManipulator>)Parameters["VehicleAssignmentManipuator"]; }
    }

    public ILookupParameter<Permutation> VehicleAssignmentParameter {
      get { return (ILookupParameter<Permutation>)Parameters["VehicleAssignment"]; }
    }

    [StorableConstructor]
    private PotvinVehicleAssignmentMainpulator(StorableConstructorFlag _) : base(_) { }

    public PotvinVehicleAssignmentMainpulator()
      : base() {
      Parameters.Add(new ValueParameter<IPermutationManipulator>("VehicleAssignmentManipuator",
      "The operator used to menipulate the vehicle assignments.", new Swap2Manipulator()));
      Parameters.Add(new LookupParameter<Permutation>("VehicleAssignment"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentMainpulator(this, cloner);
    }

    private PotvinVehicleAssignmentMainpulator(PotvinVehicleAssignmentMainpulator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation InstrumentedApply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is PotvinEncoding)) {
        VRPToursParameter.ActualValue = PotvinEncoding.ConvertFrom(solution, ProblemInstance);
      }

      OperationCollection next = new OperationCollection(base.InstrumentedApply());

      VehicleAssignmentParameter.ActualValue = (VRPToursParameter.ActualValue as PotvinEncoding).VehicleAssignment;
      VehicleAssignmentManipuator.Value.PermutationParameter.ActualName = VehicleAssignmentParameter.ActualName;
      next.Insert(0, ExecutionContext.CreateOperation(VehicleAssignmentManipuator.Value));

      return next;
    }
  }
}
