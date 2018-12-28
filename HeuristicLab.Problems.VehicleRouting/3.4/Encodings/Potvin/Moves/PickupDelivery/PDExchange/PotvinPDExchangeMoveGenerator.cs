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
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDExchangeMoveGenerator", "Generates exchange moves from a given PDP encoding.")]
  [StorableType("1E0B89BB-5290-4D7B-8D4E-27667D29B2CE")]
  public abstract class PotvinPDExchangeMoveGenerator : PotvinMoveGenerator, IPotvinPDExchangeMoveOperator {
    public ILookupParameter<PotvinPDExchangeMove> PDExchangeMoveParameter {
      get { return (ILookupParameter<PotvinPDExchangeMove>)Parameters["PotvinPDExchangeMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return PDExchangeMoveParameter; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected PotvinPDExchangeMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public PotvinPDExchangeMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<PotvinPDExchangeMove>("PotvinPDExchangeMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    protected PotvinPDExchangeMoveGenerator(PotvinPDExchangeMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract PotvinPDExchangeMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance);

    public override IOperation InstrumentedApply() {
      IOperation next = base.InstrumentedApply();

      PotvinEncoding individual = VRPToursParameter.ActualValue as PotvinEncoding;
      PotvinPDExchangeMove[] moves = GenerateMoves(individual, ProblemInstance);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(PDExchangeMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);

      return next;
    }
  }
}
