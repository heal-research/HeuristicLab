#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Analysis;
using HeuristicLab.Random;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.NSGA2 {
  /// <summary>
  /// The Nondominated Sorting Genetic Algorithm II was introduced in Deb et al. 2002. A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), pp. 182-197.
  /// </summary>
  [Item("NSGA-II", "The Nondominated Sorting Genetic Algorithm II was introduced in Deb et al. 2002. A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), pp. 182-197.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public class NSGA2 : EngineAlgorithm {
    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(IMultiObjectiveProblem); }
    }
    public new IMultiObjectiveProblem Problem {
      get { return (IMultiObjectiveProblem)base.Problem; }
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
    private ConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (ConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    private ValueParameter<PercentValue> CrossoverProbabilityParameter {
      get { return (ValueParameter<PercentValue>)Parameters["CrossoverProbability"]; }
    }
    private ConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (ConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    private ValueParameter<PercentValue> MutationProbabilityParameter {
      get { return (ValueParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    private OptionalConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (OptionalConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    private ValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaximumGenerations"]; }
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
    public ISelector Selector {
      get { return SelectorParameter.Value; }
      set { SelectorParameter.Value = value; }
    }
    public PercentValue CrossoverProbability {
      get { return CrossoverProbabilityParameter.Value; }
      set { CrossoverProbabilityParameter.Value = value; }
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
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    public IntValue MaximumGenerations {
      get { return MaximumGenerationsParameter.Value; }
      set { MaximumGenerationsParameter.Value = value; }
    }
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    private SolutionsCreator SolutionsCreator {
      get { return (SolutionsCreator)RandomCreator.Successor; }
    }
    private NSGA2MainLoop MainLoop {
      get { return (NSGA2MainLoop)SolutionsCreator.Successor; }
    }
    #endregion

    [StorableConstructor]
    public NSGA2(bool deserializing) : base(deserializing) { }
    public NSGA2() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The size of the population of solutions.", new IntValue(100)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueParameter<PercentValue>("CrossoverProbability", "The probability that the crossover operator is applied on two parents.", new PercentValue(0.05)));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new OptionalConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze each generation.", new MultiAnalyzer()));
      Parameters.Add(new ValueParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed.", new IntValue(1000)));

      RandomCreator randomCreator = new RandomCreator();
      SolutionsCreator solutionsCreator = new SolutionsCreator();
      NSGA2MainLoop mainLoop = new NSGA2MainLoop();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = solutionsCreator;

      solutionsCreator.NumberOfSolutionsParameter.ActualName = PopulationSizeParameter.Name;
      solutionsCreator.Successor = mainLoop;

      mainLoop.SelectorParameter.ActualName = SelectorParameter.Name;
      mainLoop.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainLoop.CrossoverProbabilityParameter.ActualName = CrossoverProbabilityParameter.Name;
      mainLoop.MaximumGenerationsParameter.ActualName = MaximumGenerationsParameter.Name;
      mainLoop.MutatorParameter.ActualName = MutatorParameter.Name;
      mainLoop.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainLoop.RandomParameter.ActualName = RandomCreator.RandomParameter.ActualName;
      mainLoop.AnalyzerParameter.ActualName = AnalyzerParameter.Name;
      mainLoop.ResultsParameter.ActualName = "Results";

      foreach (ISelector selector in ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name))
        SelectorParameter.ValidValues.Add(selector);
      ISelector proportionalSelector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("ProportionalSelector"));
      if (proportionalSelector != null) SelectorParameter.Value = proportionalSelector;
      
      // TODO: parameterize selectors

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      NSGA2 clone = (NSGA2)base.Clone(cloner);
      // TODO: Clone additional fields
      return clone;
    }

    #region Events
    protected override void OnProblemChanged() {
      // parameterize pretty much everything
      base.OnProblemChanged();
    }
    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      // parameterize SolutionCreator
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      // parameterize StochasticOperators
      // parameterize SolutionsCreator
      // parameterize MainLoop
      // parameterize Selectors
      // parameterize Analyzers
      Problem.Evaluator.QualitiesParameter.ActualNameChanged += new EventHandler(Evaluator_QualitiesParameter_ActualNameChanged);
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      base.Problem_OperatorsChanged(sender, e);
    }
    protected override void Problem_Reset(object sender, EventArgs e) {
      base.Problem_Reset(sender, e);
    }

    private void PopulationSizeParameter_ValueChanged(object sender, EventArgs e) {
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      // parameterize Selectors
    }
    private void PopulationSize_ValueChanged(object sender, EventArgs e) {
      // parameterize Selectors
    }

    private void Evaluator_QualitiesParameter_ActualNameChanged(object sender, EventArgs e) {
      // parameterize Analyzers
      // parameterize MainLoop
      // parameterize Selectors
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AttachEventHandlers() {
      PopulationSizeParameter.ValueChanged += new EventHandler(PopulationSizeParameter_ValueChanged);
      PopulationSize.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      if (Problem != null) {
        Problem.Evaluator.QualitiesParameter.ActualNameChanged += new EventHandler(Evaluator_QualitiesParameter_ActualNameChanged);
      }
    }
    #endregion
  }
}
