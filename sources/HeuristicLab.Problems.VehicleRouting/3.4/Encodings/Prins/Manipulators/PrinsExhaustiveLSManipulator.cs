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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsExhaustiveLSManipulator", "An operator which manipulates a VRP representation by using the exhaustive Prins local search.  It is implemented as described in Prins, C. (2004). A simple and effective evolutionary algorithm for the vehicle routing problem. Computers & Operations Research, 12:1985-2002.")]
  [StorableClass]
  public sealed class PrinsExhaustiveLSManipulator : PrinsLSManipulator {
    [StorableConstructor]
    private PrinsExhaustiveLSManipulator(bool deserializing) : base(deserializing) { }

    public PrinsExhaustiveLSManipulator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PrinsExhaustiveLSManipulator(this, cloner);
    }

    private PrinsExhaustiveLSManipulator(PrinsExhaustiveLSManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Manipulate(IRandom random, PrinsEncoding individual) {
      List<Tour> tours = individual.GetTours();
      bool improvement = false;
      int iterations = 0;

      do {
        int u = depot;
        improvement = false;
        double originalQuality = GetQuality(individual);
        PrinsEncoding child = null;

        while (!improvement && u < ProblemInstance.Cities.Value) {
          int v = depot;
          while (!improvement && v < ProblemInstance.Cities.Value) {
            if (u != v) {
              child = Manipulate(individual,
                originalQuality, u, v);

              improvement = child != null;
            }
            v++;
          }
          u++;
        }

        if (improvement) {
          for (int i = 0; i < child.Length; i++) {
            individual[i] = child[i];
          }
        }

        iterations++;
      } while (improvement &&
        iterations < Iterations.Value.Value);
    }
  }
}
