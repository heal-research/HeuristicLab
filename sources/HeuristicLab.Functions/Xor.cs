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
using System.Linq;
using System.Text;
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public class Xor : FunctionBase {
    public override string Description {
      get {
        return @"Logical XOR operation. Only defined for sub-tree-results 0.0 and 1.0.";
      }
    }

    public Xor()
      : base() {
      AddConstraint(new NumberOfSubOperatorsConstraint(2, 2));
    }

    public override double Apply(Dataset dataset, int sampleIndex, double[] args) {
      if(args[0] == 0.0 && args[1] == 0.0) return 0.0;
      if(args[0] * args[1] == 0.0) return 1.0;
      return 0.0;
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
