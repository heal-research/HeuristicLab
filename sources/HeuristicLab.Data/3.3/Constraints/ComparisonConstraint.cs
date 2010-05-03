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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [StorableClass]
  [Item("ComparisonConstraint", "A constraint which checks for specified compare operation.")]
  public class ComparisonConstraint : Constraint {
    /// <summary>
    /// Protected default constructor for constructor chaining and cloning.
    /// </summary>
    [StorableConstructor]
    protected ComparisonConstraint()
      : base() {
    }

    public ComparisonConstraint(IItem constrainedValue, ComparisonOperation comparisonOperation, object comparisonValue)
      : base() {
      this.ConstrainedValue = constrainedValue;
      this.ComparisonOperation = comparisonOperation;
      this.ComparisonValue = comparisonValue;
    }

    public override IEnumerable<ComparisonOperation> AllowedComparisonOperations {
      get { return new List<ComparisonOperation>() { ComparisonOperation.Equal, ComparisonOperation.NotEqual, ComparisonOperation.Lesser, ComparisonOperation.LesserOrEqual, ComparisonOperation.Greater, ComparisonOperation.GreaterOrEqual }; }
    }

    protected override bool Check(object constrainedMember) {
      IComparable comparableMember = constrainedMember as IComparable;
      if (comparableMember == null)
        throw new InvalidOperationException("Constrained member must be of type IComparable to be used with ComparisonConstraint.");

      int compareResult = comparableMember.CompareTo(this.ComparisonValue);
      bool result = false;
      if (this.ComparisonOperation == ComparisonOperation.Lesser)
        result = compareResult < 0;
      else if (this.ComparisonOperation == ComparisonOperation.LesserOrEqual)
        result = compareResult <= 0;
      else if (this.ComparisonOperation == ComparisonOperation.Equal)
        result = compareResult == 0;
      else if (this.ComparisonOperation == ComparisonOperation.GreaterOrEqual)
        result = compareResult >= 0;
      else if (this.ComparisonOperation == ComparisonOperation.Greater)
        result = compareResult > 0;
      else if (this.ComparisonOperation == ComparisonOperation.NotEqual)
        result = compareResult != 0;
      else
        throw new InvalidOperationException("CompareOperation " + this.ComparisonOperation + " is not defined for ComparisonConstraint.");

      return result;
    }
  }
}
