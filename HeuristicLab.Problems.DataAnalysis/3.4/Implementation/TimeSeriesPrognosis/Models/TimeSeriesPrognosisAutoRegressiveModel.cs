#region License Information
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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Autoregressive TimeSeries Model", "A linear autoregressive time series model used to predict future values.")]
  public class TimeSeriesPrognosisAutoRegressiveModel : NamedItem, ITimeSeriesPrognosisModel {
    [Storable]
    public double[] Phi { get; private set; }
    [Storable]
    public double Constant { get; private set; }
    [Storable]
    public string TargetVariable { get; private set; }

    public int TimeOffset { get { return Phi.Length; } }

    [StorableConstructor]
    protected TimeSeriesPrognosisAutoRegressiveModel(bool deserializing) : base(deserializing) { }
    protected TimeSeriesPrognosisAutoRegressiveModel(TimeSeriesPrognosisAutoRegressiveModel original, Cloner cloner)
      : base(original, cloner) {
      this.Phi = (double[])original.Phi.Clone();
      this.Constant = original.Constant;
      this.TargetVariable = original.TargetVariable;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TimeSeriesPrognosisAutoRegressiveModel(this, cloner);
    }
    public TimeSeriesPrognosisAutoRegressiveModel(string targetVariable, double[] phi, double constant)
      : base("AR(1) Model") {
      Phi = (double[])phi.Clone();
      Constant = constant;
      TargetVariable = targetVariable;
    }

    public IEnumerable<IEnumerable<double>> GetPrognosedValues(IDataset dataset, IEnumerable<int> rows, IEnumerable<int> horizons) {
      var rowsEnumerator = rows.GetEnumerator();
      var horizonsEnumerator = horizons.GetEnumerator();
      var targetValues = dataset.GetReadOnlyDoubleValues(TargetVariable);
      // produce a n-step forecast for all rows
      while (rowsEnumerator.MoveNext() & horizonsEnumerator.MoveNext()) {
        int row = rowsEnumerator.Current;
        int horizon = horizonsEnumerator.Current;
        if (row - TimeOffset < 0) {
          yield return Enumerable.Repeat(double.NaN, horizon);
          continue;
        }

        double[] prognosis = new double[horizon];
        for (int h = 0; h < horizon; h++) {
          double estimatedValue = 0.0;
          for (int i = 1; i <= TimeOffset; i++) {
            int offset = h - i;
            if (offset >= 0) estimatedValue += prognosis[offset] * Phi[i - 1];
            else estimatedValue += targetValues[row + offset] * Phi[i - 1];

          }
          estimatedValue += Constant;
          prognosis[h] = estimatedValue;
        }

        yield return prognosis;
      }

      if (rowsEnumerator.MoveNext() || horizonsEnumerator.MoveNext())
        throw new ArgumentException("Number of elements in rows and horizon enumerations doesn't match.");
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      var targetVariables = dataset.GetReadOnlyDoubleValues(TargetVariable);
      foreach (int row in rows) {
        double estimatedValue = 0.0;
        if (row - TimeOffset < 0) {
          yield return double.NaN;
          continue;
        }

        for (int i = 1; i <= TimeOffset; i++) {
          estimatedValue += targetVariables[row - i] * Phi[i - 1];
        }
        estimatedValue += Constant;
        yield return estimatedValue;
      }
    }

    public ITimeSeriesPrognosisSolution CreateTimeSeriesPrognosisSolution(ITimeSeriesPrognosisProblemData problemData) {
      return new TimeSeriesPrognosisSolution(this, new TimeSeriesPrognosisProblemData(problemData));
    }
    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      throw new NotSupportedException();
    }

  }
}
