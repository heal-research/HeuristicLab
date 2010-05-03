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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  [StorableClass]
  [Item("TypeConstraint", "A constraint which checks the types for specified compare operation.")]
  public class TypeConstraint : Constraint {
    /// <summary>
    /// Protected default constructor for constructor chaining and cloning.
    /// </summary>
    [StorableConstructor]
    protected TypeConstraint()
      : base() {
    }

    public TypeConstraint(IItem constrainedValue, ComparisonOperation comparisonOperation, Type comparisonValue)
      : base() {
      this.ConstrainedValue = constrainedValue;
      this.ComparisonOperation = comparisonOperation;
      this.ComparisonValue = comparisonValue;
    }

    public new Type ComparisonValue {
      get { return (Type)base.ComparisonValue; }
      set { base.ComparisonValue = value; }
    }

    public override IEnumerable<ComparisonOperation> AllowedComparisonOperations {
      get { return new List<ComparisonOperation>() { ComparisonOperation.IsTypeOf, ComparisonOperation.IsNotTypeOf }; }
    }

    protected override bool Check(object constrainedMember) {
      Type constrainedMemberType = constrainedMember.GetType();

      if (ComparisonValue == null)
        throw new InvalidOperationException("TypeConstraint can only be applied on members of type ICompareable.");

      bool compareValue = ComparisonValue.IsAssignableFrom(constrainedMemberType);
      bool result;
      if (ComparisonOperation == ComparisonOperation.IsTypeOf)
        result = compareValue;
      else if (ComparisonOperation == ComparisonOperation.IsNotTypeOf)
        result = !compareValue;
      else
        throw new InvalidOperationException("CompareOperation " + this.ComparisonOperation + " is not defined for TypeConstraint.");

      return result;
    }
  }
}
