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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class NguyenFunctionFour : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Nguyen F4 = x^6 + x^5 + x^4 + x³ + x² + x"; } }
    public override string Description {
      get {
        return "Paper: Semantically-based Crossover in Genetic Programming: Application to Real-valued Symbolic Regression" + Environment.NewLine
        + "Authors: Nguyen Quang Uy · Nguyen Xuan Hoai · Michael O’Neill · R.I. McKay · Edgar Galvan-Lopez" + Environment.NewLine
        + "Function: F4 = x^6 + x^5 + x^4 + x³ + x² + x" + Environment.NewLine
        + "Fitcases: 20 random points in [-1, 1]" + Environment.NewLine
        + "Non-terminals: +, -, *, % (protected division), sin, cos, exp, ln(|x|) (protected log)" + Environment.NewLine
        + "Terminals: only variables (no random constants)";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 20; } }
    protected override int TestPartitionStart { get { return 20; } }
    protected override int TestPartitionEnd { get { return 520; } }
    public int Seed { get; private set; }

    public NguyenFunctionFour() : this((int)System.DateTime.Now.Ticks) { }
    public NguyenFunctionFour(int seed) : base() {
      Seed = seed;
    }
    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      data.Add(ValueGenerator.GenerateUniformDistributedValues(Seed, 520, -1, 1).ToList());

      double x;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        results.Add(Math.Pow(x, 6) + Math.Pow(x, 5) + Math.Pow(x, 4) + Math.Pow(x, 3) + x * x + x);
      }
      data.Add(results);

      return data;
    }
  }
}
