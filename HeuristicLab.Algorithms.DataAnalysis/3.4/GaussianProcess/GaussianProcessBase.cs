
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

using HeuristicLab.Algorithms.GradientDescent;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Base class for Gaussian process data analysis algorithms (regression and classification).
  /// </summary>
  [StorableClass]
  public abstract class GaussianProcessBase : EngineAlgorithm {
    protected const string MeanFunctionParameterName = "MeanFunction";
    protected const string CovarianceFunctionParameterName = "CovarianceFunction";
    protected const string MinimizationIterationsParameterName = "Iterations";
    protected const string ApproximateGradientsParameterName = "ApproximateGradients";
    protected const string SeedParameterName = "Seed";
    protected const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    protected const string ModelCreatorParameterName = "GaussianProcessModelCreator";
    protected const string NegativeLogLikelihoodParameterName = "NegativeLogLikelihood";
    protected const string HyperparameterParameterName = "Hyperparameter";
    protected const string HyperparameterGradientsParameterName = "HyperparameterGradients";
    protected const string SolutionCreatorParameterName = "GaussianProcessSolutionCreator";
    protected const string ScaleInputValuesParameterName = "ScaleInputValues";

    public new IDataAnalysisProblem Problem {
      get { return (IDataAnalysisProblem)base.Problem; }
      set { base.Problem = value; }
    }

    #region parameter properties
    public IValueParameter<IMeanFunction> MeanFunctionParameter {
      get { return (IValueParameter<IMeanFunction>)Parameters[MeanFunctionParameterName]; }
    }
    public IValueParameter<ICovarianceFunction> CovarianceFunctionParameter {
      get { return (IValueParameter<ICovarianceFunction>)Parameters[CovarianceFunctionParameterName]; }
    }
    public IValueParameter<IntValue> MinimizationIterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters[MinimizationIterationsParameterName]; }
    }
    public IValueParameter<IntValue> SeedParameter {
      get { return (IValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public IValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    public IFixedValueParameter<BoolValue> ScaleInputValuesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ScaleInputValuesParameterName]; }
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
    public int Seed { get { return SeedParameter.Value.Value; } set { SeedParameter.Value.Value = value; } }
    public bool SetSeedRandomly { get { return SetSeedRandomlyParameter.Value.Value; } set { SetSeedRandomlyParameter.Value.Value = value; } }

    public bool ScaleInputValues {
      get { return ScaleInputValuesParameter.Value.Value; }
      set { ScaleInputValuesParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected GaussianProcessBase(bool deserializing) : base(deserializing) { }
    protected GaussianProcessBase(GaussianProcessBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected GaussianProcessBase(IDataAnalysisProblem problem)
      : base() {
      Problem = problem;
      Parameters.Add(new ValueParameter<IMeanFunction>(MeanFunctionParameterName, "The mean function to use.", new MeanConst()));
      Parameters.Add(new ValueParameter<ICovarianceFunction>(CovarianceFunctionParameterName, "The covariance function to use.", new CovarianceSquaredExponentialIso()));
      Parameters.Add(new ValueParameter<IntValue>(MinimizationIterationsParameterName, "The number of iterations for likelihood optimization with LM-BFGS.", new IntValue(20)));
      Parameters.Add(new ValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));

      Parameters.Add(new ValueParameter<BoolValue>(ApproximateGradientsParameterName, "Indicates that gradients should not be approximated (necessary for LM-BFGS).", new BoolValue(false)));
      Parameters[ApproximateGradientsParameterName].Hidden = true; // should not be changed

      Parameters.Add(new FixedValueParameter<BoolValue>(ScaleInputValuesParameterName,
        "Determines if the input variable values are scaled to the range [0..1] for training.", new BoolValue(true)));
      Parameters[ScaleInputValuesParameterName].Hidden = true;

      // necessary for BFGS
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", new BoolValue(false)));
      Parameters["Maximization"].Hidden = true;

      var randomCreator = new HeuristicLab.Random.RandomCreator();
      var gpInitializer = new GaussianProcessHyperparameterInitializer();
      var bfgsInitializer = new LbfgsInitializer();
      var makeStep = new LbfgsMakeStep();
      var branch = new ConditionalBranch();
      var modelCreator = new Placeholder();
      var updateResults = new LbfgsUpdateResults();
      var analyzer = new LbfgsAnalyzer();
      var finalModelCreator = new Placeholder();
      var finalAnalyzer = new LbfgsAnalyzer();
      var solutionCreator = new Placeholder();

      OperatorGraph.InitialOperator = randomCreator;
      randomCreator.SeedParameter.ActualName = SeedParameterName;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameterName;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = gpInitializer;

      gpInitializer.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;
      gpInitializer.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      gpInitializer.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      gpInitializer.HyperparameterParameter.ActualName = HyperparameterParameterName;
      gpInitializer.RandomParameter.ActualName = randomCreator.RandomParameter.Name;
      gpInitializer.Successor = bfgsInitializer;

      bfgsInitializer.IterationsParameter.ActualName = MinimizationIterationsParameterName;
      bfgsInitializer.PointParameter.ActualName = HyperparameterParameterName;
      bfgsInitializer.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      bfgsInitializer.Successor = makeStep;

      makeStep.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      makeStep.PointParameter.ActualName = HyperparameterParameterName;
      makeStep.Successor = branch;

      branch.ConditionParameter.ActualName = makeStep.TerminationCriterionParameter.Name;
      branch.FalseBranch = modelCreator;
      branch.TrueBranch = finalModelCreator;

      modelCreator.OperatorParameter.ActualName = ModelCreatorParameterName;
      modelCreator.Successor = updateResults;

      updateResults.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      updateResults.QualityParameter.ActualName = NegativeLogLikelihoodParameterName;
      updateResults.QualityGradientsParameter.ActualName = HyperparameterGradientsParameterName;
      updateResults.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      updateResults.Successor = analyzer;

      analyzer.QualityParameter.ActualName = NegativeLogLikelihoodParameterName;
      analyzer.PointParameter.ActualName = HyperparameterParameterName;
      analyzer.QualityGradientsParameter.ActualName = HyperparameterGradientsParameterName;
      analyzer.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      analyzer.PointsTableParameter.ActualName = "Hyperparameter table";
      analyzer.QualityGradientsTableParameter.ActualName = "Gradients table";
      analyzer.QualitiesTableParameter.ActualName = "Negative log likelihood table";
      analyzer.Successor = makeStep;

      finalModelCreator.OperatorParameter.ActualName = ModelCreatorParameterName;
      finalModelCreator.Successor = finalAnalyzer;

      finalAnalyzer.QualityParameter.ActualName = NegativeLogLikelihoodParameterName;
      finalAnalyzer.PointParameter.ActualName = HyperparameterParameterName;
      finalAnalyzer.QualityGradientsParameter.ActualName = HyperparameterGradientsParameterName;
      finalAnalyzer.PointsTableParameter.ActualName = analyzer.PointsTableParameter.ActualName;
      finalAnalyzer.QualityGradientsTableParameter.ActualName = analyzer.QualityGradientsTableParameter.ActualName;
      finalAnalyzer.QualitiesTableParameter.ActualName = analyzer.QualitiesTableParameter.ActualName;
      finalAnalyzer.Successor = solutionCreator;

      solutionCreator.OperatorParameter.ActualName = SolutionCreatorParameterName;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      if (!Parameters.ContainsKey("Maximization")) {
        Parameters.Add(new ValueParameter<BoolValue>("Maximization", new BoolValue(false)));
        Parameters["Maximization"].Hidden = true;
      }

      if (!Parameters.ContainsKey(ScaleInputValuesParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(ScaleInputValuesParameterName,
          "Determines if the input variable values are scaled to the range [0..1] for training.", new BoolValue(true)));
        Parameters[ScaleInputValuesParameterName].Hidden = true;
      }
      #endregion
    }
  }
}
