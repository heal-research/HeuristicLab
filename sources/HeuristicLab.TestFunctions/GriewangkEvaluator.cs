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
using HeuristicLab.Operators;

namespace HeuristicLab.TestFunctions {
  public class GriewangkEvaluator : TestFunctionEvaluatorBase {
    public override string Description {
      get { return
@"Griewangk Function

Domain:  [-600.0 , 600.0]^n
Optimum: 0.0 at (0, 0, ..., 0)";
          }
    }

    public static double Apply(double[] point) {
      double result = 0;
      double val = 0;

      for (int i = 0; i < point.Length; i++)
        result += point[i] * point[i];
      result = result / 4000;

      val = Math.Cos(point[0]);
      for (int i = 1; i < point.Length; i++)
        val *= Math.Cos(point[i] / Math.Sqrt(i + 1));

      result = result - val + 1;
      return (result);
    }

    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
