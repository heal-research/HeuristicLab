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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("EqualityConstraint", "A constraint which checks for equality.")]
  public class EqualityConstraint : Constraint {
    [StorableConstructor]
    protected EqualityConstraint(bool deserializing) : base(deserializing) { }
    protected EqualityConstraint(EqualityConstraint original, Cloner cloner) : base(original, cloner) { }
    public EqualityConstraint() : base() { }
    public EqualityConstraint(IItem constrainedValue, ConstraintOperation constraintOperation, object constraintData)
      : base(constrainedValue, constraintOperation, constraintData) {
    }
    public EqualityConstraint(IItem constrainedValue, ConstraintOperation constraintOperation, object constraintData, bool active)
      : base(constrainedValue, constraintOperation, constraintData, active) {
    }

    public override IEnumerable<ConstraintOperation> AllowedConstraintOperations {
      get { return new ConstraintOperation[2] { ConstraintOperation.Equal, ConstraintOperation.NotEqual }; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EqualityConstraint(this, cloner);
    }

    protected override bool Check(object constrainedMember) {
      if (constrainedMember == null)
        return false;

      IComparable comparableMember = constrainedMember as IComparable;
      bool compareValue;
      if (comparableMember != null && this.ConstraintData != null)
        compareValue = comparableMember.CompareTo(this.ConstraintData) == 0;
      else
        compareValue = constrainedMember.Equals(this.ConstraintData);

      bool result;
      if (ConstraintOperation == ConstraintOperation.Equal)
        result = compareValue;
      else if (ConstraintOperation == ConstraintOperation.NotEqual)
        result = !compareValue;
      else
        throw new InvalidOperationException("Constraint operation " + this.ConstraintOperation + " is not defined for TypeConstraint.");

      return result;
    }

    protected override bool Check(object constrainedMember, out string errorMessage) {
      bool result = Check(constrainedMember);
      errorMessage = string.Empty;
      if (!result)
        errorMessage = constrainedMember.ToString() + " must be " + ConstraintOperation.ToString() + " to " + ConstraintData.ToString() + ".";
      return result;
    }
  }
}
