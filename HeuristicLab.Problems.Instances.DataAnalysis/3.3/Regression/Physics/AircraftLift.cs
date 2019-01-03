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
  public class AircraftLift : ArtificialRegressionDataDescriptor {
    public override string Name { get { return "Aircraft Lift Coefficient C_L = C_La (a - a0) + C_Ld_e d_e S_HT / S_ref"; } }

    public override string Description {
      get {
        return "A full description of this problem instance is given in: " + Environment.NewLine +
          "Chen Chen, Changtong Luo, Zonglin Jiang, \"A multilevel block building algorithm for fast " +
          "modeling generalized separable systems\", Expert Systems with Applications, Volume 109, 2018, " +
          "Pages 25-34 https://doi.org/10.1016/j.eswa.2018.05.021. " + Environment.NewLine +
          "Function: C_L = C_La (a - a0) + C_Ld_e d_e S_HT / S_ref" + Environment.NewLine +
          "with C_La ∈ [0.4, 0.8]," + Environment.NewLine +
          "a ∈ [5°, 10°]," + Environment.NewLine +
          "C_Ld_e ∈ [0.4, 0.8]," + Environment.NewLine +
          "d_e ∈ [5°, 10°]," + Environment.NewLine +
          "S_HT ∈ [1m², 1.5m²]," + Environment.NewLine +
          "S_ref ∈ [5m², 7m²]," + Environment.NewLine +
          "a0 is set to -2°";
      }
    }

    protected override string TargetVariable { get { return "C_L"; } }
    protected override string[] VariableNames { get { return new string[] { "C_La", "a", "a0", "C_Ld_e", "d_e", "S_HT", "C_L" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "C_La", "a", "a0", "C_Ld_e", "d_e", "S_HT" }; } }
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

      List<double> C_L = new List<double>();
      data.Add(C_La);
      data.Add(a);
      data.Add(C_Ld_e);
      data.Add(d_e);
      data.Add(S_HT);
      data.Add(S_ref);
      data.Add(C_L);

      double a0 = -2.0;

      for (int i = 0; i < C_La.Count; i++) {
        double C_Li = C_La[i] * (a[i] - a0) + C_Ld_e[i] * d_e[i] * S_HT[i] / S_ref[i];
        C_L.Add(C_Li);
      }

      return data;
    }
  }
}
