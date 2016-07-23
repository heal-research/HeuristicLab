#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class KeijzerFunctionSix : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Keijzer 6 f(x) = Sum(1 / i) From 1 to X"; } }
    public override string Description {
      get {
        return "Paper: Improving Symbolic Regression with Interval Arithmetic and Linear Scaling" + Environment.NewLine
        + "Authors: Maarten Keijzer" + Environment.NewLine
        + "Function: f(x) = Sum(1 / i) From 1 to X" + Environment.NewLine
        + "range(train): x = [1:1:50]" + Environment.NewLine
        + "range(test): x = [1:1:120]" + Environment.NewLine
        + "Function Set: x + y, x * y, 1/x, -x, sqrt(x)";
      }
    }
    protected override string TargetVariable { get { return "F"; } }
    protected override string[] VariableNames { get { return new string[] { "X", "F" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 50; } }
    protected override int TestPartitionStart { get { return 50; } }
    protected override int TestPartitionEnd { get { return 170; } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      data.Add(SequenceGenerator.GenerateSteps(1m, 50, 1).Select(v => (double)v).ToList());
      data[0].AddRange(SequenceGenerator.GenerateSteps(1m, 120, 1).Select(v => (double)v));

      double x;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        results.Add(Enumerable.Range(1, (int)x).Sum(j => 1.0 / j));
      }
      data.Add(results);

      return data;
    }
  }
}
