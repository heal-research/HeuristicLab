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

namespace HeuristicLab.Problems.Instances.Regression {
  public class SpatialCoevolution : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Spatial co-evolution F(x,y) = 1/(1+power(x,-4)) + 1/(1+pow(y,-4))"; } }
    public override string Description {
      get {
        return "Paper: Evolutionary consequences of coevolving targets" + Environment.NewLine
        + "Authors: Ludo Pagie and Paulien Hogeweg" + Environment.NewLine
        + "Function: F(x,y) = 1/(1+power(x,-4)) + 1/(1+pow(y,-4))" + Environment.NewLine
        + "Terminal set: x, y" + Environment.NewLine
        + "The fitness of a solution is defined as the mean of the absolute differences between "
        + "the target function and the solution over all problems on the basis of which it is evaluated. "
        + "A solution is considered completely ’correct’ if, for all 676 problems in the ’complete’ "
        + "problem set used in the static evaluation scheme, the absolute difference between "
        + "solution and target function is less than 0:01 (this is a so-called hit).";
      }
    }
    protected override string TargetVariable { get { return "F"; } }
    protected override string[] InputVariables { get { return new string[] { "X", "Y", "F" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X", "Y" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 250; } }
    protected override int TestPartitionStart { get { return 250; } }
    protected override int TestPartitionEnd { get { return 500; } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();

      List<double> oneVariableTestData = ValueGenerator.GenerateSteps(-5, 5, 0.4).ToList();
      List<List<double>> testData = new List<List<double>>() { oneVariableTestData, oneVariableTestData };
      var combinations = ValueGenerator.GenerateAllCombinationsOfValuesInLists(testData).ToList<IEnumerable<double>>();

      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(1000, -5, 5).ToList());
        data[i].AddRange(combinations[i]);
      }

      double x, y;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        y = data[1][i];
        results.Add(1 / (1 + Math.Pow(x, -4)) + 1 / (1 + Math.Pow(y, -4)));
      }
      data.Add(results);

      return data;
    }
  }
}
