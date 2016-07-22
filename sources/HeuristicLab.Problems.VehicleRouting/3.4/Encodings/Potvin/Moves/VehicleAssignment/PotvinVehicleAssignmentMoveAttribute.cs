#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinVehicleAssignmentMoveAttribute", "vehicle assignment move attribute")]
  [StorableClass]
  public class PotvinVehicleAssignmentMoveAttribute : VRPMoveAttribute {
    [Storable]
    public int Tour { get; private set; }
    [Storable]
    public int Vehicle { get; private set; }

    [Storable]
    public double Distance { get; private set; }
    [Storable]
    public double Overload { get; private set; }
    [Storable]
    public double Tardiness { get; private set; }

    [StorableConstructor]
    protected PotvinVehicleAssignmentMoveAttribute(bool deserializing) : base(deserializing) { }
    protected PotvinVehicleAssignmentMoveAttribute(PotvinVehicleAssignmentMoveAttribute original, Cloner cloner)
      : base(original, cloner) {
      this.Tour = original.Tour;
      this.Vehicle = original.Vehicle;
      this.Distance = original.Distance;
      this.Overload = original.Overload;
      this.Tardiness = original.Tardiness;
    }
    public PotvinVehicleAssignmentMoveAttribute(double moveQuality, int tour, int vehicle,
      double distance, double overload, double tardiness)
      : base(moveQuality) {
      Tour = tour;
      Vehicle = vehicle;
      Distance = distance;
      Overload = overload;
      Tardiness = tardiness;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinVehicleAssignmentMoveAttribute(this, cloner);
    }

    public override int GetHashCode() {
      return Tour.GetHashCode() + Vehicle.GetHashCode();
    }

    public override bool Equals(object obj) {
      if (obj is PotvinVehicleAssignmentMoveAttribute) {
        PotvinVehicleAssignmentMoveAttribute other = obj as PotvinVehicleAssignmentMoveAttribute;

        return Tour == other.Tour && Vehicle == other.Vehicle;
      } else {
        return base.Equals(obj);
      }
    }
  }
}
