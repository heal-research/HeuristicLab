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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.ALPS {

  [Item("AlpsOffspringSelectionGeneticAlgorithmMainLoop", "An ALPS offspring selection genetic algorithm main loop operator.")]
  [StorableType("C8DF9B75-7008-405D-B38A-15C1C22110B0")]
  public sealed class AlpsOffspringSelectionGeneticAlgorithmMainLoop : AlgorithmOperator {
    #region Parameter Properties
    public IValueLookupParameter<IRandom> GlobalRandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters["GlobalRandom"]; }
    }
    public IValueLookupParameter<IRandom> LocalRandomParameter {
      get { return (IValueLookupParameter<IRandom>)Parameters["LocalRandom"]; }
    }

    public IValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public IValueLookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public ILookupParameter<IOperator> AnalyzerParameter {
      get { return (ILookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public ILookupParameter<IOperator> LayerAnalyzerParameter {
      get { return (ILookupParameter<IOperator>)Parameters["LayerAnalyzer"]; }
    }

    public IValueLookupParameter<IntValue> NumberOfLayersParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NumberOfLayers"]; }
    }
    public IValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public ILookupParameter<IntValue> CurrentPopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["CurrentPopulationSize"]; }
    }

    public IValueLookupParameter<IOperator> SelectorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Selector"]; }
    }
    public IValueLookupParameter<IOperator> CrossoverParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Crossover"]; }
    }
    public IValueLookupParameter<IOperator> MutatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Mutator"]; }
    }
    public IValueLookupParameter<PercentValue> MutationProbabilityParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public IValueLookupParameter<IntValue> ElitesParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Elites"]; }
    }
    public IValueLookupParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }

    public IValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public ILookupParameter<DoubleValue> ComparisonFactorParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    public IValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public IValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    public IValueLookupParameter<BoolValue> FillPopulationWithParentsParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["FillPopulationWithParents"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> AgeParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Age"]; }
    }
    public IValueLookupParameter<IntValue> AgeGapParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["AgeGap"]; }
    }
    public IValueLookupParameter<DoubleValue> AgeInheritanceParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["AgeInheritance"]; }
    }
    public IValueLookupParameter<IntArray> AgeLimitsParameter {
      get { return (IValueLookupParameter<IntArray>)Parameters["AgeLimits"]; }
    }

    public IValueLookupParameter<IntValue> MatingPoolRangeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MatingPoolRange"]; }
    }
    public IValueLookupParameter<BoolValue> ReduceToPopulationSizeParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["ReduceToPopulationSize"]; }
    }

    public IValueLookupParameter<IOperator> TerminatorParameter {
      get { return (IValueLookupParameter<IOperator>)Parameters["Terminator"]; }
    }
    #endregion

    [StorableConstructor]
    private AlpsOffspringSelectionGeneticAlgorithmMainLoop(StorableConstructorFlag _) : base(_) { }
    private AlpsOffspringSelectionGeneticAlgorithmMainLoop(AlpsOffspringSelectionGeneticAlgorithmMainLoop original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlpsOffspringSelectionGeneticAlgorithmMainLoop(this, cloner);
    }
    public AlpsOffspringSelectionGeneticAlgorithmMainLoop()
      : base() {
      Parameters.Add(new ValueLookupParameter<IRandom>("GlobalRandom", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<IRandom>("LocalRandom", "A pseudo random number generator."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<IntValue>("EvaluatedSolutions", "The number of times solutions have been evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze all individuals from all layers combined."));
      Parameters.Add(new ValueLookupParameter<IOperator>("LayerAnalyzer", "The operator used to analyze each layer."));

      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfLayers", "The number of layers."));
      Parameters.Add(new ValueLookupParameter<IntValue>("PopulationSize", "The size of the population of solutions in each layer."));
      Parameters.Add(new LookupParameter<IntValue>("CurrentPopulationSize", "The current size of the population."));

      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)"));

      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1]."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Age", "The age of individuals."));
      Parameters.Add(new ValueLookupParameter<IntValue>("AgeGap", "The frequency of reseeding the lowest layer and scaling factor for the age-limits for the layers."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("AgeInheritance", "A weight that determines the age of a child after crossover based on the older (1.0) and younger (0.0) parent."));
      Parameters.Add(new ValueLookupParameter<IntArray>("AgeLimits", "The maximum age an individual is allowed to reach in a certain layer."));

      Parameters.Add(new ValueLookupParameter<IntValue>("MatingPoolRange", "The range of sub - populations used for creating a mating pool. (1 = current + previous sub-population)"));
      Parameters.Add(new ValueLookupParameter<BoolValue>("ReduceToPopulationSize", "Reduce the CurrentPopulationSize after elder migration to PopulationSize"));

      Parameters.Add(new ValueLookupParameter<IOperator>("Terminator", "The termination criteria that defines if the algorithm should continue or stop"));


      var variableCreator = new VariableCreator() { Name = "Initialize" };
      var initLayerAnalyzerProcessor = new SubScopesProcessor();
      var layerVariableCreator = new VariableCreator() { Name = "Initialize Layer" };
      var initLayerAnalyzerPlaceholder = new Placeholder() { Name = "LayerAnalyzer (Placeholder)" };
      var layerResultCollector = new ResultsCollector() { Name = "Collect layer results" };
      var initAnalyzerPlaceholder = new Placeholder() { Name = "Analyzer (Placeholder)" };
      var resultsCollector = new ResultsCollector();
      var matingPoolCreator = new MatingPoolCreator() { Name = "Create Mating Pools" };
      var matingPoolProcessor = new UniformSubScopesProcessor() { Name = "Process Mating Pools" };
      var initializeLayer = new Assigner() { Name = "Reset LayerEvaluatedSolutions" };
      var mainOperator = new AlpsOffspringSelectionGeneticAlgorithmMainOperator();
      var generationsIcrementor = new IntCounter() { Name = "Increment Generations" };
      var evaluatedSolutionsReducer = new DataReducer() { Name = "Increment EvaluatedSolutions" };
      var eldersEmigrator = CreateEldersEmigrator();
      var layerOpener = CreateLayerOpener();
      var layerReseeder = CreateReseeder();
      var layerAnalyzerProcessor = new UniformSubScopesProcessor();
      var layerAnalyzerPlaceholder = new Placeholder() { Name = "LayerAnalyzer (Placeholder)" };
      var analyzerPlaceholder = new Placeholder() { Name = "Analyzer (Placeholder)" };
      var termination = new TerminationOperator();

      OperatorGraph.InitialOperator = variableCreator;

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("OpenLayers", new IntValue(1)));
      variableCreator.Successor = initLayerAnalyzerProcessor;

      initLayerAnalyzerProcessor.Operators.Add(layerVariableCreator);
      initLayerAnalyzerProcessor.Successor = initAnalyzerPlaceholder;

      layerVariableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Layer", new IntValue(0)));
      layerVariableCreator.CollectedValues.Add(new ValueParameter<ResultCollection>("LayerResults"));
      layerVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));
      layerVariableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("CurrentSuccessRatio", new DoubleValue(0)));
      layerVariableCreator.Successor = initLayerAnalyzerPlaceholder;

      initLayerAnalyzerPlaceholder.OperatorParameter.ActualName = LayerAnalyzerParameter.Name;
      initLayerAnalyzerPlaceholder.Successor = layerResultCollector;

      layerResultCollector.ResultsParameter.ActualName = "LayerResults";
      layerResultCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", "Displays the rising selection pressure during a generation.", "SelectionPressure"));
      layerResultCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", "Indicates how many successful children were already found during a generation (relative to the population size).", "CurrentSuccessRatio"));
      layerResultCollector.Successor = null;

      initAnalyzerPlaceholder.OperatorParameter.ActualName = AnalyzerParameter.Name;
      initAnalyzerPlaceholder.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector.CollectedValues.Add(new ScopeTreeLookupParameter<ResultCollection>("LayerResults", "Result set for each Layer", "LayerResults"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("OpenLayers"));
      resultsCollector.CopyValue = new BoolValue(false);
      resultsCollector.Successor = matingPoolCreator;

      matingPoolCreator.MatingPoolRangeParameter.Value = null;
      matingPoolCreator.MatingPoolRangeParameter.ActualName = MatingPoolRangeParameter.Name;
      matingPoolCreator.Successor = matingPoolProcessor;

      matingPoolProcessor.Parallel.Value = true;
      matingPoolProcessor.Operator = initializeLayer;
      matingPoolProcessor.Successor = generationsIcrementor;

      initializeLayer.LeftSideParameter.ActualName = "LayerEvaluatedSolutions";
      initializeLayer.RightSideParameter.Value = new IntValue(0);
      initializeLayer.Successor = mainOperator;

      mainOperator.RandomParameter.ActualName = LocalRandomParameter.Name;
      mainOperator.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      mainOperator.EvaluatedSolutionsParameter.ActualName = "LayerEvaluatedSolutions";
      mainOperator.QualityParameter.ActualName = QualityParameter.Name;
      mainOperator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      mainOperator.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      mainOperator.SelectorParameter.ActualName = SelectorParameter.Name;
      mainOperator.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainOperator.MutatorParameter.ActualName = MutatorParameter.ActualName;
      mainOperator.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainOperator.ElitesParameter.ActualName = ElitesParameter.Name;
      mainOperator.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      mainOperator.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      mainOperator.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      mainOperator.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      mainOperator.SelectionPressureParameter.ActualName = "SelectionPressure";
      mainOperator.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainOperator.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainOperator.FillPopulationWithParentsParameter.ActualName = FillPopulationWithParentsParameter.Name;
      mainOperator.AgeParameter.ActualName = AgeParameter.Name;
      mainOperator.AgeInheritanceParameter.ActualName = AgeInheritanceParameter.Name;
      mainOperator.AgeIncrementParameter.Value = new DoubleValue(1.0);
      mainOperator.Successor = null;

      generationsIcrementor.ValueParameter.ActualName = "Generations";
      generationsIcrementor.Increment = new IntValue(1);
      generationsIcrementor.Successor = evaluatedSolutionsReducer;

      evaluatedSolutionsReducer.ParameterToReduce.ActualName = "LayerEvaluatedSolutions";
      evaluatedSolutionsReducer.TargetParameter.ActualName = EvaluatedSolutionsParameter.Name;
      evaluatedSolutionsReducer.ReductionOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      evaluatedSolutionsReducer.TargetOperation.Value = new ReductionOperation(ReductionOperations.Sum);
      evaluatedSolutionsReducer.Successor = eldersEmigrator;

      eldersEmigrator.Successor = layerOpener;

      layerOpener.Successor = layerReseeder;

      layerReseeder.Successor = layerAnalyzerProcessor;

      layerAnalyzerProcessor.Operator = layerAnalyzerPlaceholder;
      layerAnalyzerProcessor.Successor = analyzerPlaceholder;

      layerAnalyzerPlaceholder.OperatorParameter.ActualName = LayerAnalyzerParameter.Name;

      analyzerPlaceholder.OperatorParameter.ActualName = AnalyzerParameter.Name;
      analyzerPlaceholder.Successor = termination;

      termination.TerminatorParameter.ActualName = TerminatorParameter.Name;
      termination.ContinueBranch = matingPoolCreator;
    }

    private CombinedOperator CreateEldersEmigrator() {
      var eldersEmigrator = new CombinedOperator() { Name = "Emigrate Elders" };
      var selectorProsessor = new UniformSubScopesProcessor();
      var eldersSelector = new EldersSelector();
      var shiftToRightMigrator = new UnidirectionalRingMigrator() { Name = "Shift elders to next layer" };
      var mergingProsessor = new UniformSubScopesProcessor();
      var mergingReducer = new MergingReducer();
      var subScopesCounter = new SubScopesCounter();
      var reduceToPopulationSizeBranch = new ConditionalBranch() { Name = "ReduceToPopulationSize?" };
      var countCalculator = new ExpressionCalculator() { Name = "CurrentPopulationSize = Min(CurrentPopulationSize, PopulationSize)" };
      var bestSelector = new BestSelector();
      var rightReducer = new RightReducer();

      eldersEmigrator.OperatorGraph.InitialOperator = selectorProsessor;

      selectorProsessor.Operator = eldersSelector;
      selectorProsessor.Successor = shiftToRightMigrator;

      eldersSelector.AgeParameter.ActualName = AgeParameter.Name;
      eldersSelector.AgeLimitsParameter.ActualName = AgeLimitsParameter.Name;
      eldersSelector.NumberOfLayersParameter.ActualName = NumberOfLayersParameter.Name;
      eldersSelector.LayerParameter.ActualName = "Layer";
      eldersSelector.Successor = null;

      shiftToRightMigrator.ClockwiseMigrationParameter.Value = new BoolValue(true);
      shiftToRightMigrator.Successor = mergingProsessor;

      mergingProsessor.Operator = mergingReducer;

      mergingReducer.Successor = subScopesCounter;

      subScopesCounter.ValueParameter.ActualName = CurrentPopulationSizeParameter.Name;
      subScopesCounter.AccumulateParameter.Value = new BoolValue(false);
      subScopesCounter.Successor = reduceToPopulationSizeBranch;

      reduceToPopulationSizeBranch.ConditionParameter.ActualName = ReduceToPopulationSizeParameter.Name;
      reduceToPopulationSizeBranch.TrueBranch = countCalculator;

      countCalculator.CollectedValues.Add(new LookupParameter<IntValue>(PopulationSizeParameter.Name));
      countCalculator.CollectedValues.Add(new LookupParameter<IntValue>(CurrentPopulationSizeParameter.Name));
      countCalculator.ExpressionParameter.Value = new StringValue("CurrentPopulationSize PopulationSize CurrentPopulationSize PopulationSize < if toint");
      countCalculator.ExpressionResultParameter.ActualName = CurrentPopulationSizeParameter.Name;
      countCalculator.Successor = bestSelector;

      bestSelector.NumberOfSelectedSubScopesParameter.ActualName = CurrentPopulationSizeParameter.Name;
      bestSelector.CopySelected = new BoolValue(false);
      bestSelector.Successor = rightReducer;

      return eldersEmigrator;
    }

    private CombinedOperator CreateLayerOpener() {
      var layerOpener = new CombinedOperator() { Name = "Open new Layer if needed" };
      var maxLayerReached = new Comparator() { Name = "MaxLayersReached = OpenLayers >= NumberOfLayers" };
      var maxLayerReachedBranch = new ConditionalBranch() { Name = "MaxLayersReached?" };
      var openNewLayerCalculator = new ExpressionCalculator() { Name = "OpenNewLayer = Generations >= AgeLimits[OpenLayers - 1]" };
      var openNewLayerBranch = new ConditionalBranch() { Name = "OpenNewLayer?" };
      var layerCreator = new LastLayerCloner() { Name = "Create Layer" };
      var updateLayerNumber = new Assigner() { Name = "Layer = OpenLayers" };
      var historyWiper = new ResultsHistoryWiper() { Name = "Clear History in Results" };
      var createChildrenViaCrossover = new AlpsOffspringSelectionGeneticAlgorithmMainOperator();
      var incrEvaluatedSolutionsForNewLayer = new SubScopesCounter() { Name = "Update EvaluatedSolutions" };
      var incrOpenLayers = new IntCounter() { Name = "Incr. OpenLayers" };
      var newLayerResultsCollector = new ResultsCollector() { Name = "Collect new Layer Results" };

      layerOpener.OperatorGraph.InitialOperator = maxLayerReached;

      maxLayerReached.LeftSideParameter.ActualName = "OpenLayers";
      maxLayerReached.RightSideParameter.ActualName = NumberOfLayersParameter.Name;
      maxLayerReached.ResultParameter.ActualName = "MaxLayerReached";
      maxLayerReached.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxLayerReached.Successor = maxLayerReachedBranch;

      maxLayerReachedBranch.ConditionParameter.ActualName = "MaxLayerReached";
      maxLayerReachedBranch.FalseBranch = openNewLayerCalculator;

      openNewLayerCalculator.CollectedValues.Add(new LookupParameter<IntArray>(AgeLimitsParameter.Name));
      openNewLayerCalculator.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      openNewLayerCalculator.CollectedValues.Add(new LookupParameter<IntValue>(NumberOfLayersParameter.Name));
      openNewLayerCalculator.CollectedValues.Add(new LookupParameter<IntValue>("OpenLayers"));
      openNewLayerCalculator.ExpressionResultParameter.ActualName = "OpenNewLayer";
      openNewLayerCalculator.ExpressionParameter.Value = new StringValue("Generations 1 + AgeLimits OpenLayers 1 - [] >");
      openNewLayerCalculator.Successor = openNewLayerBranch;

      openNewLayerBranch.ConditionParameter.ActualName = "OpenNewLayer";
      openNewLayerBranch.TrueBranch = layerCreator;

      layerCreator.NewLayerOperator = updateLayerNumber;
      layerCreator.Successor = incrOpenLayers;

      updateLayerNumber.LeftSideParameter.ActualName = "Layer";
      updateLayerNumber.RightSideParameter.ActualName = "OpenLayers";
      updateLayerNumber.Successor = historyWiper;

      historyWiper.ResultsParameter.ActualName = "LayerResults";
      historyWiper.Successor = createChildrenViaCrossover;

      // Maybe use only crossover and no elitism instead of "default operator"
      createChildrenViaCrossover.RandomParameter.ActualName = LocalRandomParameter.Name;
      createChildrenViaCrossover.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      createChildrenViaCrossover.EvaluatedSolutionsParameter.ActualName = "LayerEvaluatedSolutions";
      createChildrenViaCrossover.QualityParameter.ActualName = QualityParameter.Name;
      createChildrenViaCrossover.MaximizationParameter.ActualName = MaximizationParameter.Name;
      createChildrenViaCrossover.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      createChildrenViaCrossover.SelectorParameter.ActualName = SelectorParameter.Name;
      createChildrenViaCrossover.CrossoverParameter.ActualName = CrossoverParameter.Name;
      createChildrenViaCrossover.MutatorParameter.ActualName = MutatorParameter.ActualName;
      createChildrenViaCrossover.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      createChildrenViaCrossover.ElitesParameter.ActualName = ElitesParameter.Name;
      createChildrenViaCrossover.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      createChildrenViaCrossover.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      createChildrenViaCrossover.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      createChildrenViaCrossover.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      createChildrenViaCrossover.SelectionPressureParameter.ActualName = "SelectionPressure";
      createChildrenViaCrossover.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      createChildrenViaCrossover.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      createChildrenViaCrossover.FillPopulationWithParentsParameter.ActualName = FillPopulationWithParentsParameter.Name;
      createChildrenViaCrossover.AgeParameter.ActualName = AgeParameter.Name;
      createChildrenViaCrossover.AgeInheritanceParameter.ActualName = AgeInheritanceParameter.Name;
      createChildrenViaCrossover.AgeIncrementParameter.Value = new DoubleValue(0.0);
      createChildrenViaCrossover.Successor = incrEvaluatedSolutionsForNewLayer;

      incrEvaluatedSolutionsForNewLayer.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      incrEvaluatedSolutionsForNewLayer.AccumulateParameter.Value = new BoolValue(true);

      incrOpenLayers.ValueParameter.ActualName = "OpenLayers";
      incrOpenLayers.Increment = new IntValue(1);
      incrOpenLayers.Successor = newLayerResultsCollector;

      newLayerResultsCollector.CollectedValues.Add(new ScopeTreeLookupParameter<ResultCollection>("LayerResults", "Result set for each layer", "LayerResults"));
      newLayerResultsCollector.CopyValue = new BoolValue(false);
      newLayerResultsCollector.Successor = null;

      return layerOpener;
    }

    private CombinedOperator CreateReseeder() {
      var reseeder = new CombinedOperator() { Name = "Reseed Layer Zero if needed" };
      var reseedingController = new ReseedingController() { Name = "Reseeding needed (Generation % AgeGap == 0)?" };
      var removeIndividuals = new SubScopesRemover();
      var createIndividuals = new SolutionsCreator();
      var initializeAgeProsessor = new UniformSubScopesProcessor();
      var initializeAge = new VariableCreator() { Name = "Initialize Age" };
      var incrEvaluatedSolutionsAfterReseeding = new SubScopesCounter() { Name = "Update EvaluatedSolutions" };

      reseeder.OperatorGraph.InitialOperator = reseedingController;

      reseedingController.GenerationsParameter.ActualName = "Generations";
      reseedingController.AgeGapParameter.ActualName = AgeGapParameter.Name;
      reseedingController.FirstLayerOperator = removeIndividuals;
      reseedingController.Successor = null;

      removeIndividuals.Successor = createIndividuals;

      createIndividuals.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      createIndividuals.Successor = initializeAgeProsessor;

      initializeAgeProsessor.Operator = initializeAge;
      initializeAgeProsessor.Successor = incrEvaluatedSolutionsAfterReseeding;

      initializeAge.CollectedValues.Add(new ValueParameter<DoubleValue>(AgeParameter.Name, new DoubleValue(0)));

      incrEvaluatedSolutionsAfterReseeding.ValueParameter.ActualName = EvaluatedSolutionsParameter.Name;
      incrEvaluatedSolutionsAfterReseeding.AccumulateParameter.Value = new BoolValue(true);

      return reseeder;
    }
  }
}