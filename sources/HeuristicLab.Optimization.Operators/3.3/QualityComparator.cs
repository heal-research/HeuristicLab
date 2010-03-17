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

using System;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  [Item("QualityComparator", "Compares two qualities and creates a boolean flag that indicates if the left side is better than the right side.")]
  [StorableClass]
  public class QualityComparator : SingleSuccessorOperator {
    public LookupParameter<DoubleValue> LeftSideParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["LeftSide"]; }
    }
    public ValueLookupParameter<DoubleValue> RightSideParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["RightSide"]; }
    }
    public LookupParameter<BoolValue> ResultParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Result"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public QualityComparator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("LeftSide", "The left side of the comparison."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("RightSide", "The right side of the comparison."));
      Parameters.Add(new LookupParameter<BoolValue>("Result", "The result of the comparison."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
    }

    public override IOperation Apply() {
      DoubleValue left = LeftSideParameter.ActualValue;
      DoubleValue right = RightSideParameter.ActualValue;
      BoolValue maximization = MaximizationParameter.ActualValue;
      bool better = maximization.Value && left.Value > right.Value
        || !maximization.Value && left.Value < right.Value;
      if (ResultParameter.ActualValue == null)
        ResultParameter.ActualValue = new BoolValue(better);
      else ResultParameter.ActualValue.Value = better;
      return base.Apply();
    }
  }
}
