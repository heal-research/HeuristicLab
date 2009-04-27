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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Operator to check whether one item is equal or greater than another.
  /// </summary>
  public class GreaterOrEqualThanComparator : ComparatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }
    /// <summary>
    /// Initializes a new instance of <see cref="GreaterOrEqualThanComparator"/>.
    /// </summary>
    public GreaterOrEqualThanComparator()
      : base() {
    }

    /// <summary>
    /// Checks whether the <paramref name="left"/> item is greater or equal to the <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns><c>true</c> if the <paramref name="left"/> item is greater or equal to the 
    /// <paramref name="right"/>, <c>false</c> otherwise.</returns>
    protected override bool Compare(IComparable left, IItem right) {
      return left.CompareTo(right) >= 0;
    }
  }
}
