#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which compares two items.
  /// </summary>
  [Item("Comparator", "An operator which compares two items.")]
  [StorableType("21C6129E-1D5C-43BB-8E6D-1F1DD5C2E995")]
  public sealed class Comparator : SingleSuccessorOperator {
    public LookupParameter<IItem> LeftSideParameter {
      get { return (LookupParameter<IItem>)Parameters["LeftSide"]; }
    }
    public ValueLookupParameter<IItem> RightSideParameter {
      get { return (ValueLookupParameter<IItem>)Parameters["RightSide"]; }
    }
    private ValueParameter<Comparison> ComparisonParameter {
      get { return (ValueParameter<Comparison>)Parameters["Comparison"]; }
    }
    public LookupParameter<BoolValue> ResultParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Result"]; }
    }
    public Comparison Comparison {
      get { return ComparisonParameter.Value; }
      set { ComparisonParameter.Value = value; }
    }

    [StorableConstructor]
    private Comparator(StorableConstructorFlag _) : base(_) { }
    private Comparator(Comparator original, Cloner cloner)
      : base(original, cloner) {
    }
    public Comparator()
      : base() {
      Parameters.Add(new LookupParameter<IItem>("LeftSide", "The left side of the comparison."));
      Parameters.Add(new ValueLookupParameter<IItem>("RightSide", "The right side of the comparison."));
      Parameters.Add(new ValueParameter<Comparison>("Comparison", "The type of comparison.", new Comparison(Data.ComparisonType.Equal)));
      Parameters.Add(new LookupParameter<BoolValue>("Result", "The result of the comparison."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Comparator(this, cloner);
    }

    public override IOperation Apply() {
      IItem left = LeftSideParameter.ActualValue;
      IItem right = RightSideParameter.ActualValue;
      IComparable comparable = left as IComparable;
      if (comparable == null) throw new InvalidOperationException();

      int i = comparable.CompareTo(right);
      bool b = false;
      switch (Comparison.Value) {
        case HeuristicLab.Data.ComparisonType.Less:
          b = i < 0; break;
        case HeuristicLab.Data.ComparisonType.LessOrEqual:
          b = i <= 0; break;
        case HeuristicLab.Data.ComparisonType.Equal:
          b = i == 0; break;
        case HeuristicLab.Data.ComparisonType.GreaterOrEqual:
          b = i >= 0; break;
        case HeuristicLab.Data.ComparisonType.Greater:
          b = i > 0; break;
        case HeuristicLab.Data.ComparisonType.NotEqual:
          b = i != 0; break;
      }
      ResultParameter.ActualValue = new BoolValue(b);
      return base.Apply();
    }
  }
}
