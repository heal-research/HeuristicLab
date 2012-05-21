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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class KeijzerFunctionFive : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Keijzer 5 f(x) = x ^ 3  * exp(-x) * cos(x) * sin(x) * (sin(x) ^ 2 * cos(x) - 1)"; } }
    public override string Description {
      get {
        return "Paper: Improving Symbolic Regression with Interval Arithmetic and Linear Scaling" + Environment.NewLine
        + "Authors: Maarten Keijzer" + Environment.NewLine
        + "Function: f(x) = x ^ 3  * exp(-x) * cos(x) * sin(x) * (sin(x) ^ 2 * cos(x) - 1)" + Environment.NewLine
        + "range(train): x = [0:0.05:10]" + Environment.NewLine
        + "range(test): x = [0.05:0.05:10.05]" + Environment.NewLine
        + "Function Set: x + y, x * y, 1/x, -x, sqrt(x)" + Environment.NewLine
        + "Note: {exp, log, sin, cos}, 100000 evals";
      }
    }
    protected override string TargetVariable { get { return "F"; } }
    protected override string[] InputVariables { get { return new string[] { "X", "F" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 201; } }
    protected override int TestPartitionStart { get { return 201; } }
    protected override int TestPartitionEnd { get { return 402; } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      data.Add(ValueGenerator.GenerateSteps(0, 10, 0.05).ToList());
      data[0].AddRange(ValueGenerator.GenerateSteps(0.05, 10.05, 0.05));

      double x;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        results.Add(Math.Pow(x, 3) * Math.Exp(-x) * Math.Cos(x) * Math.Sin(x) * (Math.Pow(Math.Sin(x), 2) * Math.Cos(x) - 1));
      }
      data.Add(results);

      return data;
    }
  }
}
