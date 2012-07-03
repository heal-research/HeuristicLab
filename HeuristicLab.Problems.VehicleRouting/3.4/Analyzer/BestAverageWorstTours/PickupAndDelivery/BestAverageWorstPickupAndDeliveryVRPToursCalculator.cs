#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("BestAverageWorstPickupAndDeliveryVRPToursCalculator", "An operator which calculates the current best, average and worst properties of VRP tours in the scope tree.")]
  [StorableClass]
  public sealed class BestAverageWorstPickupAndDeliveryVRPToursCalculator : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<IntValue> PickupViolationsParameter {
      get { return (ScopeTreeLookupParameter<IntValue>)Parameters["PickupViolations"]; }
    }
    public ValueLookupParameter<IntValue> BestPickupViolationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["BestPickupViolations"]; }
    }
    public ValueLookupParameter<DoubleValue> AveragePickupViolationsParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AveragePickupViolations"]; }
    }
    public ValueLookupParameter<IntValue> WorstPickupViolationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["WorstPickupViolations"]; }
    }

    public BestAverageWorstPickupAndDeliveryVRPToursCalculator()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("PickupViolations", "The pickup violations of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("BestPickupViolations", "The best pickup violations value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AveragePickupViolations", "The average pickup violations value of all solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("WorstPickupViolations", "The worst pickup violations value of all solutions."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstPickupAndDeliveryVRPToursCalculator(this, cloner);
    }

    private BestAverageWorstPickupAndDeliveryVRPToursCalculator(BestAverageWorstPickupAndDeliveryVRPToursCalculator original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    private BestAverageWorstPickupAndDeliveryVRPToursCalculator(bool deserializing) : base(deserializing) { }

    private void UpdatePickupViolations() {
      ItemArray<IntValue> pickupViolations = PickupViolationsParameter.ActualValue;
      if (pickupViolations.Length > 0) {
        int min = int.MaxValue, max = int.MinValue, sum = 0;
        for (int i = 0; i < pickupViolations.Length; i++) {
          if (pickupViolations[i].Value < min) min = pickupViolations[i].Value;
          if (pickupViolations[i].Value > max) max = pickupViolations[i].Value;
          sum += pickupViolations[i].Value;
        }

        IntValue best = BestPickupViolationsParameter.ActualValue;
        if (best == null) BestPickupViolationsParameter.ActualValue = new IntValue(min);
        else best.Value = min;
        DoubleValue average = AveragePickupViolationsParameter.ActualValue;
        if (average == null) AveragePickupViolationsParameter.ActualValue = new DoubleValue(sum / pickupViolations.Length);
        else average.Value = sum / pickupViolations.Length;
        IntValue worst = WorstPickupViolationsParameter.ActualValue;
        if (worst == null) WorstPickupViolationsParameter.ActualValue = new IntValue(max);
        else worst.Value = max;
      }
    }

    public override IOperation Apply() {
      UpdatePickupViolations();

      return base.Apply();
    }
  }
}
