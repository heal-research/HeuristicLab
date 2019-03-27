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
using HEAL.Attic;

namespace HeuristicLab.Core {
  [StorableType("0BE7E71A-DA13-440F-83C3-0AD15C9E519D")]
  [Item("TypeCompatibilityConstraint", "A constraint that checks for compatible types.")]
  public class TypeCompatibilityConstraint : Constraint {
    [StorableConstructor]
    protected TypeCompatibilityConstraint(StorableConstructorFlag _) : base(_) { }
    protected TypeCompatibilityConstraint(TypeCompatibilityConstraint original, Cloner cloner) : base(original, cloner) { }
    public TypeCompatibilityConstraint() : base() { }
    public TypeCompatibilityConstraint(IItem constrainedValue, ConstraintOperation constraintOperation, Type constraintData)
      : base(constrainedValue, constraintOperation, constraintData) {
    }
    public TypeCompatibilityConstraint(IItem constrainedValue, ConstraintOperation constraintOperation, Type constraintData, bool active)
      : base(constrainedValue, constraintOperation, constraintData, active) {
    }

    public new Type ConstraintData {
      get { return (Type)base.ConstraintData; }
      set { base.ConstraintData = value; }
    }

    public override IEnumerable<ConstraintOperation> AllowedConstraintOperations {
      get { return new ConstraintOperation[2] { ConstraintOperation.IsTypeCompatible, ConstraintOperation.IsTypeNotCompatible }; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TypeCompatibilityConstraint(this, cloner);
    }

    protected override bool Check(object constrainedMember) {
      if (constrainedMember == null)
        return false;

      Type constrainedMemberType = constrainedMember.GetType();
      bool compareValue = ConstraintData.IsAssignableFrom(constrainedMemberType);
      bool result;
      if (ConstraintOperation == ConstraintOperation.IsTypeCompatible)
        result = compareValue;
      else if (ConstraintOperation == ConstraintOperation.IsTypeNotCompatible)
        result = !compareValue;
      else
        throw new InvalidOperationException("Constraint operation " + this.ConstraintOperation + " is not defined for TypeCompatibilityConstraint.");

      return result;
    }

    protected override bool Check(object constrainedMember, out string errorMessage) {
      bool result = Check(constrainedMember);
      errorMessage = string.Empty;
      if (!result)
        errorMessage = "The type of " + constrainedMember.ToString() + " must be " + ConstraintOperation.ToString() + " compatible to " + ConstraintData.ToString() + ".";
      return result;
    }
  }
}
