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
using System.Diagnostics;
using HeuristicLab.Constraints;
using System.Linq;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public sealed class Substraction : FunctionBase {
    public override string Description {
      get {
        return @"Substracts the results of sub-tree 2..n from the result of the first sub-tree.
    (- 3) => -3
    (- 2 3) => -1
    (- 3 4 5) => -6";
      }
    }

    public Substraction()
      : base() {
      // 2 - 3 seems like an reasonable defaut (used for +,-,*,/) (discussion with swinkler and maffenze)
      AddConstraint(new NumberOfSubOperatorsConstraint(2, 3));
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
