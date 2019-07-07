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
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinTwoOptStarMoveAttribute", "Two opt star move attribute")]
  [StorableType("BE6E1C1B-335A-4FDB-9105-A7304380510E")]
  public class PotvinTwoOptStarMoveAttribute : VRPMoveAttribute {
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

    [StorableConstructor]
    protected PotvinTwoOptStarMoveAttribute(StorableConstructorFlag _) : base(_) { }
    protected PotvinTwoOptStarMoveAttribute(PotvinTwoOptStarMoveAttribute original, Cloner cloner)
      : base(original, cloner) {
      this.Tour = original.Tour;
      this.City = original.City;
      this.Distance = original.Distance;
      this.Overload = original.Overload;
      this.Tardiness = original.Tardiness;
    }
    public PotvinTwoOptStarMoveAttribute(double moveQuality, int tour, int city,
      double distance, double overload, double tardiness)
      : base(moveQuality) {
      Tour = tour;
      City = city;
      Distance = distance;
      Overload = overload;
      Tardiness = tardiness;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinTwoOptStarMoveAttribute(this, cloner);
    }

    public override int GetHashCode() {
      return Tour.GetHashCode() + City.GetHashCode();
    }

    public override bool Equals(object obj) {
      if (obj is PotvinTwoOptStarMoveAttribute) {
        PotvinTwoOptStarMoveAttribute other = obj as PotvinTwoOptStarMoveAttribute;

        return Tour == other.Tour && City == other.City;
      } else {
        return base.Equals(obj);
      }
    }
  }
}
