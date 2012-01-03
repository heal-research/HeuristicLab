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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a neural network ensembel model for regression and classification
  /// </summary>
  [StorableClass]
  [Item("NeuralNetworkEnsembleModel", "Represents a neural network ensemble for regression and classification.")]
  public sealed class NeuralNetworkEnsembleModel : NamedItem, INeuralNetworkEnsembleModel {

    private alglib.mlpensemble mlpEnsemble;
    public alglib.mlpensemble MultiLayerPerceptronEnsemble {
      get { return mlpEnsemble; }
      set {
        if (value != mlpEnsemble) {
          if (value == null) throw new ArgumentNullException();
          mlpEnsemble = value;
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
    private NeuralNetworkEnsembleModel(bool deserializing)
      : base(deserializing) {
      if (deserializing)
        mlpEnsemble = new alglib.mlpensemble();
    }
    private NeuralNetworkEnsembleModel(NeuralNetworkEnsembleModel original, Cloner cloner)
      : base(original, cloner) {
      mlpEnsemble = new alglib.mlpensemble();
      mlpEnsemble.innerobj.columnmeans = (double[])original.mlpEnsemble.innerobj.columnmeans.Clone();
      mlpEnsemble.innerobj.columnsigmas = (double[])original.mlpEnsemble.innerobj.columnsigmas.Clone();
      mlpEnsemble.innerobj.dfdnet = (double[])original.mlpEnsemble.innerobj.dfdnet.Clone();
      mlpEnsemble.innerobj.ensemblesize = original.mlpEnsemble.innerobj.ensemblesize;
      mlpEnsemble.innerobj.issoftmax = original.mlpEnsemble.innerobj.issoftmax;
      mlpEnsemble.innerobj.neurons = (double[])original.mlpEnsemble.innerobj.neurons.Clone();
      mlpEnsemble.innerobj.nin = original.mlpEnsemble.innerobj.nin;
      mlpEnsemble.innerobj.nout = original.mlpEnsemble.innerobj.nout;
      mlpEnsemble.innerobj.postprocessing = original.mlpEnsemble.innerobj.postprocessing;
      mlpEnsemble.innerobj.serializedlen = original.mlpEnsemble.innerobj.serializedlen;
      mlpEnsemble.innerobj.serializedmlp = (double[])original.mlpEnsemble.innerobj.serializedmlp.Clone();
      mlpEnsemble.innerobj.structinfo = (int[])original.mlpEnsemble.innerobj.structinfo.Clone();
      mlpEnsemble.innerobj.tmpmeans = (double[])original.mlpEnsemble.innerobj.tmpmeans.Clone();
      mlpEnsemble.innerobj.tmpsigmas = (double[])original.mlpEnsemble.innerobj.tmpsigmas.Clone();
      mlpEnsemble.innerobj.tmpweights = (double[])original.mlpEnsemble.innerobj.tmpweights.Clone();
      mlpEnsemble.innerobj.wcount = original.mlpEnsemble.innerobj.wcount;
      mlpEnsemble.innerobj.weights = (double[])original.mlpEnsemble.innerobj.weights.Clone();
      mlpEnsemble.innerobj.y = (double[])original.mlpEnsemble.innerobj.y.Clone();
      targetVariable = original.targetVariable;
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public NeuralNetworkEnsembleModel(alglib.mlpensemble mlpEnsemble, string targetVariable, IEnumerable<string> allowedInputVariables, double[] classValues = null)
      : base() {
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

    public IEnumerable<double> GetEstimatedValues(Dataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.mlpeprocess(mlpEnsemble, x, ref y);
        yield return y[0];
      }
    }

    public IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows) {
      double[,] inputData = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables, rows);

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] y = new double[classValues.Length];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        alglib.mlpeprocess(mlpEnsemble, x, ref y);
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

    public INeuralNetworkEnsembleRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new NeuralNetworkEnsembleRegressionSolution(problemData, this);
    }
    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return CreateRegressionSolution(problemData);
    }
    public INeuralNetworkEnsembleClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new NeuralNetworkEnsembleClassificationSolution(problemData, this);
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
    private double[] MultiLayerPerceptronEnsembleColumnMeans {
      get {
        return mlpEnsemble.innerobj.columnmeans;
      }
      set {
        mlpEnsemble.innerobj.columnmeans = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleColumnSigmas {
      get {
        return mlpEnsemble.innerobj.columnsigmas;
      }
      set {
        mlpEnsemble.innerobj.columnsigmas = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleDfdnet {
      get {
        return mlpEnsemble.innerobj.dfdnet;
      }
      set {
        mlpEnsemble.innerobj.dfdnet = value;
      }
    }
    [Storable]
    private int MultiLayerPerceptronEnsembleSize {
      get {
        return mlpEnsemble.innerobj.ensemblesize;
      }
      set {
        mlpEnsemble.innerobj.ensemblesize = value;
      }
    }
    [Storable]
    private bool MultiLayerPerceptronEnsembleIsSoftMax {
      get {
        return mlpEnsemble.innerobj.issoftmax;
      }
      set {
        mlpEnsemble.innerobj.issoftmax = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleNeurons {
      get {
        return mlpEnsemble.innerobj.neurons;
      }
      set {
        mlpEnsemble.innerobj.neurons = value;
      }
    }
    [Storable]
    private int MultiLayerPerceptronEnsembleNin {
      get {
        return mlpEnsemble.innerobj.nin;
      }
      set {
        mlpEnsemble.innerobj.nin = value;
      }
    }
    [Storable]
    private int MultiLayerPerceptronEnsembleNout {
      get {
        return mlpEnsemble.innerobj.nout;
      }
      set {
        mlpEnsemble.innerobj.nout = value;
      }
    }
    [Storable]
    private bool MultiLayerPerceptronEnsemblePostprocessing {
      get {
        return mlpEnsemble.innerobj.postprocessing;
      }
      set {
        mlpEnsemble.innerobj.postprocessing = value;
      }
    }
    [Storable]
    private int MultiLayerPerceptronEnsembleSerializedLen {
      get {
        return mlpEnsemble.innerobj.serializedlen;
      }
      set {
        mlpEnsemble.innerobj.serializedlen = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleSerializedMlp {
      get {
        return mlpEnsemble.innerobj.serializedmlp;
      }
      set {
        mlpEnsemble.innerobj.serializedmlp = value;
      }
    }
    [Storable]
    private int[] MultiLayerPerceptronStuctinfo {
      get {
        return mlpEnsemble.innerobj.structinfo;
      }
      set {
        mlpEnsemble.innerobj.structinfo = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleTmpMeans {
      get {
        return mlpEnsemble.innerobj.tmpmeans;
      }
      set {
        mlpEnsemble.innerobj.tmpmeans = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleTmpSigmas {
      get {
        return mlpEnsemble.innerobj.tmpsigmas;
      }
      set {
        mlpEnsemble.innerobj.tmpsigmas = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronEnsembleTmpWeights {
      get {
        return mlpEnsemble.innerobj.tmpweights;
      }
      set {
        mlpEnsemble.innerobj.tmpweights = value;
      }
    }
    [Storable]
    private int MultiLayerPerceptronEnsembleWCount {
      get {
        return mlpEnsemble.innerobj.wcount;
      }
      set {
        mlpEnsemble.innerobj.wcount = value;
      }
    }

    [Storable]
    private double[] MultiLayerPerceptronWeights {
      get {
        return mlpEnsemble.innerobj.weights;
      }
      set {
        mlpEnsemble.innerobj.weights = value;
      }
    }
    [Storable]
    private double[] MultiLayerPerceptronY {
      get {
        return mlpEnsemble.innerobj.y;
      }
      set {
        mlpEnsemble.innerobj.y = value;
      }
    }
    #endregion
  }
}
