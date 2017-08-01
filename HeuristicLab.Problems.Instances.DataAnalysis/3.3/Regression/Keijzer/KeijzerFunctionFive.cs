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
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class KeijzerFunctionFive : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Keijzer 5 f(x) = (30 * x * z) / ((x - 10)  * y²)"; } }
    public override string Description {
      get {
        return "Paper: Improving Symbolic Regression with Interval Arithmetic and Linear Scaling" + Environment.NewLine
        + "Authors: Maarten Keijzer" + Environment.NewLine
        + "Function: f(x) = (30 * x * z) / ((x - 10)  * y²)" + Environment.NewLine
        + "range(train): 1000 points x,z = rnd(-1, 1), y = rnd(1, 2)" + Environment.NewLine
        + "range(test): 10000 points x,z = rnd(-1, 1), y = rnd(1, 2)" + Environment.NewLine
        + "Function Set: x + y, x * y, 1/x, -x, sqrt(x)";
      }
    }
    protected override string TargetVariable { get { return "F"; } }
    protected override string[] VariableNames { get { return new string[] { "X", "Y", "Z", "F" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X", "Y", "Z" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 1000; } }
    protected override int TestPartitionStart { get { return 1000; } }
    protected override int TestPartitionEnd { get { return 11000; } }
    public int Seed { get; private set; }

    public KeijzerFunctionFive() : this((int)System.DateTime.Now.Ticks) {
    }
    public KeijzerFunctionFive(int seed) : base() {
      Seed = seed;
    }
    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      var rand = new MersenneTwister((uint)Seed);

      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, -1, 1).ToList());
      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList());
      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, -1, 1).ToList());

      double x, y, z;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        y = data[1][i];
        z = data[2][i];
        results.Add((30 * x * z) / ((x - 10) * Math.Pow(y, 2)));
      }
      data.Add(results);

      return data;
    }
  }
}
