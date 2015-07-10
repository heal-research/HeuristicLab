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
using HeuristicLab.Data;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDShiftMoveTabuMaker", "Declares a given shift move as tabu.")]
  [StorableClass]
  public class PotvinPDShiftMoveTabuMaker : TabuMaker, IPotvinPDShiftMoveOperator, IPotvinOperator, IVRPMoveOperator {
    public ILookupParameter<PotvinPDShiftMove> PDShiftMoveParameter {
      get { return (ILookupParameter<PotvinPDShiftMove>)Parameters["PotvinPDShiftMove"]; }
    }
    public ILookupParameter VRPMoveParameter {
      get { return PDShiftMoveParameter; }
    }
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public ILookupParameter<DoubleValue> DistanceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ILookupParameter<DoubleValue> OverloadParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ILookupParameter<DoubleValue> TardinessParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }
    public ILookupParameter<IntValue> PickupViolationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["PickupViolations"]; }
    }

    [StorableConstructor]
    protected PotvinPDShiftMoveTabuMaker(bool deserializing) : base(deserializing) { }
    protected PotvinPDShiftMoveTabuMaker(PotvinPDShiftMoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public PotvinPDShiftMoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<PotvinPDShiftMove>("PotvinPDShiftMove", "The moves that should be made."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours considered in the move."));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));

      Parameters.Add(new LookupParameter<DoubleValue>("Distance", "The distance of the individual"));
      Parameters.Add(new LookupParameter<DoubleValue>("Overload", "The overload of the individual"));
      Parameters.Add(new LookupParameter<DoubleValue>("Tardiness", "The tardiness of the individual"));
      Parameters.Add(new LookupParameter<IntValue>("PickupViolations", "The number of pickup violations."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDShiftMoveTabuMaker(this, cloner);
    }

    protected override IItem GetTabuAttribute(bool maximization, double quality, double moveQuality) {
      PotvinPDShiftMove move = PDShiftMoveParameter.ActualValue;
      double baseQuality = moveQuality;
      if (quality < moveQuality) baseQuality = quality; // we make an uphill move, the lower bound is the solution quality

      double distance = 0;
      if (DistanceParameter.ActualValue != null)
        distance = DistanceParameter.ActualValue.Value;

      double overload = 0;
      if (OverloadParameter.ActualValue != null)
        overload = OverloadParameter.ActualValue.Value;

      double tardiness = 0;
      if (TardinessParameter.ActualValue != null)
        tardiness = TardinessParameter.ActualValue.Value;

      int pickupViolations = 0;
      if (PickupViolationsParameter.ActualValue != null)
        pickupViolations = PickupViolationsParameter.ActualValue.Value;

      return new PotvinPDRelocateMoveAttribute(baseQuality, move.OldTour, move.City, distance, overload, tardiness, pickupViolations);
    }
  }
}
