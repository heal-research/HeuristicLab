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
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("6FCBEAF5-62A1-48F9-B1A8-F81D4B15A4AE")]
  [Item("ComparisonConstraint", "A constraint which checks for specified compare operation.")]
  public class ComparisonConstraint : Constraint {
    [StorableConstructor]
    protected ComparisonConstraint(StorableConstructorFlag _) : base(_) { }
    protected ComparisonConstraint(ComparisonConstraint original, Cloner cloner) : base(original, cloner) { }
    public ComparisonConstraint() : base() { }
    public ComparisonConstraint(IItem constrainedValue, ConstraintOperation comparisonOperation, object comparisonValue)
      : base(constrainedValue, comparisonOperation, comparisonValue) {
    }
    public ComparisonConstraint(IItem constrainedValue, ConstraintOperation comparisonOperation, object comparisonValue, bool active)
      : base(constrainedValue, comparisonOperation, comparisonValue, active) {
    }

    public override IEnumerable<ConstraintOperation> AllowedConstraintOperations {
      get { return new ConstraintOperation[6] { ConstraintOperation.Less, ConstraintOperation.LessOrEqual, ConstraintOperation.Equal, ConstraintOperation.GreaterOrEqual, ConstraintOperation.Greater, ConstraintOperation.NotEqual }; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ComparisonConstraint(this, cloner);
    }

    protected override bool Check(object constrainedMember) {
      if (constrainedMember == null)
        return false;

      IComparable comparableMember = constrainedMember as IComparable;
      if (comparableMember == null)
        throw new InvalidOperationException("Constrained member must be of type IComparable to be used with ComparisonConstraint.");

      int compareResult = comparableMember.CompareTo(this.ConstraintData);
      bool result = false;
      if (this.ConstraintOperation == ConstraintOperation.Less)
        result = compareResult < 0;
      else if (this.ConstraintOperation == ConstraintOperation.LessOrEqual)
        result = compareResult <= 0;
      else if (this.ConstraintOperation == ConstraintOperation.Equal)
        result = compareResult == 0;
      else if (this.ConstraintOperation == ConstraintOperation.GreaterOrEqual)
        result = compareResult >= 0;
      else if (this.ConstraintOperation == ConstraintOperation.Greater)
        result = compareResult > 0;
      else if (this.ConstraintOperation == ConstraintOperation.NotEqual)
        result = compareResult != 0;
      else
        throw new InvalidOperationException("Constraint operation " + this.ConstraintOperation + " is not defined for ComparisonConstraint.");

      return result;
    }

    protected override bool Check(object constrainedMember, out string errorMessage) {
      bool result = Check(constrainedMember);
      errorMessage = string.Empty;
      if (!result)
        errorMessage = constrainedMember.ToString() + " must be " + ConstraintOperation.ToString() + " than " + ConstraintData.ToString() + ".";
      return result;
    }
  }
}
