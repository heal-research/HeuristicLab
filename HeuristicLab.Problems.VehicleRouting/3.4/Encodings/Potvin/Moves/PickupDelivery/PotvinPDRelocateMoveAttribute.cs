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
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDRelocateMoveAttribute", "PD relocation move attribute")]
  [StorableType("F8315360-2078-4117-BB0E-AEBFFD0DB196")]
  public class PotvinPDRelocateMoveAttribute : VRPMoveAttribute {
    [Storable]
    public int Tour { get; private set; }
    [Storable]
    public int City { get; private set; }

    [Storable]
    public double Distance { get; private set; }
    [Storable]
    public double Overload { get; private set; }
    [Storable]
    public double Tardiness { get; private set; }
    [Storable]
    public int PickupViolations { get; private set; }

    [StorableConstructor]
    protected PotvinPDRelocateMoveAttribute(StorableConstructorFlag _) : base(_) { }
    protected PotvinPDRelocateMoveAttribute(PotvinPDRelocateMoveAttribute original, Cloner cloner)
      : base(original, cloner) {
      this.Tour = original.Tour;
      this.City = original.City;
      this.Distance = original.Distance;
      this.Overload = original.Overload;
      this.Tardiness = original.Tardiness;
      this.PickupViolations = original.PickupViolations;
    }
    public PotvinPDRelocateMoveAttribute(double moveQuality, int tour, int city,
      double distance, double overload, double tardiness, int pickupViolations)
      : base(moveQuality) {
      Tour = tour;
      City = city;
      Distance = distance;
      Overload = overload;
      Tardiness = tardiness;
      PickupViolations = pickupViolations;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDRelocateMoveAttribute(this, cloner);
    }

    public override int GetHashCode() {
      return Tour.GetHashCode() + City.GetHashCode();
    }

    public override bool Equals(object obj) {
      if (obj is PotvinPDRelocateMoveAttribute) {
        PotvinPDRelocateMoveAttribute other = obj as PotvinPDRelocateMoveAttribute;

        return Tour == other.Tour && City == other.City;
      } else {
        return base.Equals(obj);
      }
    }
  }
}
