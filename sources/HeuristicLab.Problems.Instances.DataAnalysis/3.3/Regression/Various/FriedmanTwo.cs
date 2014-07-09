#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FriedmanTwo : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Friedman - II"; } }
    public override string Description {
      get {
        return "Paper: Multivariate Adaptive Regression Splines" + Environment.NewLine
        + "Authors: Jerome H. Friedman";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X1", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "X9", "X10", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X1", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "X9", "X10" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 5000; } }
    protected override int TestPartitionStart { get { return 5000; } }
    protected override int TestPartitionEnd { get { return 10000; } }

    protected static FastRandom rand = new FastRandom();

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(10000, 0, 1).ToList());
      }

      double x1, x2, x3, x4, x5;
      double f;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x1 = data[0][i];
        x2 = data[1][i];
        x3 = data[2][i];
        x4 = data[3][i];
        x5 = data[4][i];

        f = 10 * Math.Sin(Math.PI * x1 * x2) + 20 * Math.Pow(x3 - 0.5, 2) + 10 * x4 + 5 * x5;

        results.Add(f + NormalDistributedRandom.NextDouble(rand, 0, 1));
      }
      data.Add(results);

      return data;
    }
  }
}
