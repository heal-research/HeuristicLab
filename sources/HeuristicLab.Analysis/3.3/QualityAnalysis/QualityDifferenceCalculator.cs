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

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which calculates the absolute and relative difference of two quality values.
  /// </summary>
  [Item("QualityDifferenceCalculator", "An operator which calculates the absolute and relative difference of two quality values.")]
  [StorableClass]
  public class QualityDifferenceCalculator : SingleSuccessorOperator {
    public IValueLookupParameter<DoubleValue> FirstQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["FirstQuality"]; }
    }
    public IValueLookupParameter<DoubleValue> SecondQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["SecondQuality"]; }
    }
    public IValueLookupParameter<DoubleValue> AbsoluteDifferenceParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["AbsoluteDifference"]; }
    }
    public IValueLookupParameter<PercentValue> RelativeDifferenceParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters["RelativeDifference"]; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    protected QualityDifferenceCalculator(bool deserializing) : base(deserializing) { }
    protected QualityDifferenceCalculator(QualityDifferenceCalculator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new QualityDifferenceCalculator(this, cloner);
    }
    #endregion
    public QualityDifferenceCalculator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("FirstQuality", "The first quality value from which the difference to the second quality value is calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SecondQuality", "The second quality value from which the difference from the first quality value is calculated."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AbsoluteDifference", "The absolute difference of the first and second quality value."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("RelativeDifference", "The relative difference of the first and second quality value."));
    }

    public override IOperation Apply() {
      DoubleValue first = FirstQualityParameter.ActualValue;
      DoubleValue second = SecondQualityParameter.ActualValue;

      if ((first != null) && (second != null)) {
        double absolute = second.Value - first.Value;
        double relative = first.Value == 0 ? double.NaN : absolute / first.Value;

        if (AbsoluteDifferenceParameter.ActualValue == null) AbsoluteDifferenceParameter.ActualValue = new DoubleValue(absolute);
        else AbsoluteDifferenceParameter.ActualValue.Value = absolute;
        if (RelativeDifferenceParameter.ActualValue == null) RelativeDifferenceParameter.ActualValue = new PercentValue(relative);
        else RelativeDifferenceParameter.ActualValue.Value = relative;
      }
      return base.Apply();
    }
  }
}
