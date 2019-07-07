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
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentMoveGenerator", "Generates vehicle assignment moves from a given VRP encoding.")]
  [StorableType("036C7D2D-50BA-415A-B5C0-8A643746F469")]
  public abstract class PotvinVehicleAssignmentMoveGenerator : PotvinMoveGenerator, IPotvinVehicleAssignmentMoveOperator {
    public ILookupParameter<PotvinVehicleAssignmentMove> VehicleAssignmentMoveParameter {
      get { return (ILookupParameter<PotvinVehicleAssignmentMove>)Parameters["PotvinVehicleAssignmentMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return VehicleAssignmentMoveParameter; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected PotvinVehicleAssignmentMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public PotvinVehicleAssignmentMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<PotvinVehicleAssignmentMove>("PotvinVehicleAssignmentMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    protected PotvinVehicleAssignmentMoveGenerator(PotvinVehicleAssignmentMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract PotvinVehicleAssignmentMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance);

    public override IOperation InstrumentedApply() {
      IOperation next = base.InstrumentedApply();

      PotvinEncoding individual = VRPToursParameter.ActualValue as PotvinEncoding;
      PotvinVehicleAssignmentMove[] moves = GenerateMoves(individual, ProblemInstance);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(VehicleAssignmentMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);

      return next;
    }
  }
}
