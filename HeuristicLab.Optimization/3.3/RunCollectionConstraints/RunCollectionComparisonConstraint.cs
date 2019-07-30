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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("655E544A-F75E-4020-BA97-F511CB4D059F")]
  [Item("RunCollectionComparisonConstraint", "A constraint which compares the members of the contained runs with the constraint data.")]
  public class RunCollectionComparisonConstraint : ComparisonConstraint, IRunCollectionColumnConstraint {
    [StorableConstructor]
    protected RunCollectionComparisonConstraint(StorableConstructorFlag _) : base(_) { }

    protected RunCollectionComparisonConstraint(RunCollectionComparisonConstraint original, Cloner cloner)
      : base(original, cloner) {
      constraintColumn = original.constraintColumn;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionComparisonConstraint(this, cloner);
    }

    public RunCollectionComparisonConstraint() : base() { }
    public RunCollectionComparisonConstraint(RunCollection constrainedValue, ConstraintOperation constraintOperation, object constraintData)
      : base(constrainedValue, constraintOperation, constraintData) {
    }
    public RunCollectionComparisonConstraint(RunCollection constrainedValue, ConstraintOperation constraintOperation, object constraintData, bool active)
      : base(constrainedValue, constraintOperation, constraintData, active) {
    }

    public new RunCollection ConstrainedValue {
      get { return (RunCollection)base.ConstrainedValue; }
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

    [Storable]
    private string constraintColumn;
    public string ConstraintColumn {
      get { return constraintColumn; }
      set {
        if (!((IStringConvertibleMatrix)ConstrainedValue).ColumnNames.Contains(value))
          throw new ArgumentException("Could not set ConstraintData to not existing column index.");
        if (constraintColumn != value) {
          constraintColumn = value;
          this.OnConstraintColumnChanged();
          this.OnToStringChanged();
        }
      }
    }

    public event EventHandler ConstraintColumnChanged;
    protected virtual void OnConstraintColumnChanged() {
      EventHandler handler = ConstraintColumnChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    protected override void OnConstrainedValueChanged() {
      base.OnConstrainedValueChanged();
      IStringConvertibleMatrix matrix = (IStringConvertibleMatrix)ConstrainedValue;
      if (constraintColumn == null && ConstrainedValue != null && matrix.Columns != 0)
        constraintColumn = matrix.ColumnNames.ElementAt(0);
    }

    protected override bool Check(object constrainedMember) {
      if (!Active)
        return true;

      foreach (IRun run in ConstrainedValue.Where(r => r.Visible)) {
        IItem item = ConstrainedValue.GetValue(run, constraintColumn);
        if (!base.Check(item))
          run.Visible = false;
      }
      return true;
    }

    protected override bool Check(object constrainedMember, out string errorMessage) {
      errorMessage = string.Empty;
      if (!Active)
        return true;

      foreach (IRun run in ConstrainedValue.Where(r => r.Visible)) {
        IItem item = ConstrainedValue.GetValue(run, constraintColumn);
        if (!base.Check(item))
          run.Visible = false;
      }
      return true;
    }

    public override string ToString() {
      string s = string.Empty;
      IStringConvertibleMatrix matrix = ConstrainedValue;
      if (matrix != null && matrix.ColumnNames.Count() != 0)
        s += constraintColumn + " ";
      else
        return "ComparisonConstraint";

      if (ConstraintOperation != null)
        s += ConstraintOperation.ToString() + " ";

      if (ConstraintData != null)
        s += ConstraintData.GetValue();
      else
        s += "null";

      return s;
    }
  }
}
