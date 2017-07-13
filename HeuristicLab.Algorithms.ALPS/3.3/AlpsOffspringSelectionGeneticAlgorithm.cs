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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
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
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.ALPS {
  [Item("ALPS OffspringSelection Genetic Algorithm", "An offspring selection genetic algorithm within an age-layered population structure.")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 162)]
  [StorableClass]
  public sealed class AlpsOffspringSelectionGeneticAlgorithm : HeuristicOptimizationEngineAlgorithm, IStorableContent {
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
    private IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }

    private IFixedValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IFixedValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private IFixedValueParameter<MultiAnalyzer> LayerAnalyzerParameter {
      get { return (IFixedValueParameter<MultiAnalyzer>)Parameters["LayerAnalyzer"]; }
    }

    private IFixedValueParameter<IntValue> NumberOfLayersParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["NumberOfLayers"]; }
    }
    private IFixedValueParameter<IntValue> PopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["PopulationSize"]; }
    }

    public IConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (IConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    public IConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (IConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    public IConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (IConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private IFixedValueParameter<PercentValue> MutationProbabilityParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    private IFixedValueParameter<IntValue> ElitesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Elites"]; }
    }
    private IFixedValueParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }

    private IFixedValueParameter<DoubleValue> SuccessRatioParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    private IFixedValueParameter<DoubleValue> ComparisonFactorParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    private IFixedValueParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    private IFixedValueParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    private IFixedValueParameter<IntValue> SelectedParentsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["SelectedParents"]; }
    }
    private IFixedValueParameter<BoolValue> FillPopulationWithParentsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["FillPopulationWithParents"]; }
    }

    private IFixedValueParameter<EnumValue<AgingScheme>> AgingSchemeParameter {
      get { return (IFixedValueParameter<EnumValue<AgingScheme>>)Parameters["AgingScheme"]; }
    }
    private IFixedValueParameter<IntValue> AgeGapParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["AgeGap"]; }
    }
    private IFixedValueParameter<DoubleValue> AgeInheritanceParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["AgeInheritance"]; }
    }
    private IFixedValueParameter<IntArray> AgeLimitsParameter {
      get { return (IFixedValueParameter<IntArray>)Parameters["AgeLimits"]; }
    }

    private IFixedValueParameter<IntValue> MatingPoolRangeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MatingPoolRange"]; }
    }
    private IFixedValueParameter<BoolValue> ReduceToPopulationSizeParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["ReduceToPopulationSize"]; }
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
    public MultiAnalyzer LayerAnalyzer {
      get { return LayerAnalyzerParameter.Value; }
    }

    public int NumberOfLayers {
      get { return NumberOfLayersParameter.Value.Value; }
      set { NumberOfLayersParameter.Value.Value = value; }
    }
    public int PopulationSize {
      get { return PopulationSizeParameter.Value.Value; }
      set { PopulationSizeParameter.Value.Value = value; }
    }

    public ISelector Selector {
      get { return SelectorParameter.Value; }
      set { SelectorParameter.Value = value; }
    }
    public ICrossover Crossover {
      get { return CrossoverParameter.Value; }
      set { CrossoverParameter.Value = value; }
    }
    public IManipulator Mutator {
      get { return MutatorParameter.Value; }
      set { MutatorParameter.Value = value; }
    }
    public double MutationProbability {
      get { return MutationProbabilityParameter.Value.Value; }
      set { MutationProbabilityParameter.Value.Value = value; }
    }
    public int Elites {
      get { return ElitesParameter.Value.Value; }
      set { ElitesParameter.Value.Value = value; }
    }
    public bool ReevaluteElites {
      get { return ReevaluateElitesParameter.Value.Value; }
      set { ReevaluateElitesParameter.Value.Value = value; }
    }

    public double SuccessRatio {
      get { return SuccessRatioParameter.Value.Value; }
      set { SuccessRatioParameter.Value.Value = value; }
    }
    public double ComparisonFactor {
      get { return ComparisonFactorParameter.Value.Value; }
      set { ComparisonFactorParameter.Value.Value = value; }
    }
    public double MaximumSelectionPressure {
      get { return MaximumSelectionPressureParameter.Value.Value; }
      set { MaximumSelectionPressureParameter.Value.Value = value; }
    }
    public bool OffspringSelectionBeforeMutation {
      get { return OffspringSelectionBeforeMutationParameter.Value.Value; }
      set { OffspringSelectionBeforeMutationParameter.Value.Value = value; }
    }
    public int SelectedParents {
      get { return SelectedParentsParameter.Value.Value; }
      set { SelectedParentsParameter.Value.Value = value; }
    }
    public bool FillPopulationWithParents {
      get { return FillPopulationWithParentsParameter.Value.Value; }
      set { FillPopulationWithParentsParameter.Value.Value = value; }
    }

    public AgingScheme AgingScheme {
      get { return AgingSchemeParameter.Value.Value; }
      set { AgingSchemeParameter.Value.Value = value; }
    }
    public int AgeGap {
      get { return AgeGapParameter.Value.Value; }
      set { AgeGapParameter.Value.Value = value; }
    }
    public double AgeInheritance {
      get { return AgeInheritanceParameter.Value.Value; }
      set { AgeInheritanceParameter.Value.Value = value; }
    }
    public IntArray AgeLimits {
      get { return AgeLimitsParameter.Value; }
      set {
        AgeLimits.Length = value.Length;
        for (int i = 0; i < value.Length; i++)
          AgeLimits[i] = value[i];
      }
    }

    public int MatingPoolRange {
      get { return MatingPoolRangeParameter.Value.Value; }
      set { MatingPoolRangeParameter.Value.Value = value; }
    }

    public MultiTerminator Terminators {
      get { return TerminatorParameter.Value; }
    }

    public int MaximumGenerations {
      get { return generationsTerminator.Threshold.Value; }
      set { generationsTerminator.Threshold.Value = value; }
    }
    #endregion

    #region Helper Properties
    private SolutionsCreator SolutionsCreator {
      get { return OperatorGraph.Iterate().OfType<SolutionsCreator>().First(); }
    }
    private AlpsOffspringSelectionGeneticAlgorithmMainLoop MainLoop {
      get { return OperatorGraph.Iterate().OfType<AlpsOffspringSelectionGeneticAlgorithmMainLoop>().First(); }
    }
    #endregion

    #region Preconfigured Analyzers
    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    [Storable]
    private BestAverageWorstQualityAnalyzer layerQualityAnalyzer;
    [Storable]
    private OldestAverageYoungestAgeAnalyzer ageAnalyzer;
    [Storable]
    private OldestAverageYoungestAgeAnalyzer layerAgeAnalyzer;
    [Storable]
    private AgeDistributionAnalyzer ageDistributionAnalyzer;
    [Storable]
    private AgeDistributionAnalyzer layerAgeDistributionAnalyzer;
    [Storable]
    private ValueAnalyzer selectionPressureAnalyzer;
    [Storable]
    private ValueAnalyzer layerSelectionPressureAnalyzer;
    [Storable]
    private ValueAnalyzer currentSuccessRatioAnalyzer;
    #endregion

    #region Preconfigured Terminators
    [Storable]
    private ComparisonTerminator<IntValue> generationsTerminator;
    [Storable]
    private ComparisonTerminator<IntValue> evaluationsTerminator;
    [Storable]
    private SingleObjectiveQualityTerminator qualityTerminator;
    [Storable]
    private ExecutionTimeTerminator executionTimeTerminator;
    #endregion

    #region Constructors
    [StorableConstructor]
    private AlpsOffspringSelectionGeneticAlgorithm(bool deserializing)
      : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      var optionalMutatorParameter = MutatorParameter as OptionalConstrainedValueParameter<IManipulator>;
      if (optionalMutatorParameter != null) {
        Parameters.Remove(optionalMutatorParameter);
        Parameters.Add(new ConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
        foreach (var m in optionalMutatorParameter.ValidValues)
          MutatorParameter.ValidValues.Add(m);
        if (optionalMutatorParameter.Value == null) MutationProbability = 0; // to guarantee that the old configuration results in the same behavior
        else Mutator = optionalMutatorParameter.Value;
        optionalMutatorParameter.ValidValues.Clear(); // to avoid dangling references to the old parameter its valid values are cleared
      }
      #endregion
      Initialize();
    }
    private AlpsOffspringSelectionGeneticAlgorithm(AlpsOffspringSelectionGeneticAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      layerQualityAnalyzer = cloner.Clone(original.layerQualityAnalyzer);
      ageAnalyzer = cloner.Clone(original.ageAnalyzer);
      layerAgeAnalyzer = cloner.Clone(original.layerAgeAnalyzer);
      ageDistributionAnalyzer = cloner.Clone(original.ageDistributionAnalyzer);
      layerAgeDistributionAnalyzer = cloner.Clone(original.layerAgeDistributionAnalyzer);
      selectionPressureAnalyzer = cloner.Clone(original.selectionPressureAnalyzer);
      layerSelectionPressureAnalyzer = cloner.Clone(original.layerSelectionPressureAnalyzer);
      currentSuccessRatioAnalyzer = cloner.Clone(original.currentSuccessRatioAnalyzer);
      generationsTerminator = cloner.Clone(original.generationsTerminator);
      evaluationsTerminator = cloner.Clone(original.evaluationsTerminator);
      qualityTerminator = cloner.Clone(original.qualityTerminator);
      executionTimeTerminator = cloner.Clone(original.executionTimeTerminator);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlpsOffspringSelectionGeneticAlgorithm(this, cloner);
    }
    public AlpsOffspringSelectionGeneticAlgorithm()
      : base() {
      #region Add parameters
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));

      Parameters.Add(new FixedValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze all individuals from all layers combined.", new MultiAnalyzer()));
      Parameters.Add(new FixedValueParameter<MultiAnalyzer>("LayerAnalyzer", "The operator used to analyze each layer.", new MultiAnalyzer()));

      Parameters.Add(new FixedValueParameter<IntValue>("NumberOfLayers", "The number of layers.", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<IntValue>("PopulationSize", "The size of the population of solutions in each layer.", new IntValue(100)));

      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new FixedValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new FixedValueParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", new BoolValue(false)) { Hidden = true });

      Parameters.Add(new FixedValueParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved.", new DoubleValue(1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("ComparisonFactor", "The comparison factor is used to determine whether the offspring should be compared to the better parent, the worse parent or a quality value linearly interpolated between them. It is in the range [0;1].", new DoubleValue(1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm.", new DoubleValue(100)));
      Parameters.Add(new FixedValueParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation.", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>("SelectedParents", "How much parents should be selected each time the offspring selection step is performed until the population is filled. This parameter should be about the same or twice the size of PopulationSize for smaller problems, and less for large problems.", new IntValue(200)));
      Parameters.Add(new FixedValueParameter<BoolValue>("FillPopulationWithParents", "True if the population should be filled with parent individual or false if worse children should be used when the maximum selection pressure is exceeded.", new BoolValue(false)) { Hidden = true });

      Parameters.Add(new FixedValueParameter<EnumValue<AgingScheme>>("AgingScheme", "The aging scheme for setting the age-limits for the layers.", new EnumValue<AgingScheme>(ALPS.AgingScheme.Polynomial)));
      Parameters.Add(new FixedValueParameter<IntValue>("AgeGap", "The frequency of reseeding the lowest layer and scaling factor for the age-limits for the layers.", new IntValue(20)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("AgeInheritance", "A weight that determines the age of a child after crossover based on the older (1.0) and younger (0.0) parent.", new DoubleValue(1.0)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<IntArray>("AgeLimits", "The maximum age an individual is allowed to reach in a certain layer.", new IntArray(new int[0])) { Hidden = true });

      Parameters.Add(new FixedValueParameter<IntValue>("MatingPoolRange", "The range of layers used for creating a mating pool. (1 = current + previous layer)", new IntValue(1)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>("ReduceToPopulationSize", "Reduce the CurrentPopulationSize after elder migration to PopulationSize", new BoolValue(true)) { Hidden = true });

      Parameters.Add(new FixedValueParameter<MultiTerminator>("Terminator", "The termination criteria that defines if the algorithm should continue or stop.", new MultiTerminator()));
      #endregion

      #region Create operators
      var globalRandomCreator = new RandomCreator();
      var layer0Creator = new SubScopesCreator() { Name = "Create Layer Zero" };
      var layer0Processor = new SubScopesProcessor();
      var localRandomCreator = new LocalRandomCreator();
      var layerSolutionsCreator = new SolutionsCreator();
      var initializeAgeProcessor = new UniformSubScopesProcessor();
      var initializeAge = new VariableCreator() { Name = "Initialize Age" };
      var initializeCurrentPopulationSize = new SubScopesCounter() { Name = "Initialize CurrentPopulationCounter" };
      var initializeLocalEvaluatedSolutions = new Assigner() { Name = "Initialize LayerEvaluatedSolutions" };
      var initializeGlobalEvaluatedSolutions = new DataReducer() { Name = "Initialize EvaluatedSolutions" };
      var resultsCollector = new ResultsCollector();
      var mainLoop = new AlpsOffspringSelectionGeneticAlgorithmMainLoop();
      #endregion

      #region Create and parameterize operator graph
      OperatorGraph.InitialOperator = globalRandomCreator;

      globalRandomCreator.RandomParameter.ActualName = "GlobalRandom";
      globalRandomCreator.SeedParameter.Value = null;
      globalRandomCreator.SeedParameter.ActualName = SeedParameter.Name;
      globalRandomCreator.SetSeedRandomlyParameter.Value = null;
      globalRandomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      globalRandomCreator.Successor = layer0Creator;

      layer0Creator.NumberOfSubScopesParameter.Value = new IntValue(1);
      layer0Creator.Successor = layer0Processor;

      layer0Processor.Operators.Add(localRandomCreator);
      layer0Processor.Successor = initializeGlobalEvaluatedSolutions;

      localRandomCreator.Successor = layerSolutionsCreator;

      layerSolutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      layerSolutionsCreator.Successor = initializeAgeProcessor;

      initializeAgeProcessor.Operator = initializeAge;
      initializeAgeProcessor.Successor = initializeCurrentPopulationSize;

      initializeCurrentPopulationSize.ValueParameter.ActualName = "CurrentPopulationSize";
      initializeCurrentPopulationSize.Successor = initializeLocalEvaluatedSolutions;

      initializeAge.CollectedValues.Add(new ValueParameter<DoubleValue>("Age", new DoubleValue(0)));
      initializeAge.Successor = null;

      initializeLocalEvaluatedSolutions.LeftSideParameter.ActualName = "LayerEvaluatedSolutions";
      initializeLocalEvaluatedSolutions.RightSideParameter.ActualName = "CurrentPopulationSize";
      initializeLocalEvaluatedSolutions.Successor = null;

      initializeGlobalEvaluatedSolutions.ReductionOperation.Value.Value = ReductionOperations.Sum;
      initializeGlobalEvaluatedSolutions.TargetOperation.Value.Value = ReductionOperations.Assign;
      initializeGlobalEvaluatedSolutions.ParameterToReduce.ActualName = "LayerEvaluatedSolutions";
      initializeGlobalEvaluatedSolutions.TargetParameter.ActualName = "EvaluatedSolutions";
      initializeGlobalEvaluatedSolutions.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector.Successor = mainLoop;

      mainLoop.GlobalRandomParameter.ActualName = "GlobalRandom";
      mainLoop.LocalRandomParameter.ActualName = localRandomCreator.LocalRandomParameter.Name;
      mainLoop.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.LayerAnalyzerParameter.ActualName = LayerAnalyzerParameter.Name;
      mainLoop.NumberOfLayersParameter.ActualName = NumberOfLayersParameter.Name;
      mainLoop.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
      mainLoop.CurrentPopulationSizeParameter.ActualName = "CurrentPopulationSize";
      mainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.ElitesParameter.ActualName = ElitesParameter.Name;
      mainLoop.ReevaluateElitesParameter.ActualName = ReevaluateElitesParameter.Name;
      mainLoop.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;
      mainLoop.ComparisonFactorParameter.ActualName = ComparisonFactorParameter.Name;
      mainLoop.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainLoop.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainLoop.FillPopulationWithParentsParameter.ActualName = FillPopulationWithParentsParameter.Name;
      mainLoop.AgeParameter.ActualName = "Age";
      mainLoop.AgeGapParameter.ActualName = AgeGapParameter.Name;
      mainLoop.AgeInheritanceParameter.ActualName = AgeInheritanceParameter.Name;
      mainLoop.AgeLimitsParameter.ActualName = AgeLimitsParameter.Name;
      mainLoop.MatingPoolRangeParameter.ActualName = MatingPoolRangeParameter.Name;
      mainLoop.ReduceToPopulationSizeParameter.ActualName = ReduceToPopulationSizeParameter.Name;
      mainLoop.TerminatorParameter.ActualName = TerminatorParameter.Name;
      #endregion

      #region Set operators
      foreach (var selector in ApplicationManager.Manager.GetInstances<ISelector>().Where(s => !(s is IMultiObjectiveSelector)).OrderBy(s => Name))
        SelectorParameter.ValidValues.Add(selector);
      var defaultSelector = SelectorParameter.ValidValues.OfType<GeneralizedRankSelector>().FirstOrDefault();
      if (defaultSelector != null) {
        defaultSelector.PressureParameter.Value = new DoubleValue(4.0);
        SelectorParameter.Value = defaultSelector;
      }
      #endregion

      #region Create analyzers
      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      layerQualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ageAnalyzer = new OldestAverageYoungestAgeAnalyzer();
      layerAgeAnalyzer = new OldestAverageYoungestAgeAnalyzer();
      ageDistributionAnalyzer = new AgeDistributionAnalyzer();
      layerAgeDistributionAnalyzer = new AgeDistributionAnalyzer();
      selectionPressureAnalyzer = new ValueAnalyzer();
      layerSelectionPressureAnalyzer = new ValueAnalyzer();
      currentSuccessRatioAnalyzer = new ValueAnalyzer();
      #endregion

      #region Create terminators
      generationsTerminator = new ComparisonTerminator<IntValue>("Generations", ComparisonType.Less, new IntValue(1000)) { Name = "Generations" };
      evaluationsTerminator = new ComparisonTerminator<IntValue>("EvaluatedSolutions", ComparisonType.Less, new IntValue(int.MaxValue)) { Name = "Evaluations" };
      qualityTerminator = new SingleObjectiveQualityTerminator() { Name = "Quality" };
      executionTimeTerminator = new ExecutionTimeTerminator(this, new TimeSpanValue(TimeSpan.FromMinutes(5)));
      #endregion

      #region Parameterize
      UpdateAnalyzers();
      ParameterizeAnalyzers();

      ParameterizeSelectors();

      UpdateTerminators();

      ParameterizeAgeLimits();
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
      ParameterizeStochasticOperatorForLayer(Problem.Evaluator);
      foreach (var @operator in Problem.Operators.OfType<IOperator>())
        ParameterizeStochasticOperator(@operator);

      ParameterizeIterationBasedOperators();

      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
      ParameterizeSelectors();
      ParameterizeTerminators();

      Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;

      UpdateAnalyzers();
      UpdateCrossovers();
      UpdateMutators();
      UpdateTerminators();
    }

    protected override void RegisterProblemEvents() {
      base.RegisterProblemEvents();
      var maximizationParameter = (IValueParameter<BoolValue>)Problem.MaximizationParameter;
      if (maximizationParameter != null) maximizationParameter.ValueChanged += new EventHandler(MaximizationParameter_ValueChanged);
    }
    protected override void DeregisterProblemEvents() {
      var maximizationParameter = (IValueParameter<BoolValue>)Problem.MaximizationParameter;
      if (maximizationParameter != null) maximizationParameter.ValueChanged -= new EventHandler(MaximizationParameter_ValueChanged);
      base.DeregisterProblemEvents();
    }

    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      base.Problem_SolutionCreatorChanged(sender, e);
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperatorForLayer(Problem.Evaluator);

      Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;

      ParameterizeSolutionsCreator();
      ParameterizeAnalyzers();
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      base.Problem_EvaluatorChanged(sender, e);
      foreach (var @operator in Problem.Operators.OfType<IOperator>())
        ParameterizeStochasticOperator(@operator);

      UpdateAnalyzers();

      ParameterizeSolutionsCreator();
      ParameterizeMainLoop();
      ParameterizeSelectors();
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      base.Problem_OperatorsChanged(sender, e);
      ParameterizeIterationBasedOperators();
      UpdateCrossovers();
      UpdateMutators();
      UpdateTerminators();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeMainLoop();
      ParameterizeAnalyzers();
      ParameterizeSelectors();
    }
    private void MaximizationParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeTerminators();
    }
    private void QualityAnalyzer_CurrentBestQualityParameter_NameChanged(object sender, EventArgs e) {
      ParameterizeTerminators();
    }

    private void AgeGap_ValueChanged(object sender, EventArgs e) {
      ParameterizeAgeLimits();
    }
    private void AgingScheme_ValueChanged(object sender, EventArgs e) {
      ParameterizeAgeLimits();
    }
    private void NumberOfLayers_ValueChanged(object sender, EventArgs e) {
      ParameterizeAgeLimits();
    }

    private void AnalyzerOperators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IAnalyzer>> e) {
      foreach (var analyzer in e.Items) {
        foreach (var parameter in analyzer.Value.Parameters.OfType<IScopeTreeLookupParameter>()) {
          parameter.Depth = 2;
        }
      }
    }
    private void LayerAnalyzerOperators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IAnalyzer>> e) {
      foreach (var analyzer in e.Items) {
        IParameter resultParameter;
        if (analyzer.Value.Parameters.TryGetValue("Results", out resultParameter)) {
          var lookupParameter = resultParameter as ILookupParameter;
          if (lookupParameter != null)
            lookupParameter.ActualName = "LayerResults";
        }
        foreach (var parameter in analyzer.Value.Parameters.OfType<IScopeTreeLookupParameter>()) {
          parameter.Depth = 1;
        }
      }
    }
    #endregion

    #region Parameterization
    private void Initialize() {
      if (Problem != null)
        Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;

      NumberOfLayersParameter.Value.ValueChanged += NumberOfLayers_ValueChanged;

      Analyzer.Operators.ItemsAdded += AnalyzerOperators_ItemsAdded;
      LayerAnalyzer.Operators.ItemsAdded += LayerAnalyzerOperators_ItemsAdded;

      AgeGapParameter.Value.ValueChanged += AgeGap_ValueChanged;
      AgingSchemeParameter.Value.ValueChanged += AgingScheme_ValueChanged;

      qualityAnalyzer.CurrentBestQualityParameter.NameChanged += QualityAnalyzer_CurrentBestQualityParameter_NameChanged;
    }
    private void ParameterizeSolutionsCreator() {
      SolutionsCreator.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      SolutionsCreator.SolutionCreatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
    }
    private void ParameterizeMainLoop() {
      MainLoop.EvaluatorParameter.ActualName = Problem.EvaluatorParameter.Name;
      MainLoop.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      MainLoop.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
    }
    private void ParameterizeAnalyzers() {
      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      qualityAnalyzer.ResultsParameter.Hidden = true;
      qualityAnalyzer.QualityParameter.Depth = 2;
      layerQualityAnalyzer.ResultsParameter.ActualName = "LayerResults";
      layerQualityAnalyzer.ResultsParameter.Hidden = true;
      layerQualityAnalyzer.QualityParameter.Depth = 1;
      selectionPressureAnalyzer.Name = "SelectionPressure Analyzer";
      selectionPressureAnalyzer.ResultsParameter.ActualName = "Results";
      selectionPressureAnalyzer.ValueParameter.ActualName = "SelectionPressure";
      selectionPressureAnalyzer.ValueParameter.Depth = 1;
      selectionPressureAnalyzer.ValuesParameter.ActualName = "Selection Pressure History";
      layerSelectionPressureAnalyzer.Name = "SelectionPressure Analyzer";
      layerSelectionPressureAnalyzer.ResultsParameter.ActualName = "LayerResults";
      layerSelectionPressureAnalyzer.ValueParameter.ActualName = "SelectionPressure";
      layerSelectionPressureAnalyzer.ValueParameter.Depth = 0;
      layerSelectionPressureAnalyzer.ValuesParameter.ActualName = "Selection Pressure History";
      currentSuccessRatioAnalyzer.Name = "CurrentSuccessRatio Analyzer";
      currentSuccessRatioAnalyzer.ResultsParameter.ActualName = "Results";
      currentSuccessRatioAnalyzer.ValueParameter.ActualName = "CurrentSuccessRatio";
      currentSuccessRatioAnalyzer.ValueParameter.Depth = 1;
      currentSuccessRatioAnalyzer.ValuesParameter.ActualName = "Success Ratio History";
      if (Problem != null) {
        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.MaximizationParameter.Hidden = true;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.QualityParameter.Hidden = true;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        qualityAnalyzer.BestKnownQualityParameter.Hidden = true;
        layerQualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        layerQualityAnalyzer.MaximizationParameter.Hidden = true;
        layerQualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        layerQualityAnalyzer.QualityParameter.Hidden = true;
        layerQualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        layerQualityAnalyzer.BestKnownQualityParameter.Hidden = true;
      }
    }
    private void ParameterizeSelectors() {
      foreach (var selector in SelectorParameter.ValidValues) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Hidden = true;
        selector.NumberOfSelectedSubScopesParameter.ActualName = SelectedParentsParameter.Name;
        ParameterizeStochasticOperatorForLayer(selector);
      }
      if (Problem != null) {
        foreach (var selector in SelectorParameter.ValidValues.OfType<ISingleObjectiveSelector>()) {
          selector.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
          selector.MaximizationParameter.Hidden = true;
          selector.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
          selector.QualityParameter.Hidden = true;
        }
      }
    }
    private void ParameterizeTerminators() {
      qualityTerminator.Parameterize(qualityAnalyzer.CurrentBestQualityParameter, Problem);
    }
    private void ParameterizeIterationBasedOperators() {
      if (Problem != null) {
        foreach (var @operator in Problem.Operators.OfType<IIterationBasedOperator>()) {
          @operator.IterationsParameter.ActualName = "Generations";
          @operator.IterationsParameter.Hidden = true;
          @operator.MaximumIterationsParameter.ActualName = generationsTerminator.ThresholdParameter.Name;
          @operator.MaximumIterationsParameter.Hidden = true;
        }
      }
    }
    private void ParameterizeAgeLimits() {
      AgeLimits = AgingScheme.CalculateAgeLimits(AgeGap, NumberOfLayers);
    }

    private void ParameterizeStochasticOperator(IOperator @operator) {
      var stochasticOperator = @operator as IStochasticOperator;
      if (stochasticOperator != null) {
        stochasticOperator.RandomParameter.ActualName = "GlobalRandom";
        stochasticOperator.RandomParameter.Hidden = true;
      }
    }

    private void ParameterizeStochasticOperatorForLayer(IOperator @operator) {
      var stochasticOperator = @operator as IStochasticOperator;
      if (stochasticOperator != null) {
        stochasticOperator.RandomParameter.ActualName = "LocalRandom";
        stochasticOperator.RandomParameter.Hidden = true;
      }
    }

    #endregion

    #region Updates
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      LayerAnalyzer.Operators.Clear();

      Analyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(ageAnalyzer, ageAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(ageDistributionAnalyzer, ageDistributionAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(selectionPressureAnalyzer, false);
      selectionPressureAnalyzer.ValueParameter.Depth = 1; // Adding analyzer sets depth to 2
      Analyzer.Operators.Add(currentSuccessRatioAnalyzer, false);
      currentSuccessRatioAnalyzer.ValueParameter.Depth = 1; // Adding analyzer sets depth to 2
      LayerAnalyzer.Operators.Add(layerQualityAnalyzer, false);
      LayerAnalyzer.Operators.Add(layerAgeAnalyzer, false);
      LayerAnalyzer.Operators.Add(layerAgeDistributionAnalyzer, false);
      LayerAnalyzer.Operators.Add(layerSelectionPressureAnalyzer, false);
      layerSelectionPressureAnalyzer.ValueParameter.Depth = 0; // Adding layer-analyzer sets depth to 1


      if (Problem != null) {
        foreach (var analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
          LayerAnalyzer.Operators.Add((IAnalyzer)analyzer.Clone(), false);
        }
      }
    }
    private void UpdateCrossovers() {
      var oldCrossover = CrossoverParameter.Value;
      var defaultCrossover = Problem.Operators.OfType<ICrossover>().FirstOrDefault();
      CrossoverParameter.ValidValues.Clear();
      foreach (var crossover in Problem.Operators.OfType<ICrossover>().OrderBy(c => c.Name)) {
        ParameterizeStochasticOperatorForLayer(crossover);
        CrossoverParameter.ValidValues.Add(crossover);
      }
      if (oldCrossover != null) {
        var crossover = CrossoverParameter.ValidValues.FirstOrDefault(c => c.GetType() == oldCrossover.GetType());
        if (crossover != null)
          CrossoverParameter.Value = crossover;
        else
          oldCrossover = null;
      }
      if (oldCrossover == null && defaultCrossover != null)
        CrossoverParameter.Value = defaultCrossover;
    }
    private void UpdateMutators() {
      IManipulator oldMutator = MutatorParameter.Value;
      MutatorParameter.ValidValues.Clear();
      IManipulator defaultMutator = Problem.Operators.OfType<IManipulator>().FirstOrDefault();

      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name)) {
        ParameterizeStochasticOperatorForLayer(mutator);
        MutatorParameter.ValidValues.Add(mutator);
      }

      if (oldMutator != null) {
        IManipulator mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
        if (mutator != null) MutatorParameter.Value = mutator;
        else oldMutator = null;
      }

      if (oldMutator == null && defaultMutator != null)
        MutatorParameter.Value = defaultMutator;
    }
    private void UpdateTerminators() {
      var newTerminators = new Dictionary<ITerminator, bool> {
        {generationsTerminator, !Terminators.Operators.Contains(generationsTerminator) || Terminators.Operators.ItemChecked(generationsTerminator)},
        {evaluationsTerminator, Terminators.Operators.Contains(evaluationsTerminator) && Terminators.Operators.ItemChecked(evaluationsTerminator)},
        {qualityTerminator, Terminators.Operators.Contains(qualityTerminator) && Terminators.Operators.ItemChecked(qualityTerminator) },
        {executionTimeTerminator, Terminators.Operators.Contains(executionTimeTerminator) && Terminators.Operators.ItemChecked(executionTimeTerminator)}
      };
      if (Problem != null) {
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
