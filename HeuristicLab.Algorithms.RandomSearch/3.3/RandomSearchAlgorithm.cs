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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.RandomSearch {
  [Item("Random Search Algorithm (RS)", "A random search algorithm.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 150)]
  [StorableType("51EFBCCE-00A7-4E97-8177-85774E71E681")]
  public sealed class RandomSearchAlgorithm : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(IHeuristicOptimizationProblem); }
    }
    private ISingleObjectiveHeuristicOptimizationProblem SingleObjectiveProblem {
      get { return Problem as ISingleObjectiveHeuristicOptimizationProblem; }
    }
    private IMultiObjectiveHeuristicOptimizationProblem MultiObjectiveProblem {
      get { return Problem as IMultiObjectiveHeuristicOptimizationProblem; }
    }
    #endregion

    #region Parameter Properties
    private IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private IFixedValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IFixedValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private IFixedValueParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaximumEvaluatedSolutions"]; }
    }
    private IFixedValueParameter<IntValue> BatchSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["BatchSize"]; }
    }
    private IFixedValueParameter<IntValue> MaximumIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private IFixedValueParameter<MultiTerminator> TerminatorParameter {
      get { return (IFixedValueParameter<MultiTerminator>)Parameters["Terminator"]; }
    }
    #endregion

    #region Properties
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
    }
    public int MaximumEvaluatedSolutions {
      get { return MaximumEvaluatedSolutionsParameter.Value.Value; }
      set { MaximumEvaluatedSolutionsParameter.Value.Value = value; }
    }
    public int BatchSize {
      get { return BatchSizeParameter.Value.Value; }
      set { BatchSizeParameter.Value.Value = value; }
    }
    public int MaximumIterations {
      get { return MaximumIterationsParameter.Value.Value; }
      set { MaximumIterationsParameter.Value.Value = value; }
    }
    public MultiTerminator Terminators {
      get { return TerminatorParameter.Value; }
    }
    #endregion

    #region Helper Properties
    private SolutionsCreator SolutionsCreator {
      get { return OperatorGraph.Iterate().OfType<SolutionsCreator>().First(); }
    }
    #endregion

    #region Preconfigured Analyzers
    [Storable]
    private BestAverageWorstQualityAnalyzer singleObjectiveQualityAnalyzer;
    #endregion

    #region Preconfigured Terminators
    [Storable]
    private ComparisonTerminator<IntValue> evaluationsTerminator;
    [Storable]
    private ExecutionTimeTerminator executionTimeTerminator;
    [Storable]
    private SingleObjectiveQualityTerminator qualityTerminator;
    #endregion

    #region Constructors
    [StorableConstructor]
    private RandomSearchAlgorithm(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private RandomSearchAlgorithm(RandomSearchAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      singleObjectiveQualityAnalyzer = cloner.Clone(original.singleObjectiveQualityAnalyzer);
      evaluationsTerminator = cloner.Clone(original.evaluationsTerminator);
      qualityTerminator = cloner.Clone(original.qualityTerminator);
      executionTimeTerminator = cloner.Clone(original.executionTimeTerminator);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomSearchAlgorithm(this, cloner);
    }

    public RandomSearchAlgorithm()
      : base() {
      #region Add parameters
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the solutions each iteration.", new MultiAnalyzer()));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumEvaluatedSolutions", "The number of random solutions the algorithm should evaluate.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("BatchSize", "The number of random solutions that are evaluated (in parallel) per iteration.", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumIterations", "The number of iterations that the algorithm will run.", new IntValue(10)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<MultiTerminator>("Terminator", "The termination criteria that defines if the algorithm should continue or stop.", new MultiTerminator()) { Hidden = true });
      #endregion

      #region Create operators
      var randomCreator = new RandomCreator();
      var variableCreator = new VariableCreator() { Name = "Initialize Variables" };
      var resultsCollector = new ResultsCollector();
      var solutionCreator = new SolutionsCreator() { Name = "Create Solutions" };
      var analyzerPlaceholder = new Placeholder() { Name = "Analyzer (Placeholder)" };
      var evaluationsCounter = new IntCounter() { Name = "Increment EvaluatedSolutions" };
      var subScopesRemover = new SubScopesRemover();
      var iterationsCounter = new IntCounter() { Name = "Increment Iterations" };
      var terminationOperator = new TerminationOperator();
      #endregion

      #region Create and parameterize operator graph
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.SeedParameter.Value = null;
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.Successor = variableCreator;

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations", "The current iteration number."));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The current number of evaluated solutions."));
      resultsCollector.Successor = solutionCreator;

      solutionCreator.NumberOfSolutionsParameter.ActualName = BatchSizeParameter.Name;
      solutionCreator.ParallelParameter.Value.Value = true;
      solutionCreator.Successor = evaluationsCounter;

      evaluationsCounter.ValueParameter.ActualName = "EvaluatedSolutions";
      evaluationsCounter.Increment = null;
      evaluationsCounter.IncrementParameter.ActualName = BatchSizeParameter.Name;
      evaluationsCounter.Successor = analyzerPlaceholder;

      analyzerPlaceholder.OperatorParameter.ActualName = AnalyzerParameter.Name;
      analyzerPlaceholder.Successor = subScopesRemover;

      subScopesRemover.RemoveAllSubScopes = true;
      subScopesRemover.Successor = iterationsCounter;

      iterationsCounter.ValueParameter.ActualName = "Iterations";
      iterationsCounter.Increment = new IntValue(1);
      iterationsCounter.Successor = terminationOperator;

      terminationOperator.TerminatorParameter.ActualName = TerminatorParameter.Name;
      terminationOperator.ContinueBranch = solutionCreator;
      #endregion

      #region Create analyzers
      singleObjectiveQualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      #endregion

      #region Create terminators
      evaluationsTerminator = new ComparisonTerminator<IntValue>("EvaluatedSolutions", ComparisonType.Less, MaximumEvaluatedSolutionsParameter) { Name = "Evaluated solutions." };
      qualityTerminator = new SingleObjectiveQualityTerminator() { Name = "Quality" };
      executionTimeTerminator = new ExecutionTimeTerminator(this, new TimeSpanValue(TimeSpan.FromMinutes(5)));
      #endregion

      #region Parameterize
      UpdateAnalyzers();
      ParameterizeAnalyzers();
      UpdateTerminators();
      #endregion

      Initialize();
    }
    #endregion

    #region Events
    public override void Prepare() {
      if (Problem != null)
        base.Prepare();
    }
    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      foreach (var @operator in Problem.Operators.OfType<IOperator>())
        ParameterizeStochasticOperator(@operator);

      ParameterizeIterationBasedOperators();

      ParameterizeSolutionsCreator();
      ParameterizeAnalyzers();
      ParameterizeTerminators();

      if (SingleObjectiveProblem != null)
        SingleObjectiveProblem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;

      UpdateAnalyzers();
      UpdateTerminators();
    }

    protected override void RegisterProblemEvents() {
      base.RegisterProblemEvents();
      if (SingleObjectiveProblem != null) {
        var maximizationParameter = (IValueParameter<BoolValue>)SingleObjectiveProblem.MaximizationParameter;
        if (maximizationParameter != null) maximizationParameter.ValueChanged += new EventHandler(MaximizationParameter_ValueChanged);
      }
    }
    protected override void DeregisterProblemEvents() {
      if (SingleObjectiveProblem != null) {
        var maximizationParameter = (IValueParameter<BoolValue>)SingleObjectiveProblem.MaximizationParameter;
        if (maximizationParameter != null) maximizationParameter.ValueChanged -= new EventHandler(MaximizationParameter_ValueChanged);
      }
      base.DeregisterProblemEvents();
    }

    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      base.Problem_SolutionCreatorChanged(sender, e);
      ParameterizeStochasticOperator(Problem.SolutionCreator);

      if (SingleObjectiveProblem != null)
        SingleObjectiveProblem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;

      ParameterizeSolutionsCreator();
      ParameterizeAnalyzers();
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      base.Problem_EvaluatorChanged(sender, e);
      foreach (var @operator in Problem.Operators.OfType<IOperator>())
        ParameterizeStochasticOperator(@operator);

      UpdateAnalyzers();

      ParameterizeSolutionsCreator();
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      base.Problem_OperatorsChanged(sender, e);
      ParameterizeIterationBasedOperators();
      UpdateTerminators();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
    }
    private void MaximizationParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeTerminators();
    }
    private void QualityAnalyzer_CurrentBestQualityParameter_NameChanged(object sender, EventArgs e) {
      ParameterizeTerminators();
    }

    #endregion

    #region Parameterization
    private void Initialize() {
      if (SingleObjectiveProblem != null)
        SingleObjectiveProblem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;

      singleObjectiveQualityAnalyzer.CurrentBestQualityParameter.NameChanged += QualityAnalyzer_CurrentBestQualityParameter_NameChanged;

      MaximumEvaluatedSolutionsParameter.Value.ValueChanged += MaximumEvaluatedSolutions_ValueChanged;
      BatchSizeParameter.Value.ValueChanged += BatchSize_ValueChanged;
    }
    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeAnalyzers() {
      singleObjectiveQualityAnalyzer.ResultsParameter.ActualName = "Results";
      singleObjectiveQualityAnalyzer.ResultsParameter.Hidden = true;
      singleObjectiveQualityAnalyzer.QualityParameter.Depth = 1;

      if (SingleObjectiveProblem != null) {
        singleObjectiveQualityAnalyzer.MaximizationParameter.ActualName = SingleObjectiveProblem.MaximizationParameter.Name;
        singleObjectiveQualityAnalyzer.MaximizationParameter.Hidden = true;
        singleObjectiveQualityAnalyzer.QualityParameter.ActualName = SingleObjectiveProblem.Evaluator.QualityParameter.ActualName;
        singleObjectiveQualityAnalyzer.QualityParameter.Hidden = true;
        singleObjectiveQualityAnalyzer.BestKnownQualityParameter.ActualName = SingleObjectiveProblem.BestKnownQualityParameter.Name;
        singleObjectiveQualityAnalyzer.BestKnownQualityParameter.Hidden = true;
      }
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (IIterationBasedOperator op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Iterations";
          op.IterationsParameter.Hidden = true;
          op.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
        }
      }
    }
    private void ParameterizeTerminators() {
      if (SingleObjectiveProblem != null)
        qualityTerminator.Parameterize(singleObjectiveQualityAnalyzer.CurrentBestQualityParameter, SingleObjectiveProblem);
    }
    private void ParameterizeStochasticOperator(IOperator @operator) {
      var stochasticOperator = @operator as IStochasticOperator;
      if (stochasticOperator != null) {
        stochasticOperator.RandomParameter.ActualName = "Random";
        stochasticOperator.RandomParameter.Hidden = true;
      }
    }
    private void MaximumEvaluatedSolutions_ValueChanged(object sender, EventArgs e) {
      MaximumIterations = MaximumEvaluatedSolutions / BatchSize;
    }
    private void BatchSize_ValueChanged(object sender, EventArgs e) {
      MaximumIterations = MaximumEvaluatedSolutions / BatchSize;
    }
    #endregion

    #region Updates
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();

      if (Problem != null) {
        if (SingleObjectiveProblem != null)
          Analyzer.Operators.Add(singleObjectiveQualityAnalyzer, singleObjectiveQualityAnalyzer.EnabledByDefault);
        foreach (var analyzer in Problem.Operators.OfType<IAnalyzer>())
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
      }
    }
    private void UpdateTerminators() {
      var newTerminators = new Dictionary<ITerminator, bool> {
        {evaluationsTerminator, !Terminators.Operators.Contains(evaluationsTerminator) || Terminators.Operators.ItemChecked(evaluationsTerminator)},
        {executionTimeTerminator, Terminators.Operators.Contains(executionTimeTerminator) && Terminators.Operators.ItemChecked(executionTimeTerminator)}
      };
      if (Problem != null) {
        if (SingleObjectiveProblem != null)
          newTerminators.Add(qualityTerminator, Terminators.Operators.Contains(qualityTerminator) && Terminators.Operators.ItemChecked(qualityTerminator));
        foreach (var terminator in Problem.Operators.OfType<ITerminator>())
          newTerminators.Add(terminator, !Terminators.Operators.Contains(terminator) || Terminators.Operators.ItemChecked(terminator));
      }

      Terminators.Operators.Clear();
      foreach (var newTerminator in newTerminators)
        Terminators.Operators.Add(newTerminator.Key, newTerminator.Value);
    }
    #endregion
  }
}