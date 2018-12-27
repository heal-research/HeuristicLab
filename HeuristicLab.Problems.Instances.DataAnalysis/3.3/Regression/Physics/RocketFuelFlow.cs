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
  public class RocketFuelFlow : ArtificialRegressionDataDescriptor {
    public override string Name { get { return "Rocket Fuel Flow m_dot = p0 A / sqrt(T0) * sqrt(γ/R (2/(γ+1))^((γ+1) / (γ-1)))"; } }

    public override string Description {
      get {
        return "A full description of this problem instance is given in: " + Environment.NewLine +
          "Chen Chen, Changtong Luo, Zonglin Jiang, \"A multilevel block building algorithm for fast " +
          "modeling generalized separable systems\", Expert Systems with Applications, Volume 109, 2018, " +
          "Pages 25-34 https://doi.org/10.1016/j.eswa.2018.05.021. " + Environment.NewLine +
          "Function: m_dot = p0 A / sqrt(T0) * sqrt(γ/R (2/(γ+1))^((γ+1) / (γ-1)))" + Environment.NewLine +
          "with p0 ∈ [4e5 Pa, 6e5 Pa]," + Environment.NewLine +
          "A ∈ [0.5m², 1.5m²]," + Environment.NewLine +
          "T0 ∈ [250°K, 260°K]," + Environment.NewLine +
          "γ=1.4 and R=287 J/(kg*K)" + Environment.NewLine +
          "The factor sqrt(γ/R (2/(γ+1))^((γ+1) / (γ-1))) is constant as γ and R are constants.";
      }
    }

    protected override string TargetVariable { get { return "m_dot"; } }
    protected override string[] VariableNames { get { return new string[] { "p0", "A", "T0", "m_dot" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "p0", "A", "T0" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 100; } }
    protected override int TestPartitionStart { get { return 100; } }
    protected override int TestPartitionEnd { get { return 200; } }

    public int Seed { get; private set; }

    public RocketFuelFlow() : this((int)System.DateTime.Now.Ticks) { }

    public RocketFuelFlow(int seed) {
      Seed = seed;
    }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint)Seed);

      List<List<double>> data = new List<List<double>>();
      var p0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 4.0e5, 6.0e5).ToList();
      var A = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 1.5).ToList();
      var T0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 250.0, 260.0).ToList();

      List<double> m_dot = new List<double>();
      data.Add(p0);
      data.Add(A);
      data.Add(T0);
      data.Add(m_dot);
      double R = 287.0;
      double γ = 1.4;
      var c = Math.Sqrt(γ / R * Math.Pow(2 / (γ + 1), (γ + 1) / (γ - 1)));
      for (int i = 0; i < p0.Count; i++) {
        double m_dot_i = p0[i] * A[i] / Math.Sqrt(T0[i]) * c;
        m_dot.Add(m_dot_i);
      }

      return data;
    }
  }
}
