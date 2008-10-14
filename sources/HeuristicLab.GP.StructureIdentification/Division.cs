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

namespace HeuristicLab.GP.StructureIdentification {
  public sealed class Division : FunctionBase {
    private const double EPSILON = 10.0E-20; // if any divisor is < EPSILON return 0

    public override string Description {
      get {
        return @"Protected division
Divides the result of the first sub-tree by the results of the following sub-tree.
In case one of the divisors is 0 returns 0.
    (/ 3) => 1/3
    (/ 2 3) => 2/3
    (/ 3 4 5) => 3/20
    (/ 2 0 4) => 0
";
      }
    }

    public Division()
      : base() {
      // 2 - 3 seems like an reasonable defaut (used for +,-,*,/) (discussion with swinkler and maffenze)
      AddConstraint(new NumberOfSubOperatorsConstraint(2, 3));
      AddConstraint(new SubOperatorTypeConstraint(0));
      AddConstraint(new SubOperatorTypeConstraint(1));
      AddConstraint(new SubOperatorTypeConstraint(2));
    }
  }
}
