#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.RAPGA {
  /// <summary>
  /// A relevant alleles preserving genetic algorithm.
  /// </summary>
  [Item("RAPGA", "A relevant alleles preserving genetic algorithm (Affenzeller, M. et al. 2007. Self-adaptive population size adjustment for genetic algorithms. Proceedings of Computer Aided Systems Theory: EuroCAST 2007, Lecture Notes in Computer Science, pp 820–828. Springer).")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class RAPGA : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    private ValueParameter<IntValue> SeedParameter {
      get { return (ValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private ValueParameter<IntValue> PopulationSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    private IValueParameter<IntValue> MinimumPopulationSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["MinimumPopulationSize"]; }
    }
    private IValueParameter<IntValue> MaximumPopulationSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumPopulationSize"]; }
    }
    private IValueParameter<DoubleValue> ComparisonFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    private IValueParameter<IntValue> EffortParameter {
      get { return (IValueParameter<IntValue>)Parameters["Effort"]; }
    }
    private IValueParameter<IntValue> BatchSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["BatchSize"]; }
    }
    public IConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (IConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    public IConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (IConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    private ValueParameter<PercentValue> MutationProbabilityParameter {
      get { return (ValueParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public IConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (IConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private ValueParameter<IntValue> ElitesParameter {
      get { return (ValueParameter<IntValue>)Parameters["Elites"]; }
    }
    private IFixedValueParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private ValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public IConstrainedValueParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IConstrainedValueParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }
    #endregion

    #region Properties
    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public IntValue PopulationSize {
      get { return PopulationSizeParameter.Value; }
      set { PopulationSizeParameter.Value = value; }
    }
    public IntValue MinimumPopulationSize {
      get { return MinimumPopulationSizeParameter.Value; }
      set { MinimumPopulationSizeParameter.Value = value; }
    }
    public IntValue MaximumPopulationSize {
      get { return MaximumPopulationSizeParameter.Value; }
      set { MaximumPopulationSizeParameter.Value = value; }
    }
    public DoubleValue ComparisonFactor {
      get { return ComparisonFactorParameter.Value; }
      set { ComparisonFactorParameter.Value = value; }
    }
    public IntValue Effort {
      get { return EffortParameter.Value; }
      set { EffortParameter.Value = value; }
    }
    public IntValue BatchSize {
      get { return BatchSizeParameter.Value; }
      set { BatchSizeParameter.Value = value; }
    }
    public ISelector Selector {
      get { return SelectorParameter.Value; }
      set { SelectorParameter.Value = value; }
    }
    public ICrossover Crossover {
      get { return CrossoverParameter.Value; }
      set { CrossoverParameter.Value = value; }
    }
    public PercentValue MutationProbability {
      get { return MutationProbabilityParameter.Value; }
      set { MutationProbabilityParameter.Value = value; }
    }
    public IManipulator Mutator {
      get { return MutatorParameter.Value; }
      set { MutatorParameter.Value = value; }
    }
    public IntValue Elites {
      get { return ElitesParameter.Value; }
      set { ElitesParameter.Value = value; }
    }
    public bool ReevaluteElites {
      get { return ReevaluateElitesParameter.Value.Value; }
      set { ReevaluateElitesParameter.Value.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public IntValue MaximumGenerations {
      get { return MaximumGenerationsParameter.Value; }
      set { MaximumGenerationsParameter.Value = value; }
    }
    public ISolutionSimilarityCalculator SimilarityCalculator {
      get { return SimilarityCalculatorParameter.Value; }
      set { SimilarityCalculatorParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private RAPGAMainLoop RAPGAMainLoop {
      get { return FindMainLoop(SolutionsCreator.Successor); }
    }
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    [Storable]
    private PopulationSizeAnalyzer populationSizeAnalyzer;
    [Storable]
    private OffspringSuccessAnalyzer offspringSuccessAnalyzer;
    [Storable]
    private SelectionPressureAnalyzer selectionPressureAnalyzer;
    #endregion

    [StorableConstructor]
    private RAPGA(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("ReevaluateElites")) {
        Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", (BoolValue)new BoolValue(false).AsReadOnly()) { Hidden = true });
      }
      if (Parameters.ContainsKey("SimilarityCalculator")) {
#pragma warning disable 0618
        var oldParameter = (IConstrainedValueParameter<ISingleObjectiveSolutionSimilarityCalculator>)Parameters["SimilarityCalculator"];
#pragma warning restore 0618
        Parameters.Remove(oldParameter);
        var newParameter = new ConstrainedValueParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The operator used to calculate the similarity between two solutions.", new ItemSet<ISolutionSimilarityCalculator>(oldParameter.ValidValues));
        var selectedSimilarityCalculator = newParameter.ValidValues.SingleOrDefault(x => x.GetType() == oldParameter.Value.GetType());
        newParameter.Value = selectedSimilarityCalculator;
        Parameters.Add(newParameter);
      }
      #endregion
      Initialize();
    }
    private RAPGA(RAPGA original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      populationSizeAnalyzer = cloner.Clone(original.populationSizeAnalyzer);
      offspringSuccessAnalyzer = cloner.Clone(original.offspringSuccessAnalyzer);
      selectionPressureAnalyzer = cloner.Clone(original.selectionPressureAnalyzer);
      Initialize();
    }
    public RAPGA()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The size of the population of solutions.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MinimumPopulationSize", "The minimum size of the population of solutions.", new IntValue(2)));
      Parameters.Add(new ValueParameter<IntValue>("MaximumPopulationSize", "The maximum size of the population of solutions.", new IntValue(300)));
      Parameters.Add(new ValueParameter<DoubleValue>("ComparisonFactor", "The comparison factor.", new DoubleValue(0.0)));
      Parameters.Add(new ValueParameter<IntValue>("Effort", "The maximum number of offspring created in each generation.", new IntValue(1000)));
      Parameters.Add(new ValueParameter<IntValue>("BatchSize", "The number of children that should be created during one iteration of the offspring creation process.", new IntValue(10)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", new BoolValue(false)) { Hidden = true });
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze each generation.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new ConstrainedValueParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The operator used to calculate the similarity between two solutions."));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      SubScopesCounter subScopesCounter = new SubScopesCounter();
      ResultsCollector resultsCollector = new ResultsCollector();
      RAPGAMainLoop mainLoop = new RAPGAMainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      solutionsCreator.Successor = subScopesCounter;

      subScopesCounter.Name = "Initialize EvaluatedSolutions";
      subScopesCounter.ValueParameter.ActualName = "EvaluatedSolutions";
      subScopesCounter.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = mainLoop;

      mainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      mainLoop.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      mainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";
      mainLoop.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      mainLoop.ResultsParameter.ActualName = "Results";

      foreach (ISelector selector in ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name))
        SelectorParameter.ValidValues.Add(selector);
      ISelector proportionalSelector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("ProportionalSelector"));
      if (proportionalSelector != null) SelectorParameter.Value = proportionalSelector;
      ParameterizeSelectors();

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      populationSizeAnalyzer = new PopulationSizeAnalyzer();
      offspringSuccessAnalyzer = new OffspringSuccessAnalyzer();
      selectionPressureAnalyzer = new SelectionPressureAnalyzer();
      ParameterizeAnalyzers();
      UpdateAnalyzers();

      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RAPGA(this, cloner);
    }

    public override void Prepare() {
      if (Problem != null && SimilarityCalculator != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeSelectors();
      ParameterizeAnalyzers();
      ParameterizeIterationBasedOperators();
      UpdateCrossovers();
      UpdateMutators();
      UpdateAnalyzers();
      UpdateSimilarityCalculators();
      ParameterizeRAPGAMainLoop();
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
      ParameterizeRAPGAMainLoop();
      ParameterizeSelectors();
      ParameterizeAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeIterationBasedOperators();
      UpdateCrossovers();
      UpdateMutators();
      UpdateAnalyzers();
      UpdateSimilarityCalculators();
      ParameterizeRAPGAMainLoop();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void SimilarityCalculatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeRAPGAMainLoop();
    }
    private void BatchSizeParameter_ValueChanged(object sender, EventArgs e) {
      BatchSize.ValueChanged += new EventHandler(BatchSize_ValueChanged);
      ParameterizeSelectors();
    }
    private void BatchSize_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void ElitesParameter_ValueChanged(object sender, EventArgs e) {
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
      ParameterizeSelectors();
    }
    private void Elites_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }

    private void PopulationSizeParameter_ValueChanged(object sender, EventArgs e) {
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ParameterizeSelectors();
    }
    private void PopulationSize_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeRAPGAMainLoop();
      ParameterizeSelectors();
      ParameterizeAnalyzers();
      ParameterizeSimilarityCalculators();
    }
    #endregion

    #region Helpers
    private void Initialize() {
      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ElitesParameter.ValueChanged += new EventHandler(ElitesParameter_ValueChanged);
      Elites.ValueChanged += new EventHandler(Elites_ValueChanged);
      BatchSizeParameter.ValueChanged += new EventHandler(BatchSizeParameter_ValueChanged);
      BatchSize.ValueChanged += new EventHandler(BatchSize_ValueChanged);
      SimilarityCalculatorParameter.ValueChanged += new EventHandler(SimilarityCalculatorParameter_ValueChanged);
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      }
    }

    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeRAPGAMainLoop() {
      RAPGAMainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      RAPGAMainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      RAPGAMainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      IStochasticOperator stochasticOp = op as IStochasticOperator;
      if (stochasticOp != null) {
        stochasticOp.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
        stochasticOp.RandomParameter.Hidden = true;
      }
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in SelectorParameter.ValidValues) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue(2 * BatchSize.Value);
        selector.NumberOfSelectedSubScopesParameter.Hidden = true;
        ParameterizeStochasticOperator(selector);
      }
      if (Problem != null) {
        foreach (ISingleObjectiveSelector selector in SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.MaximizationParameter.Hidden = true;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
          selector.QualityParameter.Hidden = true;
        }
      }
    }
    private void ParameterizeAnalyzers() {
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      qualityAnalyzer.ResultsParameter.Hidden = true;
      populationSizeAnalyzer.ResultsParameter.ActualName = "Results";
      populationSizeAnalyzer.ResultsParameter.Hidden = true;
      offspringSuccessAnalyzer.ResultsParameter.ActualName = "Results";
      offspringSuccessAnalyzer.ResultsParameter.Hidden = true;
      selectionPressureAnalyzer.ResultsParameter.ActualName = "Results";
      selectionPressureAnalyzer.ResultsParameter.Hidden = true;
      if (Problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.MaximizationParameter.Hidden = true;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.QualityParameter.Depth = 1;
        qualityAnalyzer.QualityParameter.Hidden = true;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        qualityAnalyzer.BestKnownQualityParameter.Hidden = true;
      }
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (IIterationBasedOperator op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Generations";
          op.IterationsParameter.Hidden = true;
          op.MaximumIterationsParameter.ActualName = "MaximumGenerations";
          op.MaximumIterationsParameter.Hidden = true;
        }
      }
    }
    private void ParameterizeSimilarityCalculators() {
      foreach (ISolutionSimilarityCalculator calc in SimilarityCalculatorParameter.ValidValues) {
        calc.QualityVariableName = Problem.Evaluator.QualityParameter.ActualName;
      }
    }
    private void UpdateCrossovers() {
      ICrossover oldCrossover = CrossoverParameter.Value;
      CrossoverParameter.ValidValues.Clear();
      ICrossover defaultCrossover = Problem.Operators.OfType<ICrossover>().FirstOrDefault();

      foreach (ICrossover crossover in Problem.Operators.OfType<ICrossover>().OrderBy(x => x.Name))
        CrossoverParameter.ValidValues.Add(crossover);

      if (oldCrossover != null) {
        ICrossover crossover = CrossoverParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldCrossover.GetType());
        if (crossover != null) CrossoverParameter.Value = crossover;
        else oldCrossover = null;
      }
      if (oldCrossover == null && defaultCrossover != null)
        CrossoverParameter.Value = defaultCrossover;
    }
    private void UpdateMutators() {
      IManipulator oldMutator = MutatorParameter.Value;
      MutatorParameter.ValidValues.Clear();
      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name))
        MutatorParameter.ValidValues.Add(mutator);
      if (oldMutator != null) {
        IManipulator mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
        if (mutator != null) MutatorParameter.Value = mutator;
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
      Analyzer.Operators.Add(populationSizeAnalyzer, populationSizeAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(offspringSuccessAnalyzer, offspringSuccessAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(selectionPressureAnalyzer, selectionPressureAnalyzer.EnabledByDefault);
    }
    private void UpdateSimilarityCalculators() {
      ISolutionSimilarityCalculator oldSimilarityCalculator = SimilarityCalculatorParameter.Value;
      SimilarityCalculatorParameter.ValidValues.Clear();
      ISolutionSimilarityCalculator defaultSimilarityCalculator = Problem.Operators.OfType<ISolutionSimilarityCalculator>().FirstOrDefault();

      foreach (ISolutionSimilarityCalculator similarityCalculator in Problem.Operators.OfType<ISolutionSimilarityCalculator>())
        SimilarityCalculatorParameter.ValidValues.Add(similarityCalculator);

      if (!SimilarityCalculatorParameter.ValidValues.Any()) {
        SimilarityCalculatorParameter.ValidValues.Add(new QualitySimilarityCalculator { QualityVariableName = Problem.Evaluator.QualityParameter.ActualName });
        SimilarityCalculatorParameter.ValidValues.Add(new NoSimilarityCalculator());
      }

      if (oldSimilarityCalculator != null) {
        ISolutionSimilarityCalculator similarityCalculator = SimilarityCalculatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldSimilarityCalculator.GetType());
        if (similarityCalculator != null) SimilarityCalculatorParameter.Value = similarityCalculator;
        else oldSimilarityCalculator = null;
      }
      if (oldSimilarityCalculator == null && defaultSimilarityCalculator != null)
        SimilarityCalculatorParameter.Value = defaultSimilarityCalculator;
    }
    private RAPGAMainLoop FindMainLoop(IOperator start) {
      IOperator mainLoop = start;
      while (mainLoop != null && !(mainLoop is RAPGAMainLoop))
        mainLoop = ((SingleSuccessorOperator)mainLoop).Successor;
      if (mainLoop == null) return null;
      else return (RAPGAMainLoop)mainLoop;
    }
    #endregion
  }
}
