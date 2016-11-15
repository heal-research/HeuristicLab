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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Neural network ensemble regression data analysis algorithm.
  /// </summary>
  [Item("Neural Network Ensemble Regression (NN)", "Neural network ensemble regression data analysis algorithm (wrapper for ALGLIB). Further documentation: http://www.alglib.net/dataanalysis/mlpensembles.php")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 140)]
  [StorableClass]
  public sealed class NeuralNetworkEnsembleRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string EnsembleSizeParameterName = "EnsembleSize";
    private const string DecayParameterName = "Decay";
    private const string HiddenLayersParameterName = "HiddenLayers";
    private const string NodesInFirstHiddenLayerParameterName = "NodesInFirstHiddenLayer";
    private const string NodesInSecondHiddenLayerParameterName = "NodesInSecondHiddenLayer";
    private const string RestartsParameterName = "Restarts";
    private const string NeuralNetworkEnsembleRegressionModelResultName = "Neural network ensemble regression solution";

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
    private NeuralNetworkEnsembleRegression(bool deserializing) : base(deserializing) { }
    private NeuralNetworkEnsembleRegression(NeuralNetworkEnsembleRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public NeuralNetworkEnsembleRegression()
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

      Problem = new RegressionProblem();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NeuralNetworkEnsembleRegression(this, cloner);
    }

    #region neural network ensemble
    protected override void Run() {
      double rmsError, avgRelError;
      var solution = CreateNeuralNetworkEnsembleRegressionSolution(Problem.ProblemData, EnsembleSize, HiddenLayers, NodesInFirstHiddenLayer, NodesInSecondHiddenLayer, Decay, Restarts, out rmsError, out avgRelError);
      Results.Add(new Result(NeuralNetworkEnsembleRegressionModelResultName, "The neural network ensemble regression solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the neural network ensemble regression solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Average relative error", "The average of relative errors of the neural network ensemble regression solution on the training set.", new PercentValue(avgRelError)));
    }

    public static IRegressionSolution CreateNeuralNetworkEnsembleRegressionSolution(IRegressionProblemData problemData, int ensembleSize, int nLayers, int nHiddenNodes1, int nHiddenNodes2, double decay, int restarts,
      out double rmsError, out double avgRelError) {
      var dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      double[,] inputMatrix = dataset.ToArray(allowedInputVariables.Concat(new string[] { targetVariable }), rows);
      if (inputMatrix.Cast<double>().Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        throw new NotSupportedException("Neural network ensemble regression does not support NaN or infinity values in the input dataset.");

      alglib.mlpensemble mlpEnsemble = null;
      if (nLayers == 0) {
        alglib.mlpecreate0(allowedInputVariables.Count(), 1, ensembleSize, out mlpEnsemble);
      } else if (nLayers == 1) {
        alglib.mlpecreate1(allowedInputVariables.Count(), nHiddenNodes1, 1, ensembleSize, out mlpEnsemble);
      } else if (nLayers == 2) {
        alglib.mlpecreate2(allowedInputVariables.Count(), nHiddenNodes1, nHiddenNodes2, 1, ensembleSize, out mlpEnsemble);
      } else throw new ArgumentException("Number of layers must be zero, one, or two.", "nLayers");
      alglib.mlpreport rep;
      int nRows = inputMatrix.GetLength(0);

      int info;
      alglib.mlpetraines(mlpEnsemble, inputMatrix, nRows, decay, restarts, out info, out rep);
      if (info != 6) throw new ArgumentException("Error in calculation of neural network ensemble regression solution");

      rmsError = alglib.mlpermserror(mlpEnsemble, inputMatrix, nRows);
      avgRelError = alglib.mlpeavgrelerror(mlpEnsemble, inputMatrix, nRows);

      return new NeuralNetworkEnsembleRegressionSolution(new NeuralNetworkEnsembleModel(mlpEnsemble, targetVariable, allowedInputVariables), (IRegressionProblemData)problemData.Clone());
    }
    #endregion
  }
}
