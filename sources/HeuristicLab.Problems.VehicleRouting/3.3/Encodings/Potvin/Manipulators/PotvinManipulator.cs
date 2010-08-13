#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinManipulator", "A VRP manipulation operation.")]
  [StorableClass]
  public abstract class PotvinManipulator : VRPManipulator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected PotvinManipulator(bool deserializing) : base(deserializing) { }

    public PotvinManipulator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    protected abstract void Manipulate(IRandom random, PotvinEncoding individual);

    protected int SelectRandomTourBiasedByLength(IRandom random, PotvinEncoding individual) {
      int tourIndex = -1;

      double sum = 0.0;
      double[] probabilities = new double[individual.Tours.Count];
      for (int i = 0; i < individual.Tours.Count; i++) {
        probabilities[i] = 1.0 / ((double)individual.Tours[i].Cities.Count / (double)Cities);
        sum += probabilities[i];
      }

      for (int i = 0; i < probabilities.Length; i++)
        probabilities[i] = probabilities[i] / sum;

      double rand = random.NextDouble();
      double cumulatedProbabilities = 0.0;
      int index = 0;
      while (tourIndex == -1 && index < probabilities.Length) {
        if (cumulatedProbabilities <= rand && rand <= cumulatedProbabilities + probabilities[index])
          tourIndex = index;

        cumulatedProbabilities += probabilities[index];
        index++;
      }

      return tourIndex;
    }

    protected bool FindInsertionPlace(PotvinEncoding individual, int city, int routeToAvoid, out int route, out int place) {
      return individual.FindInsertionPlace(
        DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue, ReadyTimeParameter.ActualValue, 
        DemandParameter.ActualValue, CapacityParameter.ActualValue, CoordinatesParameter.ActualValue, 
        DistanceMatrixParameter, UseDistanceMatrixParameter.ActualValue,
        city, routeToAvoid, out route, out place);
    }
    
    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is PotvinEncoding)) {
        VRPToursParameter.ActualValue = PotvinEncoding.ConvertFrom(solution);
      }
      
      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as PotvinEncoding);

      return base.Apply();
    }
  }
}
