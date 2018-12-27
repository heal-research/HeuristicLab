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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  [Item("QualityComparator", "Compares two qualities and creates a boolean flag that indicates if the left side is better than the right side.")]
  [StorableClass]
  public class QualityComparator : SingleSuccessorOperator, IQualityComparator {
    public ILookupParameter<DoubleValue> LeftSideParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["LeftSide"]; }
    }
    public IValueLookupParameter<DoubleValue> RightSideParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["RightSide"]; }
    }
    public ILookupParameter<BoolValue> ResultParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Result"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    [StorableConstructor]
    protected QualityComparator(bool deserializing) : base(deserializing) { }
    protected QualityComparator(QualityComparator original, Cloner cloner) : base(original, cloner) { }
    public QualityComparator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("LeftSide", "The left side of the comparison."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("RightSide", "The right side of the comparison."));
      Parameters.Add(new LookupParameter<BoolValue>("Result", "The result of the comparison, true if the quality on the LeftSide is better than the quality on the RightSide."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QualityComparator(this, cloner);
    }

    public override IOperation Apply() {
      DoubleValue left = LeftSideParameter.ActualValue;
      DoubleValue right = RightSideParameter.ActualValue;
      BoolValue maximization = MaximizationParameter.ActualValue;
      bool better = Compare(maximization.Value, left.Value, right.Value);
      if (ResultParameter.ActualValue == null)
        ResultParameter.ActualValue = new BoolValue(better);
      else ResultParameter.ActualValue.Value = better;
      return base.Apply();
    }

    protected virtual bool Compare(bool maximization, double left, double right) {
      return maximization && left > right || !maximization && left < right;
    }
  }
}
