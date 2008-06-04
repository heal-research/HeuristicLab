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
  public class ZakharovEvaluator : TestFunctionEvaluatorBase {
    public override string Description {
      get { return
@"Zakharov Function

Domain:  [-5.0 , 10.0]^n
Optimum: 0.0 at (0.0, 0.0, ..., 0.0)";
          }
    }

    public static double Apply(double[] point) {
      int length = point.Length;
      double s1 = 0;
      double s2 = 0;

      for (int i = 0; i < length; i++) {
        s1 = s1 + point[i] * point[i];
        s2 = s2 + 0.5 * i * point[i];
      }
      return s1 + s2 * s2 + s2 * s2 * s2 * s2;
    }

    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
