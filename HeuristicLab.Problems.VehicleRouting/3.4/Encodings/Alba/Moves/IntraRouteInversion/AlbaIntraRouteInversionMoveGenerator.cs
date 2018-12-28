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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaIntraRouteInversionMoveGenerator", "Generates intra route inversion moves from a given VRP encoding.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableType("F2D66918-2307-4D0F-A7ED-2E429DA1B6F5")]
  public abstract class AlbaIntraRouteInversionMoveGenerator : AlbaMoveGenerator, IExhaustiveMoveGenerator, IAlbaIntraRouteInversionMoveOperator {
    public ILookupParameter<AlbaIntraRouteInversionMove> IntraRouteInversionMoveParameter {
      get { return (ILookupParameter<AlbaIntraRouteInversionMove>)Parameters["AlbaIntraRouteInversionMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return IntraRouteInversionMoveParameter; }
    }

    protected ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    [StorableConstructor]
    protected AlbaIntraRouteInversionMoveGenerator(StorableConstructorFlag _) : base(_) { }

    public AlbaIntraRouteInversionMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<AlbaIntraRouteInversionMove>("AlbaIntraRouteInversionMove", "The moves that should be generated in subscopes."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope where the moves should be added as subscopes."));
    }

    protected AlbaIntraRouteInversionMoveGenerator(AlbaIntraRouteInversionMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract AlbaIntraRouteInversionMove[] GenerateMoves(AlbaEncoding individual, IVRPProblemInstance problemInstance);

    public override IOperation InstrumentedApply() {
      IOperation next = base.InstrumentedApply();

      AlbaEncoding individual = VRPToursParameter.ActualValue as AlbaEncoding;
      AlbaIntraRouteInversionMove[] moves = GenerateMoves(individual, ProblemInstance);
      Scope[] moveScopes = new Scope[moves.Length];
      for (int i = 0; i < moveScopes.Length; i++) {
        moveScopes[i] = new Scope(i.ToString());
        moveScopes[i].Variables.Add(new Variable(IntraRouteInversionMoveParameter.ActualName, moves[i]));
      }
      CurrentScopeParameter.ActualValue.SubScopes.AddRange(moveScopes);

      return next;
    }
  }
}
