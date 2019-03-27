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
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinCustomerRelocationMoveTabuMaker", "Declares a given customer relocation move as tabu.")]
  [StorableType("F2F78496-2059-4A16-B896-E1C779D913D0")]
  public class PotvinCustomerRelocationMoveTabuMaker : TabuMaker, IPotvinCustomerRelocationMoveOperator, IPotvinOperator {
    public ILookupParameter<PotvinCustomerRelocationMove> CustomerRelocationMoveParameter {
      get { return (ILookupParameter<PotvinCustomerRelocationMove>)Parameters["PotvinCustomerRelocationMove"]; }
    }
    public ILookupParameter VRPMoveParameter {
      get { return CustomerRelocationMoveParameter; }
    }
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    [StorableConstructor]
    protected PotvinCustomerRelocationMoveTabuMaker(StorableConstructorFlag _) : base(_) { }
    protected PotvinCustomerRelocationMoveTabuMaker(PotvinCustomerRelocationMoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public PotvinCustomerRelocationMoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<PotvinCustomerRelocationMove>("PotvinCustomerRelocationMove", "The moves that should be made."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours considered in the move."));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinCustomerRelocationMoveTabuMaker(this, cloner);
    }

    protected override IItem GetTabuAttribute(bool maximization, double quality, double moveQuality) {
      PotvinCustomerRelocationMove move = CustomerRelocationMoveParameter.ActualValue;
      double baseQuality = moveQuality;
      //if (quality < moveQuality) baseQuality = quality; // we make an uphill move, the lower bound is the solution quality
      return new PotvinCustomerRelocationMoveAttribute(baseQuality, move.OldTour, move.City);
    }
  }
}
