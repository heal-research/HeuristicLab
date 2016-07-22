#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading.Tasks;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [NonDiscoverableType]
  public sealed class FeatureCorrelationCalculator : AbstractFeatureCorrelationCalculator {
    public FeatureCorrelationCalculator() : base() { }

    public void CalculateElements(IDataAnalysisProblemData problemData, IDependencyCalculator calc, string partition, bool ignoreMissingValues) {
      var indices = GetRelevantIndices(problemData, partition);
      var info = new BackgroundWorkerInfo {
        Dataset = problemData.Dataset, Calculator = calc, Partition = partition, Indices = indices, IgnoreMissingValues = ignoreMissingValues
      };

      StartCalculation(info);
    }

    protected override void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      BackgroundWorker worker = (BackgroundWorker)sender;
      BackgroundWorkerInfo bwInfo = (BackgroundWorkerInfo)e.Argument;

      var dataset = bwInfo.Dataset;
      var indices = bwInfo.Indices.ToArray();
      IDependencyCalculator calc = bwInfo.Calculator;

      IList<string> doubleVariableNames = dataset.DoubleVariables.ToList();
      OnlineCalculatorError error = OnlineCalculatorError.None;
      int length = doubleVariableNames.Count;
      double[,] elements = new double[length, length];

      worker.ReportProgress(0);

      for (int counter = 0; counter < length; counter++) {
        if (worker.CancellationPending) {
          worker.ReportProgress(100);
          e.Cancel = true;
          return;
        }

        var i = counter;
        Parallel.ForEach(Enumerable.Range(i, length - i), j => {
          var var1 = dataset.GetDoubleValues(doubleVariableNames[i], indices);
          var var2 = dataset.GetDoubleValues(doubleVariableNames[j], indices);

          if (bwInfo.IgnoreMissingValues) {
            var filtered = FilterNaNValues(var1, var2);
            elements[i, j] = calc.Calculate(filtered, out error);
          } else
            elements[i, j] = calc.Calculate(var1, var2, out error);

          if (!error.Equals(OnlineCalculatorError.None)) {
            elements[i, j] = double.NaN;
          }
          elements[j, i] = elements[i, j];

        });
        worker.ReportProgress((int)(((double)counter) / length * 100));
      }
      e.Result = elements;
      worker.ReportProgress(100);
    }


    private static IEnumerable<Tuple<double, double>> FilterNaNValues(IEnumerable<double> first, IEnumerable<double> second) {
      var firstEnumerator = first.GetEnumerator();
      var secondEnumerator = second.GetEnumerator();

      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        var firstValue = firstEnumerator.Current;
        var secondValue = secondEnumerator.Current;

        if (double.IsNaN(firstValue)) continue;
        if (double.IsNaN(secondValue)) continue;

        yield return Tuple.Create(firstValue, secondValue);
      }

      if (firstEnumerator.MoveNext() || secondEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      }
    }
  }
}
