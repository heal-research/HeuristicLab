#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaIntraRouteInversionMoveMaker", "Peforms the SLS move on a given VRP encoding and updates the quality.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class AlbaIntraRouteInversionMoveMaker : AlbaMoveMaker, IAlbaIntraRouteInversionMoveOperator, IMoveMaker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<AlbaIntraRouteInversionMove> IntraRouteInversionMoveParameter {
      get { return (ILookupParameter<AlbaIntraRouteInversionMove>)Parameters["AlbaIntraRouteInversionMove"]; }
    }

    [StorableConstructor]
    protected AlbaIntraRouteInversionMoveMaker(bool deserializing) : base(deserializing) { }
    protected AlbaIntraRouteInversionMoveMaker(AlbaIntraRouteInversionMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public AlbaIntraRouteInversionMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<AlbaIntraRouteInversionMove>("AlbaIntraRouteInversionMove", "The move to make."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaIntraRouteInversionMoveMaker(this, cloner);
    }

    public static void Apply(AlbaEncoding solution, AlbaIntraRouteInversionMove move) {
      AlbaIntraRouteInversionManipulator.Apply(solution, move.Index1, move.Index2);
    }

    public override IOperation Apply() {
      IOperation next = base.Apply();
      
      AlbaIntraRouteInversionMove move = IntraRouteInversionMoveParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;
     
      //perform move
      VRPToursParameter.ActualValue = move.MakeMove();

      quality.Value = moveQuality.Value;

      return next;
    }
  }
}
