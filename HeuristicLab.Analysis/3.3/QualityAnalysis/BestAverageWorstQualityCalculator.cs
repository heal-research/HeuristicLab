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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which calculates the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("BestAverageWorstQualityCalculator", "An operator which calculates the best, average and worst quality of solutions in the scope tree.")]
  [StorableType("29E14C28-D4F7-4F26-9D18-61867457A693")]
  public sealed class BestAverageWorstQualityCalculator : SingleSuccessorOperator, ISingleObjectiveOperator {
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<DoubleValue> BestQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> AverageQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["AverageQuality"]; }
    }
    public ValueLookupParameter<DoubleValue> WorstQualityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["WorstQuality"]; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    private BestAverageWorstQualityCalculator(StorableConstructorFlag _) : base(_) { }
    private BestAverageWorstQualityCalculator(BestAverageWorstQualityCalculator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstQualityCalculator(this, cloner);
    }
    #endregion
    public BestAverageWorstQualityCalculator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the current problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value contained in the scope tree which represents the solution quality."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestQuality", "The quality value of the best solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AverageQuality", "The average quality of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("WorstQuality", "The quality value of the worst solution."));

      BestQualityParameter.Hidden = true;
      AverageQualityParameter.Hidden = true;
      WorstQualityParameter.Hidden = true;
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
