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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a linear regression model
  /// </summary>
  [StorableClass]
  [Item("Linear Regression Model", "Represents a linear regression model.")]
  public sealed class LinearRegressionModel : RegressionModel, IConfidenceRegressionModel {

    [Storable]
    public double[,] C {
      get; private set;
    }
    [Storable]
    public double[] W {
      get; private set;
    }

    [Storable]
    public double NoiseSigma {
      get; private set;
    }
    
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables.Union(factorVariables.Select(f => f.Key)); }
    }

    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private List<KeyValuePair<string, IEnumerable<string>>> factorVariables;

    [StorableConstructor]
    private LinearRegressionModel(bool deserializing)
      : base(deserializing) {
    }
    private LinearRegressionModel(LinearRegressionModel original, Cloner cloner)
      : base(original, cloner) {
      this.W = original.W;
      this.C = original.C;
      this.NoiseSigma = original.NoiseSigma;

      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      this.factorVariables = original.factorVariables.Select(kvp => new KeyValuePair<string, IEnumerable<string>>(kvp.Key, new List<string>(kvp.Value))).ToList();
    }
    public LinearRegressionModel(double[] w, double[,] covariance, double noiseSigma, string targetVariable, IEnumerable<string> doubleInputVariables, IEnumerable<KeyValuePair<string, IEnumerable<string>>> factorVariables)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      this.W = new double[w.Length];
      Array.Copy(w, W, w.Length);
      this.C = new double[covariance.GetLength(0),covariance.GetLength(1)];
      Array.Copy(covariance, C, covariance.Length);
      this.NoiseSigma = noiseSigma;
      var stringInputVariables = factorVariables.Select(f => f.Key).Distinct();
      this.allowedInputVariables = doubleInputVariables.ToArray();
      this.factorVariables = factorVariables.Select(kvp => new KeyValuePair<string, IEnumerable<string>>(kvp.Key, new List<string>(kvp.Value))).ToList();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearRegressionModel(this, cloner);
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = dataset.ToArray(allowedInputVariables, rows);
      double[,] factorData = dataset.ToArray(factorVariables, rows);

      inputData = factorData.HorzCat(inputData);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);

      for (int row = 0; row < n; row++) {
        double p = 0.0;
        for (int column = 0; column < columns; column++) {
          p += W[column] * inputData[row, column];
        }
        p += W[columns];
        yield return p;
      }
    }

    public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = dataset.ToArray(allowedInputVariables, rows);
      double[,] factorData = dataset.ToArray(factorVariables, rows);

      inputData = factorData.HorzCat(inputData);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);

      double[] d = new double[C.GetLength(0)];
      
      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          d[column] = inputData[row,column];
        }
        d[columns] = 1;

        double var = 0.0;
        for(int i=0;i<d.Length;i++) {
          for(int j = 0;j<d.Length;j++) {
            var += d[i] * C[i, j] * d[j];
          }
        }
        yield return var + NoiseSigma*NoiseSigma;
      }
    }


    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new ConfidenceRegressionSolution(this, new RegressionProblemData(problemData));
    }
  }
}
