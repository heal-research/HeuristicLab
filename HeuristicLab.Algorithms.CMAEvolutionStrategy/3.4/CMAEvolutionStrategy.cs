#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("CMA Evolution Strategy (CMAES)", "An evolution strategy based on covariance matrix adaptation.")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 210)]
  [StorableClass]
  public sealed class CMAEvolutionStrategy : HeuristicOptimizationEngineAlgorithm, IStorableContent {
    public string Filename { get; set; }
    #region Strings
    private const string SeedName = "Seed";
    private const string SetSeedRandomlyName = "SetSeedRandomly";
    private const string PopulationSizeName = "PopulationSize";
    private const string InitialIterationsName = "InitialIterations";
    private const string InitialSigmaName = "InitialSigma";
    private const string MuName = "Mu";
    private const string CMAInitializerName = "CMAInitializer";
    private const string CMAMutatorName = "CMAMutator";
    private const string CMARecombinatorName = "CMARecombinator";
    private const string CMAUpdaterName = "CMAUpdater";
    private const string AnalyzerName = "Analyzer";
    private const string MaximumGenerationsName = "MaximumGenerations";
    private const string MaximumEvaluatedSolutionsName = "MaximumEvaluatedSolutions";
    private const string TargetQualityName = "TargetQuality";
    private const string MinimumQualityChangeName = "MinimumQualityChange";
    private const string MinimumQualityHistoryChangeName = "MinimumQualityHistoryChange";
    private const string MinimumStandardDeviationName = "MinimumStandardDeviation";
    private const string MaximumStandardDeviationChangeName = "MaximumStandardDeviationChange";
    #endregion

    #region Problem Properties

    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblem<RealVectorEncoding, RealVector>); }
    }
    public new ISingleObjectiveProblem<RealVectorEncoding, RealVector> Problem {
      get { return (ISingleObjectiveProblem<RealVectorEncoding, RealVector>)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region Parameter Properties
    public IValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (IValueParameter<MultiAnalyzer>)Parameters[AnalyzerName]; }
    }
    private IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedName]; }
    }
    private IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyName]; }
    }
    private IFixedValueParameter<IntValue> PopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[PopulationSizeName]; }
    }
    private IFixedValueParameter<IntValue> InitialIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[InitialIterationsName]; }
    }
    public IValueParameter<DoubleArray> InitialSigmaParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[InitialSigmaName]; }
    }
    private OptionalValueParameter<IntValue> MuParameter {
      get { return (OptionalValueParameter<IntValue>)Parameters[MuName]; }
    }
    public IConstrainedValueParameter<ICMAInitializer> CMAInitializerParameter {
      get { return (IConstrainedValueParameter<ICMAInitializer>)Parameters[CMAInitializerName]; }
    }
    public IConstrainedValueParameter<ICMAManipulator> CMAMutatorParameter {
      get { return (IConstrainedValueParameter<ICMAManipulator>)Parameters[CMAMutatorName]; }
    }
    public IConstrainedValueParameter<ICMARecombinator> CMARecombinatorParameter {
      get { return (IConstrainedValueParameter<ICMARecombinator>)Parameters[CMARecombinatorName]; }
    }
    public IConstrainedValueParameter<ICMAUpdater> CMAUpdaterParameter {
      get { return (IConstrainedValueParameter<ICMAUpdater>)Parameters[CMAUpdaterName]; }
    }
    private IFixedValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumGenerationsName]; }
    }
    private IFixedValueParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumEvaluatedSolutionsName]; }
    }
    private IFixedValueParameter<DoubleValue> TargetQualityParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[TargetQualityName]; }
    }
    private IFixedValueParameter<DoubleValue> MinimumQualityChangeParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[MinimumQualityChangeName]; }
    }
    private IFixedValueParameter<DoubleValue> MinimumQualityHistoryChangeParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[MinimumQualityHistoryChangeName]; }
    }
    private IFixedValueParameter<DoubleValue> MinimumStandardDeviationParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[MinimumStandardDeviationName]; }
    }
    private IFixedValueParameter<DoubleValue> MaximumStandardDeviationChangeParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[MaximumStandardDeviationChangeName]; }
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
    public int PopulationSize {
      get { return PopulationSizeParameter.Value.Value; }
      set { PopulationSizeParameter.Value.Value = value; }
    }
    public int InitialIterations {
      get { return InitialIterationsParameter.Value.Value; }
      set { InitialIterationsParameter.Value.Value = value; }
    }
    public int MaximumGenerations {
      get { return MaximumGenerationsParameter.Value.Value; }
      set { MaximumGenerationsParameter.Value.Value = value; }
    }
    public int MaximumEvaluatedSolutions {
      get { return MaximumEvaluatedSolutionsParameter.Value.Value; }
      set { MaximumEvaluatedSolutionsParameter.Value.Value = value; }
    }
    public double TargetQuality {
      get { return TargetQualityParameter.Value.Value; }
      set { TargetQualityParameter.Value.Value = value; }
    }
    public double MinimumQualityChange {
      get { return MinimumQualityChangeParameter.Value.Value; }
      set { MinimumQualityChangeParameter.Value.Value = value; }
    }
    public double MinimumQualityHistoryChange {
      get { return MinimumQualityHistoryChangeParameter.Value.Value; }
      set { MinimumQualityHistoryChangeParameter.Value.Value = value; }
    }
    public double MinimumStandardDeviation {
      get { return MinimumStandardDeviationParameter.Value.Value; }
      set { MinimumStandardDeviationParameter.Value.Value = value; }
    }
    public double MaximumStandardDeviationChange {
      get { return MaximumStandardDeviationChangeParameter.Value.Value; }
      set { MaximumStandardDeviationChangeParameter.Value.Value = value; }
    }
    public DoubleArray InitialSigma {
      get { return InitialSigmaParameter.Value; }
      set { InitialSigmaParameter.Value = value; }
    }
    public IntValue Mu {
      get { return MuParameter.Value; }
      set { MuParameter.Value = value; }
    }
    public ICMAInitializer CMAInitializer {
      get { return CMAInitializerParameter.Value; }
      set { CMAInitializerParameter.Value = value; }
    }
    public ICMAManipulator CMAMutator {
      get { return CMAMutatorParameter.Value; }
      set { CMAMutatorParameter.Value = value; }
    }
    public ICMARecombinator CMARecombinator {
      get { return CMARecombinatorParameter.Value; }
      set { CMARecombinatorParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public ICMAUpdater CMAUpdater {
      get { return CMAUpdaterParameter.Value; }
      set { CMAUpdaterParameter.Value = value; }
    }

    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    [Storable]
    private CMAAnalyzer cmaAnalyzer;
    [Storable]
    private Placeholder solutionCreator;
    [Storable]
    private Placeholder populationSolutionCreator;
    [Storable]
    private Placeholder evaluator;
    [Storable]
    private SubScopesSorter sorter;
    [Storable]
    private Terminator terminator;
    #endregion

    [StorableConstructor]
    private CMAEvolutionStrategy(bool deserializing) : base(deserializing) { }
    private CMAEvolutionStrategy(CMAEvolutionStrategy original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      cmaAnalyzer = cloner.Clone(original.cmaAnalyzer);
      solutionCreator = cloner.Clone(original.solutionCreator);
      populationSolutionCreator = cloner.Clone(original.populationSolutionCreator);
      evaluator = cloner.Clone(original.evaluator);
      sorter = cloner.Clone(original.sorter);
      terminator = cloner.Clone(original.terminator);
      RegisterEventHandlers();
    }
    public CMAEvolutionStrategy()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(SeedName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(PopulationSizeName, "λ (lambda) - the size of the offspring population.", new IntValue(20)));
      Parameters.Add(new FixedValueParameter<IntValue>(InitialIterationsName, "The number of iterations that should be performed with only axis parallel mutation.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<DoubleArray>(InitialSigmaName, "The initial sigma can be a single value or a value for each dimension. All values need to be > 0.", new DoubleArray(new[] { 0.5 })));
      Parameters.Add(new OptionalValueParameter<IntValue>(MuName, "Optional, the mu best offspring that should be considered for update of the new mean and strategy parameters. If not given it will be automatically calculated."));
      Parameters.Add(new ConstrainedValueParameter<ICMARecombinator>(CMARecombinatorName, "The operator used to calculate the new mean."));
      Parameters.Add(new ConstrainedValueParameter<ICMAManipulator>(CMAMutatorName, "The operator used to manipulate a point."));
      Parameters.Add(new ConstrainedValueParameter<ICMAInitializer>(CMAInitializerName, "The operator that initializes the covariance matrix and strategy parameters."));
      Parameters.Add(new ConstrainedValueParameter<ICMAUpdater>(CMAUpdaterName, "The operator that updates the covariance matrix and strategy parameters."));
      Parameters.Add(new ValueParameter<MultiAnalyzer>(AnalyzerName, "The operator used to analyze each generation.", new MultiAnalyzer()));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumGenerationsName, "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumEvaluatedSolutionsName, "The maximum number of evaluated solutions that should be computed.", new IntValue(int.MaxValue)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(TargetQualityName, "(stopFitness) Surpassing this quality value terminates the algorithm.", new DoubleValue(double.NaN)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MinimumQualityChangeName, "(stopTolFun) If the range of fitness values is less than a certain value the algorithm terminates (set to 0 or positive value to enable).", new DoubleValue(double.NaN)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MinimumQualityHistoryChangeName, "(stopTolFunHist) If the range of fitness values is less than a certain value for a certain time the algorithm terminates (set to 0 or positive to enable).", new DoubleValue(double.NaN)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MinimumStandardDeviationName, "(stopTolXFactor) If the standard deviation falls below a certain value the algorithm terminates (set to 0 or positive to enable).", new DoubleValue(double.NaN)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MaximumStandardDeviationChangeName, "(stopTolUpXFactor) If the standard deviation changes by a value larger than this parameter the algorithm stops (set to a value > 0 to enable).", new DoubleValue(double.NaN)));

      var randomCreator = new RandomCreator();
      var variableCreator = new VariableCreator();
      var resultsCollector = new ResultsCollector();
      var cmaInitializer = new Placeholder();
      solutionCreator = new Placeholder();
      var subScopesCreator = new SubScopesCreator();
      var ussp1 = new UniformSubScopesProcessor();
      populationSolutionCreator = new Placeholder();
      var cmaMutator = new Placeholder();
      var ussp2 = new UniformSubScopesProcessor();
      evaluator = new Placeholder();
      var subScopesCounter = new SubScopesCounter();
      sorter = new SubScopesSorter();
      var analyzer = new Placeholder();
      var cmaRecombinator = new Placeholder();
      var generationsCounter = new IntCounter();
      var cmaUpdater = new Placeholder();
      terminator = new Terminator();

      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = variableCreator;

      variableCreator.Name = "Initialize Variables";
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      variableCreator.Successor = resultsCollector;

      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("EvaluatedSolutions"));
      resultsCollector.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector.ResultsParameter.ActualName = "Results";
      resultsCollector.Successor = cmaInitializer;

      cmaInitializer.Name = "Initialize Strategy Parameters";
      cmaInitializer.OperatorParameter.ActualName = CMAInitializerParameter.Name;
      cmaInitializer.Successor = subScopesCreator;

      subScopesCreator.NumberOfSubScopesParameter.ActualName = PopulationSizeParameter.Name;
      subScopesCreator.Successor = ussp1;

      ussp1.Name = "Create population";
      ussp1.Parallel = new BoolValue(false);
      ussp1.Operator = populationSolutionCreator;
      ussp1.Successor = solutionCreator;

      populationSolutionCreator.Name = "Initialize arx";
      // populationSolutionCreator.OperatorParameter will be wired
      populationSolutionCreator.Successor = null;

      solutionCreator.Name = "Initialize xmean";
      // solutionCreator.OperatorParameter will be wired
      solutionCreator.Successor = cmaMutator;

      cmaMutator.Name = "Sample population";
      cmaMutator.OperatorParameter.ActualName = CMAMutatorParameter.Name;
      cmaMutator.Successor = ussp2;

      ussp2.Name = "Evaluate offspring";
      ussp2.Parallel = new BoolValue(true);
      ussp2.Operator = evaluator;
      ussp2.Successor = subScopesCounter;

      evaluator.Name = "Evaluator";
      // evaluator.OperatorParameter will be wired
      evaluator.Successor = null;

      subScopesCounter.Name = "Count EvaluatedSolutions";
      subScopesCounter.AccumulateParameter.Value = new BoolValue(true);
      subScopesCounter.ValueParameter.ActualName = "EvaluatedSolutions";
      subScopesCounter.Successor = sorter;

      // sorter.ValueParameter will be wired
      // sorter.DescendingParameter will be wired
      sorter.Successor = analyzer;

      analyzer.Name = "Analyzer";
      analyzer.OperatorParameter.ActualName = AnalyzerParameter.Name;
      analyzer.Successor = cmaRecombinator;

      cmaRecombinator.Name = "Create new xmean";
      cmaRecombinator.OperatorParameter.ActualName = CMARecombinatorParameter.Name;
      cmaRecombinator.Successor = generationsCounter;

      generationsCounter.Name = "Generations++";
      generationsCounter.IncrementParameter.Value = new IntValue(1);
      generationsCounter.ValueParameter.ActualName = "Generations";
      generationsCounter.Successor = cmaUpdater;

      cmaUpdater.Name = "Update distributions";
      cmaUpdater.OperatorParameter.ActualName = CMAUpdaterParameter.Name;
      cmaUpdater.Successor = terminator;

      terminator.Continue = cmaMutator;
      terminator.Terminate = null;

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      cmaAnalyzer = new CMAAnalyzer();

      InitializeOperators();
      RegisterEventHandlers();
      Parameterize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMAEvolutionStrategy(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      var creator = Problem.SolutionCreator as IRealVectorCreator;
      if (creator != null) {
        creator.RealVectorParameter.ActualNameChanged += RealVectorCreator_Changed;
        creator.LengthParameter.ActualNameChanged += RealVectorCreator_Changed;
        creator.LengthParameter.ValueChanged += RealVectorCreator_Changed;
        creator.BoundsParameter.ActualNameChanged += RealVectorCreator_Changed;
        creator.BoundsParameter.ValueChanged += RealVectorCreator_Changed;
      }
      UpdateOperators();
      UpdateAnalyzers();
      Parameterize();
      base.OnProblemChanged();
    }
    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      var creator = Problem.SolutionCreator as IRealVectorCreator;
      if (creator != null) {
        creator.RealVectorParameter.ActualNameChanged += RealVectorCreator_Changed;
        creator.LengthParameter.ActualNameChanged += RealVectorCreator_Changed;
        creator.LengthParameter.ValueChanged += RealVectorCreator_Changed;
        creator.BoundsParameter.ActualNameChanged += RealVectorCreator_Changed;
        creator.BoundsParameter.ValueChanged += RealVectorCreator_Changed;
      }
      Parameterize();
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      Parameterize();
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      UpdateOperators();
      UpdateAnalyzers();
      Parameterize();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      Parameterize();
    }
    private bool cmaesInitializerSync;
    private void CMAESInitializerParameter_ValueChanged(object sender, EventArgs e) {
      if (cmaesInitializerSync) return;
      UpdateOperators();
      Parameterize();
    }
    private void RealVectorCreator_Changed(object sender, EventArgs e) {
      Parameterize();
    }
    #endregion

    #region Helpers
    private void RegisterEventHandlers() {
      CMAInitializerParameter.ValueChanged += CMAESInitializerParameter_ValueChanged;
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
        var creator = Problem.SolutionCreator as IRealVectorCreator;
        if (creator != null) {
          creator.RealVectorParameter.ActualNameChanged += RealVectorCreator_Changed;
          creator.LengthParameter.ActualNameChanged += RealVectorCreator_Changed;
          creator.LengthParameter.ValueChanged += RealVectorCreator_Changed;
          creator.BoundsParameter.ActualNameChanged += RealVectorCreator_Changed;
          creator.BoundsParameter.ValueChanged += RealVectorCreator_Changed;
        }
      }
    }
    private void InitializeOperators() {
      foreach (var op in ApplicationManager.Manager.GetInstances<ICMAInitializer>())
        CMAInitializerParameter.ValidValues.Add(op);
      foreach (var op in ApplicationManager.Manager.GetInstances<ICMAManipulator>().Where(x => x.CMAType == CMAInitializer.CMAType))
        CMAMutatorParameter.ValidValues.Add(op);
      foreach (var op in ApplicationManager.Manager.GetInstances<ICMARecombinator>().Where(x => x.CMAType == CMAInitializer.CMAType))
        CMARecombinatorParameter.ValidValues.Add(op);
      foreach (var op in ApplicationManager.Manager.GetInstances<ICMAUpdater>().Where(x => x.CMAType == CMAInitializer.CMAType))
        CMAUpdaterParameter.ValidValues.Add(op);
    }
    private void UpdateOperators() {
      cmaesInitializerSync = true;
      try {
        var oldMutator = CMAMutator;
        var oldRecombinator = CMARecombinator;
        var oldUpdater = CMAUpdater;

        if (CMAInitializer != null && (oldMutator == null || oldMutator.CMAType != CMAInitializer.CMAType)) {
          CMAMutatorParameter.ValidValues.Clear();
          foreach (var op in ApplicationManager.Manager.GetInstances<ICMAManipulator>().Where(x => x.CMAType == CMAInitializer.CMAType))
            CMAMutatorParameter.ValidValues.Add(op);
          CMAMutator = CMAMutatorParameter.ValidValues.First();
        }

        if (CMAInitializer != null && (oldRecombinator == null || oldRecombinator.CMAType != CMAInitializer.CMAType)) {
          CMARecombinatorParameter.ValidValues.Clear();
          foreach (var op in ApplicationManager.Manager.GetInstances<ICMARecombinator>().Where(x => x.CMAType == CMAInitializer.CMAType))
            CMARecombinatorParameter.ValidValues.Add(op);
          CMARecombinator = CMARecombinatorParameter.ValidValues.First();
        }

        if (CMAInitializer != null && (oldUpdater == null || oldUpdater.CMAType != CMAInitializer.CMAType)) {
          CMAUpdaterParameter.ValidValues.Clear();
          foreach (var op in ApplicationManager.Manager.GetInstances<ICMAUpdater>().Where(x => x.CMAType == CMAInitializer.CMAType))
            CMAUpdaterParameter.ValidValues.Add(op);
          CMAUpdater = CMAUpdaterParameter.ValidValues.First();
        }
      }
      finally { cmaesInitializerSync = false; }
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (var analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (var param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 1;
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
      Analyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
      Analyzer.Operators.Add(cmaAnalyzer, cmaAnalyzer.EnabledByDefault);
    }
    private void Parameterize() {

      foreach (var op in CMAInitializerParameter.ValidValues) {
        op.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
        op.PopulationSizeParameter.Hidden = true;
        op.MuParameter.ActualName = MuParameter.Name;
        op.MuParameter.Hidden = true;
        op.InitialIterationsParameter.Value = null;
        op.InitialIterationsParameter.ActualName = InitialIterationsParameter.Name;
        op.InitialIterationsParameter.Hidden = true;
        op.InitialSigmaParameter.Value = null;
        op.InitialSigmaParameter.ActualName = InitialSigmaParameter.Name;
        op.InitialSigmaParameter.Hidden = true;

        op.DimensionParameter.Hidden = false;

        ParameterizeStochasticOperator(op);
        ParameterizeIterationBasedOperator(op);
      }

      foreach (var op in CMAMutatorParameter.ValidValues) {
        op.PopulationSizeParameter.ActualName = PopulationSizeParameter.Name;
        op.PopulationSizeParameter.Hidden = true;

        op.MeanParameter.Hidden = false;
        op.BoundsParameter.Hidden = false;
        op.RealVectorParameter.Hidden = false;

        ParameterizeStochasticOperator(op);
        ParameterizeIterationBasedOperator(op);
      }

      foreach (var op in CMARecombinatorParameter.ValidValues) {
        op.OldMeanParameter.ActualName = "XOld";
        op.OldMeanParameter.Hidden = true;

        op.OffspringParameter.Hidden = false;
        op.MeanParameter.Hidden = false;

        ParameterizeStochasticOperator(op);
        ParameterizeIterationBasedOperator(op);
      }

      foreach (var op in CMAUpdaterParameter.ValidValues) {
        op.MaximumEvaluatedSolutionsParameter.ActualName = MaximumEvaluatedSolutionsParameter.Name;
        op.MaximumEvaluatedSolutionsParameter.Hidden = true;
        op.OldMeanParameter.ActualName = "XOld";
        op.OldMeanParameter.Hidden = true;

        op.MeanParameter.Hidden = false;
        op.OffspringParameter.Hidden = false;
        op.QualityParameter.Hidden = false;

        ParameterizeStochasticOperator(op);
        ParameterizeIterationBasedOperator(op);
      }

      terminator.IterationsParameter.ActualName = "Generations";
      terminator.MaximumIterationsParameter.ActualName = MaximumGenerationsParameter.Name;
      terminator.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";
      terminator.InitialSigmaParameter.ActualName = InitialSigmaParameter.Name;
      terminator.MaximumEvaluatedSolutionsParameter.ActualName = MaximumEvaluatedSolutionsParameter.Name;
      terminator.MaximumStandardDeviationChangeParameter.ActualName = MaximumStandardDeviationChangeParameter.Name;
      terminator.MinimumQualityChangeParameter.ActualName = MinimumQualityChangeParameter.Name;
      terminator.MinimumQualityHistoryChangeParameter.ActualName = MinimumQualityHistoryChangeParameter.Name;
      terminator.MinimumStandardDeviationParameter.ActualName = MinimumStandardDeviationParameter.Name;
      terminator.TargetQualityParameter.ActualName = TargetQualityParameter.Name;
      if (CMAUpdater != null)
        terminator.DegenerateStateParameter.ActualName = CMAUpdater.DegenerateStateParameter.ActualName;

      var creator = Problem != null ? (Problem.SolutionCreator as IRealVectorCreator) : null;
      if (Problem != null && creator != null) {

        solutionCreator.OperatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
        #region Backwards compatible code, remove with 3.4
        if (populationSolutionCreator != null) // BackwardsCompatibility3.3
          // ONLY REMOVE THE CONDITION
          populationSolutionCreator.OperatorParameter.ActualName = Problem.SolutionCreatorParameter.Name;
        #endregion
        evaluator.OperatorParameter.ActualName = Problem.EvaluatorParameter.Name;
        sorter.DescendingParameter.ActualName = Problem.MaximizationParameter.Name;
        sorter.ValueParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        terminator.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        terminator.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.Name;

        foreach (var op in CMAInitializerParameter.ValidValues) {
          if (creator.LengthParameter.Value == null) {
            op.DimensionParameter.ActualName = creator.LengthParameter.ActualName;
            op.DimensionParameter.Value = null;
          } else op.DimensionParameter.Value = creator.LengthParameter.Value;
          op.DimensionParameter.Hidden = true;
        }

        foreach (var op in CMAMutatorParameter.ValidValues) {
          op.MeanParameter.ActualName = creator.RealVectorParameter.ActualName;
          op.MeanParameter.Hidden = true;
          if (creator.BoundsParameter.Value == null) {
            op.BoundsParameter.ActualName = creator.BoundsParameter.ActualName;
            op.BoundsParameter.Value = null;
          } else op.BoundsParameter.Value = creator.BoundsParameter.Value;
          op.BoundsParameter.Hidden = true;
          op.RealVectorParameter.ActualName = creator.RealVectorParameter.ActualName;
          op.RealVectorParameter.Depth = 1;
          op.RealVectorParameter.Hidden = true;
        }

        foreach (var op in CMARecombinatorParameter.ValidValues) {
          op.MeanParameter.ActualName = creator.RealVectorParameter.ActualName;
          op.MeanParameter.Hidden = true;
          op.OffspringParameter.ActualName = creator.RealVectorParameter.ActualName;
          op.OffspringParameter.Depth = 1;
          op.OffspringParameter.Hidden = true;
        }

        foreach (var op in CMAUpdaterParameter.ValidValues) {
          op.MeanParameter.ActualName = creator.RealVectorParameter.ActualName;
          op.MeanParameter.Hidden = true;
          op.OffspringParameter.ActualName = creator.RealVectorParameter.ActualName;
          op.OffspringParameter.Depth = 1;
          op.OffspringParameter.Hidden = true;
          op.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
          op.QualityParameter.Hidden = true;
        }

        foreach (var op in Problem.Operators.OfType<IStochasticOperator>()) {
          op.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
          op.RandomParameter.Hidden = true;
        }

        qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
        qualityAnalyzer.MaximizationParameter.Hidden = true;
        qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        qualityAnalyzer.QualityParameter.Depth = 1;
        qualityAnalyzer.QualityParameter.Hidden = true;
        qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
        qualityAnalyzer.BestKnownQualityParameter.Hidden = true;

        cmaAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
        cmaAnalyzer.QualityParameter.Depth = 1;
        cmaAnalyzer.QualityParameter.Hidden = true;
        cmaAnalyzer.MeanParameter.ActualName = creator.RealVectorParameter.ActualName;
        cmaAnalyzer.MeanParameter.Hidden = true;

        foreach (var op in Problem.Operators.OfType<IIterationBasedOperator>()) {
          op.IterationsParameter.ActualName = "Generations";
          op.IterationsParameter.Hidden = true;
          op.MaximumIterationsParameter.ActualName = MaximumGenerationsParameter.Name;
          op.MaximumIterationsParameter.Hidden = true;
        }
      } else {
        qualityAnalyzer.MaximizationParameter.Hidden = false;
        qualityAnalyzer.QualityParameter.Hidden = false;
        qualityAnalyzer.BestKnownQualityParameter.Hidden = false;

        cmaAnalyzer.MeanParameter.Hidden = false;
      }

      qualityAnalyzer.ResultsParameter.ActualName = "Results";
      qualityAnalyzer.ResultsParameter.Hidden = true;
      cmaAnalyzer.ResultsParameter.ActualName = "Results";
      cmaAnalyzer.ResultsParameter.Hidden = true;
    }

    private void ParameterizeStochasticOperator(IOperator op) {
      var sOp = op as IStochasticOperator;
      if (sOp != null) sOp.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
    }

    private void ParameterizeIterationBasedOperator(IOperator op) {
      var iOp = op as IIterationBasedOperator;
      if (iOp != null) {
        iOp.IterationsParameter.ActualName = "Generations";
        iOp.MaximumIterationsParameter.ActualName = MaximumGenerationsParameter.Name;
      }
    }
    #endregion
  }
}
