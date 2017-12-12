#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  [Item("WeightedParentsQualityComparator", "Compares the quality against that of its parents (assumes the parents are subscopes to the child scope). This operator works with any number of subscopes > 0.")]
  [StorableClass]
  public class WeightedParentsQualityComparator : SingleSuccessorOperator, ISubScopesQualityComparator {
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> LeftSideParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["LeftSide"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> RightSideParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["RightSide"]; }
    }
    public ILookupParameter<BoolValue> ResultParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Result"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }

    [StorableConstructor]
    protected WeightedParentsQualityComparator(bool deserializing) : base(deserializing) { }
    protected WeightedParentsQualityComparator(WeightedParentsQualityComparator original, Cloner cloner) : base(original, cloner) { }
    public WeightedParentsQualityComparator()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, false otherwise"));
      Parameters.Add(new LookupParameter<DoubleValue>("LeftSide", "The quality of the child."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("RightSide", "The qualities of the parents."));
      Parameters.Add(new LookupParameter<BoolValue>("Result", "The result of the comparison: True means Quality is better, False means it is worse than parents."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactor", "Determines if the quality should be compared to the better parent (1.0), to the worse (0.0) or to any linearly interpolated value between them."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new WeightedParentsQualityComparator(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> rightQualities = RightSideParameter.ActualValue;
      if (rightQualities.Length < 1) throw new InvalidOperationException(Name + ": No subscopes found.");
      double compFact = ComparisonFactorParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      double leftQuality = LeftSideParameter.ActualValue.Value;

      double threshold = 0;

      #region Calculate threshold
      if (rightQualities.Length == 2) { // this case will probably be used most often
        double minQuality = Math.Min(rightQualities[0].Value, rightQualities[1].Value);
        double maxQuality = Math.Max(rightQualities[0].Value, rightQualities[1].Value);
        if (maximization)
          threshold = minQuality + (maxQuality - minQuality) * compFact;
        else
          threshold = maxQuality - (maxQuality - minQuality) * compFact;
      } else if (rightQualities.Length == 1) { // case for just one parent
        threshold = rightQualities[0].Value;
      } else { // general case extended to 3 or more parents
        List<double> sortedQualities = rightQualities.Select(x => x.Value).ToList();
        sortedQualities.Sort();
        double minimumQuality = sortedQualities.First();

        double integral = 0;
        for (int i = 0; i < sortedQualities.Count - 1; i++) {
          integral += (sortedQualities[i] + sortedQualities[i + 1]) / 2.0; // sum of the trapezoid
        }
        integral -= minimumQuality * sortedQualities.Count;
        if (integral == 0) threshold = sortedQualities[0]; // all qualities are equal
        else {
          double selectedArea = integral * (maximization ? compFact : (1 - compFact));
          integral = 0;
          for (int i = 0; i < sortedQualities.Count - 1; i++) {
            double currentSliceArea = (sortedQualities[i] + sortedQualities[i + 1]) / 2.0;
            double windowedSliceArea = currentSliceArea - minimumQuality;
            if (windowedSliceArea == 0) continue;
            integral += windowedSliceArea;
            if (integral >= selectedArea) {
              double factor = 1 - ((integral - selectedArea) / (windowedSliceArea));
              threshold = sortedQualities[i] + (sortedQualities[i + 1] - sortedQualities[i]) * factor;
              break;
            }
          }
        }
      }
      #endregion

      bool result = maximization && leftQuality > threshold || !maximization && leftQuality < threshold;
      BoolValue resultValue = ResultParameter.ActualValue;
      if (resultValue == null) {
        ResultParameter.ActualValue = new BoolValue(result);
      } else {
        resultValue.Value = result;
      }

      return base.Apply();
    }
  }
}
