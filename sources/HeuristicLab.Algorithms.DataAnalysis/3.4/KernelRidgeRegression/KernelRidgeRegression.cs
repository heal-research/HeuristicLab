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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Kernel Ridge Regression", "Kernelized ridge regression e.g. for radial basis function (RBF) regression.")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 100)]
  [StorableClass]
  public sealed class KernelRidgeRegression : BasicAlgorithm, IDataAnalysisAlgorithm<IRegressionProblem> {
    private const string SolutionResultName = "Kernel ridge regression solution";

    public override bool SupportsPause {
      get { return false; }
    }
    public override Type ProblemType {
      get { return typeof(IRegressionProblem); }
    }
    public new IRegressionProblem Problem {
      get { return (IRegressionProblem)base.Problem; }
      set { base.Problem = value; }
    }

    #region parameter names
    private const string KernelParameterName = "Kernel";
    private const string ScaleInputVariablesParameterName = "ScaleInputVariables";
    private const string LambdaParameterName = "LogLambda";
    private const string BetaParameterName = "Beta";
    #endregion

    #region parameter properties
    public IConstrainedValueParameter<IKernel> KernelParameter {
      get { return (IConstrainedValueParameter<IKernel>)Parameters[KernelParameterName]; }
    }

    public IFixedValueParameter<BoolValue> ScaleInputVariablesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ScaleInputVariablesParameterName]; }
    }

    public IFixedValueParameter<DoubleValue> LogLambdaParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[LambdaParameterName]; }
    }

    public IFixedValueParameter<DoubleValue> BetaParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[BetaParameterName]; }
    }
    #endregion

    #region properties
    public IKernel Kernel {
      get { return KernelParameter.Value; }
    }

    public bool ScaleInputVariables {
      get { return ScaleInputVariablesParameter.Value.Value; }
      set { ScaleInputVariablesParameter.Value.Value = value; }
    }

    public double LogLambda {
      get { return LogLambdaParameter.Value.Value; }
      set { LogLambdaParameter.Value.Value = value; }
    }

    public double Beta {
      get { return BetaParameter.Value.Value; }
      set { BetaParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private KernelRidgeRegression(bool deserializing) : base(deserializing) { }
    private KernelRidgeRegression(KernelRidgeRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public KernelRidgeRegression() {
      Problem = new RegressionProblem();
      var values = new ItemSet<IKernel>(ApplicationManager.Manager.GetInstances<IKernel>());
      Parameters.Add(new ConstrainedValueParameter<IKernel>(KernelParameterName, "The kernel", values, values.OfType<GaussianKernel>().FirstOrDefault()));
      Parameters.Add(new FixedValueParameter<BoolValue>(ScaleInputVariablesParameterName, "Set to true if the input variables should be scaled to the interval [0..1]", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(LambdaParameterName, "The log10-transformed weight for the regularization term lambda [-inf..+inf]. Small values produce more complex models, large values produce models with larger errors. Set to very small value (e.g. -1.0e15) for almost exact approximation", new DoubleValue(-2)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(BetaParameterName, "The inverse width of the kernel ]0..+inf]. The distance between points is divided by this value before being plugged into the kernel.", new DoubleValue(2)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KernelRidgeRegression(this, cloner);
    }

    protected override void Run(CancellationToken cancellationToken) {
      double rmsError, looCvRMSE;
      var kernel = Kernel;
      kernel.Beta = Beta;
      var solution = CreateRadialBasisRegressionSolution(Problem.ProblemData, kernel, Math.Pow(10, LogLambda), ScaleInputVariables, out rmsError, out looCvRMSE);
      Results.Add(new Result(SolutionResultName, "The kernel ridge regression solution.", solution));
      Results.Add(new Result("RMSE (test)", "The root mean squared error of the solution on the test set.", new DoubleValue(rmsError)));
      Results.Add(new Result("RMSE (LOO-CV)", "The leave-one-out-cross-validation root mean squared error", new DoubleValue(looCvRMSE)));
    }

    public static IRegressionSolution CreateRadialBasisRegressionSolution(IRegressionProblemData problemData, ICovarianceFunction kernel, double lambda, bool scaleInputs, out double rmsError, out double looCvRMSE) {
      var model = KernelRidgeRegressionModel.Create(problemData.Dataset, problemData.TargetVariable, problemData.AllowedInputVariables, problemData.TrainingIndices, scaleInputs, kernel, lambda);
      rmsError = double.NaN;
      if (problemData.TestIndices.Any()) {
        rmsError = Math.Sqrt(model.GetEstimatedValues(problemData.Dataset, problemData.TestIndices)
          .Zip(problemData.TargetVariableTestValues, (a, b) => (a - b) * (a - b))
          .Average());
      }
      var solution = model.CreateRegressionSolution((IRegressionProblemData)problemData.Clone());
      solution.Model.Name = "Kernel ridge regression model";
      solution.Name = SolutionResultName;
      looCvRMSE = model.LooCvRMSE;
      return solution;
    }
  }
}
