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
  public class AckleyEvaluator : TestFunctionEvaluatorBase {
    public override string Description {
      get { return
@"Ackley Function

Domain:  [-32.768 , 32.768]^n
Optimum: 0.0 at (0, 0, ..., 0)";
          }
    }

    public static double Apply(double[] point) {
      double result = 20 + Math.E;
      double val;

      val = 0;
      for (int i = 0; i < point.Length; i++)
        val += point[i] * point[i];
      val *= 1.0 / point.Length;
      val = Math.Sqrt(val);
      val *= -0.2;
      result -= 20 * Math.Exp(val);

      val = 0;
      for (int i = 0; i < point.Length; i++)
        val += Math.Cos(2 * Math.PI * point[i]);
      val *= 1.0 / point.Length;
      result -= Math.Exp(val);
      return (result);
    }

    protected override double EvaluateFunction(double[] point) {
      return Apply(point);
    }
  }
}
