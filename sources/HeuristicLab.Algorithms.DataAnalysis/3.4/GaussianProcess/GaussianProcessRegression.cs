
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

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
      Problem = new RegressionProblem();

      List<IMeanFunction> meanFunctions = ApplicationManager.Manager.GetInstances<IMeanFunction>().ToList();
      List<ICovarianceFunction> covFunctions = ApplicationManager.Manager.GetInstances<ICovarianceFunction>().ToList();

      Parameters.Add(new ConstrainedValueParameter<IMeanFunction>(MeanFunctionParameterName, "The mean function to use.",
        new ItemSet<IMeanFunction>(meanFunctions), meanFunctions.First()));
      Parameters.Add(new ConstrainedValueParameter<ICovarianceFunction>(CovarianceFunctionParameterName, "The covariance function to use.",
        new ItemSet<ICovarianceFunction>(covFunctions), covFunctions.First()));
      Parameters.Add(new ValueParameter<IntValue>(MinimizationIterationsParameterName, "The number of iterations for likelihood optimization with BFGS.", new IntValue(20)));

      var setParameterLength = new GaussianProcessSetHyperparameterLength();
      var initializer = new BFGSInitializer();
      var makeStep = new BFGSMakeStep();
      var branch = new ConditionalBranch();
      var modelCreator = new GaussianProcessRegressionModelCreator();
      var updateResults = new BFGSUpdateResults();
      var analyzer = new BFGSAnalyzer();
      var finalModelCreator = new GaussianProcessRegressionModelCreator();
      var finalAnalyzer = new BFGSAnalyzer();
      var solutionCreator = new GaussianProcessRegressionSolutionCreator();

      OperatorGraph.InitialOperator = setParameterLength;

      setParameterLength.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;
      setParameterLength.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      setParameterLength.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      setParameterLength.Successor = initializer;

      initializer.IterationsParameter.ActualName = MinimizationIterationsParameterName;
      initializer.DimensionParameter.ActualName = setParameterLength.NumberOfHyperparameterParameter.Name;
      initializer.PointParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      initializer.Successor = makeStep;

      makeStep.BFGSStateParameter.ActualName = initializer.BFGSStateParameter.Name;
      makeStep.PointParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      makeStep.Successor = branch;

      branch.ConditionParameter.ActualName = makeStep.TerminationCriterionParameter.Name;
      branch.FalseBranch = modelCreator;
      branch.TrueBranch = finalModelCreator;

      modelCreator.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      modelCreator.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      modelCreator.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;
      modelCreator.Successor = updateResults;

      updateResults.BFGSStateParameter.ActualName = initializer.BFGSStateParameter.Name;
      updateResults.QualityParameter.ActualName = modelCreator.NegativeLogLikelihoodParameter.Name;
      updateResults.QualityGradientsParameter.ActualName = modelCreator.HyperparameterGradientsParameter.Name;
      updateResults.Successor = analyzer;

      analyzer.QualityParameter.ActualName = modelCreator.NegativeLogLikelihoodParameter.Name;
      analyzer.PointParameter.ActualName = modelCreator.HyperparameterParameter.Name;
      analyzer.QualityGradientsParameter.ActualName = modelCreator.HyperparameterGradientsParameter.Name;
      analyzer.BFGSStateParameter.ActualName = initializer.BFGSStateParameter.Name;
      analyzer.PointsTableParameter.ActualName = "Hyperparameter table";
      analyzer.QualityGradientsTableParameter.ActualName = "Gradients table";
      analyzer.QualitiesTableParameter.ActualName = "Negative log likelihood table";
      analyzer.Successor = makeStep;

      finalModelCreator.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      finalModelCreator.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      finalModelCreator.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;
      finalModelCreator.HyperparameterParameter.ActualName = initializer.PointParameter.ActualName;
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
