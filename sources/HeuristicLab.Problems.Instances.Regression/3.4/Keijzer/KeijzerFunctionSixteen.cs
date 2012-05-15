#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.Regression {
  public class KeijzerFunctionSixteen : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Keijzer 16 f(x) = x^3 / 5 + y^3 / 2 - y - x"; } }
    public override string Description {
      get {
        return "Paper: Improving Symbolic Regression with Interval Arithmetic and Linear Scaling" + Environment.NewLine
        + "Authors: Maarten Keijzer" + Environment.NewLine
        + "Function: f(x, y) = x^3 / 5 + y^3 / 2 - y - x" + Environment.NewLine
        + "range(train): 20 Training cases x,y = rnd(-3, 3)" + Environment.NewLine
        + "range(test): x,y = [-3:0.01:3]" + Environment.NewLine
        + "Function Set: x + y, x * y, 1/x, -x, sqrt(x)" + Environment.NewLine + Environment.NewLine
        + "Note: Test partition has been adjusted to only 100 random uniformly distributed test cases in the intercal [-3, 3] (not ca. 360000 as described) "
        + ", but 5000 cases are created";
      }
    }
    protected override string TargetVariable { get { return "F"; } }
    protected override string[] InputVariables { get { return new string[] { "X", "Y", "F" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X", "Y" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 20; } }
    protected override int TestPartitionStart { get { return 2500; } }
    protected override int TestPartitionEnd { get { return 5000; } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(5000, -3, 3).ToList());
      }

      double x, y;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        y = data[1][i];
        results.Add(Math.Pow(x, 3) / 5 + Math.Pow(y, 3) / 2 - y - x);
      }
      data.Add(results);

      return data;
    }
  }
}
