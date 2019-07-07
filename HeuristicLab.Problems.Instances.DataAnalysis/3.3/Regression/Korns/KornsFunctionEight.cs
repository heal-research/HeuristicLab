#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class KornsFunctionEight : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Korns 8 y = 6.87 + (11 * sqrt(7.23 * X0 * X3 * X4))"; } }
    public override string Description {
      get {
        return "Paper: Accuracy in Symbolic Regression" + Environment.NewLine
        + "Authors: Michael F. Korns" + Environment.NewLine
        + "Function: y = 6.87 + (11 * sqrt(7.23 * X0 * X3 * X4))" + Environment.NewLine
        + "Binary Operators: +, -, *, % (protected division)" + Environment.NewLine
        + "Unary Operators: sqrt, square, cube, cos, sin, tan, tanh, ln(|x|) (protected log), exp" + Environment.NewLine
        + "Constants: random finit 64-bit IEEE double" + Environment.NewLine
        + "\"Our testing regimen uses only statistical best practices out-of-sample testing techniques. "
        + "We test each of the test cases on matrices of 10000 rows by 1 to 5 columns with no noise. "
        + "For each test a training matrix is filled with random numbers between -50 and +50. The test case "
        + "target expressions are limited to one basis function whose maximum depth is three grammar nodes.\"" + Environment.NewLine + Environment.NewLine
        + "Note: Because of the square root only non-negatic values are created for the input variables!";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X0", "X1", "X2", "X3", "X4", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X0", "X1", "X2", "X3", "X4" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 10000; } }
    protected override int TestPartitionStart { get { return 10000; } }
    protected override int TestPartitionEnd { get { return 20000; } }

    public int Seed { get; private set; }

    public KornsFunctionEight() : this((int)System.DateTime.Now.Ticks) {
    }
    public KornsFunctionEight(int seed) : base() {
      Seed = seed;
    }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      var rand = new MersenneTwister((uint)Seed);

      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 50).ToList()); // note: range is only [0,50] to prevent NaN values (deviates from gp benchmark paper)
      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, -50, 50).ToList());
      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, -50, 50).ToList());
      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 50).ToList()); // note: range is only [0,50] to prevent NaN values (deviates from gp benchmark paper)
      data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 50).ToList()); // note: range is only [0,50] to prevent NaN values (deviates from gp benchmark paper)

      double x0, x3, x4;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x0 = data[0][i];
        x3 = data[3][i];
        x4 = data[4][i];
        results.Add(6.87 + (11 * Math.Sqrt(7.23 * x0 * x3 * x4)));
      }
      data.Add(results);

      return data;
    }
  }
}
