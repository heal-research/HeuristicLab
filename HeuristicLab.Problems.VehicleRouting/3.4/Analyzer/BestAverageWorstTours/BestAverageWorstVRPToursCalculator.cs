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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("BestAverageWorstVRPToursCalculator", "An operator which calculates the current best, average and worst properties of VRP tours in the scope tree.")]
  [StorableClass]
  public sealed class BestAverageWorstVRPToursCalculator : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> DistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ValueLookupParameter<DoubleValue> BestDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestDistance"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageDistance"]; }
    }
    public ValueLookupParameter<DoubleValue> WorstDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["WorstDistance"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> VehiclesUtilizedParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }
    public ValueLookupParameter<DoubleValue> BestVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestVehiclesUtilized"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageVehiclesUtilized"]; }
    }
    public ValueLookupParameter<DoubleValue> WorstVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["WorstVehiclesUtilized"]; }
    }

    public BestAverageWorstVRPToursCalculator()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Distance", "The distances of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestDistance", "The best distance value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageDistance", "The average distance value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("WorstDistance", "The worst distance value of all solutions."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("VehiclesUtilized", "The utilized vehicles of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestVehiclesUtilized", "The best utilized vehicles value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageVehiclesUtilized", "The average utilized vehicles value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("WorstVehiclesUtilized", "The worst utilized vehicles value of all solutions."));
    }

    [StorableConstructor]
    private BestAverageWorstVRPToursCalculator(bool deserializing) : base(deserializing) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstVRPToursCalculator(this, cloner);
    }

    private BestAverageWorstVRPToursCalculator(BestAverageWorstVRPToursCalculator original, Cloner cloner)
      : base(original, cloner) {
    }

    private void UpdateDistances() {
      ItemArray<DoubleValue> distances = DistanceParameter.ActualValue;
      if (distances.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0.0;
        for (int i = 0; i < distances.Length; i++) {
          if (distances[i].Value < min) min = distances[i].Value;
          if (distances[i].Value > max) max = distances[i].Value;
          sum += distances[i].Value;
        }

        DoubleValue best = BestDistanceParameter.ActualValue;
        if (best == null) BestDistanceParameter.ActualValue = new DoubleValue(min);
        else best.Value = min;
        DoubleValue average = AverageDistanceParameter.ActualValue;
        if (average == null) AverageDistanceParameter.ActualValue = new DoubleValue(sum / distances.Length);
        else average.Value = sum / distances.Length;
        DoubleValue worst = WorstDistanceParameter.ActualValue;
        if (worst == null) WorstDistanceParameter.ActualValue = new DoubleValue(max);
        else worst.Value = max;
      }
    }


    private void UpdateVehiclesUtilized() {
      ItemArray<DoubleValue> vehiclesUtilized = VehiclesUtilizedParameter.ActualValue;
      if (vehiclesUtilized.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0.0;
        for (int i = 0; i < vehiclesUtilized.Length; i++) {
          if (vehiclesUtilized[i].Value < min) min = vehiclesUtilized[i].Value;
          if (vehiclesUtilized[i].Value > max) max = vehiclesUtilized[i].Value;
          sum += vehiclesUtilized[i].Value;
        }

        DoubleValue best = BestVehiclesUtilizedParameter.ActualValue;
        if (best == null) BestVehiclesUtilizedParameter.ActualValue = new DoubleValue(min);
        else best.Value = min;
        DoubleValue average = AverageVehiclesUtilizedParameter.ActualValue;
        if (average == null) AverageVehiclesUtilizedParameter.ActualValue = new DoubleValue(sum / vehiclesUtilized.Length);
        else average.Value = sum / vehiclesUtilized.Length;
        DoubleValue worst = WorstVehiclesUtilizedParameter.ActualValue;
        if (worst == null) WorstVehiclesUtilizedParameter.ActualValue = new DoubleValue(max);
        else worst.Value = max;
      }
    }

    public override IOperation Apply() {
      UpdateDistances();
      UpdateVehiclesUtilized();

      return base.Apply();
    }
  }
}
