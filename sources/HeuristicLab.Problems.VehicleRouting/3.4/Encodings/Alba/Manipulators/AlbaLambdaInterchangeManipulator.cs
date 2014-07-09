#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaLambdaInterchangeManipulator", "An operator which applies the lambda interchange operation to a VRP representation. It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaLambdaInterchangeManipulator : AlbaManipulator {
    public IValueParameter<IntValue> LambdaParameter {
      get { return (IValueParameter<IntValue>)Parameters["Lambda"]; }
    }

    [StorableConstructor]
    private AlbaLambdaInterchangeManipulator(bool deserializing) : base(deserializing) { }

    public AlbaLambdaInterchangeManipulator()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Lambda", "The lambda value.", new IntValue(1)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaLambdaInterchangeManipulator(this, cloner);
    }

    private AlbaLambdaInterchangeManipulator(AlbaLambdaInterchangeManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    public static void Apply(AlbaEncoding individual, int tour1Index, int position1, int length1,
      int tour2Index, int position2, int length2) {
      List<Tour> tours = individual.GetTours();

      Tour tour1 = tours[tour1Index];
      int tour1Start = -1;
      for (int i = 0; i < individual.Length; i++) {
        if (individual[i] == tour1.Stops[0] - 1) {
          tour1Start = i;
          break;
        }
      }

      Tour tour2 = tours[tour2Index];
      int tour2Start = -1;
      for (int i = 0; i < individual.Length; i++) {
        if (individual[i] == tour2.Stops[0] - 1) {
          tour2Start = i;
          break;
        }
      }

      AlbaEncoding original = individual.Clone() as AlbaEncoding;
      int index = 0;

      int start1 = tour1Start + position1;
      int end1 = start1 + length1;

      int start2 = tour2Start + position2;
      int end2 = start2 + length2;

      for (int i = 0; i < original.Length; i++) {
        if (index == start1) {
          if (end2 - start2 == 0)
            index = end1;
          else
            index = start2;
        } else if (index == start2) {
          if (end1 - start1 == 0)
            index = end2;
          else
            index = start1;
        } else if (index == end1) {
          index = end2;
        } else if (index == end2) {
          index = end1;
        }

        individual[i] = original[index];

        index++;
      }
    }

    protected override void Manipulate(IRandom rand, AlbaEncoding individual) {
      List<Tour> tours = individual.GetTours();
      if (tours.Count > 1) {
        int lambda = LambdaParameter.Value.Value;

        int route1Index = rand.Next(tours.Count);
        Tour route1 = tours[route1Index];


        int route2Index = rand.Next(tours.Count - 1);
        if (route2Index >= route1Index)
          route2Index += 1;
        Tour route2 = tours[route2Index];

        int length1 = rand.Next(Math.Min(lambda + 1, route1.Stops.Count + 1));
        int index1 = rand.Next(route1.Stops.Count - length1 + 1);

        int l2Min = 0;
        if (length1 == 0)
          l2Min = 1;
        int length2 = rand.Next(l2Min, Math.Min(lambda + 1, route2.Stops.Count + 1));
        int index2 = rand.Next(route2.Stops.Count - length2 + 1);

        Apply(individual, route1Index, index1, length1,
          route2Index, index2, length2);
      }
    }
  }
}
