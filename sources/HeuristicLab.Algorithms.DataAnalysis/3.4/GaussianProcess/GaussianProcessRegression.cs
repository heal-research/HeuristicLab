
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
using HeuristicLab.Algorithms.GradientDescent;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  ///Gaussian process regression data analysis algorithm.
  /// </summary>
  [Item("Gaussian Process Regression", "Gaussian process regression data analysis algorithm.")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class GaussianProcessRegression : EngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    public override Type ProblemType { get { return typeof(IRegressionProblem); } }
    public new IRegressionProblem Problem {
      get { return (IRegressionProblem)base.Problem; }
      set { base.Problem = value; }
    }

    private const string MeanFunctionParameterName = "MeanFunction";
    private const string CovarianceFunctionParameterName = "CovarianceFunction";
    private const string MinimizationIterationsParameterName = "Iterations";
    private const string ApproximateGradientsParameterName = "ApproximateGradients";

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
      this.name = ItemName;
      this.description = ItemDescription;

      Problem = new RegressionProblem();

      List<IMeanFunction> meanFunctions = ApplicationManager.Manager.GetInstances<IMeanFunction>().ToList();
      List<ICovarianceFunction> covFunctions = ApplicationManager.Manager.GetInstances<ICovarianceFunction>().ToList();

      Parameters.Add(new ConstrainedValueParameter<IMeanFunction>(MeanFunctionParameterName, "The mean function to use.",
        new ItemSet<IMeanFunction>(meanFunctions), meanFunctions.First()));
      Parameters.Add(new ConstrainedValueParameter<ICovarianceFunction>(CovarianceFunctionParameterName, "The covariance function to use.",
        new ItemSet<ICovarianceFunction>(covFunctions), covFunctions.First()));
      Parameters.Add(new ValueParameter<IntValue>(MinimizationIterationsParameterName, "The number of iterations for likelihood optimization with LM-BFGS.", new IntValue(20)));
      Parameters.Add(new ValueParameter<BoolValue>(ApproximateGradientsParameterName, "Indicates that gradients should not be approximated (necessary for LM-BFGS).", new BoolValue(false)));
      Parameters[ApproximateGradientsParameterName].Hidden = true; // should not be changed

      var gpInitializer = new GaussianProcessHyperparameterInitializer();
      var bfgsInitializer = new LbfgsInitializer();
      var makeStep = new LbfgsMakeStep();
      var branch = new ConditionalBranch();
      var modelCreator = new GaussianProcessRegressionModelCreator();
      var updateResults = new LbfgsUpdateResults();
      var analyzer = new LbfgsAnalyzer();
      var finalModelCreator = new GaussianProcessRegressionModelCreator();
      var finalAnalyzer = new LbfgsAnalyzer();
      var solutionCreator = new GaussianProcessRegressionSolutionCreator();

      OperatorGraph.InitialOperator = gpInitializer;

      gpInitializer.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;
      gpInitializer.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      gpInitializer.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      gpInitializer.HyperparameterParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      gpInitializer.Successor = bfgsInitializer;

      bfgsInitializer.IterationsParameter.ActualName = MinimizationIterationsParameterName;
      bfgsInitializer.PointParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      bfgsInitializer.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      bfgsInitializer.Successor = makeStep;

      makeStep.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      makeStep.PointParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      makeStep.Successor = branch;

      branch.ConditionParameter.ActualName = makeStep.TerminationCriterionParameter.Name;
      branch.FalseBranch = modelCreator;
      branch.TrueBranch = finalModelCreator;

      modelCreator.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      modelCreator.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      modelCreator.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;
      modelCreator.Successor = updateResults;

      updateResults.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      updateResults.QualityParameter.ActualName = modelCreator.NegativeLogLikelihoodParameter.Name;
      updateResults.QualityGradientsParameter.ActualName = modelCreator.HyperparameterGradientsParameter.Name;
      updateResults.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      updateResults.Successor = analyzer;

      analyzer.QualityParameter.ActualName = modelCreator.NegativeLogLikelihoodParameter.Name;
      analyzer.PointParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      analyzer.QualityGradientsParameter.ActualName = modelCreator.HyperparameterGradientsParameter.Name;
      analyzer.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      analyzer.PointsTableParameter.ActualName = "Hyperparameter table";
      analyzer.QualityGradientsTableParameter.ActualName = "Gradients table";
      analyzer.QualitiesTableParameter.ActualName = "Negative log likelihood table";
      analyzer.Successor = makeStep;

      finalModelCreator.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      finalModelCreator.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      finalModelCreator.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;
      finalModelCreator.HyperparameterParameter.ActualName = bfgsInitializer.PointParameter.ActualName;
      finalModelCreator.Successor = finalAnalyzer;

      finalAnalyzer.QualityParameter.ActualName = modelCreator.NegativeLogLikelihoodParameter.Name;
      finalAnalyzer.PointParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      finalAnalyzer.QualityGradientsParameter.ActualName = modelCreator.HyperparameterGradientsParameter.Name;
      finalAnalyzer.PointsTableParameter.ActualName = analyzer.PointsTableParameter.ActualName;
      finalAnalyzer.QualityGradientsTableParameter.ActualName = analyzer.QualityGradientsTableParameter.ActualName;
      finalAnalyzer.QualitiesTableParameter.ActualName = analyzer.QualitiesTableParameter.ActualName;
      finalAnalyzer.Successor = solutionCreator;

      solutionCreator.ModelParameter.ActualName = finalModelCreator.ModelParameter.Name;
      solutionCreator.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessRegression(this, cloner);
    }
  }
}
