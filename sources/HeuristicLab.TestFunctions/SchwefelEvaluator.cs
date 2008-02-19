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
  public class SchwefelEvaluator : TestFunctionEvaluatorBase {
    public override string Description {
      get { return
@"Schwefel Function (Sine Root)

Domain:  [-500.0 , 500.0]^n
Optimum: 0.0 at (420.968746453712, 420.968746453712, ..., 420.968746453712)";
          }
    }

    protected override double EvaluateFunction(double[] point) {
      double result = 418.982887272433 * point.Length;
      for (int i = 0; i < point.Length; i++)
        result -= point[i] * Math.Sin(Math.Sqrt(Math.Abs(point[i])));
      return (result);
    }
  }
}
