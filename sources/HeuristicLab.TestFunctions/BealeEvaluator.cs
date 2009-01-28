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

namespace HeuristicLab.TestFunctions {   
  public class BealeEvaluator : TestFunctionEvaluatorBase {
    public override string Description {
      get { return
@"Beale Function

Domain:  [-4.5 , 4.5]^2
Optimum: 0.0 at (3.0, 0.5)";
          }
    }

    public static double Apply(double[] point) {
      return Math.Pow(1.5 - point[0] * (1 - point[1]), 2) + Math.Pow(2.25 - point[0] * (1 - (point[1] * point[1])), 2) + Math.Pow((2.625 - point[0] * (1 - (point[1] * point[1] * point[1]))), 2);
    }

    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
