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
  public class LevyEvaluator : TestFunctionEvaluatorBase {
    public override string Description {
      get { return
@"Levy Function

Domain:  [-10.0 , 10.0]^n
Optimum: 0.0 at (1.0, 1.0, ..., 1.0)";
          }
    }

    public static double Apply(double[] point) {
      int length = point.Length;
      double[] z = new double[length];
      double s;

      for (int i = 0; i < length; i++) {
        z[i] = 1 + (point[i] - 1) / 4;
      }

      s = Math.Pow(Math.Sin(Math.PI * z[1]), 2);

      for (int i = 0; i < length - 1; i++) {
        s += Math.Pow(z[i] - 1, 2) * (1 + 10 * Math.Pow(Math.Sin(Math.PI * z[i] + 1), 2));
      }

      return s + Math.Pow(z[length - 1] - 1, 2) * (1 + Math.Pow(Math.Sin(2 * Math.PI * z[length - 1]), 2));
    }

    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
