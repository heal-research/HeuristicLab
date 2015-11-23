#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaIntraRouteInversionMoveMaker", "Peforms the SLS move on a given VRP encoding and updates the quality.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class AlbaIntraRouteInversionMoveMaker : AlbaMoveMaker, IAlbaIntraRouteInversionMoveOperator, IMoveMaker {
    public ILookupParameter<AlbaIntraRouteInversionMove> IntraRouteInversionMoveParameter {
      get { return (ILookupParameter<AlbaIntraRouteInversionMove>)Parameters["AlbaIntraRouteInversionMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return IntraRouteInversionMoveParameter; }
    }

    [StorableConstructor]
    protected AlbaIntraRouteInversionMoveMaker(bool deserializing) : base(deserializing) { }

    public AlbaIntraRouteInversionMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<AlbaIntraRouteInversionMove>("AlbaIntraRouteInversionMove", "The move to make."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaIntraRouteInversionMoveMaker(this, cloner);
    }

    protected AlbaIntraRouteInversionMoveMaker(AlbaIntraRouteInversionMoveMaker original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(AlbaEncoding solution, AlbaIntraRouteInversionMove move) {
      AlbaIntraRouteInversionManipulator.Apply(solution, move.Index1, move.Index2);
    }

    protected override void PerformMove() {
      AlbaIntraRouteInversionMove move = IntraRouteInversionMoveParameter.ActualValue;

      Apply(move.Individual as AlbaEncoding, move);
      VRPToursParameter.ActualValue = move.Individual as AlbaEncoding;
    }
  }
}
