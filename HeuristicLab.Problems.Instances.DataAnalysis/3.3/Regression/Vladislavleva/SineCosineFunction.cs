﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class SineCosineFunction : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Vladislavleva-6 F6(X1, X2) = 6 * sin(X1) * cos(X2)"; } }
    public override string Description {
      get {
        return "Paper: Order of Nonlinearity as a Complexity Measure for Models Generated by Symbolic Regression via Pareto Genetic Programming " + Environment.NewLine
        + "Authors: Ekaterina J. Vladislavleva, Member, IEEE, Guido F. Smits, Member, IEEE, and Dick den Hertog" + Environment.NewLine
        + "Function: F6(X1, X2) = 6 * sin(X1) * cos(X2)" + Environment.NewLine
        + "Training Data: 30 points X1, X2 = Rand(0.1, 5.9)" + Environment.NewLine
        + "Test Data: 306*306 points X1, X2 = (-0.05:0.02:6.05)" + Environment.NewLine
        + "Function Set: +, -, *, /, square, e^x, e^-x, x^eps, x + eps, x * eps";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X1", "X2", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X1", "X2" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 30; } }
    protected override int TestPartitionStart { get { return 30; } }
    protected override int TestPartitionEnd { get { return 30 + (306 * 306); } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      List<double> oneVariableTestData = SequenceGenerator.GenerateSteps(-0.05m, 6.05m, 0.02m).Select(v => (double)v).ToList();
      List<List<double>> testData = new List<List<double>>() { oneVariableTestData, oneVariableTestData };
      var combinations = ValueGenerator.GenerateAllCombinationsOfValuesInLists(testData).ToList<IEnumerable<double>>();

      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(30, 0.1, 5.9).ToList());
        data[i].AddRange(combinations[i]);
      }

      double x1, x2;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x1 = data[0][i];
        x2 = data[1][i];
        results.Add(6 * Math.Sin(x1) * Math.Cos(x2));
      }
      data.Add(results);

      return data;
    }
  }
}
