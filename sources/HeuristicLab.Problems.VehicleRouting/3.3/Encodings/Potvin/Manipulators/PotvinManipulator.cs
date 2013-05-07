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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinManipulator", "A VRP manipulation operation.")]
  [StorableClass]
  public abstract class PotvinManipulator : VRPManipulator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueParameter<BoolValue> AllowInfeasibleSolutions {
      get { return (IValueParameter<BoolValue>)Parameters["AllowInfeasibleSolutions"]; }
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (!Parameters.ContainsKey("AllowInfeasibleSolutions")) {
        Parameters.Add(new ValueParameter<BoolValue>("AllowInfeasibleSolutions", "Indicates if infeasible solutions should be allowed.", new BoolValue(false)));
      }
      #endregion
    }

    [StorableConstructor]
    protected PotvinManipulator(bool deserializing) : base(deserializing) { }
    protected PotvinManipulator(PotvinManipulator original, Cloner cloner)
      : base(original, cloner) {
    }
    public PotvinManipulator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ValueParameter<BoolValue>("AllowInfeasibleSolutions", "Indicates if infeasible solutions should be allowed.", new BoolValue(false)));
    }

    protected abstract void Manipulate(IRandom random, PotvinEncoding individual);

    protected static int SelectRandomTourBiasedByLength(IRandom random, PotvinEncoding individual) {
      int tourIndex = -1;

      double sum = 0.0;
      double[] probabilities = new double[individual.Tours.Count];
      for (int i = 0; i < individual.Tours.Count; i++) {
        probabilities[i] = 1.0 / ((double)individual.Tours[i].Cities.Count / (double)individual.Cities);
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

    protected static bool FindInsertionPlace(PotvinEncoding individual, int city, int routeToAvoid,
      DoubleArray dueTime, DoubleArray serviceTime, DoubleArray readyTime, DoubleArray demand,
      DoubleValue capacity, DistanceMatrix distMatrix,  bool allowInfeasible,
      out int route, out int place) {
      return individual.FindInsertionPlace(
        dueTime, serviceTime, readyTime,
        demand, capacity, distMatrix,
        city, routeToAvoid, allowInfeasible, 
        out route, out place);
    }
      

    public override IOperation Apply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is PotvinEncoding)) {
        VRPToursParameter.ActualValue = PotvinEncoding.ConvertFrom(solution, DistanceMatrixParameter);
      }

      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as PotvinEncoding);

      return base.Apply();
    }
  }
}
