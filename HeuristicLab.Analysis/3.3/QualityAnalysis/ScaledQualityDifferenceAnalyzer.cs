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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis.QualityAnalysis {
  [Item("ScaledQualityDifferenceAnalyzer", @"Calculates the quality value relative to a certain range given with a minimum and a maximum value.
The difference lies in the interval [0;1] if the range [min;max] is as large as the observed quality values, otherwise the difference will become < 0 or > 1.
A value towards 0 always means that it's closer to the better fitness value, while a value towards 1 means that it's closer to the worse fitness value.")]
  [StorableClass]
  public class ScaledQualityDifferenceAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public virtual bool EnabledByDefault {
      get { return true; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> ScaledDifferenceParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["ScaledDifference"]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public IValueLookupParameter<DoubleValue> MinimumQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MinimumQuality"]; }
    }
    public IValueLookupParameter<DoubleValue> MaximumQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MaximumQuality"]; }
    }

    [StorableConstructor]
    protected ScaledQualityDifferenceAnalyzer(bool deserializing) : base(deserializing) { }
    protected ScaledQualityDifferenceAnalyzer(ScaledQualityDifferenceAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public ScaledQualityDifferenceAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value that should be compared."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, false otherwise."));
      Parameters.Add(new LookupParameter<DoubleValue>("ScaledDifference", "The value that describes whether the quality value lies towards the better quality in the range (<=0.5) or more towards the worse quality (> 0.5)."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The result collection where the difference will be stored (in addition to the scope)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MinimumQuality", "The lower bound of the quality range to which the quality is compared."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumQuality", "The upper bound of the quality range to which the quality is compared."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScaledQualityDifferenceAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      bool maximization = MaximizationParameter.ActualValue.Value;
      double quality = QualityParameter.ActualValue.Value, max = MaximumQualityParameter.ActualValue.Value, min = MinimumQualityParameter.ActualValue.Value;
      double difference;

      difference = ((quality - min) / (max - min));
      if (maximization) difference = 1.0 - difference;

      DoubleValue differenceValue = ScaledDifferenceParameter.ActualValue;
      if (differenceValue == null) {
        differenceValue = new DoubleValue(difference);
        ScaledDifferenceParameter.ActualValue = differenceValue;
      } else differenceValue.Value = difference;

      ResultCollection results = ResultsParameter.ActualValue;
      if (results != null) {
        IResult r;
        if (!results.TryGetValue(ScaledDifferenceParameter.TranslatedName, out r)) {
          r = new Result(ScaledDifferenceParameter.TranslatedName, differenceValue);
          results.Add(r);
        }
      }

      return base.Apply();
    }
  }
}
