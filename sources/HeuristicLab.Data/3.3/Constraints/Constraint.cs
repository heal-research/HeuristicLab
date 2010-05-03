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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [StorableClass]
  public abstract class Constraint : Item, IConstraint {
    /// <summary>
    /// Protected default constructor for constructor chaining, cloning and persisting of constraints.
    /// </summary>
    [StorableConstructor]
    protected Constraint() {
      this.Active = true;
    }

    public Constraint(IItem constrainedValue,ComparisonOperation comparisonOperation, object comparisonValue)
      : this() {
      this.ConstrainedValue = constrainedValue;
      this.ComparisonOperation = comparisonOperation;
      this.ComparisonValue = comparisonValue;
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
      protected set {
        if (value == null)
          throw new ArgumentNullException("Constraint value cannot be null.");
        if (this.constrainedValue != value) {
          this.constrainedValue = value;
          this.OnToStringChanged();
        }
      }
    }

    [Storable]
    private object comparisonValue;
    public object ComparisonValue {
      get { return this.comparisonValue; }
      set {
        if (this.comparisonValue != value) {
          this.comparisonValue = value;
          this.OnComparisonValueChanged();
          this.OnToStringChanged();
        }
      }
    }

    public abstract IEnumerable<ComparisonOperation> AllowedComparisonOperations { get; }
    [Storable]
    private ComparisonOperation comparisonOperation;
    public ComparisonOperation ComparisonOperation {
      get { return this.comparisonOperation; }
      set {
        if (value == null)
          throw new ArgumentNullException("Comparison operation cannot be null.");
        if (!AllowedComparisonOperations.Contains(value))
          throw new ArgumentException("Comparison operation is not contained in the allowed ComparisonOperations.");
        if (this.comparisonOperation != value) {
          this.comparisonOperation = value;
          this.OnComparisonOperationChanged();
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
    public bool Check() {
      if (!Active)
        return true;

      IItem constrainedMember = this.GetConstrainedMember();
      return this.Check(constrainedMember);
    }
    protected abstract bool Check(object constrainedMember);

    #region events
    public event EventHandler ActiveChanged;
    protected virtual void OnActiveChanged() {
      EventHandler handler = ActiveChanged;
      if (handler != null)
        ActiveChanged(this, EventArgs.Empty);
    }

    public event EventHandler ComparisonValueChanged;
    protected virtual void OnComparisonValueChanged() {
      EventHandler handler = ComparisonValueChanged;
      if (handler != null)
        ActiveChanged(this, EventArgs.Empty);
    }

    public event EventHandler ComparisonOperationChanged;
    protected virtual void OnComparisonOperationChanged() {
      EventHandler handler = ComparisonOperationChanged;
      if (handler != null)
        ActiveChanged(this, EventArgs.Empty);
    }
    #endregion

    #region overriden item methods
    public override string ToString() {
      IItem constrainedValue = GetConstrainedMember();
      string s = string.Empty;
      if (constrainedValue != null)
        s += constrainedValue.ToString();
      else
        return "Could not determine constraint value.";

      s += " " + comparisonOperation.ToString() + " ";

      if (comparisonValue != null)
        s += comparisonValue.ToString();
      else
        s += "null";

      s += ".";
      return s;
    }

    public override IDeepCloneable Clone(HeuristicLab.Common.Cloner cloner) {
      Constraint clone = (Constraint)base.Clone(cloner);
      clone.constrainedValue = (IItem)cloner.Clone(this.constrainedValue);

      IItem comparisonItem = this.comparisonValue as IItem;
      if (comparisonItem != null)
        clone.comparisonValue = (IItem)cloner.Clone(comparisonItem);
      else
        clone.comparisonValue = comparisonValue;
      clone.comparisonOperation = this.comparisonOperation;

      return clone;
    }
    #endregion
  }
}
