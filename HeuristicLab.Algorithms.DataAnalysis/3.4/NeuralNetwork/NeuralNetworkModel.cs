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

extern alias alglib_3_7;
using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a neural network model for regression and classification
  /// </summary>
  [StorableType("DABDBD64-E93B-4F50-A343-C8A92C1C48A4")]
  [Item("NeuralNetworkModel", "Represents a neural network for regression and classification.")]
  public sealed class NeuralNetworkModel : ClassificationModel, INeuralNetworkModel {

    private object mlpLocker = new object();



    private alglib.multilayerperceptron multiLayerPerceptron;
    [Storable]
    private string SerializedMultiLayerPerceptron {
      get { alglib.mlpserialize(multiLayerPerceptron, out var ser); return ser; }
      set { if (value != null) alglib.mlpunserialize(value, out multiLayerPerceptron); }
    }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [StorableConstructor]
    private NeuralNetworkModel(StorableConstructorFlag _) : base(_) { }
    private NeuralNetworkModel(NeuralNetworkModel original, Cloner cloner)
      : base(original, cloner) {
      if (original.multiLayerPerceptron != null)
        multiLayerPerceptron = (alglib.multilayerperceptron)original.multiLayerPerceptron.make_copy();
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public NeuralNetworkModel(alglib.multilayerperceptron multiLayerPerceptron, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues = null)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      this.multiLayerPerceptron = (alglib.multilayerperceptron)multiLayerPerceptron.make_copy();
      this.allowedInputVariables = allowedInputVariables.ToArray();
      if (classValues != null)
        this.classValues = (double[])classValues.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NeuralNetworkModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = dataset.ToArray(allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        // NOTE: mlpprocess changes data in multiLayerPerceptron and is therefore not thread-safe!
        lock (mlpLocker) {
          alglib.mlpprocess(multiLayerPerceptron, x, ref y);
        }
        yield return y[0];
      }
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = dataset.ToArray(allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[classValues.Length];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        // NOTE: mlpprocess changes data in multiLayerPerceptron and is therefore not thread-safe!
        lock (mlpLocker) {
          alglib.mlpprocess(multiLayerPerceptron, x, ref y);
        }
        // find class for with the largest probability value
        int maxProbClassIndex = 0;
        double maxProb = y[0];
        for (int i = 1; i < y.Length; i++) {
          if (maxProb < y[i]) {
            maxProb = y[i];
            maxProbClassIndex = i;
          }
        }
        yield return classValues[maxProbClassIndex];
      }
    }

    public bool IsProblemDataCompatible(IRegressionProblemData problemData, out string errorMessage) {
      return RegressionModel.IsProblemDataCompatible(this, problemData, out errorMessage);
    }

    public override bool IsProblemDataCompatible(IDataAnalysisProblemData problemData, out string errorMessage) {
      if (problemData == null) throw new ArgumentNullException("problemData", "The provided problemData is null.");

      var regressionProblemData = problemData as IRegressionProblemData;
      if (regressionProblemData != null)
        return IsProblemDataCompatible(regressionProblemData, out errorMessage);

      var classificationProblemData = problemData as IClassificationProblemData;
      if (classificationProblemData != null)
        return IsProblemDataCompatible(classificationProblemData, out errorMessage);

      throw new ArgumentException("The problem data is not compatible with this neural network. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
    }

    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new NeuralNetworkRegressionSolution(this, new RegressionProblemData(problemData));
    }
    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new NeuralNetworkClassificationSolution(this, new ClassificationProblemData(problemData));
    }
  }
}
