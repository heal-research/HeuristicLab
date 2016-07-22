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
using HeuristicLab.Random;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.ScatterSearch {
  /// <summary>
  /// A scatter search algorithm.
  /// </summary>
  [Item("Scatter Search (SS)", "A scatter search algorithm.")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 500)]
  [StorableClass]
  public sealed class ScatterSearch : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    public IValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    public IConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (IConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    public IValueParameter<BoolValue> ExecutePathRelinkingParameter {
      get { return (IValueParameter<BoolValue>)Parameters["ExecutePathRelinking"]; }
    }
    public IConstrainedValueParameter<IImprovementOperator> ImproverParameter {
      get { return (IConstrainedValueParameter<IImprovementOperator>)Parameters["Improver"]; }
    }
    public IValueParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public IValueParameter<IntValue> NumberOfHighQualitySolutionsParameter {
      get { return (IValueParameter<IntValue>)Parameters["NumberOfHighQualitySolutions"]; }
    }
    public IConstrainedValueParameter<IPathRelinker> PathRelinkerParameter {
      get { return (IConstrainedValueParameter<IPathRelinker>)Parameters["PathRelinker"]; }
    }
    public IValueParameter<IntValue> PopulationSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public IValueParameter<IntValue> ReferenceSetSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["ReferenceSetSize"]; }
    }
    public IValueParameter<IntValue> SeedParameter {
      get { return (IValueParameter<IntValue>)Parameters["Seed"]; }
    }
    public IValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    public IConstrainedValueParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IConstrainedValueParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }
    #endregion

    #region Properties
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public ICrossover Crossover {
      get { return CrossoverParameter.Value; }
      set { CrossoverParameter.Value = value; }
    }
    public BoolValue ExecutePathRelinking {
      get { return ExecutePathRelinkingParameter.Value; }
      set { ExecutePathRelinkingParameter.Value = value; }
    }
    public IImprovementOperator Improver {
      get { return ImproverParameter.Value; }
      set { ImproverParameter.Value = value; }
    }
    public IntValue MaximumIterations {
      get { return MaximumIterationsParameter.Value; }
      set { MaximumIterationsParameter.Value = value; }
    }
    public IntValue NumberOfHighQualitySolutions {
      get { return NumberOfHighQualitySolutionsParameter.Value; }
      set { NumberOfHighQualitySolutionsParameter.Value = value; }
    }
    public IPathRelinker PathRelinker {
      get { return PathRelinkerParameter.Value; }
      set { PathRelinkerParameter.Value = value; }
    }
    public IntValue PopulationSize {
      get { return PopulationSizeParameter.Value; }
      set { PopulationSizeParameter.Value = value; }
    }
    public IntValue ReferenceSetSize {
      get { return ReferenceSetSizeParameter.Value; }
      set { ReferenceSetSizeParameter.Value = value; }
    }
    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
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
    private ScatterSearchMainLoop MainLoop {
      get { return FindMainLoop(SolutionsCreator.Successor); }
    }

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    #endregion

    [StorableConstructor]
    private ScatterSearch(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
#pragma warning disable 0618
      if (Parameters.ContainsKey("SimilarityCalculator") && Parameters["SimilarityCalculator"] is IConstrainedValueParameter<ISingleObjectiveSolutionSimilarityCalculator>) {
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
    private ScatterSearch(ScatterSearch original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterSearch(this, cloner);
    }
    public ScatterSearch()
      : base() {
      #region Create parameters
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The analyzer used to analyze each iteration.", new MultiAnalyzer()));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<BoolValue>("ExecutePathRelinking", "True if path relinking should be executed instead of crossover, otherwise false.", new BoolValue(false)));
      Parameters.Add(new ConstrainedValueParameter<IImprovementOperator>("Improver", "The operator used to improve solutions."));
      Parameters.Add(new ValueParameter<IntValue>("MaximumIterations", "The maximum number of iterations which should be processed.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("NumberOfHighQualitySolutions", "The number of high quality solutions in the reference set.", new IntValue(5)));
      Parameters.Add(new ConstrainedValueParameter<IPathRelinker>("PathRelinker", "The operator used to execute path relinking."));
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The size of the population of solutions.", new IntValue(50)));
      Parameters.Add(new ValueParameter<IntValue>("ReferenceSetSize", "The size of the reference set.", new IntValue(20)));
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ConstrainedValueParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The operator used to calculate the similarity between two solutions."));
      #endregion

      #region Create operators
      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      UniformSubScopesProcessor uniformSubScopesProcessor = new UniformSubScopesProcessor();
      Placeholder solutionEvaluator = new Placeholder();
      Placeholder solutionImprover = new Placeholder();
      VariableCreator variableCreator = new VariableCreator();
      DataReducer dataReducer = new DataReducer();
      ResultsCollector resultsCollector = new ResultsCollector();
      BestSelector bestSelector = new BestSelector();
      ScatterSearchMainLoop mainLoop = new ScatterSearchMainLoop();
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = randomCreator;
      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.Name = "DiversificationGenerationMethod";
      solutionsCreator.NumberOfSolutionsParameter.ActualName = "PopulationSize";
      solutionsCreator.Successor = uniformSubScopesProcessor;

      uniformSubScopesProcessor.Operator = solutionImprover;
      uniformSubScopesProcessor.ParallelParameter.Value = new BoolValue(true);
      uniformSubScopesProcessor.Successor = variableCreator;

      solutionImprover.Name = "SolutionImprover";
      solutionImprover.OperatorParameter.ActualName = "Improver";
      solutionImprover.Successor = solutionEvaluator;

      solutionEvaluator.Name = "SolutionEvaluator";
      solutionEvaluator.OperatorParameter.ActualName = "Evaluator";
      solutionEvaluator.Successor = null;

      variableCreator.Name = "Initialize EvaluatedSolutions";
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue()));
      variableCreator.Successor = dataReducer;

      dataReducer.Name = "Increment EvaluatedSolutions";
      dataReducer.ParameterToReduce.ActualName = "LocalEvaluatedSolutions";
      dataReducer.TargetParameter.ActualName = "EvaluatedSolutions";
      dataReducer.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      dataReducer.TargetOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      dataReducer.Successor = resultsCollector;

      resultsCollector.Name = "ResultsCollector";
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("EvaluatedSolutions", null, "EvaluatedSolutions"));
      resultsCollector.Successor = bestSelector;

      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = NumberOfHighQualitySolutionsParameter.Name;
      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.Successor = mainLoop;

      mainLoop.MaximumIterationsParameter.ActualName = MaximumIterationsParameter.Name;
      mainLoop.RandomParameter.ActualName = randomCreator.RandomParameter.ActualName;
      mainLoop.ResultsParameter.ActualName = "Results";
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.IterationsParameter.ActualName = "Iterations";
      mainLoop.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      mainLoop.NumberOfHighQualitySolutionsParameter.ActualName = NumberOfHighQualitySolutionsParameter.Name;
      mainLoop.Successor = null;
      #endregion

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
      UpdateAnalyzers();

      Initialize();
    }

    public override void Prepare() {
      if (Problem != null && Improver != null && (PathRelinker != null || ExecutePathRelinking.Value == false) && SimilarityCalculator != null)
        base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeAnalyzers();
      ParameterizeSolutionsCreator();
      ParameterizeBestSelector();
      UpdateAnalyzers();
      UpdateCrossovers();
      UpdateImprovers();
      UpdatePathRelinkers();
      UpdateSimilarityCalculators();
      ParameterizeMainLoop();
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
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      UpdateAnalyzers();
      UpdateCrossovers();
      UpdatePathRelinkers();
      UpdateSimilarityCalculators();
      UpdateImprovers();
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void PathRelinkerParameter_ValueChanged(object sender, EventArgs e) {
      ExecutePathRelinking.Value = PathRelinkerParameter.Value != null;
    }
    private void SimilarityCalculatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
      ParameterizeBestSelector();
      ParameterizeSimilarityCalculators();
    }
    #endregion

    #region Helpers
    private void Initialize() {
      PathRelinkerParameter.ValueChanged += new EventHandler(PathRelinkerParameter_ValueChanged);
      SimilarityCalculatorParameter.ValueChanged += new EventHandler(SimilarityCalculatorParameter_ValueChanged);
      if (Problem != null)
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
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
    private void UpdateImprovers() {
      IImprovementOperator oldImprover = ImproverParameter.Value;
      ImproverParameter.ValidValues.Clear();
      IImprovementOperator defaultImprover = Problem.Operators.OfType<IImprovementOperator>().FirstOrDefault();

      foreach (IImprovementOperator improver in Problem.Operators.OfType<IImprovementOperator>().OrderBy(x => x.Name))
        ImproverParameter.ValidValues.Add(improver);

      if (oldImprover != null) {
        IImprovementOperator improver = ImproverParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldImprover.GetType());
        if (improver != null) ImproverParameter.Value = improver;
        else oldImprover = null;
      }
      if (oldImprover == null && defaultImprover != null)
        ImproverParameter.Value = defaultImprover;
    }
    private void UpdatePathRelinkers() {
      IPathRelinker oldPathRelinker = PathRelinkerParameter.Value;
      PathRelinkerParameter.ValidValues.Clear();
      IPathRelinker defaultPathRelinker = Problem.Operators.OfType<IPathRelinker>().FirstOrDefault();

      foreach (IPathRelinker pathRelinker in Problem.Operators.OfType<IPathRelinker>().OrderBy(x => x.Name))
        PathRelinkerParameter.ValidValues.Add(pathRelinker);

      if (oldPathRelinker != null) {
        IPathRelinker pathRelinker = PathRelinkerParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldPathRelinker.GetType());
        if (pathRelinker != null) PathRelinkerParameter.Value = pathRelinker;
        else oldPathRelinker = null;
      }
      if (oldPathRelinker == null && defaultPathRelinker != null)
        PathRelinkerParameter.Value = defaultPathRelinker;
    }
    private void UpdateSimilarityCalculators() {
      ISolutionSimilarityCalculator oldSimilarityCalculator = SimilarityCalculatorParameter.Value;
      SimilarityCalculatorParameter.ValidValues.Clear();
      ISolutionSimilarityCalculator defaultSimilarityCalculator = Problem.Operators.OfType<ISolutionSimilarityCalculator>().FirstOrDefault();

      foreach (ISolutionSimilarityCalculator similarityCalculator in Problem.Operators.OfType<ISolutionSimilarityCalculator>())
        SimilarityCalculatorParameter.ValidValues.Add(similarityCalculator);

      if (!SimilarityCalculatorParameter.ValidValues.OfType<QualitySimilarityCalculator>().Any())
        SimilarityCalculatorParameter.ValidValues.Add(new QualitySimilarityCalculator {
          QualityVariableName = Problem.Evaluator.QualityParameter.ActualName
        });
      if (!SimilarityCalculatorParameter.ValidValues.OfType<NoSimilarityCalculator>().Any())
        SimilarityCalculatorParameter.ValidValues.Add(new NoSimilarityCalculator());

      if (oldSimilarityCalculator != null) {
        ISolutionSimilarityCalculator similarityCalculator = SimilarityCalculatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldSimilarityCalculator.GetType());
        if (similarityCalculator != null) SimilarityCalculatorParameter.Value = similarityCalculator;
        else oldSimilarityCalculator = null;
      }
      if (oldSimilarityCalculator == null && defaultSimilarityCalculator != null)
        SimilarityCalculatorParameter.Value = defaultSimilarityCalculator;
    }
    private void ParameterizeBestSelector() {
      OperatorGraph.Operators.OfType<ISingleObjectiveSelector>()
                             .First().QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
    }
    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeMainLoop() {
      if (Problem != null && Improver != null) {
        MainLoop.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        MainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        MainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        MainLoop.OperatorGraph.Operators.OfType<PopulationRebuildMethod>().Single().QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        MainLoop.OperatorGraph.Operators.OfType<SolutionPoolUpdateMethod>().Single().QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      }
    }
    private void ParameterizeStochasticOperator(IOperator op) {
      if (op is IStochasticOperator) {
        IStochasticOperator stOp = (IStochasticOperator)op;
        stOp.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
        stOp.RandomParameter.Hidden = true;
      }
    }
    private void ParameterizeAnalyzers() {
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      qualityAnalyzer.ResultsParameter.Hidden = true;
      if (Problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.MaximizationParameter.Hidden = true;
        qualityAnalyzer.QualityParameter.Hidden = false;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        qualityAnalyzer.BestKnownQualityParameter.Hidden = true;
      } else {
        qualityAnalyzer.MaximizationParameter.Hidden = false;
        qualityAnalyzer.BestKnownQualityParameter.Hidden = false;
      }
    }
    private void ParameterizeSimilarityCalculators() {
      foreach (ISolutionSimilarityCalculator calc in SimilarityCalculatorParameter.ValidValues) {
        calc.QualityVariableName = Problem.Evaluator.QualityParameter.ActualName;
      }
    }
    private ScatterSearchMainLoop FindMainLoop(IOperator start) {
      IOperator mainLoop = start;
      while (mainLoop != null && !(mainLoop is ScatterSearchMainLoop))
        mainLoop = ((SingleSuccessorOperator)mainLoop).Successor;
      return mainLoop as ScatterSearchMainLoop;
    }
    #endregion
  }
}
