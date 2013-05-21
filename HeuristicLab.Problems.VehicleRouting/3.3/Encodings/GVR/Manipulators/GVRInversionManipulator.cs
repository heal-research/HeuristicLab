#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVRInversionManipulator", "An operator which manipulates a GVR representation by inverting a subroute. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableClass]
  public sealed class GVRInversionManipulator : GVRManipulator {
    [StorableConstructor]
    private GVRInversionManipulator(bool deserializing) : base(deserializing) { }
    private GVRInversionManipulator(GVRInversionManipulator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVRInversionManipulator(this, cloner);
    }
    public GVRInversionManipulator() : base() { }

    protected override void Manipulate(IRandom random, GVREncoding individual) {
      Tour tour = individual.Tours[random.Next(individual.Tours.Count)];
      int breakPoint1 = random.Next(tour.Cities.Count);
      int length = random.Next(1, tour.Cities.Count - breakPoint1 + 1);

      tour.Cities.Reverse(breakPoint1, length);
    }
  }
}
