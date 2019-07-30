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
using System.ComponentModel;
using System.Linq;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [NonDiscoverableType]
  public class TimeframeFeatureCorrelationCalculator : AbstractFeatureCorrelationCalculator {
    public TimeframeFeatureCorrelationCalculator() : base() { }

    // returns true if any calculation takes place
    public bool CalculateTimeframeElements(IDataAnalysisProblemData problemData, IDependencyCalculator calc, string partition, string variable, int frames, double[,] correlation = null) {
      if (correlation != null && correlation.GetLength(1) > frames) return false;

      var indices = GetRelevantIndices(problemData, partition);
      var info = new BackgroundWorkerInfo {
        Dataset = problemData.Dataset, Calculator = calc, Partition = partition, Indices = indices, Variable = variable, Frames = frames, AlreadyCalculated = correlation
      };

      StartCalculation(info);
      return true;
    }

    protected override void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      BackgroundWorker worker = (BackgroundWorker)sender;
      BackgroundWorkerInfo bwInfo = (BackgroundWorkerInfo)e.Argument;

      var dataset = bwInfo.Dataset;
      var indices = bwInfo.Indices.ToArray();
      IDependencyCalculator calc = bwInfo.Calculator;
      string variable = bwInfo.Variable;
      int frames = bwInfo.Frames;
      double[,] alreadyCalculated = bwInfo.AlreadyCalculated;

      IList<string> doubleVariableNames = dataset.DoubleVariables.ToList();
      int length = doubleVariableNames.Count;
      double[,] elements = new double[length, frames + 1];
      double calculations = (frames + 1) * length;

      worker.ReportProgress(0);

      int start = 0;
      if (alreadyCalculated != null) {
        for (int i = 0; i < alreadyCalculated.GetLength(0); i++) {
          Array.Copy(alreadyCalculated, i * alreadyCalculated.GetLength(1), elements, i * elements.GetLength(1), alreadyCalculated.GetLength(1));
        }
        start = alreadyCalculated.GetLength(1);
      }

      var var1 = dataset.GetDoubleValues(variable, indices).ToArray();

      for (int i = 0; i < length; i++) {
        for (int j = start; j <= frames; j++) {
          if (worker.CancellationPending) {
            worker.ReportProgress(100);
            e.Cancel = true;
            return;
          }

          IEnumerable<double> var2 = dataset.GetDoubleValues(doubleVariableNames[i], indices);

          var error = OnlineCalculatorError.None;
          elements[i, j] = calc.Calculate(var1.Skip(j), var2.Take(var1.Length-j), out error);

          if (!error.Equals(OnlineCalculatorError.None)) {
            elements[i, j] = double.NaN;
          }
          worker.ReportProgress((int)((100.0 / calculations) * (i * (frames + 1) + j + 1)));
        }
      }
      e.Result = elements;
      worker.ReportProgress(100);
    }
  }
}
