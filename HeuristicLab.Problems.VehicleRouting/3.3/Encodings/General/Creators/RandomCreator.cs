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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("RandomCreator", "Creates a randomly initialized VRP solution.")]
  [StorableClass]
  public sealed class RandomCreator : DefaultRepresentationCreator, IStochasticOperator {
    #region IStochasticOperator Members
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    #endregion

    [StorableConstructor]
    private RandomCreator(bool deserializing) : base(deserializing) { }
    private RandomCreator(RandomCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomCreator(this, cloner);
    }

    public RandomCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
    }

    protected override List<int> CreateSolution() {
      int cities = Cities;
      int vehicles = VehiclesParameter.ActualValue.Value;

      int[] perm = new int[cities + vehicles];
      int[] sortingArray = new int[perm.Length - 2];
      for (int i = 0; i < cities; i++)
        perm[i] = i + 1;
      for (int i = cities; i < cities + vehicles; i++)
        perm[i] = 0;
      for (int i = 0; i < perm.Length - 2; i++) {
        sortingArray[i] = RandomParameter.ActualValue.Next(perm.Length * 2);
      }
      // shuffle perm from 1...n-1 by sorting sortingArray (perm[0] and perm[n] must stay 0)
      bool done;
      do {
        done = true;
        for (int i = 0; i < perm.Length - 3; i++) {
          if (sortingArray[i] > sortingArray[i + 1]) {
            int h = sortingArray[i];
            sortingArray[i] = sortingArray[i + 1];
            sortingArray[i + 1] = h;
            h = perm[i + 1];
            perm[i + 1] = perm[i + 2];
            perm[i + 2] = h;
            done = false;
          }
        }
      } while (!done);

      return new List<int>(perm);
    }
  }
}
