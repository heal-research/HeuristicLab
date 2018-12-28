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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("BestAverageWorstTimeWindowedVRPToursCalculator", "An operator which calculates the current best, average and worst properties of VRP tours in the scope tree.")]
  [StorableType("2B7EA9C1-4036-497F-861D-771679ADAAE7")]
  public sealed class BestAverageWorstTimeWindowedVRPToursCalculator : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> TardinessParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> BestTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageTardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> WorstTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["WorstTardiness"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> BestTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageTravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> WorstTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["WorstTravelTime"]; }
    }

    public BestAverageWorstTimeWindowedVRPToursCalculator()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Tardiness", "The tardiness of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTardiness", "The best tardiness value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageTardiness", "The average tardiness value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("WorstTardiness", "The worst tardiness value of all solutions."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("TravelTime", "The travel times of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTravelTime", "The best travel time value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageTravelTime", "The average travel time value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("WorstTravelTime", "The worst travel time value of all solutions."));
    }

    [StorableConstructor]
    private BestAverageWorstTimeWindowedVRPToursCalculator(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstTimeWindowedVRPToursCalculator(this, cloner);
    }

    private BestAverageWorstTimeWindowedVRPToursCalculator(BestAverageWorstTimeWindowedVRPToursCalculator original, Cloner cloner)
      : base(original, cloner) {
    }

    private void UpdateTardiness() {
      ItemArray<DoubleValue> tardiness = TardinessParameter.ActualValue;
      if (tardiness.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0.0;
        for (int i = 0; i < tardiness.Length; i++) {
          if (tardiness[i].Value < min) min = tardiness[i].Value;
          if (tardiness[i].Value > max) max = tardiness[i].Value;
          sum += tardiness[i].Value;
        }

        DoubleValue best = BestTardinessParameter.ActualValue;
        if (best == null) BestTardinessParameter.ActualValue = new DoubleValue(min);
        else best.Value = min;
        DoubleValue average = AverageTardinessParameter.ActualValue;
        if (average == null) AverageTardinessParameter.ActualValue = new DoubleValue(sum / tardiness.Length);
        else average.Value = sum / tardiness.Length;
        DoubleValue worst = WorstTardinessParameter.ActualValue;
        if (worst == null) WorstTardinessParameter.ActualValue = new DoubleValue(max);
        else worst.Value = max;
      }
    }

    private void UpdateTravelTimes() {
      ItemArray<DoubleValue> travelTimes = TravelTimeParameter.ActualValue;
      if (travelTimes.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0.0;
        for (int i = 0; i < travelTimes.Length; i++) {
          if (travelTimes[i].Value < min) min = travelTimes[i].Value;
          if (travelTimes[i].Value > max) max = travelTimes[i].Value;
          sum += travelTimes[i].Value;
        }

        DoubleValue best = BestTravelTimeParameter.ActualValue;
        if (best == null) BestTravelTimeParameter.ActualValue = new DoubleValue(min);
        else best.Value = min;
        DoubleValue average = AverageTravelTimeParameter.ActualValue;
        if (average == null) AverageTravelTimeParameter.ActualValue = new DoubleValue(sum / travelTimes.Length);
        else average.Value = sum / travelTimes.Length;
        DoubleValue worst = WorstTravelTimeParameter.ActualValue;
        if (worst == null) WorstTravelTimeParameter.ActualValue = new DoubleValue(max);
        else worst.Value = max;
      }
    }

    public override IOperation Apply() {
      UpdateTardiness();
      UpdateTravelTimes();

      return base.Apply();
    }
  }
}
