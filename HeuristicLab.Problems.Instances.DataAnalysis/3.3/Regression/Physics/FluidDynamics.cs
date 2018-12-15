#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  class FluidDynamics : ArtificialRegressionDataDescriptor {
    public override string Name { get { return "Flow psi = x1*x2*x5*(1 - x4²/x5²) + 1/(2*Pi) * x3*log(x5/x4)"; } }

    public override string Description {
      get {
        return "A full description of this problem instance is given in the paper: A multilevel block building algorithm for fast modeling generalized separable systems. " + Environment.NewLine +
               "Authors: Chen Chen, Changtong Luo, Zonglin Jiang" + Environment.NewLine +
               "Function: f(X) = x1*x2*x5*(1 - x4²/x5²) + 1/(2*Pi) * x3*log(x5/x4)" + Environment.NewLine +
               "with x1 in [60,65], x2 in [30, 40], x3 in [5,10], x4 in [0.5,0.8], x5 in [0.2,0.5]";
      }
    }

    protected override string TargetVariable { get { return "f(X)"; } }
    protected override string[] VariableNames { get { return new string[] { "x1", "x2", "x3", "x4", "x5", "f(X)" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "x1", "x2", "x3", "x4", "x5" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 100; } }
    protected override int TestPartitionStart { get { return 100; } }
    protected override int TestPartitionEnd { get { return 200; } }

    public int Seed { get; private set; }

    public FluidDynamics() : this((int)System.DateTime.Now.Ticks) { }

    public FluidDynamics(int seed) {
      Seed = seed;
    }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint)Seed);

      List<List<double>> data = new List<List<double>>();
      var x1 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 60.0, 65.0).ToList();
      var x2 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 30.0, 40.0).ToList();
      var x3 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5.0, 10.0).ToList();
      var x4 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 0.8).ToList();
      var x5 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.2, 0.5).ToList();

      List<double> fx = new List<double>();
      data.Add(x1);
      data.Add(x2);
      data.Add(x3);
      data.Add(x4);
      data.Add(x5);
      data.Add(fx);

      for (int i = 0; i < x1.Count; i++) {
        double fxi = x1[i] * x2[i] * x5[i] * (1 - (x4[i] * x4[i]) / (x5[i] * x5[i])) +
                     (1 / (2 * Math.PI)) * x3[i] * Math.Log(x5[i] / x4[i]);
        fx.Add(fxi);
      }

      return data;
    }
  }
}
