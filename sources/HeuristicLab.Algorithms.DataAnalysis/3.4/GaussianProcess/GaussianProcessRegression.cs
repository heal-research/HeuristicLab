
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis.GaussianProcess {
  /// <summary>
  ///Gaussian process regression data analysis algorithm.
  /// </summary>
  [Item("Gaussian Process Regression", "Gaussian process regression data analysis algorithm.")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class GaussianProcessRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string MeanFunctionParameterName = "MeanFunction";
    private const string CovarianceFunctionParameterName = "CovarianceFunction";
    private const string MinimizationIterationsParameterName = "MinimizationIterations";
    private const string NegativeLogLikelihoodTableParameterName = "NegativeLogLikelihoodTable";
    private const string HyperParametersTableParameterName = "HyperParametersTable";

    #region parameter properties
    public IConstrainedValueParameter<IMeanFunction> MeanFunctionParameter {
      get { return (IConstrainedValueParameter<IMeanFunction>)Parameters[MeanFunctionParameterName]; }
    }
    public IConstrainedValueParameter<ICovarianceFunction> CovarianceFunctionParameter {
      get { return (IConstrainedValueParameter<ICovarianceFunction>)Parameters[CovarianceFunctionParameterName]; }
    }
    public IValueParameter<IntValue> MinimizationIterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters[MinimizationIterationsParameterName]; }
    }
    //public ILookupParameter<DataTable> NegativeLogLikelihoodTableParameter {
    //  get { return (ILookupParameter<DataTable>)Parameters[NegativeLogLikelihoodTableParameterName]; }
    //}
    //public ILookupParameter<DataTable> HyperParametersTableParameter {
    //  get { return (ILookupParameter<DataTable>)Parameters[HyperParametersTableParameterName]; }
    //}
    #endregion
    #region properties
    public IMeanFunction MeanFunction {
      set { MeanFunctionParameter.Value = value; }
      get { return MeanFunctionParameter.Value; }
    }
    public ICovarianceFunction CovarianceFunction {
      set { CovarianceFunctionParameter.Value = value; }
      get { return CovarianceFunctionParameter.Value; }
    }
    public int MinimizationIterations {
      set { MinimizationIterationsParameter.Value.Value = value; }
      get { return MinimizationIterationsParameter.Value.Value; }
    }
    #endregion
    [StorableConstructor]
    private GaussianProcessRegression(bool deserializing) : base(deserializing) { }
    private GaussianProcessRegression(GaussianProcessRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public GaussianProcessRegression()
      : base() {
      Problem = new RegressionProblem();

      List<IMeanFunction> meanFunctions = ApplicationManager.Manager.GetInstances<IMeanFunction>().ToList();
      List<ICovarianceFunction> covFunctions = ApplicationManager.Manager.GetInstances<ICovarianceFunction>().ToList();

      Parameters.Add(new ConstrainedValueParameter<IMeanFunction>(MeanFunctionParameterName, "The mean function to use.",
        new ItemSet<IMeanFunction>(meanFunctions), meanFunctions.First()));
      Parameters.Add(new ConstrainedValueParameter<ICovarianceFunction>(CovarianceFunctionParameterName, "The covariance function to use.",
        new ItemSet<ICovarianceFunction>(covFunctions), covFunctions.First()));
      Parameters.Add(new ValueParameter<IntValue>(MinimizationIterationsParameterName, "The number of iterations for likelihood optimization.", new IntValue(20)));
      //Parameters.Add(new LookupParameter<DataTable>(NegativeLogLikelihoodTableParameterName, "The negative log likelihood values over the whole run."));
      //Parameters.Add(new LookupParameter<DataTable>(HyperParametersTableParameterName, "The values of the hyper-parameters over the whole run."));
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessRegression(this, cloner);
    }

    #region Gaussian process regression
    protected override void Run() {
      IRegressionProblemData problemData = Problem.ProblemData;

      int nAllowedVariables = problemData.AllowedInputVariables.Count();
      var mt = new MersenneTwister();

      var hyp0 =
        Enumerable.Range(0,
                         1 + MeanFunction.GetNumberOfParameters(nAllowedVariables) +
                         CovarianceFunction.GetNumberOfParameters(nAllowedVariables))
          .Select(i => mt.NextDouble())
          .ToArray();

      double[] hyp;

      // find hyperparameters

      double epsg = 0;
      double epsf = 0.00001;
      double epsx = 0;

      alglib.minlbfgsstate state;
      alglib.minlbfgsreport rep;

      alglib.minlbfgscreate(1, hyp0, out state);
      alglib.minlbfgssetcond(state, epsg, epsf, epsx, MinimizationIterations);
      alglib.minlbfgssetxrep(state, true);
      alglib.minlbfgsoptimize(state, OptimizeGaussianProcessParameters, Report, new object[] { MeanFunction, CovarianceFunction, problemData });
      alglib.minlbfgsresults(state, out hyp, out rep);


      double trainR2, testR2, negativeLogLikelihood;
      var solution = CreateGaussianProcessSolution(problemData, hyp, MeanFunction, CovarianceFunction,
        out negativeLogLikelihood, out trainR2, out testR2);

      Results.Add(new Result("Gaussian process regression solution", "The Gaussian process regression solution.", solution));
      Results.Add(new Result("Training R²", "The Pearson's R² of the Gaussian process solution on the training partition.", new DoubleValue(trainR2)));
      Results.Add(new Result("Test R²", "The Pearson's R² of the Gaussian process solution on the test partition.", new DoubleValue(testR2)));
      Results.Add(new Result("Negative log likelihood", "The negative log likelihood of the Gaussian process.", new DoubleValue(negativeLogLikelihood)));
    }

    public static GaussianProcessRegressionSolution CreateGaussianProcessSolution(IRegressionProblemData problemData,
      IEnumerable<double> hyp, IMeanFunction mean, ICovarianceFunction cov,
      out double negativeLogLikelihood, out double trainingR2, out double testR2) {

      Dataset dataset = problemData.Dataset;
      var allowedInputVariables = problemData.AllowedInputVariables;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<int> rows = problemData.TrainingIndices;

      var model = new GaussianProcessModel(dataset, targetVariable, allowedInputVariables, rows, hyp, mean, cov);
      var solution = new GaussianProcessRegressionSolution(model, (IRegressionProblemData)problemData.Clone());
      negativeLogLikelihood = model.NegativeLogLikelihood;
      trainingR2 = solution.TrainingRSquared;
      testR2 = solution.TestRSquared;
      return solution;
    }

    private static void OptimizeGaussianProcessParameters(double[] hyp, ref double func, double[] grad, object obj) {
      var objArr = (object[])obj;
      var meanFunction = (IMeanFunction)objArr[0];
      var covarianceFunction = (ICovarianceFunction)objArr[1];
      var problemData = (RegressionProblemData)objArr[2];
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;

      Dataset ds = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<int> rows = problemData.TrainingIndices;


      IEnumerable<double> dHyp;
      var model = new GaussianProcessModel(ds, targetVariable, allowedInputVariables, rows, hyp, meanFunction,
                                           covarianceFunction);
      dHyp = model.GetHyperparameterGradients();

      int i = 0;
      foreach (var e in dHyp) {
        grad[i++] = e;
      }
      func = model.NegativeLogLikelihood;
    }

    public void Report(double[] arg, double func, object obj) {
      if (!Results.ContainsKey(NegativeLogLikelihoodTableParameterName)) {
        Results.Add(new Result(NegativeLogLikelihoodTableParameterName, new DataTable()));
      }
      if (!Results.ContainsKey(HyperParametersTableParameterName)) {
        Results.Add(new Result(HyperParametersTableParameterName, new DataTable()));
      }

      var nllTable = (DataTable)Results[NegativeLogLikelihoodTableParameterName].Value;
      if (!nllTable.Rows.ContainsKey("Negative log likelihood"))
        nllTable.Rows.Add(new DataRow("Negative log likelihood"));
      var nllRow = nllTable.Rows["Negative log likelihood"];

      nllRow.Values.Add(func);

      var hypTable = (DataTable)Results[HyperParametersTableParameterName].Value;
      if (hypTable.Rows.Count == 0) {
        for (int i = 0; i < arg.Length; i++)
          hypTable.Rows.Add(new DataRow(i.ToString()));
      }
      for (int i = 0; i < arg.Length; i++) {
        hypTable.Rows[i.ToString()].Values.Add(arg[i]);
      }
    }

    #endregion
  }
}
