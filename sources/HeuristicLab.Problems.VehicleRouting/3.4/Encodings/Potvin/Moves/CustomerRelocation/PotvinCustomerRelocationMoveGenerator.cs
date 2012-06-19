#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCustomerRelocationMoveGenerator", "Generates customer relocation moves from a given VRP encoding.")]
  [StorableClass]
  public abstract class PotvinCustomerRelocationMoveGenerator : PotvinMoveGenerator, IPotvinCustomerRelocationMoveOperator {
    public ILookupParameter<PotvinCustomerRelocationMove> CustomerRelocationMoveParameter {
      get { return (ILookupParameter<PotvinCustomerRelocationMove>)Parameters["PotvinCustomerRelocationMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return CustomerRelocationMoveParameter; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected PotvinCustomerRelocationMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinCustomerRelocationMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<PotvinCustomerRelocationMove>("PotvinCustomerRelocationMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    protected PotvinCustomerRelocationMoveGenerator(PotvinCustomerRelocationMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract PotvinCustomerRelocationMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance);

    public override IOperation Apply() {
      IOperation next = base.Apply();

      PotvinEncoding individual = VRPToursParameter.ActualValue as PotvinEncoding;
      PotvinCustomerRelocationMove[] moves = GenerateMoves(individual, ProblemInstance);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(CustomerRelocationMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);

      return next;
    }
  }
}
