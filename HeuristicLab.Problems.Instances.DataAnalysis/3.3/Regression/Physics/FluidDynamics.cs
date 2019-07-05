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
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FluidDynamics : ArtificialRegressionDataDescriptor {
    public override string Name { get { return "Spinning cylinder flow Ψ = V_∞ r sin(θ) (1 - R²/r²) + Γ/(2 π) ln(r/R)"; } }

    public override string Description {
      get {
        return "A full description of this problem instance is given in: " + Environment.NewLine +
          "Chen Chen, Changtong Luo, Zonglin Jiang, \"A multilevel block building algorithm for fast " +
          "modeling generalized separable systems\", Expert Systems with Applications, Volume 109, 2018, " +
          "Pages 25-34 https://doi.org/10.1016/j.eswa.2018.05.021. " + Environment.NewLine +
          "Function: Ψ = V_∞ r sin(θ) (1 - R²/r²) + Γ/(2 π) ln(r/R)" + Environment.NewLine +
          "with uniform stream velocity V_∞ ∈ [60 m/s, 65 m/s]," + Environment.NewLine +
          "angle for polar coordinate vector field θ ∈ [30°, 40°]," + Environment.NewLine +
          "radius for polar coordinate vector field r ∈ [0.5m, 0.8m]," + Environment.NewLine +
          "radius of cylinder R ∈ [0.2m, 0.5m]," + Environment.NewLine +
          "vortex strength (induced by spinning) Γ ∈ [5 m²/s, 10 m²/s]" + Environment.NewLine +
          "Note: the definition deviates from the definition used in the source above because here we have r > R meaning we want to calculate the flow _outside_ of the cylinder.";
      }
    }

    protected override string TargetVariable { get { return "Ψ"; } }
    protected override string[] VariableNames { get { return new string[] { "V_∞", "θ", "r", "R", "Γ", "Ψ", "Ψ_noise" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "V_∞", "θ", "r", "R", "Γ" }; } }
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
      var r = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 0.8).ToList();
      var R = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.2, 0.5).ToList();
      var G = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5, 10).ToList();

      var Psi = new List<double>();
      var Psi_noise = new List<double>();

      data.Add(V_inf);
      data.Add(th);
      data.Add(r);
      data.Add(R);
      data.Add(G);
      data.Add(Psi);
      data.Add(Psi_noise);

      for (int i = 0; i < V_inf.Count; i++) {
        var th_rad = Math.PI * th[i] / 180.0;
        double Psi_i = V_inf[i] * r[i] * Math.Sin(th_rad) * (1 - (R[i] * R[i]) / (r[i] * r[i])) +
                     (G[i] / (2 * Math.PI)) * Math.Log(r[i] / R[i]);
        Psi.Add(Psi_i);
      }

      var sigma_noise = 0.05 * Psi.StandardDeviationPop();
      Psi_noise.AddRange(Psi.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));

      return data;
    }
  }
}
