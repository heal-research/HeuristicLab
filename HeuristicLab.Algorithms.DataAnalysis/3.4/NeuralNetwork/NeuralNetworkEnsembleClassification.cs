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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Neural network ensemble classification data analysis algorithm.
  /// </summary>
  [Item("Neural Network Ensemble Classification (NN)", "Neural network ensemble classification data analysis algorithm (wrapper for ALGLIB). Further documentation: http://www.alglib.net/dataanalysis/mlpensembles.php")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisClassification, Priority = 140)]
  [StorableClass]
  public sealed class NeuralNetworkEnsembleClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string EnsembleSizeParameterName = "EnsembleSize";
    private const string DecayParameterName = "Decay";
    private const string HiddenLayersParameterName = "HiddenLayers";
    private const string NodesInFirstHiddenLayerParameterName = "NodesInFirstHiddenLayer";
    private const string NodesInSecondHiddenLayerParameterName = "NodesInSecondHiddenLayer";
    private const string RestartsParameterName = "Restarts";
    private const string NeuralNetworkEnsembleClassificationModelResultName = "Neural network ensemble classification solution";

    #region parameter properties
    public IFixedValueParameter<IntValue> EnsembleSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[EnsembleSizeParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> DecayParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[DecayParameterName]; }
    }
    public IConstrainedValueParameter<IntValue> HiddenLayersParameter {
      get { return (IConstrainedValueParameter<IntValue>)Parameters[HiddenLayersParameterName]; }
    }
    public IFixedValueParameter<IntValue> NodesInFirstHiddenLayerParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[NodesInFirstHiddenLayerParameterName]; }
    }
    public IFixedValueParameter<IntValue> NodesInSecondHiddenLayerParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[NodesInSecondHiddenLayerParameterName]; }
    }
    public IFixedValueParameter<IntValue> RestartsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[RestartsParameterName]; }
    }
    #endregion

    #region properties
    public int EnsembleSize {
      get { return EnsembleSizeParameter.Value.Value; }
      set {
        if (value < 1) throw new ArgumentException("The number of models in the ensemble must be positive and at least one.", "EnsembleSize");
        EnsembleSizeParameter.Value.Value = value;
      }
    }
    public double Decay {
      get { return DecayParameter.Value.Value; }
      set {
        if (value < 0.001 || value > 100) throw new ArgumentException("The decay parameter should be set to a value between 0.001 and 100.", "Decay");
        DecayParameter.Value.Value = value;
      }
    }
    public int HiddenLayers {
      get { return HiddenLayersParameter.Value.Value; }
      set {
        if (value < 0 || value > 2) throw new ArgumentException("The number of hidden layers should be set to 0, 1, or 2.", "HiddenLayers");
        HiddenLayersParameter.Value = (from v in HiddenLayersParameter.ValidValues
                                       where v.Value == value
                                       select v)
                                      .Single();
      }
    }
    public int NodesInFirstHiddenLayer {
      get { return NodesInFirstHiddenLayerParameter.Value.Value; }
      set {
        if (value < 1) throw new ArgumentException("The number of nodes in the first hidden layer must be at least one.", "NodesInFirstHiddenLayer");
        NodesInFirstHiddenLayerParameter.Value.Value = value;
      }
    }
    public int NodesInSecondHiddenLayer {
      get { return NodesInSecondHiddenLayerParameter.Value.Value; }
      set {
        if (value < 1) throw new ArgumentException("The number of nodes in the first second layer must be at least one.", "NodesInSecondHiddenLayer");
        NodesInSecondHiddenLayerParameter.Value.Value = value;
      }
    }
    public int Restarts {
      get { return RestartsParameter.Value.Value; }
      set {
        if (value < 0) throw new ArgumentException("The number of restarts must be positive.", "Restarts");
        RestartsParameter.Value.Value = value;
      }
    }
    #endregion


    [StorableConstructor]
    private NeuralNetworkEnsembleClassification(bool deserializing) : base(deserializing) { }
    private NeuralNetworkEnsembleClassification(NeuralNetworkEnsembleClassification original, Cloner cloner)
      : base(original, cloner) {
    }
    public NeuralNetworkEnsembleClassification()
      : base() {
      var validHiddenLayerValues = new ItemSet<IntValue>(new IntValue[] {
        (IntValue)new IntValue(0).AsReadOnly(),
        (IntValue)new IntValue(1).AsReadOnly(),
        (IntValue)new IntValue(2).AsReadOnly() });
      var selectedHiddenLayerValue = (from v in validHiddenLayerValues
                                      where v.Value == 1
                                      select v)
                                     .Single();
      Parameters.Add(new FixedValueParameter<IntValue>(EnsembleSizeParameterName, "The number of simple neural network models in the ensemble. A good value is 10.", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(DecayParameterName, "The decay parameter for the training phase of the neural network. This parameter determines the strengh of regularization and should be set to a value between 0.001 (weak regularization) to 100 (very strong regularization). The correct value should be determined via cross-validation.", new DoubleValue(0.001)));
      Parameters.Add(new ConstrainedValueParameter<IntValue>(HiddenLayersParameterName, "The number of hidden layers for the neural network (0, 1, or 2)", validHiddenLayerValues, selectedHiddenLayerValue));
      Parameters.Add(new FixedValueParameter<IntValue>(NodesInFirstHiddenLayerParameterName, "The number of nodes in the first hidden layer. The value should be rather large (30-100 nodes) in order to make the network highly flexible and run into the early stopping criterion). This value is not used if the number of hidden layers is zero.", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<IntValue>(NodesInSecondHiddenLayerParameterName, "The number of nodes in the second hidden layer. This value is not used if the number of hidden layers is zero or one.", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<IntValue>(RestartsParameterName, "The number of restarts for learning.", new IntValue(2)));

      HiddenLayersParameter.Hidden = true;
      NodesInFirstHiddenLayerParameter.Hidden = true;
      NodesInSecondHiddenLayerParameter.Hidden = true;
      RestartsParameter.Hidden = true;

      Problem = new ClassificationProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NeuralNetworkEnsembleClassification(this, cloner);
    }

    #region neural network ensemble
    protected override void Run(CancellationToken cancellationToken) {
      double rmsError, avgRelError, relClassError;
      var solution = CreateNeuralNetworkEnsembleClassificationSolution(Problem.ProblemData, EnsembleSize, HiddenLayers, NodesInFirstHiddenLayer, NodesInSecondHiddenLayer, Decay, Restarts, out rmsError, out avgRelError, out relClassError);
      Results.Add(new Result(NeuralNetworkEnsembleClassificationModelResultName, "The neural network ensemble classification solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the neural network ensemble classification solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Average relative error", "The average of relative errors of the neural network ensemble classification solution on the training set.", new PercentValue(avgRelError)));
      Results.Add(new Result("Relative classification error", "The percentage of misclassified samples.", new PercentValue(relClassError)));
    }

    public static IClassificationSolution CreateNeuralNetworkEnsembleClassificationSolution(IClassificationProblemData problemData, int ensembleSize, int nLayers, int nHiddenNodes1, int nHiddenNodes2, double decay, int restarts,
      out double rmsError, out double avgRelError, out double relClassError) {
      var dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      double[,] inputMatrix = dataset.ToArray(allowedInputVariables.Concat(new string[] { targetVariable }), rows);
      if (inputMatrix.ContainsNanOrInfinity())
        throw new NotSupportedException("Neural network ensemble classification does not support NaN or infinity values in the input dataset.");

      int nRows = inputMatrix.GetLength(0);
      int nFeatures = inputMatrix.GetLength(1) - 1;
      double[] classValues = dataset.GetDoubleValues(targetVariable).Distinct().OrderBy(x => x).ToArray();
      int nClasses = classValues.Count();
      // map original class values to values [0..nClasses-1]
      Dictionary<double, double> classIndices = new Dictionary<double, double>();
      for (int i = 0; i < nClasses; i++) {
        classIndices[classValues[i]] = i;
      }
      for (int row = 0; row < nRows; row++) {
        inputMatrix[row, nFeatures] = classIndices[inputMatrix[row, nFeatures]];
      }

      alglib.mlpensemble mlpEnsemble = null;
      if (nLayers == 0) {
        alglib.mlpecreatec0(allowedInputVariables.Count(), nClasses, ensembleSize, out mlpEnsemble);
      } else if (nLayers == 1) {
        alglib.mlpecreatec1(allowedInputVariables.Count(), nHiddenNodes1, nClasses, ensembleSize, out mlpEnsemble);
      } else if (nLayers == 2) {
        alglib.mlpecreatec2(allowedInputVariables.Count(), nHiddenNodes1, nHiddenNodes2, nClasses, ensembleSize, out mlpEnsemble);
      } else throw new ArgumentException("Number of layers must be zero, one, or two.", "nLayers");
      alglib.mlpreport rep;

      int info;
      alglib.mlpetraines(mlpEnsemble, inputMatrix, nRows, decay, restarts, out info, out rep);
      if (info != 6) throw new ArgumentException("Error in calculation of neural network ensemble classification solution");

      rmsError = alglib.mlpermserror(mlpEnsemble, inputMatrix, nRows);
      avgRelError = alglib.mlpeavgrelerror(mlpEnsemble, inputMatrix, nRows);
      relClassError = alglib.mlperelclserror(mlpEnsemble, inputMatrix, nRows);
      var problemDataClone = (IClassificationProblemData)problemData.Clone();
      return new NeuralNetworkEnsembleClassificationSolution(new NeuralNetworkEnsembleModel(mlpEnsemble, targetVariable, allowedInputVariables, problemDataClone.ClassValues.ToArray()), problemDataClone);
    }
    #endregion
  }
}
