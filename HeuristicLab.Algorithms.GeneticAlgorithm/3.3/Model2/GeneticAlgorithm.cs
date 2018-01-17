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
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Model2;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.GeneticAlgorithm.Model2 {
  /// <summary>
  /// A genetic algorithm.
  /// </summary>
  [Item("Genetic Algorithm (GA) v2", "A genetic algorithm.")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 100)]
  [StorableClass]
  public sealed class GeneticAlgorithm : StochasticAlgorithm<PopulationContext<SolutionScope>> {
    public override bool SupportsPause {
      get { return true; }
    }

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
    private FixedValueParameter<IntValue> PopulationSizeParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    public IConstrainedValueParameter<ISelector> SelectorParameter {
      get { return (IConstrainedValueParameter<ISelector>)Parameters["Selector"]; }
    }
    public IConstrainedValueParameter<ICrossover> CrossoverParameter {
      get { return (IConstrainedValueParameter<ICrossover>)Parameters["Crossover"]; }
    }
    private FixedValueParameter<PercentValue> MutationProbabilityParameter {
      get { return (FixedValueParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public IConstrainedValueParameter<IManipulator> MutatorParameter {
      get { return (IConstrainedValueParameter<IManipulator>)Parameters["Mutator"]; }
    }
    private FixedValueParameter<IntValue> ElitesParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["Elites"]; }
    }
    private IFixedValueParameter<BoolValue> ReevaluateElitesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["ReevaluateElites"]; }
    }
    #endregion

    #region Properties
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
    public double MutationProbability {
      get { return MutationProbabilityParameter.Value.Value; }
      set { MutationProbabilityParameter.Value.Value = value; }
    }
    public IManipulator Mutator {
      get { return MutatorParameter.Value; }
      set { MutatorParameter.Value = value; }
    }
    public int Elites {
      get { return ElitesParameter.Value.Value; }
      set { ElitesParameter.Value.Value = value; }
    }
    public bool ReevaluteElites {
      get { return ReevaluateElitesParameter.Value.Value; }
      set { ReevaluateElitesParameter.Value.Value = value; }
    }

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;
    #endregion

    public GeneticAlgorithm()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("PopulationSize", "The size of the population of solutions.", new IntValue(100)));
      Parameters.Add(new ConstrainedValueParameter<ISelector>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ConstrainedValueParameter<ICrossover>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new FixedValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.05)));
      Parameters.Add(new ConstrainedValueParameter<IManipulator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new FixedValueParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>("ReevaluateElites", "Flag to determine if elite individuals should be reevaluated (i.e., if stochastic fitness functions are used.)", new BoolValue(false)) { Hidden = true });

      Analyzer = new MultiAnalyzer();

      foreach (ISelector selector in ApplicationManager.Manager.GetInstances<ISelector>().Where(x => !(x is IMultiObjectiveSelector)).OrderBy(x => x.Name))
        SelectorParameter.ValidValues.Add(selector);
      ISelector proportionalSelector = SelectorParameter.ValidValues.FirstOrDefault(x => x.GetType().Name.Equals("ProportionalSelector"));
      if (proportionalSelector != null) SelectorParameter.Value = proportionalSelector;
      ParameterizeSelectors();

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      ParameterizeAnalyzers();
      UpdateAnalyzers();

      RegisterEventHandlers();
    }
    [StorableConstructor]
    private GeneticAlgorithm(bool deserializing) : base(deserializing) {
      RegisterEventHandlers();
    }

    private GeneticAlgorithm(GeneticAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeneticAlgorithm(this, cloner);
    }

    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);
      var max = ((IValueParameter<BoolValue>)Problem.MaximizationParameter).Value.Value;
      Context.BestQuality = max ? double.MinValue : double.MaxValue;

      for (var m = 0; m < PopulationSize; m++) {
        Context.AddToPopulation(new SolutionScope(Problem.Evaluator.QualityParameter.ActualName));
        Context.RunOperator(Problem.SolutionCreator, m, cancellationToken);
      }

      var locker = new object();
      Parallel.For(0, PopulationSize, (i) => {
        Context.RunOperator(Problem.Evaluator, Context.AtPopulation(i), cancellationToken);
        var fit = Context.AtPopulation(i).Fitness;
        if (max && Context.BestQuality < fit || !max && Context.BestQuality > fit) {
          lock (locker) {
            if (max && Context.BestQuality < fit || !max && Context.BestQuality > fit) {
              Context.BestQuality = fit;
            }
          }
        }
      });

      Context.EvaluatedSolutions += PopulationSize;

      Results.Add(new Result("Iterations", new IntValue(Context.Iterations)));
      Results.Add(new Result("EvaluatedSolutions", new IntValue(Context.EvaluatedSolutions)));
      Results.Add(new Result("BestQuality", new DoubleValue(Context.BestQuality)));

      Context.RunOperator(Analyzer, cancellationToken);
    }

    protected override void Run(CancellationToken cancellationToken) {
      var lastUpdate = ExecutionTime;
      var max = ((IValueParameter<BoolValue>)Problem.MaximizationParameter).Value.Value;

      IResult result;
      while (!StoppingCriterion()) {
        Context.Iterations++;
        
        Context.RunOperator(Selector, cancellationToken);
        var currentGen = Context.Scope.SubScopes[0];
        var nextGen = Context.Scope.SubScopes[1];
        for (var i = 0; i < PopulationSize - Elites; i++) {
          var offspring = new SolutionScope(Problem.Evaluator.QualityParameter.ActualName);
          var p1 = nextGen.SubScopes[0];
          nextGen.SubScopes.RemoveAt(0);
          offspring.SubScopes.Add(p1);
          var p2 = nextGen.SubScopes[0];
          nextGen.SubScopes.RemoveAt(0);
          offspring.SubScopes.Add(p2);
          nextGen.SubScopes.Add(offspring);

          Context.RunOperator(Crossover, offspring, cancellationToken);
          offspring.SubScopes.Clear();
          if (Context.Random.NextDouble() < MutationProbability) {
            Context.RunOperator(Mutator, offspring, cancellationToken);
          }
        }

        var locker = new object();
        Parallel.For(0, PopulationSize - Elites, (i) => {
          Context.RunOperator(Problem.Evaluator, nextGen.SubScopes[i], cancellationToken);
          var fit = ((ISolutionScope)nextGen.SubScopes[i]).Fitness;
          if (max && Context.BestQuality < fit || !max && Context.BestQuality > fit) {
            lock (locker) {
              if (max && Context.BestQuality < fit || !max && Context.BestQuality > fit) {
                Context.BestQuality = fit;
              }
            }
          }
        });
        Context.EvaluatedSolutions += PopulationSize - Elites;
        var elites = (max ? currentGen.SubScopes.OfType<SolutionScope>()
          .OrderByDescending(x => x.Fitness).Take(Elites)
          : currentGen.SubScopes.OfType<SolutionScope>()
          .OrderBy(x => x.Fitness).Take(Elites)).ToList();
        if (ReevaluteElites) {
          Parallel.For(0, Elites, (i) => {
            Context.RunOperator(Problem.Evaluator, elites[i], cancellationToken);
          });
          Context.EvaluatedSolutions += Elites;
        }
        nextGen.SubScopes.AddRange(elites);
        Context.Scope.SubScopes.Replace(nextGen.SubScopes);

        if (ExecutionTime - lastUpdate > TimeSpan.FromSeconds(1)) {
          if (Results.TryGetValue("Iterations", out result))
            ((IntValue)result.Value).Value = Context.Iterations;
          else Results.Add(new Result("Iterations", new IntValue(Context.Iterations)));
          if (Results.TryGetValue("EvaluatedSolutions", out result))
            ((IntValue)result.Value).Value = Context.EvaluatedSolutions;
          else Results.Add(new Result("EvaluatedSolutions", new IntValue(Context.EvaluatedSolutions)));
          lastUpdate = ExecutionTime;
        }
        if (Results.TryGetValue("BestQuality", out result))
          ((DoubleValue)result.Value).Value = Context.BestQuality;
        else Results.Add(new Result("BestQuality", new DoubleValue(Context.BestQuality)));

        Context.RunOperator(Analyzer, cancellationToken);

        if (cancellationToken.IsCancellationRequested) break;
      }

      if (Results.TryGetValue("Iterations", out result))
        ((IntValue)result.Value).Value = Context.Iterations;
      else Results.Add(new Result("Iterations", new IntValue(Context.Iterations)));
      if (Results.TryGetValue("EvaluatedSolutions", out result))
        ((IntValue)result.Value).Value = Context.EvaluatedSolutions;
      else Results.Add(new Result("EvaluatedSolutions", new IntValue(Context.EvaluatedSolutions)));
      
      if (Results.TryGetValue("BestQuality", out result))
        ((DoubleValue) result.Value).Value = Context.BestQuality;
      else Results.Add(new Result("BestQuality", new DoubleValue(Context.BestQuality)));
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      ParameterizeStochasticOperator(Problem.SolutionCreator);
      ParameterizeStochasticOperator(Problem.Evaluator);
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeSelectors();
      ParameterizeAnalyzers();
      ParameterizeIterationBasedOperators();
      UpdateCrossovers();
      UpdateMutators();
      UpdateAnalyzers();
      Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      base.OnProblemChanged();
    }
    
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      foreach (IOperator op in Problem.Operators.OfType<IOperator>()) ParameterizeStochasticOperator(op);
      ParameterizeIterationBasedOperators();
      UpdateCrossovers();
      UpdateMutators();
      UpdateAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }
    private void Elites_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void PopulationSize_ValueChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeSelectors();
      ParameterizeAnalyzers();
    }
    #endregion

    #region Helpers
    private void RegisterEventHandlers() {
      PopulationSizeParameter.Value.ValueChanged += new EventHandler(PopulationSize_ValueChanged);
      ElitesParameter.Value.ValueChanged += new EventHandler(Elites_ValueChanged);
      if (Problem != null) {
        Problem.Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      }
    }
    
    private void ParameterizeStochasticOperator(IOperator op) {
      IStochasticOperator stochasticOp = op as IStochasticOperator;
      if (stochasticOp != null) {
        stochasticOp.RandomParameter.ActualName = nameof(Context.Random);
        stochasticOp.RandomParameter.Hidden = true;
      }
    }
    private void ParameterizeSelectors() {
      foreach (ISelector selector in SelectorParameter.ValidValues) {
        selector.CopySelected = new BoolValue(true);
        selector.NumberOfSelectedSubScopesParameter.Value = new IntValue(2 * (PopulationSizeParameter.Value.Value - ElitesParameter.Value.Value));
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
      IManipulator defaultMutator = Problem.Operators.OfType<IManipulator>().FirstOrDefault();

      foreach (IManipulator mutator in Problem.Operators.OfType<IManipulator>().OrderBy(x => x.Name))
        MutatorParameter.ValidValues.Add(mutator);

      if (oldMutator != null) {
        IManipulator mutator = MutatorParameter.ValidValues.FirstOrDefault(x => x.GetType() == oldMutator.GetType());
        if (mutator != null) MutatorParameter.Value = mutator;
        else oldMutator = null;
      }

      if (oldMutator == null && defaultMutator != null)
        MutatorParameter.Value = defaultMutator;
    }
    private void UpdateAnalyzers() {
      var multiAnalyzer = (MultiAnalyzer)Analyzer;
      multiAnalyzer.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 1;
          multiAnalyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
      multiAnalyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
    }
    #endregion
  }
}
