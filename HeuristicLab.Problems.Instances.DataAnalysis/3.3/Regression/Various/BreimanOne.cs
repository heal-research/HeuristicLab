#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class BreimanOne : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Breiman - I"; } }
    public override string Description {
      get {
        return "Paper: Classification and Regression Trees" + Environment.NewLine
        + "Authors: Leo Breiman, Jerome H. Friedman, Charles J. Stone and R. A. Olson";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X1", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "X9", "X10", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X1", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "X9", "X10" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 5001; } }
    protected override int TestPartitionStart { get { return 5001; } }
    protected override int TestPartitionEnd { get { return 10001; } }

    public int Seed { get; private set; }

    public BreimanOne() : this((int)DateTime.Now.Ticks) { }
    public BreimanOne(int seed) : base() {
      Seed = seed;
    }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      List<int> values = new List<int>() { -1, 1 };
      var rand = new MersenneTwister((uint)Seed);
      data.Add(GenerateUniformIntegerDistribution(rand, values, TestPartitionEnd));
      values.Add(0);
      for (int i = 0; i < AllowedInputVariables.Count() - 1; i++) {
        data.Add(GenerateUniformIntegerDistribution(rand, values, TestPartitionEnd));
      }
      double x1, x2, x3, x4, x5, x6, x7;
      double f;
      List<double> results = new List<double>();
      double sigma = Math.Sqrt(2);
      for (int i = 0; i < data[0].Count; i++) {
        x1 = data[0][i];
        x2 = data[1][i];
        x3 = data[2][i];
        x4 = data[3][i];
        x5 = data[4][i];
        x6 = data[5][i];
        x7 = data[6][i];

        if (x1.Equals(1))
          f = 3 + 3 * x2 + 2 * x3 + x4;
        else
          f = -3 + 3 * x5 + 2 * x6 + x7;

        results.Add(f + NormalDistributedRandom.NextDouble(rand, 0, sigma));
      }
      data.Add(results);

      return data;
    }

    private List<double> GenerateUniformIntegerDistribution(IRandom rand, List<int> classes, int amount) {
      List<double> values = new List<double>();
      for (int i = 0; i < amount; i++) {
        values.Add(classes[rand.Next(0, classes.Count)]);
      }
      return values;
    }
  }
}
