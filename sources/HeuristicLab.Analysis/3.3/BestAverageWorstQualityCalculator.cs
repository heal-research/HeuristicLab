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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which calculates the best, average and worst solution quality of the current population.
  /// </summary>
  [Item("BestAverageWorstQualityCalculator", "An operator which calculates the best, average and worst solution quality of the current population.")]
  [StorableClass]
  [Creatable("Test")]
  public sealed class BestAverageWorstQualityCalculator : SingleSuccessorOperator {
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<DoubleValue> BestQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public IValueLookupParameter<DoubleValue> AverageQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["AverageQuality"]; }
    }
    public IValueLookupParameter<DoubleValue> WorstQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["WorstQuality"]; }
    }

    public BestAverageWorstQualityCalculator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the current problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The value contained in each sub-scope which represents the solution quality."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestQuality", "The quality value of the best solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageQuality", "The average quality of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("WorstQuality", "The quality value of the worst solution."));
    }


    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;

      if (qualities.Length > 0) {
        double min = double.MaxValue, max = double.MinValue, sum = 0.0;
        for (int i = 0; i < qualities.Length; i++) {
          if (qualities[i].Value < min) min = qualities[i].Value;
          if (qualities[i].Value > max) max = qualities[i].Value;
          sum += qualities[i].Value;
        }
        if (!maximization) {
          double temp = min;
          min = max;
          max = temp;
        }

        DoubleValue best = BestQualityParameter.ActualValue;
        if (best == null) BestQualityParameter.ActualValue = new DoubleValue(max);
        else best.Value = max;
        DoubleValue average = AverageQualityParameter.ActualValue;
        if (average == null) AverageQualityParameter.ActualValue = new DoubleValue(sum / qualities.Length);
        else average.Value = sum / qualities.Length;
        DoubleValue worst = WorstQualityParameter.ActualValue;
        if (worst == null) WorstQualityParameter.ActualValue = new DoubleValue(min);
        else worst.Value = min;
      }
      return base.Apply();
    }
  }
}
