using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.VariableNeighborhoodSearch {
  [Item("Variable Neighborhood Search", "A variable neighborhood search algorithm.")]
  [Creatable("Algorithms")]
  [StorableClass]
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
    private ValueParameter<IntValue> SeedParameter {
      get { return (ValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private ValueParameter<ILocalImprovementOperator> LocalImprovementParameter {
      get { return (ValueParameter<ILocalImprovementOperator>)Parameters["LocalImprovement"]; }
    }
    private ValueParameter<IShakingOperator> ShakingParameter {
      get { return (ValueParameter<IShakingOperator>)Parameters["Shaking"]; }
    }
    private ValueParameter<IntValue> MaximumIterationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private VariableNeighborhoodSearchMainLoop VNSMainLoop {
      get { return FindMainLoop(SolutionsCreator.Successor); }
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
    #endregion

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [StorableConstructor]
    private VariableNeighborhoodSearch(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {

    }
    private VariableNeighborhoodSearch(VariableNeighborhoodSearch original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableNeighborhoodSearch(this, cloner);
    }
    public VariableNeighborhoodSearch()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<ILocalImprovementOperator>("LocalImprovement", "The local improvement operation", new LocalSearchImprovementOperator()));
      Parameters.Add(new ValueParameter<IShakingOperator>("Shaking", "The shaking operation"));
      Parameters.Add(new ValueParameter<IntValue>("MaximumIterations", "The maximum number of generations which should be processed.", new IntValue(1000)));
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
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue()));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = mainLoop;

      mainLoop.LocalImprovementParameter.ActualName = LocalImprovementParameter.Name;
      mainLoop.ShakingParameter.ActualName = ShakingParameter.Name;
      mainLoop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      mainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
      UpdateAnalyzers();

      Initialize();
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    private void Initialize() {
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      }
      LocalImprovementParameter.ValueChanged += new EventHandler(LocalImprovementParameter_ValueChanged);
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      ParameterizeSolutionsCreator();
      ParameterizeVNSMainLoop();
      ParameterizeAnalyzers();
      ParameterizeIterationBasedOperators();
      UpdateShakingOperator();
      UpdateLocalImprovementOperator();
      UpdateAnalyzers();
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
      ParameterizeVNSMainLoop();
      ParameterizeAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (IOperator op in Problem.Operators) ParameterizeStochasticOperator(op);
      ParameterizeIterationBasedOperators();
      UpdateShakingOperator();
      UpdateLocalImprovementOperator();
      UpdateAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }

    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeVNSMainLoop();
      ParameterizeAnalyzers();
    }

    void LocalImprovementParameter_ValueChanged(object sender, EventArgs e) {
      if (LocalImprovementParameter.Value != null)
        LocalImprovementParameter.Value.OnProblemChanged(Problem);
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
    private void ParameterizeVNSMainLoop() {
      VNSMainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      VNSMainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      VNSMainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
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
          op.MaximumIterationsParameter.ActualName = "MaximumIterations";
        }
      }
    }
    private void UpdateShakingOperator() {
      Type manipulatorType = typeof(IManipulator);
      List<Type> manipulatorInterfaces = new List<Type>();

      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name)) {
        Type t = mutator.GetType();
        Type[] interfaces = t.GetInterfaces();

        for (int i = 0; i < interfaces.Length; i++) {
          if (manipulatorType.IsAssignableFrom(interfaces[i])) {
            bool assignable = false;
            for (int j = 0; j < interfaces.Length; j++) {
              if (i != j && interfaces[i].IsAssignableFrom(interfaces[j])) {
                assignable = true;
                break;
              }
            }

            if (!assignable)
              manipulatorInterfaces.Add(interfaces[i]);
          }
        }
      }

      foreach (Type manipulatorInterface in manipulatorInterfaces) {
        //manipulatorInterface is more specific
        if (manipulatorType.IsAssignableFrom(manipulatorInterface)) {
          //and compatible to all other found manipulator types
          bool compatible = true;
          foreach (Type manipulatorInterface2 in manipulatorInterfaces) {
            if (!manipulatorInterface.IsAssignableFrom(manipulatorInterface2)) {
              compatible = false;
              break;
            }
          }

          if (compatible)
            manipulatorType = manipulatorInterface;
        }
      }

      Type genericType = typeof(ShakingOperator<>).MakeGenericType(manipulatorType);
      ShakingParameter.Value = (IShakingOperator)Activator.CreateInstance(genericType, new object[] { });

      ShakingParameter.Value.OnProblemChanged(Problem);
    }
    private void UpdateLocalImprovementOperator() {
      LocalImprovementParameter.Value.OnProblemChanged(Problem);
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 1;
          Analyzer.Operators.Add(analyzer);
        }
      }
      Analyzer.Operators.Add(qualityAnalyzer);
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
