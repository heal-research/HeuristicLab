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
  [StorableType("AEB9B960-FCA6-4A6D-BD5F-27BCE9CC5BEA")]
  [Item("NeuralNetworkModel", "Represents a neural network for regression and classification.")]
  [Obsolete("This version uses alglib version 3.7. Use NeuralNetworkModel instead.")]
  public sealed class NeuralNetworkModelAlglib_3_7 : ClassificationModel, INeuralNetworkModel {

    private object mlpLocker = new object();
    private alglib_3_7.alglib.multilayerperceptron multiLayerPerceptron;

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [StorableConstructor]
    private NeuralNetworkModelAlglib_3_7(StorableConstructorFlag _) : base(_) {
      multiLayerPerceptron = new alglib_3_7.alglib.multilayerperceptron();
    }
    private NeuralNetworkModelAlglib_3_7(NeuralNetworkModelAlglib_3_7 original, Cloner cloner)
      : base(original, cloner) {
      multiLayerPerceptron = new alglib_3_7.alglib.multilayerperceptron();
      multiLayerPerceptron.innerobj.chunks = (double[,])original.multiLayerPerceptron.innerobj.chunks.Clone();
      multiLayerPerceptron.innerobj.columnmeans = (double[])original.multiLayerPerceptron.innerobj.columnmeans.Clone();
      multiLayerPerceptron.innerobj.columnsigmas = (double[])original.multiLayerPerceptron.innerobj.columnsigmas.Clone();
      multiLayerPerceptron.innerobj.derror = (double[])original.multiLayerPerceptron.innerobj.derror.Clone();
      multiLayerPerceptron.innerobj.dfdnet = (double[])original.multiLayerPerceptron.innerobj.dfdnet.Clone();
      multiLayerPerceptron.innerobj.neurons = (double[])original.multiLayerPerceptron.innerobj.neurons.Clone();
      multiLayerPerceptron.innerobj.nwbuf = (double[])original.multiLayerPerceptron.innerobj.nwbuf.Clone();
      multiLayerPerceptron.innerobj.structinfo = (int[])original.multiLayerPerceptron.innerobj.structinfo.Clone();
      multiLayerPerceptron.innerobj.weights = (double[])original.multiLayerPerceptron.innerobj.weights.Clone();
      multiLayerPerceptron.innerobj.x = (double[])original.multiLayerPerceptron.innerobj.x.Clone();
      multiLayerPerceptron.innerobj.y = (double[])original.multiLayerPerceptron.innerobj.y.Clone();
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public NeuralNetworkModelAlglib_3_7(alglib_3_7.alglib.multilayerperceptron multiLayerPerceptron, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues = null)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      this.multiLayerPerceptron = multiLayerPerceptron;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      if (classValues != null)
        this.classValues = (double[])classValues.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NeuralNetworkModelAlglib_3_7(this, cloner);
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
        // NOTE: mlpprocess changes data in multiLayerPerceptron and is therefore not thread-save!
        lock (mlpLocker) {
          alglib_3_7.alglib.mlpprocess(multiLayerPerceptron, x, ref y);
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
        // NOTE: mlpprocess changes data in multiLayerPerceptron and is therefore not thread-save!
        lock (mlpLocker) {
          alglib_3_7.alglib.mlpprocess(multiLayerPerceptron, x, ref y);
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

    #region persistence
    [Storable]
    private double[,] MultiLayerPerceptronChunks {
      get {
        return multiLayerPerceptron.innerobj.chunks;
      }
      set {
        multiLayerPerceptron.innerobj.chunks = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronColumnMeans {
      get {
        return multiLayerPerceptron.innerobj.columnmeans;
      }
      set {
        multiLayerPerceptron.innerobj.columnmeans = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronColumnSigmas {
      get {
        return multiLayerPerceptron.innerobj.columnsigmas;
      }
      set {
        multiLayerPerceptron.innerobj.columnsigmas = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronDError {
      get {
        return multiLayerPerceptron.innerobj.derror;
      }
      set {
        multiLayerPerceptron.innerobj.derror = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronDfdnet {
      get {
        return multiLayerPerceptron.innerobj.dfdnet;
      }
      set {
        multiLayerPerceptron.innerobj.dfdnet = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronNeurons {
      get {
        return multiLayerPerceptron.innerobj.neurons;
      }
      set {
        multiLayerPerceptron.innerobj.neurons = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronNwbuf {
      get {
        return multiLayerPerceptron.innerobj.nwbuf;
      }
      set {
        multiLayerPerceptron.innerobj.nwbuf = value;
      }
    }
    [Storable]
    private int[] MultiLayerPerceptronStuctinfo {
      get {
        return multiLayerPerceptron.innerobj.structinfo;
      }
      set {
        multiLayerPerceptron.innerobj.structinfo = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronWeights {
      get {
        return multiLayerPerceptron.innerobj.weights;
      }
      set {
        multiLayerPerceptron.innerobj.weights = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronX {
      get {
        return multiLayerPerceptron.innerobj.x;
      }
      set {
        multiLayerPerceptron.innerobj.x = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronY {
      get {
        return multiLayerPerceptron.innerobj.y;
      }
      set {
        multiLayerPerceptron.innerobj.y = value;
      }
    }
    #endregion
  }
}
