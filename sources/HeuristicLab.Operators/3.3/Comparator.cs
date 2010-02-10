#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Operator which increments an integer variable.
  /// </summary>
  [Item("Comparator", "An operator which compares two items.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class Comparator : SingleSuccessorOperator {
    public LookupParameter<IItem> LeftSideParameter {
      get { return (LookupParameter<IItem>)Parameters["LeftSide"]; }
    }
    public ValueLookupParameter<IItem> RightSideParameter {
      get { return (ValueLookupParameter<IItem>)Parameters["RightSide"]; }
    }
    private ValueParameter<ComparisonData> ComparisonParameter {
      get { return (ValueParameter<ComparisonData>)Parameters["Comparison"]; }
    }
    public LookupParameter<BoolData> ResultParameter {
      get { return (LookupParameter<BoolData>)Parameters["Result"]; }
    }
    public ComparisonData Comparison {
      get { return ComparisonParameter.Value; }
      set { ComparisonParameter.Value = value; }
    }

    public Comparator()
      : base() {
      Parameters.Add(new LookupParameter<IItem>("LeftSide", "The left side of the comparison."));
      Parameters.Add(new ValueLookupParameter<IItem>("RightSide", "The right side of the comparison."));
      Parameters.Add(new ValueParameter<ComparisonData>("Comparison", "The type of comparison.", new ComparisonData()));
      Parameters.Add(new LookupParameter<BoolData>("Result", "The result of the comparison."));
    }

    public override IExecutionSequence Apply() {
      IItem left = LeftSideParameter.ActualValue;
      IItem right = RightSideParameter.ActualValue;
      IComparable comparable = left as IComparable;
      if (comparable == null) throw new InvalidOperationException();

      int i = comparable.CompareTo(right);
      bool b = false;
      switch (Comparison.Value) {
        case HeuristicLab.Data.Comparison.Less:
          b = i < 0; break;
        case HeuristicLab.Data.Comparison.LessOrEqual:
          b = i <= 0; break;
        case HeuristicLab.Data.Comparison.Equal:
          b = i == 0; break;
        case HeuristicLab.Data.Comparison.GreaterOrEqual:
          b = i > 0; break;
        case HeuristicLab.Data.Comparison.Greater:
          b = i >= 0; break;
        case HeuristicLab.Data.Comparison.NotEqual:
          b = i != 0; break;
      }
      ResultParameter.ActualValue = new BoolData(b);
      return base.Apply();
    }
  }
}
