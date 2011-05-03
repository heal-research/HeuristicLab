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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Prins {
  [Item("PrinsStochasticLSManipulator", "An operator which manipulates a VRP representation by using the stochastic version of the Prins local search.  It is implemented as described in Prins, C. (2004). A simple and effective evolutionary algorithm for the vehicle routing problem. Computers & Operations Research, 12:1985-2002.")]
  [StorableClass]
  public sealed class PrinsStochasticLSManipulator : PrinsLSManipulator {
    public IValueParameter<IntValue> SampleSize {
      get { return (IValueParameter<IntValue>)Parameters["SampleSize"]; }
    }
    
    [StorableConstructor]
    private PrinsStochasticLSManipulator(bool deserializing) : base(deserializing) { }
    private PrinsStochasticLSManipulator(PrinsStochasticLSManipulator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PrinsStochasticLSManipulator(this, cloner);
    }
    public PrinsStochasticLSManipulator()
      : base() {
        Parameters.Add(new ValueParameter<IntValue>("SampleSize", "The sample size.", new IntValue(200)));
    }

    protected override void Manipulate(IRandom random, PrinsEncoding individual) {
      List<Tour> tours = individual.GetTours(DistanceMatrixParameter);
      bool improvement = false;
      int iterations = 0;

      do {
        improvement = false;
        double originalQuality = GetQuality(individual);
        PrinsEncoding child = null;

        int samples = 0;
        while (!improvement &&
          samples < SampleSize.Value.Value) {
            int u = random.Next(Cities);
            int v = random.Next(Cities);

            child = Manipulate(individual,
                  originalQuality, u, v);

            improvement = child != null;

            samples++;
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
