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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a neural network model for regression and classification
  /// </summary>
  [StorableClass]
  [Item("NeuralNetworkModel", "Represents a neural network for regression and classification.")]
  public sealed class NeuralNetworkModel : NamedItem, INeuralNetworkModel {

    private alglib.multilayerperceptron multiLayerPerceptron;
    public alglib.multilayerperceptron MultiLayerPerceptron {
      get { return multiLayerPerceptron; }
      set {
        if (value != multiLayerPerceptron) {
          if (value == null) throw new ArgumentNullException();
          multiLayerPerceptron = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable]
    private string targetVariable;
    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [StorableConstructor]
    private NeuralNetworkModel(bool deserializing)
      : base(deserializing) {
      if (deserializing)
        multiLayerPerceptron = new alglib.multilayerperceptron();
    }
    private NeuralNetworkModel(NeuralNetworkModel original, Cloner cloner)
      : base(original, cloner) {
      multiLayerPerceptron = new alglib.multilayerperceptron();

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
      targetVariable = original.targetVariable;
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public NeuralNetworkModel(alglib.multilayerperceptron multiLayerPerceptron, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues = null)
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
      this.multiLayerPerceptron = multiLayerPerceptron;
      this.targetVariable = targetVariable;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      if (classValues != null)
        this.classValues = (double[])classValues.Clone();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NeuralNetworkModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.mlpprocess(multiLayerPerceptron, x, ref y);
        yield return y[0];
      }
    }

    public IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[classValues.Length];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.mlpprocess(multiLayerPerceptron, x, ref y);
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

    public INeuralNetworkRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new NeuralNetworkRegressionSolution(new RegressionProblemData(problemData), this);
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }
    public INeuralNetworkClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new NeuralNetworkClassificationSolution(new ClassificationProblemData(problemData), this);
    }
    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateClassificationSolution(problemData);
    }

    #region events
    public event EventHandler Changed;
    private void OnChanged(EventArgs e) {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
    #endregion

    #region persistence
    [Storable]
    private double[,] MultiLayerPerceptronChunks {
      get {
        // ignore this property in alglib version >= 3.9.0
        return null;
      }
      set {
        // ignore this property in alglib version >= 3.9.0
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