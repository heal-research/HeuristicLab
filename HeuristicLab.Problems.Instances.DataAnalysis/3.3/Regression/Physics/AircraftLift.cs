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
  public class AircraftLift : ArtificialRegressionDataDescriptor {
    public override string Name { get { return "Aircraft Lift Coefficient C_L = C_Lα (α + α0) + C_Lδ_e δ_e S_HT / S_ref"; } }

    public override string Description {
      get {
        return "A full description of this problem instance is given in: " + Environment.NewLine +
          "Chen Chen, Changtong Luo, Zonglin Jiang, \"A multilevel block building algorithm for fast " +
          "modeling generalized separable systems\", Expert Systems with Applications, Volume 109, 2018, " +
          "Pages 25-34 https://doi.org/10.1016/j.eswa.2018.05.021. " + Environment.NewLine +
          "Function: C_L = C_Lα (α + α0) + C_Lδ_e δ_e S_HT / S_ref" + Environment.NewLine +
          "the lift coefficient of the main airfoil C_Lα ∈ [0.4, 0.8]," + Environment.NewLine +
          "tha angle of attack α ∈ [5°, 10°]," + Environment.NewLine +
          "the lift coefficient of the horizontal tail C_Lδ_e ∈ [0.4, 0.8]," + Environment.NewLine +
          "δ_e ∈ [5°, 10°]," + Environment.NewLine +
          "S_HT ∈ [1m², 1.5m²]," + Environment.NewLine +
          "S_ref ∈ [5m², 7m²]," + Environment.NewLine +
          "the zero-lift angle of attack α0 is set to -2°";
      }
    }

    protected override string TargetVariable { get { return "C_L"; } }
    protected override string[] VariableNames { get { return new string[] { "C_Lα", "α", "C_Lδ_e", "δ_e", "S_HT", "S_ref", "C_L", "C_L_noise" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "C_Lα", "α", "C_Lδ_e", "δ_e", "S_HT", "S_ref" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 100; } }
    protected override int TestPartitionStart { get { return 100; } }
    protected override int TestPartitionEnd { get { return 200; } }

    public int Seed { get; private set; }

    public AircraftLift() : this((int)System.DateTime.Now.Ticks) { }

    public AircraftLift(int seed) {
      Seed = seed;
    }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint)Seed);

      List<List<double>> data = new List<List<double>>();
      var C_La = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.4, 0.8).ToList();
      var a = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5.0, 10.0).ToList();
      var C_Ld_e = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.4, 0.8).ToList();
      var d_e = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5.0, 10.0).ToList();
      var S_HT = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1.0, 1.5).ToList();
      var S_ref = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5.0, 7.0).ToList();

      var C_L = new List<double>();
      var C_L_noise = new List<double>();
      data.Add(C_La);
      data.Add(a);
      data.Add(C_Ld_e);
      data.Add(d_e);
      data.Add(S_HT);
      data.Add(S_ref);
      data.Add(C_L);
      data.Add(C_L_noise);

      double a0 = -2.0;

      for (int i = 0; i < C_La.Count; i++) {
        double C_Li = C_La[i] * (a[i] + a0) + C_Ld_e[i] * d_e[i] * S_HT[i] / S_ref[i];
        C_L.Add(C_Li);
      }


      var sigma_noise = 0.05 * C_L.StandardDeviationPop();
      C_L_noise.AddRange(C_L.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));

      return data;
    }
  }
}
