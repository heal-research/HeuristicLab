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
  public class FluidDynamics : ArtificialRegressionDataDescriptor {
    public override string Name { get { return "Flow Psi = V_inf r sin(th) (1 - R²/r²) + G/(2 π) ln(r/R)"; } }

    public override string Description {
      get {
        return "A full description of this problem instance is given in: " + Environment.NewLine +
          "Chen Chen, Changtong Luo, Zonglin Jiang, \"A multilevel block building algorithm for fast " +
          "modeling generalized separable systems\", Expert Systems with Applications, Volume 109, 2018, " +
          "Pages 25-34 https://doi.org/10.1016/j.eswa.2018.05.021. " + Environment.NewLine +
          "Function: Psi = V_inf r sin(th) (1 - R²/r²) + G/(2 π) ln(r/R)" + Environment.NewLine +
          "with V_inf ∈ [60 m/s, 65 m/s]," + Environment.NewLine +
          "th ∈ [30°, 40°]," + Environment.NewLine +
          "r ∈ [0.2m, 0.5m]," + Environment.NewLine +
          "R ∈ [0.5m, 0.8m]," + Environment.NewLine +
          "G ∈ [5 m²/s, 10 m²/s]";
      }
    }

    protected override string TargetVariable { get { return "Psi"; } }
    protected override string[] VariableNames { get { return new string[] { "V_inf", "th", "r", "R", "G", "Psi" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "V_inf", "th", "r", "R", "G" }; } }
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
      var V_inf = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 60.0, 65.0).ToList();
      var th = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 30.0, 40.0).ToList();
      var r = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.2, 0.5).ToList();
      var R = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 0.8).ToList();
      var G = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5, 10).ToList();

      List<double> Psi = new List<double>();
      data.Add(V_inf);
      data.Add(th);
      data.Add(r);
      data.Add(R);
      data.Add(G);
      data.Add(Psi);

      for (int i = 0; i < V_inf.Count; i++) {
        var th_rad = Math.PI * th[i] / 180.0;
        double Psi_i = V_inf[i] * r[i] * Math.Sin(th_rad) * (1 - (R[i] * R[i]) / (r[i] * r[i])) +
                     (G[i] / (2 * Math.PI)) * Math.Log(r[i] / R[i]);
        Psi.Add(Psi_i);
      }

      return data;
    }
  }
}
