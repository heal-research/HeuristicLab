#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.VariableNeighborhoodSearch {
  [Item("Variable Neighborhood Search (VNS)", "A variable neighborhood search algorithm based on the description in Mladenovic, N. and Hansen, P. (1997). Variable neighborhood search. Computers & Operations Research Volume 24, Issue 11, pp. 1097-1100.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 110)]
  [StorableType("E5FA1D1E-2611-4059-A836-4E89BB24342A")]
  public sealed class VariableNeighborhoodSearch : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(ISingleObjectiveHeuristicOptimizationProblem); }
    }
    public new ISingleObjectiveHeuristicOptimizationProblem Problem {
      get { return (ISingleObjectiveHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region Parameter Properties
    private FixedValueParameter<IntValue> SeedParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    public IConstrainedValueParameter<ILocalImprovementOperator> LocalImprovementParameter {
      get { return (IConstrainedValueParameter<ILocalImprovementOperator>)Parameters["LocalImprovement"]; }
    }
    public IConstrainedValueParameter<IMultiNeighborhoodShakingOperator> ShakingOperatorParameter {
      get { return (IConstrainedValueParameter<IMultiNeighborhoodShakingOperator>)Parameters["ShakingOperator"]; }
    }
    private FixedValueParameter<IntValue> MaximumIterationsParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    public FixedValueParameter<IntValue> LocalImprovementMaximumIterationsParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["LocalImprovementMaximumIterations"]; }
    }
    #endregion

    #region Properties
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private VariableNeighborhoodSearchMainLoop MainLoop {
      get { return FindMainLoop(SolutionsCreator.Successor); }
    }
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public ILocalImprovementOperator LocalImprovement {
      get { return LocalImprovementParameter.Value; }
      set { LocalImprovementParameter.Value = value; }
    }
    public IMultiNeighborhoodShakingOperator ShakingOperator {
      get { return ShakingOperatorParameter.Value; }
      set { ShakingOperatorParameter.Value = value; }
    }
    public int MaximumIterations {
      get { return MaximumIterationsParameter.Value.Value; }
      set { MaximumIterationsParameter.Value.Value = value; }
    }
    public int LocalImprovementMaximumIterations {
      get { return LocalImprovementMaximumIterationsParameter.Value.Value; }
      set { LocalImprovementMaximumIterationsParameter.Value.Value = value; }
    }
    #endregion

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [StorableConstructor]
    private VariableNeighborhoodSearch(StorableConstructorFlag _) : base(_) { }
    private VariableNeighborhoodSearch(VariableNeighborhoodSearch original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      RegisterEventHandlers();
    }
    public VariableNeighborhoodSearch()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ConstrainedValueParameter<ILocalImprovementOperator>("LocalImprovement", "The local improvement operation"));
      Parameters.Add(new ConstrainedValueParameter<IMultiNeighborhoodShakingOperator>("ShakingOperator", "The operator that performs the shaking of solutions."));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumIterations", "The maximum number of iterations which should be processed.", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<IntValue>("LocalImprovementMaximumIterations", "The maximum number of iterations which should be performed in the local improvement phase.", new IntValue(50)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze the solution and moves.", new MultiAnalyzer()));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      VariableCreator variableCreator = new VariableCreator();
      ResultsCollector resultsCollector = new ResultsCollector();
      VariableNeighborhoodSearchMainLoop mainLoop = new VariableNeighborhoodSearchMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutions = new IntValue(1);
      solutionsCreator.Successor = variableCreator;

      variableCreator.Name = "Initialize Evaluated Solutions";

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("CurrentNeighborhoodIndex", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("NeighborhoodCount", new IntValue(0)));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Iterations"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = mainLoop;

      mainLoop.IterationsParameter.ActualName = "Iterations";
      mainLoop.CurrentNeighborhoodIndexParameter.ActualName = "CurrentNeighborhoodIndex";
      mainLoop.NeighborhoodCountParameter.ActualName = "NeighborhoodCount";
      mainLoop.LocalImprovementParameter.ActualName = LocalImprovementParameter.Name;
      mainLoop.ShakingOperatorParameter.ActualName = ShakingOperatorParameter.Name;
      mainLoop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      mainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";

      InitializeLocalImprovementOperators();
      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
      UpdateAnalyzers();

      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableNeighborhoodSearch(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    private void RegisterEventHandlers() {
      LocalImprovementParameter.ValueChanged += new EventHandler(LocalImprovementParameter_ValueChanged);
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      }
    }

    #region Events
    protected override void OnProblemChanged() {
      InitializeLocalImprovementOperators();
      UpdateShakingOperators();
      UpdateAnalyzers();

      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
      ParameterizeIterationBasedOperators();
      ParameterizeLocalImprovementOperators();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.OnProblemChanged();
    }
    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeSolutionsCreator();
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.Evaluator);
      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      UpdateShakingOperators();
      UpdateAnalyzers();
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeIterationBasedOperators();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
    }
    private void LocalImprovementParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeLocalImprovementOperators();
    }
    #endregion

    #region Helpers
    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator)
        ((IStochasticOperator)op).RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }
    private void ParameterizeMainLoop() {
      MainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      MainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      MainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    }
    private void ParameterizeAnalyzers() {
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      if (Problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.QualityParameter.Depth = 1;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      }
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (IIterationBasedOperator op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Iterations";
          op.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
        }
      }
    }
    private void ParameterizeLocalImprovementOperators() {
      foreach (ILocalImprovementOperator op in LocalImprovementParameter.ValidValues) {
        op.MaximumIterationsParameter.Value = null;
        op.MaximumIterationsParameter.ActualName = LocalImprovementMaximumIterationsParameter.Name;

        var algOp = op as ILocalImprovementAlgorithmOperator;
        if (algOp != null && algOp != LocalImprovementParameter.Value) algOp.Problem = null;
      }
      if (LocalImprovementParameter.Value is ILocalImprovementAlgorithmOperator)
        ((ILocalImprovementAlgorithmOperator)LocalImprovementParameter.Value).Problem = Problem;
    }
    private void InitializeLocalImprovementOperators() {
      LocalImprovementParameter.ValidValues.Clear();
      if (Problem != null) {
        // Regular ILocalImprovementOperators queried from Problem
        foreach (var op in Problem.Operators.OfType<ILocalImprovementOperator>().Where(x => !(x is ILocalImprovementAlgorithmOperator))) {
          LocalImprovementParameter.ValidValues.Add(op);
        }
        // ILocalImprovementAlgorithmOperators queried from ApplicationManager
        var algOps = ApplicationManager.Manager.GetInstances<ILocalImprovementAlgorithmOperator>()
                                               .Where(x => x.ProblemType.IsInstanceOfType(Problem));
        foreach (var op in algOps) {
          if (LocalImprovementParameter.ValidValues.All(x => x.GetType() != op.GetType()))
            LocalImprovementParameter.ValidValues.Add(op);
        }
      }
    }
    private void UpdateShakingOperators() {
      IMultiNeighborhoodShakingOperator oldShakingOperator = ShakingOperator;
      IMultiNeighborhoodShakingOperator defaultShakingOperator = Problem.Operators.OfType<IMultiNeighborhoodShakingOperator>().FirstOrDefault();
      ShakingOperatorParameter.ValidValues.Clear();
      foreach (IMultiNeighborhoodShakingOperator op in Problem.Operators.OfType<IMultiNeighborhoodShakingOperator>().OrderBy(op => op.Name)) {
        ShakingOperatorParameter.ValidValues.Add(op);
        op.CurrentNeighborhoodIndexParameter.ActualName = "CurrentNeighborhoodIndex";
        op.NeighborhoodCountParameter.ActualName = "NeighborhoodCount";
      }
      if (oldShakingOperator != null) {
        IMultiNeighborhoodShakingOperator shakingOperator = ShakingOperatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldShakingOperator.GetType());
        if (shakingOperator != null) ShakingOperator = shakingOperator;
        else oldShakingOperator = null;
      }
      if (oldShakingOperator == null && defaultShakingOperator != null) {
        ShakingOperator = defaultShakingOperator;
      }
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 1;
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
      Analyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
    }
    private VariableNeighborhoodSearchMainLoop FindMainLoop(IOperator start) {
      IOperator mainLoop = start;
      while (mainLoop != null && !(mainLoop is VariableNeighborhoodSearchMainLoop))
        mainLoop = ((SingleSuccessorOperator)mainLoop).Successor;
      if (mainLoop == null) return null;
      else return (VariableNeighborhoodSearchMainLoop)mainLoop;
    }
    #endregion
  }
}
