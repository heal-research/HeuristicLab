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
using HeuristicLab.Common;

namespace HeuristicLab.Data.Constraints {
  [StorableClass]
  [Item("EqualityConstraint", "A constraint which checks for equality")]
  public class EqualityConstraint : Constraint{
    /// <summary>
    /// Protected default constructor for constructor chaining and cloning.
    /// </summary>
    [StorableConstructor]
    protected EqualityConstraint()
      : base() {
    }

    public EqualityConstraint(IItem constrainedValue)
      : base(constrainedValue) {
    }

    public override IEnumerable<ComparisonOperation> AllowedComparisonOperations {
      get { return new List<ComparisonOperation>() { ComparisonOperation.Equal, ComparisonOperation.NotEqual }; }
    }

    protected override bool Check(object constrainedMember) {
      IComparable comparableMember = constrainedMember as IComparable;
      bool compareValue;
      if (comparableMember != null && this.ComparisonValue != null)
        compareValue = comparableMember.CompareTo(this.ComparisonValue) == 0;
      else
        compareValue = constrainedMember == this.ComparisonValue;

      bool result;
      if (ComparisonOperation == ComparisonOperation.Equal)
        result = compareValue;
      else if (ComparisonOperation == ComparisonOperation.NotEqual)
        result = !compareValue;
      else
        throw new InvalidOperationException("CompareOperation " + this.ComparisonOperation + " is not defined for TypeConstraint.");

      return result;
    }
  }
}
