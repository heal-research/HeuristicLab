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
  [Item("BestAverageWorstCapacitatedVRPToursCalculator", "An operator which calculates the current best, average and worst properties of VRP tours in the scope tree.")]
  [StorableType("CAF65472-0C4E-4564-B750-61F6924AA284")]
  public sealed class BestAverageWorstCapacitatedVRPToursCalculator : SingleSuccessorOperator {
    public ScopeTreeLookupParameter<DoubleValue> OverloadParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ValueLookupParameter<DoubleValue> BestOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> WorstOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["WorstOverload"]; }
    }

    public BestAverageWorstCapacitatedVRPToursCalculator()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestOverload", "The best overload value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageOverload", "The average overload value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("WorstOverload", "The worst overload value of all solutions."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstCapacitatedVRPToursCalculator(this, cloner);
    }

    private BestAverageWorstCapacitatedVRPToursCalculator(BestAverageWorstCapacitatedVRPToursCalculator original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    private BestAverageWorstCapacitatedVRPToursCalculator(StorableConstructorFlag _) : base(_) { }

    private void UpdateOverloads() {
      ItemArray<DoubleValue> overloads = OverloadParameter.ActualValue;
      if (overloads.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0.0;
        for (int i = 0; i < overloads.Length; i++) {
          if (overloads[i].Value < min) min = overloads[i].Value;
          if (overloads[i].Value > max) max = overloads[i].Value;
          sum += overloads[i].Value;
        }

        DoubleValue best = BestOverloadParameter.ActualValue;
        if (best == null) BestOverloadParameter.ActualValue = new DoubleValue(min);
        else best.Value = min;
        DoubleValue average = AverageOverloadParameter.ActualValue;
        if (average == null) AverageOverloadParameter.ActualValue = new DoubleValue(sum / overloads.Length);
        else average.Value = sum / overloads.Length;
        DoubleValue worst = WorstOverloadParameter.ActualValue;
        if (worst == null) WorstOverloadParameter.ActualValue = new DoubleValue(max);
        else worst.Value = max;
      }
    }

    public override IOperation Apply() {
      UpdateOverloads();

      return base.Apply();
    }
  }
}
