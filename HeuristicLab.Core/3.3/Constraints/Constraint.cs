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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("02F8AFE2-BF67-4378-9E38-B18FF4D63609")]
  public abstract class Constraint : Item, IConstraint {
    [StorableConstructor]
    protected Constraint(StorableConstructorFlag _) : base(_) { }
    protected Constraint(Constraint original, Cloner cloner)
      : base(original, cloner) {
      constrainedValue = null;  //mkommend: intentionally set to null;

      IDeepCloneable constraintDataDeepCloneable = original.constraintData as IDeepCloneable;
      ICloneable constraintDataCloneable = original.constraintData as ICloneable;
      if (constraintDataDeepCloneable != null)
        constraintData = cloner.Clone(constraintDataDeepCloneable);
      else if (constraintDataCloneable != null)
        constraintData = constraintDataCloneable.Clone();
      else
        constraintData = original.constraintData;

      constraintOperation = original.constraintOperation;
    }
    protected Constraint() {
      this.Active = false;
      if (AllowedConstraintOperations != null && AllowedConstraintOperations.Any())
        this.ConstraintOperation = AllowedConstraintOperations.ElementAt(0);
    }
    protected Constraint(IItem constrainedValue, ConstraintOperation constraintOperation, object constraintData)
      : this() {
      this.ConstrainedValue = constrainedValue;
      this.ConstraintOperation = constraintOperation;
      this.ConstraintData = constraintData;
    }
    protected Constraint(IItem constrainedValue, ConstraintOperation constraintOperation, object constraintData, bool active) {
      this.ConstrainedValue = constrainedValue;
      this.ConstraintOperation = constraintOperation;
      this.ConstraintData = constraintData;
      this.Active = active;
    }

    [Storable]
    private bool active;
    public bool Active {
      get { return this.active; }
      set {
        if (this.active != value) {
          this.active = value;
          this.OnActiveChanged();
        }
      }
    }

    [Storable]
    private IItem constrainedValue;
    public IItem ConstrainedValue {
      get { return this.constrainedValue; }
      set {
        if (value == null)
          throw new ArgumentNullException("Constraint value cannot be null.");
        if (this.constrainedValue != value) {
          this.constrainedValue = value;
          this.OnConstrainedValueChanged();
          this.OnToStringChanged();
        }
      }
    }

    [Storable]
    private object constraintData;
    public object ConstraintData {
      get { return this.constraintData; }
      set {
        if (this.constraintData != value) {
          this.constraintData = value;
          this.OnConstraintDataChanged();
          this.OnToStringChanged();
        }
      }
    }

    public abstract IEnumerable<ConstraintOperation> AllowedConstraintOperations { get; }
    [Storable]
    private ConstraintOperation constraintOperation;
    public ConstraintOperation ConstraintOperation {
      get { return this.constraintOperation; }
      set {
        if (value == null)
          throw new ArgumentNullException("Comparison operation cannot be null.");
        if (!AllowedConstraintOperations.Contains(value))
          throw new ArgumentException("Comparison operation is not contained in the allowed ComparisonOperations.");
        if (this.constraintOperation != value) {
          this.constraintOperation = value;
          this.OnConstraintOperationChanged();
          this.OnToStringChanged();
        }
      }
    }

    /// <summary>
    /// This method is called to determine which member of the constrained value should be compared.
    /// </summary>
    /// <returns></returns>
    protected virtual IItem GetConstrainedMember() {
      return this.constrainedValue;
    }
    protected abstract bool Check(object constrainedMember);
    protected abstract bool Check(object constrainedMember, out string errorMessage);

    public bool Check() {
      if (!Active)
        return true;

      IItem constrainedMember = this.GetConstrainedMember();
      return this.Check(constrainedMember);
    }
    public bool Check(out string errorMessage) {
      errorMessage = string.Empty;
      if (!Active)
        return true;

      IItem constrainedMember = this.GetConstrainedMember();
      return this.Check(constrainedMember, out errorMessage);
    }

    #region events
    public event EventHandler ActiveChanged;
    protected virtual void OnActiveChanged() {
      EventHandler handler = ActiveChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ConstrainedValueChanged;
    protected virtual void OnConstrainedValueChanged() {
      EventHandler handler = ConstrainedValueChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ConstraintDataChanged;
    protected virtual void OnConstraintDataChanged() {
      EventHandler handler = ConstraintDataChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ConstraintOperationChanged;
    protected virtual void OnConstraintOperationChanged() {
      EventHandler handler = ConstraintOperationChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region overriden item methods
    public override string ToString() {
      IItem constrainedMember = GetConstrainedMember();
      string s = string.Empty;
      if (constrainedMember != null)
        s += constrainedMember.ToString() + " ";

      if (constraintOperation != null)
        s += ConstraintOperation.ToString() + " ";

      if (constraintData != null)
        s += constraintData.ToString();
      else
        s += "null";

      s += ".";
      return s;
    }
    #endregion
  }
}
