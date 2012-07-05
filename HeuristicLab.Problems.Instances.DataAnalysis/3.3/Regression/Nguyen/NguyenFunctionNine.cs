#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class NguyenFunctionNine : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Nguyen F9 = sin(x) + sin(y²)"; } }
    public override string Description {
      get {
        return "Paper: Semantically-based Crossover in Genetic Programming: Application to Real-valued Symbolic Regression" + Environment.NewLine
        + "Authors: Nguyen Quang Uy · Nguyen Xuan Hoai · Michael O’Neill · R.I. McKay · Edgar Galvan-Lopez" + Environment.NewLine
        + "Function: F9 = sin(x) + sin(y²)" + Environment.NewLine
        + "Fitcases: 20 random points in [0, 1]x[0, 1]" + Environment.NewLine
        + "Non-terminals: +, -, *, % (protected division), sin, cos, exp, ln(|x|) (protected log)" + Environment.NewLine
        + "Terminals: only variables (no random constants)";
      }
    }
    protected override string TargetVariable { get { return "Z"; } }
    protected override string[] InputVariables { get { return new string[] { "X", "Y", "Z" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X", "Y" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 20; } }
    protected override int TestPartitionStart { get { return 500; } }
    protected override int TestPartitionEnd { get { return 1000; } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      data.Add(ValueGenerator.GenerateUniformDistributedValues(TestPartitionEnd, 0, 1).ToList());
      data.Add(ValueGenerator.GenerateUniformDistributedValues(TestPartitionEnd, 0, 1).ToList());

      double x, y;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        y = data[1][i];
        results.Add(Math.Sin(x) + Math.Sin(Math.Pow(y, 2)));
      }
      data.Add(results);

      return data;
    }
  }
}
