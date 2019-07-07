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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinManipulator", "A VRP manipulation operation.")]
  [StorableType("AF20AEB4-28EC-4291-9D27-6C5FF938ADA4")]
  public abstract class PotvinManipulator : VRPManipulator, IStochasticOperator, IPotvinOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueParameter<BoolValue> AllowInfeasibleSolutions {
      get { return (IValueParameter<BoolValue>)Parameters["AllowInfeasibleSolutions"]; }
    }

    [StorableConstructor]
    protected PotvinManipulator(StorableConstructorFlag _) : base(_) { }

    public PotvinManipulator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ValueParameter<BoolValue>("AllowInfeasibleSolutions", "Indicates if infeasible solutions should be allowed.", new BoolValue(false)));
    }

    protected PotvinManipulator(PotvinManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected abstract void Manipulate(IRandom random, PotvinEncoding individual);

    protected static int SelectRandomTourBiasedByLength(IRandom random, PotvinEncoding individual, IVRPProblemInstance instance) {
      int tourIndex = -1;

      double sum = 0.0;
      double[] probabilities = new double[individual.Tours.Count];
      for (int i = 0; i < individual.Tours.Count; i++) {
        probabilities[i] = 1.0 / ((double)individual.Tours[i].Stops.Count / (double)instance.Cities.Value);
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

    protected static bool FindInsertionPlace(PotvinEncoding individual, int city, int routeToAvoid, bool allowInfeasible, out int route, out int place) {
      return individual.FindInsertionPlace(
        city, routeToAvoid, allowInfeasible, out route, out place);
    }

    public override IOperation InstrumentedApply() {
      IVRPEncoding solution = VRPToursParameter.ActualValue;
      if (!(solution is PotvinEncoding)) {
        VRPToursParameter.ActualValue = PotvinEncoding.ConvertFrom(solution, ProblemInstance);
      }

      Manipulate(RandomParameter.ActualValue, VRPToursParameter.ActualValue as PotvinEncoding);
      (VRPToursParameter.ActualValue as PotvinEncoding).Repair();

      return base.InstrumentedApply();
    }
  }
}
