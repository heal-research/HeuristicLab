#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a neural network ensembel model for regression and classification
  /// </summary>
  [StorableType("51B29670-27BD-405C-A521-39814E4BD857")]
  [Item("NeuralNetworkEnsembleModel", "Represents a neural network ensemble for regression and classification.")]
  public sealed class NeuralNetworkEnsembleModel : ClassificationModel, INeuralNetworkEnsembleModel {

    private object mlpEnsembleLocker = new object();
    private alglib.mlpensemble mlpEnsemble;

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private string targetVariable;
    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [StorableConstructor]
    private NeuralNetworkEnsembleModel(StorableConstructorFlag _) : base(_) {
      mlpEnsemble = new alglib.mlpensemble();
    }
    private NeuralNetworkEnsembleModel(NeuralNetworkEnsembleModel original, Cloner cloner)
      : base(original, cloner) {
      mlpEnsemble = new alglib.mlpensemble();
      string serializedEnsemble;
      alglib.mlpeserialize(original.mlpEnsemble, out serializedEnsemble);
      alglib.mlpeunserialize(serializedEnsemble, out this.mlpEnsemble);
      targetVariable = original.targetVariable;
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public NeuralNetworkEnsembleModel(alglib.mlpensemble mlpEnsemble, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues = null)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      this.mlpEnsemble = mlpEnsemble;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      if (classValues != null)
        this.classValues = (double[])classValues.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NeuralNetworkEnsembleModel(this, cloner);
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
        // mlpeprocess writes data in mlpEnsemble and is therefore not thread-safe
        lock (mlpEnsembleLocker) {
          alglib.mlpeprocess(mlpEnsemble, x, ref y);
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
        // mlpeprocess writes data in mlpEnsemble and is therefore not thread-safe
        lock (mlpEnsembleLocker) {
          alglib.mlpeprocess(mlpEnsemble, x, ref y);
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

      throw new ArgumentException("The problem data is not a regression nor a classification problem data. Instead a " + problemData.GetType().GetPrettyName() + " was provided.", "problemData");
    }

    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new NeuralNetworkEnsembleRegressionSolution(this, new RegressionEnsembleProblemData(problemData));
    }
    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new NeuralNetworkEnsembleClassificationSolution(this, new ClassificationEnsembleProblemData(problemData));
    }
   
    #region persistence
    [Storable]
    private string MultiLayerPerceptronEnsembleNetwork {
      get {
        string serializedNetwork;
        alglib.mlpeserialize(this.mlpEnsemble, out serializedNetwork);
        return serializedNetwork;
      }
      set {
        alglib.mlpeunserialize(value, out this.mlpEnsemble);
      }
    }

    [Storable]
    private double[] MultiLayerPerceptronEnsembleColumnMeans {
      get { return mlpEnsemble.innerobj.columnmeans; }
      set {
        mlpEnsemble.innerobj.columnmeans = value;
        mlpEnsemble.innerobj.network.columnmeans = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleColumnSigmas {
      get { return mlpEnsemble.innerobj.columnsigmas; }
      set {
        mlpEnsemble.innerobj.columnsigmas = value;
        mlpEnsemble.innerobj.network.columnsigmas = value;
      }
    }
    [Storable(AllowOneWay = true)]
    private double[] MultiLayerPerceptronEnsembleDfdnet {
      set {
        mlpEnsemble.innerobj.network.dfdnet = value;
      }
    }
    [Storable]
    private int MultiLayerPerceptronEnsembleSize {
      get { return mlpEnsemble.innerobj.ensemblesize; }
      set {
        mlpEnsemble.innerobj.ensemblesize = value;
        mlpEnsemble.innerobj.ensemblesize = value;
      }
    }
    [Storable(AllowOneWay = true)]
    private double[] MultiLayerPerceptronEnsembleNeurons {
      set { mlpEnsemble.innerobj.network.neurons = value; }
    }
    [Storable(AllowOneWay = true)]
    private double[] MultiLayerPerceptronEnsembleSerializedMlp {
      set {
        mlpEnsemble.innerobj.network.dfdnet = value;
      }
    }
    [Storable(AllowOneWay = true)]
    private int[] MultiLayerPerceptronStuctinfo {
      set {
        mlpEnsemble.innerobj.network.structinfo = value;
      }
    }

    [Storable]
    private double[] MultiLayerPerceptronWeights {
      get {
        return mlpEnsemble.innerobj.weights;
      }
      set {
        mlpEnsemble.innerobj.weights = value;
        mlpEnsemble.innerobj.network.weights = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronY {
      get {
        return mlpEnsemble.innerobj.y;
      }
      set {
        mlpEnsemble.innerobj.y = value;
        mlpEnsemble.innerobj.network.y = value;
      }
    }
    #endregion
  }
}
