#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("PotvinPDShiftMoveGenerator", "Generates shift moves from a given PDP encoding.")]
  [StorableClass]
  public abstract class PotvinPDShiftMoveGenerator : PotvinMoveGenerator, IPotvinPDShiftMoveOperator {
    public ILookupParameter<PotvinPDShiftMove> PDShiftMoveParameter {
      get { return (ILookupParameter<PotvinPDShiftMove>)Parameters["PotvinPDShiftMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return PDShiftMoveParameter; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected PotvinPDShiftMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinPDShiftMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<PotvinPDShiftMove>("PotvinPDShiftMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    protected PotvinPDShiftMoveGenerator(PotvinPDShiftMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract PotvinPDShiftMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance);

    public override IOperation InstrumentedApply() {
      IOperation next = base.InstrumentedApply();

      PotvinEncoding individual = VRPToursParameter.ActualValue as PotvinEncoding;
      PotvinPDShiftMove[] moves = GenerateMoves(individual, ProblemInstance);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(PDShiftMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);

      return next;
    }
  }
}
