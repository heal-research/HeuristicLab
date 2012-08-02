
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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.TestFunctions;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Limited-Memory BFGS optimization algorithm.
  /// </summary>
  [Item("LM-BFGS", "The limited-memory BFGS (Broyden–Fletcher–Goldfarb–Shanno) optimization algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class LbfgsAlgorithm : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public override Type ProblemType {
      get { return typeof(SingleObjectiveTestFunctionProblem); }
    }

    public new SingleObjectiveTestFunctionProblem Problem {
      get { return (SingleObjectiveTestFunctionProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public string Filename { get; set; }

    private const string MaxIterationsParameterName = "MaxIterations";
    private const string ApproximateGradientsParameterName = "ApproximateGradients";

    #region parameter properties
    public IValueParameter<IntValue> MaxIterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters[MaxIterationsParameterName]; }
    }
    #endregion
    #region properties
    public int MaxIterations {
      set { MaxIterationsParameter.Value.Value = value; }
      get { return MaxIterationsParameter.Value.Value; }
    }
    #endregion
    [StorableConstructor]
    private LbfgsAlgorithm(bool deserializing) : base(deserializing) { }
    private LbfgsAlgorithm(LbfgsAlgorithm original, Cloner cloner)
      : base(original, cloner) {
    }
    public LbfgsAlgorithm()
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;

      Problem = new SingleObjectiveTestFunctionProblem();

      Parameters.Add(new ValueParameter<IntValue>(MaxIterationsParameterName, "The maximal number of iterations for.", new IntValue(20)));
      Parameters.Add(new ValueParameter<BoolValue>(ApproximateGradientsParameterName, "Indicates that gradients should be approximated.", new BoolValue(true)));
      Parameters[ApproximateGradientsParameterName].Hidden = true; // should not be changed

      var randomCreator = new RandomCreator();
      var solutionCreator = new Placeholder();
      var bfgsInitializer = new LbfgsInitializer();
      var makeStep = new LbfgsMakeStep();
      var branch = new ConditionalBranch();
      var evaluator = new Placeholder();
      var updateResults = new LbfgsUpdateResults();
      var analyzer = new LbfgsAnalyzer();
      var finalAnalyzer = new LbfgsAnalyzer();

      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.Successor = solutionCreator;

      solutionCreator.OperatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
      solutionCreator.Successor = bfgsInitializer;

      bfgsInitializer.IterationsParameter.ActualName = MaxIterationsParameterName;
      bfgsInitializer.PointParameter.ActualName = Problem.SolutionCreator.RealVectorParameter.ActualName;
      bfgsInitializer.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      bfgsInitializer.Successor = makeStep;

      makeStep.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      makeStep.PointParameter.ActualName = bfgsInitializer.PointParameter.ActualName;
      makeStep.Successor = branch;

      branch.ConditionParameter.ActualName = makeStep.TerminationCriterionParameter.Name;
      branch.FalseBranch = evaluator;
      branch.TrueBranch = finalAnalyzer;

      evaluator.OperatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      evaluator.Successor = updateResults;

      updateResults.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      updateResults.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.Name;
      updateResults.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      updateResults.Successor = analyzer;

      analyzer.QualityParameter.ActualName = updateResults.QualityParameter.ActualName;
      analyzer.PointParameter.ActualName = makeStep.PointParameter.ActualName;
      analyzer.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      analyzer.Successor = makeStep;

      finalAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.Name;
      finalAnalyzer.PointParameter.ActualName = makeStep.PointParameter.ActualName;
      finalAnalyzer.PointsTableParameter.ActualName = analyzer.PointsTableParameter.ActualName;
      finalAnalyzer.QualityGradientsTableParameter.ActualName = analyzer.QualityGradientsTableParameter.ActualName;
      finalAnalyzer.QualitiesTableParameter.ActualName = analyzer.QualitiesTableParameter.ActualName;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LbfgsAlgorithm(this, cloner);
    }
  }
}
