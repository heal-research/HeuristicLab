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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("IntraRouteInversionMoveMaker", "Peforms the SLS move on a given VRP encoding and updates the quality.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public class IntraRouteInversionMoveMaker : AlbaMoveMaker, IAlbaIntraRouteInversionMoveOperator, IMoveMaker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<IntraRouteInversionMove> IntraRouteInversionMoveParameter {
      get { return (ILookupParameter<IntraRouteInversionMove>)Parameters["IntraRouteInversionMove"]; }
    }

    [StorableConstructor]
    private IntraRouteInversionMoveMaker(bool deserializing) : base(deserializing) { }

    public IntraRouteInversionMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<IntraRouteInversionMove>("IntraRouteInversionMove", "The move to make."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
    }

    public static void Apply(AlbaEncoding solution, IntraRouteInversionMove move) {
      IntraRouteInversionManipulator.Apply(solution, move.Index1, move.Index2);
    }

    public override IOperation Apply() {
      IOperation next = base.Apply();
      
      AlbaEncoding solution = VRPToursParameter.ActualValue as AlbaEncoding;

      IntraRouteInversionMove move = IntraRouteInversionMoveParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;
     
      //perform move
      Apply(solution, move);

      quality.Value = moveQuality.Value;

      return next;
    }
  }
}
