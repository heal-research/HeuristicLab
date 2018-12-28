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

using System;
using System.Drawing;
using HEAL.Fossil;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.DataPreprocessing.Filter {
  [Item("ComparisonFilter", "A filter which compares the member of the preprocessing data with the constraint data.")]
  public class ComparisonFilter : ComparisonConstraint, IFilter {
    public override string ItemName {
      get { return "ComparisonFilter"; }
    }

    public override Image ItemImage {
      get { return VSImageLibrary.Filter; }
    }

    public new IPreprocessingData ConstrainedValue {
      get { return (IPreprocessingData)base.ConstrainedValue; }
      set { base.ConstrainedValue = value; }
    }

    public new IStringConvertibleValue ConstraintData {
      get { return (IStringConvertibleValue)base.ConstraintData; }
      set {
        if (!(value is IComparable))
          throw new ArgumentException("Only IComparables allowed for ConstraintData");
        base.ConstraintData = value;
      }
    }

    public ComparisonFilter() : base() { }
    [StorableConstructor]
    protected ComparisonFilter(StorableConstructorFlag _) : base(_) { }

    public ComparisonFilter(IPreprocessingData constrainedValue, ConstraintOperation constraintOperation, object constraintData)
      : base(constrainedValue, constraintOperation, constraintData) {
    }

    public ComparisonFilter(IPreprocessingData constrainedValue, ConstraintOperation constraintOperation, object constraintData, bool active)
      : base(constrainedValue, constraintOperation, constraintData, active) {
    }

    protected ComparisonFilter(ComparisonFilter original, Cloner cloner)
      : base(original, cloner) {
      constraintColumn = original.constraintColumn;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ComparisonFilter(this, cloner);
    }

    private int constraintColumn;
    public int ConstraintColumn {
      get { return constraintColumn; }
      set {
        if (ConstrainedValue.Columns < value)
          throw new ArgumentException("Could not set ConstraintData to not existing column index.");

        if (constraintColumn != value) {
          constraintColumn = value;
          this.OnConstraintColumnChanged();
          this.OnToStringChanged();
        }
      }
    }

    // return remaining rows
    public new bool[] Check() {
      bool[] result = new bool[ConstrainedValue.Rows];

      if (!Active)
        return result;

      for (int row = 0; row < ConstrainedValue.Rows; ++row) {
        object item = null;
        if (ConstrainedValue.VariableHasType<double>(constraintColumn)) {
          item = new HeuristicLab.Data.DoubleValue(ConstrainedValue.GetCell<double>(ConstraintColumn, row));
        } else if (ConstrainedValue.VariableHasType<DateTime>(constraintColumn)) {
          item = new HeuristicLab.Data.DateTimeValue(ConstrainedValue.GetCell<DateTime>(ConstraintColumn, row));
        } else {
          item = new StringValue(ConstrainedValue.GetCell<string>(ConstraintColumn, row));
        }

        result[row] = base.Check(item);
      }

      return result;
    }

    public new bool[] Check(out string errorMessage) {
      errorMessage = string.Empty;
      return this.Check();
    }

    public override string ToString() {
      string s = string.Empty;
      if (ConstrainedValue != null)
        s += ConstrainedValue.GetVariableName(ConstraintColumn) + " ";

      if (ConstraintOperation != null)
        s += ConstraintOperation.ToString() + " ";

      if (ConstraintData != null)
        s += ConstraintData.ToString();
      else
        s += "null";

      s += ".";
      return s;
    }

    public event EventHandler ConstraintColumnChanged;
    protected virtual void OnConstraintColumnChanged() {
      EventHandler handler = ConstraintColumnChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
  }
}
