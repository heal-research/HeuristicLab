
using System;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Random;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.MultiObjectiveLocalSearch {
  [Item("Simple Evolutionary Multiobjective Algorithm (G-SEMO)", "Global Simple Evolutionary MultiObjective Algorithm is implemented as described in the literature (e.g. Laumanns, M., Thiele, L. and Zitzler, E., 2004. Running time analysis of evolutionary algorithms on a simplified multiobjective knapsack problem. Natural Computing, 3(1), pp. 37-51), but adds the option to evaluate additional children in each generation.")]
  [StorableType("D9025144-B783-4484-B35B-7543C5A6D031")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms)]
  public class GSEMO : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public string Filename { get; set; }

    public override Type ProblemType => typeof(IMultiObjectiveHeuristicOptimizationProblem);

    public new IMultiObjectiveHeuristicOptimizationProblem Problem {
      get { return (IMultiObjectiveHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }

    private IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Seed"]; }
    }

    private IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }

    public IFixedValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }


    public IFixedValueParameter<IntValue> PopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["PopulationSize"]; }
    }

    public IFixedValueParameter<IntValue> OffspringParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Offspring"]; }
    }

    public IConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (IConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }

    public IValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }

    public IFixedValueParameter<BoolValue> DominateOnEqualQualitiesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["DominateOnEqualQualities"]; }
    }

    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }

    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }

    public int MaximumGenerations {
      get { return MaximumGenerationsParameter.Value.Value; }
      set { MaximumGenerationsParameter.Value.Value = value; }
    }

    public int PopulationSize {
      get { return PopulationSizeParameter.Value.Value; }
      set { PopulationSizeParameter.Value.Value = value; }
    }

    public int Offspring {
      get { return OffspringParameter.Value.Value; }
      set { OffspringParameter.Value.Value = value; }
    }

    public IManipulator Mutator {
      get { return MutatorParameter.Value; }
      set {
        if (!MutatorParameter.ValidValues.Contains(value))
          MutatorParameter.ValidValues.Add(value);
        MutatorParameter.Value = value;
      }
    }

    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }

    public bool DominateOnEqualQualities {
      get { return DominateOnEqualQualitiesParameter.Value.Value; }
      set { DominateOnEqualQualitiesParameter.Value.Value = value; }
    }

    [Storable]
    private RankBasedParetoFrontAnalyzer paretoFrontAnalyzer;
    [Storable]
    private RandomCreator randomCreator;
    [Storable]
    private SolutionsCreator solutionsCreator;
    [Storable]
    private CrowdingDistanceAssignment popSizeCrowding;

    [StorableConstructor]
    protected GSEMO(StorableConstructorFlag _) : base(_) { }
    protected GSEMO(GSEMO original, Cloner cloner)
    : base(original, cloner) {
      paretoFrontAnalyzer = cloner.Clone(original.paretoFrontAnalyzer);
      randomCreator = cloner.Clone(original.randomCreator);
      solutionsCreator = cloner.Clone(original.solutionsCreator);
      popSizeCrowding = cloner.Clone(original.popSizeCrowding);
      RegisterEventhandlers();
    }
    public GSEMO() {
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumGenerations", "Stopping criterion, terminates after this number of generations.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("PopulationSize", "The size of the pareto archive, the NSGA-II's rank and crowding sorter will be used to determine which solutions remain in the population.", new IntValue(int.MaxValue)));
      Parameters.Add(new FixedValueParameter<IntValue>("Offspring", "The number of offspring to generate in each generation.", new IntValue(1)));
      Parameters.Add(new ConstrainedValueParameter<IManipulator>("Mutator", "The operator that mutates a solutions slightly."));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze each generation.", new MultiAnalyzer()));
      Parameters.Add(new FixedValueParameter<BoolValue>("DominateOnEqualQualities", "Flag which determines wether solutions with equal quality values should be treated as dominated.", new BoolValue(false)) { Hidden = true });

      randomCreator = new RandomCreator();
      solutionsCreator = new SolutionsCreator();
      var countInitialEvaluatedSolutions = new SubScopesCounter();
      var resultsCollectorLoop = new ResultsCollector();
      var variableCreator = new VariableCreator();
      var analyzerLoop = new Placeholder();
      var selector = new RandomSelector();
      var generationProcessor = new SubScopesProcessor();
      var usspManipulation = new UniformSubScopesProcessor();
      var manipulatorPlaceholder = new Placeholder();
      var usspEvaluation = new UniformSubScopesProcessor();
      var evaluatorPlaceholder = new Placeholder();
      var countEvaluatedSolutions = new SubScopesCounter();
      var merge = new MergingReducer();
      var nonDominatedSort = new FastNonDominatedSort();
      var frontSelector = new LeftReducer();

      var popSizeCounter = new SubScopesCounter();
      var popSizeComparator = new Comparator();
      var popSizeBranch = new ConditionalBranch();
      popSizeCrowding = new CrowdingDistanceAssignment();
      var popSizeSorter = new CrowdedComparisonSorter();
      var popSizeSelector = new LeftSelector();
      var popSizeTrimmer = new RightReducer();

      var incrementGenerations = new IntCounter();
      var comparator = new Comparator();
      var conditionalBranch = new ConditionalBranch();
      var resultsCollectorFinish = new ResultsCollector();
      var analyzerFinish = new Placeholder();

      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = variableCreator;

      variableCreator.Name = "Generations := 0";
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      variableCreator.Successor = solutionsCreator;

      solutionsCreator.Name = "Create initial solution";
      solutionsCreator.NumberOfSolutionsParameter.Value = new IntValue(1);
      solutionsCreator.Successor = countInitialEvaluatedSolutions;

      countInitialEvaluatedSolutions.Name = "Initialize EvaluatedSolutions";
      countInitialEvaluatedSolutions.ValueParameter.ActualName = "EvaluatedSolutions";
      countInitialEvaluatedSolutions.Successor = resultsCollectorLoop;

      resultsCollectorLoop.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollectorLoop.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollectorLoop.ResultsParameter.ActualName = "Results";
      resultsCollectorLoop.Successor = analyzerLoop;

      analyzerLoop.Name = "(Analyzer)";
      analyzerLoop.OperatorParameter.ActualName = AnalyzerParameter.Name;
      analyzerLoop.Successor = selector;

      selector.NumberOfSelectedSubScopesParameter.Value = null;
      selector.NumberOfSelectedSubScopesParameter.ActualName = OffspringParameter.Name;
      selector.RandomParameter.ActualName = "Random";
      selector.CopySelected = new BoolValue(true);
      selector.Successor = generationProcessor;

      generationProcessor.Name = "Process Generations";
      generationProcessor.Operators.Add(new EmptyOperator() { Name = "Leave Parent Population" });
      generationProcessor.Operators.Add(usspManipulation);
      generationProcessor.Successor = merge;

      usspManipulation.Name = "Mutate Child Population";
      usspManipulation.Parallel = new BoolValue(false);
      usspManipulation.Operator = manipulatorPlaceholder;
      usspManipulation.Successor = usspEvaluation;

      manipulatorPlaceholder.Name = "(Manipulator)";
      manipulatorPlaceholder.OperatorParameter.ActualName = MutatorParameter.Name;

      usspEvaluation.Parallel = new BoolValue(true);
      usspEvaluation.Operator = evaluatorPlaceholder;
      usspEvaluation.Successor = countEvaluatedSolutions;

      evaluatorPlaceholder.Name = "(Evaluate)";
      evaluatorPlaceholder.OperatorParameter.ActualName = "Evaluator";

      countEvaluatedSolutions.Name = "Increment EvaluatedSolutions";
      countEvaluatedSolutions.ValueParameter.ActualName = "EvaluatedSolutions";

      merge.Successor = nonDominatedSort;

      nonDominatedSort.Name = "Calculate Fronts";
      nonDominatedSort.DominateOnEqualQualitiesParameter.ActualName = DominateOnEqualQualitiesParameter.Name;
      nonDominatedSort.Successor = frontSelector;

      frontSelector.Name = "Keep Best Front";
      frontSelector.Successor = popSizeCounter;

      popSizeCounter.Name = "Count Population";
      popSizeCounter.AccumulateParameter.Value = new BoolValue(false);
      popSizeCounter.ValueParameter.ActualName = "CurrentPopulationSize";
      popSizeCounter.Successor = popSizeComparator;

      popSizeComparator.Name = "CurrentPopulationSize > PopulationSize ?";
      popSizeComparator.LeftSideParameter.ActualName = "CurrentPopulationSize";
      popSizeComparator.RightSideParameter.Value = null;
      popSizeComparator.RightSideParameter.ActualName = PopulationSizeParameter.Name;
      popSizeComparator.Comparison.Value = ComparisonType.Greater;
      popSizeComparator.ResultParameter.ActualName = "TrimPopulation";
      popSizeComparator.Successor = popSizeBranch;

      popSizeBranch.Name = "Trim Population ?";
      popSizeBranch.ConditionParameter.ActualName = "TrimPopulation";
      popSizeBranch.TrueBranch = popSizeCrowding;
      popSizeBranch.Successor = incrementGenerations;

      popSizeCrowding.CrowdingDistanceParameter.Depth = 1;
      popSizeCrowding.Successor = popSizeSorter;

      popSizeSorter.CrowdingDistanceParameter.ActualName = popSizeCrowding.CrowdingDistanceParameter.ActualName;
      popSizeSorter.RankParameter.ActualName = nonDominatedSort.RankParameter.ActualName;
      popSizeSorter.RankParameter.Depth = 1;
      popSizeSorter.Successor = popSizeSelector;

      popSizeSelector.CopySelected = new BoolValue(false);
      popSizeSelector.NumberOfSelectedSubScopesParameter.Value = null;
      popSizeSelector.NumberOfSelectedSubScopesParameter.ActualName = PopulationSizeParameter.Name;
      popSizeSelector.Successor = popSizeTrimmer;

      popSizeTrimmer.Name = "Trim PopulationSize";
      popSizeTrimmer.Successor = null;

      incrementGenerations.Name = "Generations++";
      incrementGenerations.Increment = new IntValue(1);
      incrementGenerations.ValueParameter.ActualName = "Generations";
      incrementGenerations.Successor = comparator;

      comparator.Name = "Generations >= MaximumGenerations ?";
      comparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      comparator.LeftSideParameter.ActualName = "Generations";
      comparator.ResultParameter.ActualName = "Terminate";
      comparator.RightSideParameter.ActualName = "MaximumGenerations";
      comparator.Successor = conditionalBranch;

      conditionalBranch.Name = "Terminate?";
      conditionalBranch.ConditionParameter.ActualName = "Terminate";
      conditionalBranch.FalseBranch = resultsCollectorLoop;
      conditionalBranch.TrueBranch = resultsCollectorFinish;

      resultsCollectorFinish.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollectorFinish.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollectorFinish.ResultsParameter.ActualName = "Results";
      resultsCollectorFinish.Successor = analyzerFinish;

      analyzerFinish.Name = "(Analyzer)";
      analyzerFinish.OperatorParameter.ActualName = AnalyzerParameter.Name;

      paretoFrontAnalyzer = new RankBasedParetoFrontAnalyzer();
      paretoFrontAnalyzer.RankParameter.ActualName = "Rank";
      paretoFrontAnalyzer.RankParameter.Depth = 1;
      paretoFrontAnalyzer.ResultsParameter.ActualName = "Results";

      ParameterizeAnalyzers();
      UpdateAnalyzers();

      RegisterEventhandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GSEMO(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventhandlers();
    }

    private void RegisterEventhandlers() {
      if (Problem != null) {
        Problem.Evaluator.QualitiesParameter.ActualNameChanged += new EventHandler(Evaluator_QualitiesParameter_ActualNameChanged);
      }
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (var op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeAnalyzers();
      ParameterizeQualitiesOperators();
      ParameterizeIterationBasedOperators();
      UpdateMutators();
      UpdateAnalyzers();
      Problem.Evaluator.QualitiesParameter.ActualNameChanged += new EventHandler(Evaluator_QualitiesParameter_ActualNameChanged);
      base.OnProblemChanged();
    }

    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      ParameterizeStochasticOperator(Problem.Evaluator);
      ParameterizeSolutionsCreator();
      ParameterizeAnalyzers();
      ParameterizeQualitiesOperators();
      Problem.Evaluator.QualitiesParameter.ActualNameChanged += new EventHandler(Evaluator_QualitiesParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (var op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeIterationBasedOperators();
      UpdateMutators();
      UpdateAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void Evaluator_QualitiesParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeQualitiesOperators();
    }
    #endregion

    private void ParameterizeSolutionsCreator() {
      solutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      solutionsCreator.SolutionCreatorParameter.ActualName = SolutionCreatorParameter.Name;
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      var stochasticOp = op as IStochasticOperator;
      if (stochasticOp != null) {
        stochasticOp.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
        stochasticOp.RandomParameter.Hidden = true;
      }
    }
    private void ParameterizeAnalyzers() {
      paretoFrontAnalyzer.ResultsParameter.ActualName = "Results";
      paretoFrontAnalyzer.ResultsParameter.Hidden = true;
      if (Problem != null) {
        paretoFrontAnalyzer.QualitiesParameter.ActualName = Problem.Evaluator.QualitiesParameter.ActualName;
        paretoFrontAnalyzer.QualitiesParameter.Depth = 1;
        paretoFrontAnalyzer.QualitiesParameter.Hidden = true;
      }
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (var op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Generations";
          op.IterationsParameter.Hidden = true;
          op.MaximumIterationsParameter.ActualName = "MaximumGenerations";
          op.MaximumIterationsParameter.Hidden = true;
        }
      }
    }
    private void ParameterizeQualitiesOperators() {
      if (Problem != null) {
        popSizeCrowding.QualitiesParameter.ActualName = Problem.Evaluator.QualitiesParameter.Name;
      }
    }
    private void UpdateMutators() {
      var oldMutator = MutatorParameter.Value;
      MutatorParameter.ValidValues.Clear();
      var defaultMutator = Problem.Operators.Where(x => !(x is ISingleObjectiveOperator)).OfType<IManipulator>().FirstOrDefault();

      foreach (var mutator in Problem.Operators.Where(x => !(x is ISingleObjectiveOperator)).OfType<IManipulator>().OrderBy(x => x.Name))
        MutatorParameter.ValidValues.Add(mutator);

      if (oldMutator != null) {
        var mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
        if (mutator != null) MutatorParameter.Value = mutator;
        else oldMutator = null;
      }

      if (oldMutator == null && defaultMutator != null)
        MutatorParameter.Value = defaultMutator;
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (var analyzer in Problem.Operators.Where(x => !(x is ISingleObjectiveOperator)).OfType<IAnalyzer>()) {
          foreach (var param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 1;
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
      Analyzer.Operators.Add(paretoFrontAnalyzer, paretoFrontAnalyzer.EnabledByDefault);
    }

  }
}
