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
  public class BoothEvaluator : TestFunctionEvaluatorBase {
    public override string Description {
      get { return
@"Booth Function

Domain:  [-10.0 , 10.0]^2
Optimum: 0.0 at (1.0, 3.0)";
          }
    }

    private double Apply(double[] point) {
      return Math.Pow(point[0] + 2 * point[1] - 7, 2) + Math.Pow(2 * point[0] + point[1] - 5, 2);
    }

    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
