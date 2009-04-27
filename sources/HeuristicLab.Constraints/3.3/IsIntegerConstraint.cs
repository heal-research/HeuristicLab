#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint that allows only integer values.
  /// </summary>
  public class IsIntegerConstraint : ConstraintBase{
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Allows only integer values."; }
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem item) {
      // ConstrainedIntData is always integer => just return true
      if(item is ConstrainedIntData)
        return true;

      // if we have an item of ConstrainedDoubleData then we check if it is integer or not
      if(item is ConstrainedDoubleData) {
        ConstrainedDoubleData d = (ConstrainedDoubleData)item;
        if(d.Data == Math.Truncate(d.Data)) {
          return true;
        } else {
          return false;
        }
      }

      // assume that all other data types are never integer
      return false;
    }
  }
}
