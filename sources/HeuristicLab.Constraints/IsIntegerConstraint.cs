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
  public class IsIntegerConstraint : ConstraintBase{
    public override string Description {
      get { return "Allows only integer values."; }
    }

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

    public override void Accept(IConstraintVisitor visitor) {
      visitor.Visit(this);
    }

  }
}
