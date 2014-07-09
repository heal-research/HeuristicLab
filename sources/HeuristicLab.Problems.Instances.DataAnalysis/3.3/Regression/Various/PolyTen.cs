﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class PolyTen : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Poly-10 y = X1*X2 + X3*X4 + X5*X6 + X1*X7*X9 + X3*X6*X10"; } }
    public override string Description {
      get {
        return "Paper: A Simple but Theoretically-motivated Method to Control Bloat in Genetic Programming" + Environment.NewLine
        + "Authors: Riccardo Poli" + Environment.NewLine
        + "Function: y = X1*X2 + X3*X4 + X5*X6 + X1*X7*X9 + X3*X6*X10" + Environment.NewLine
        + "Terminal set: x1, x2, x3, x4, x5, x6, x7, x8, x9, x10" + Environment.NewLine
        + "Fitness was minus the sum of the absolute values of the errors made over 50 fitness cases. "
        + "These were generated by randomly assigning values to the variables xiin the range [1, 1].";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X1", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "X9", "X10", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X1", "X2", "X3", "X4", "X5", "X6", "X7", "X8", "X9", "X10" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 250; } }
    protected override int TestPartitionStart { get { return 250; } }
    protected override int TestPartitionEnd { get { return 500; } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(TestPartitionEnd, -1, 1).ToList());
      }

      double x1, x2, x3, x4, x5, x6, x7, x8, x9, x10;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x1 = data[0][i];
        x2 = data[1][i];
        x3 = data[2][i];
        x4 = data[3][i];
        x5 = data[4][i];
        x6 = data[5][i];
        x7 = data[6][i];
        x8 = data[7][i];
        x9 = data[8][i];
        x10 = data[9][i];
        results.Add(x1 * x2 + x3 * x4 + x5 * x6 + x1 * x7 * x9 + x3 * x6 * x10);
      }
      data.Add(results);

      return data;
    }
  }
}
