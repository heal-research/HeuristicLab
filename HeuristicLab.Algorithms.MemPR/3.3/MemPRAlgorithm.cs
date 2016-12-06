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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Algorithms.MemPR.Util;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR {
  [Item("MemPR Algorithm", "Base class for MemPR algorithms")]
  [StorableClass]
  public abstract class MemPRAlgorithm<TProblem, TSolution, TPopulationContext, TSolutionContext> : BasicAlgorithm, INotifyPropertyChanged
      where TProblem : class, IItem, ISingleObjectiveHeuristicOptimizationProblem, ISingleObjectiveProblemDefinition
      where TSolution : class, IItem
      where TPopulationContext : MemPRPopulationContext<TProblem, TSolution, TPopulationContext, TSolutionContext>, new()
      where TSolutionContext : MemPRSolutionContext<TProblem, TSolution, TPopulationContext, TSolutionContext> {
    private const double MutationProbabilityMagicConst = 0.1;

    public override Type ProblemType {
      get { return typeof(TProblem); }
    }

    public new TProblem Problem {
      get { return (TProblem)base.Problem; }
      set { base.Problem = value; }
    }

    protected string QualityName {
      get { return Problem != null && Problem.Evaluator != null ? Problem.Evaluator.QualityParameter.ActualName : null; }
    }

    public int? MaximumEvaluations {
      get {
        var val = ((OptionalValueParameter<IntValue>)Parameters["MaximumEvaluations"]).Value;
        return val != null ? val.Value : (int?)null;
      }
      set {
        var param = (OptionalValueParameter<IntValue>)Parameters["MaximumEvaluations"];
        param.Value = value.HasValue ? new IntValue(value.Value) : null;
      }
    }

    public TimeSpan? MaximumExecutionTime {
      get {
        var val = ((OptionalValueParameter<TimeSpanValue>)Parameters["MaximumExecutionTime"]).Value;
        return val != null ? val.Value : (TimeSpan?)null;
      }
      set {
        var param = (OptionalValueParameter<TimeSpanValue>)Parameters["MaximumExecutionTime"];
        param.Value = value.HasValue ? new TimeSpanValue(value.Value) : null;
      }
    }

    public double? TargetQuality {
      get {
        var val = ((OptionalValueParameter<DoubleValue>)Parameters["TargetQuality"]).Value;
        return val != null ? val.Value : (double?)null;
      }
      set {
        var param = (OptionalValueParameter<DoubleValue>)Parameters["TargetQuality"];
        param.Value = value.HasValue ? new DoubleValue(value.Value) : null;
      }
    }

    protected FixedValueParameter<IntValue> MaximumPopulationSizeParameter {
      get { return ((FixedValueParameter<IntValue>)Parameters["MaximumPopulationSize"]); }
    }
    public int MaximumPopulationSize {
      get { return MaximumPopulationSizeParameter.Value.Value; }
      set { MaximumPopulationSizeParameter.Value.Value = value; }
    }

    public bool SetSeedRandomly {
      get { return ((FixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]).Value.Value; }
      set { ((FixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]).Value.Value = value; }
    }

    public int Seed {
      get { return ((FixedValueParameter<IntValue>)Parameters["Seed"]).Value.Value; }
      set { ((FixedValueParameter<IntValue>)Parameters["Seed"]).Value.Value = value; }
    }

    public IAnalyzer Analyzer {
      get { return ((ValueParameter<IAnalyzer>)Parameters["Analyzer"]).Value; }
      set { ((ValueParameter<IAnalyzer>)Parameters["Analyzer"]).Value = value; }
    }

    public IConstrainedValueParameter<ISolutionModelTrainer<TPopulationContext>> SolutionModelTrainerParameter {
      get { return (IConstrainedValueParameter<ISolutionModelTrainer<TPopulationContext>>)Parameters["SolutionModelTrainer"]; }
    }

    public IConstrainedValueParameter<ILocalSearch<TSolutionContext>> LocalSearchParameter {
      get { return (IConstrainedValueParameter<ILocalSearch<TSolutionContext>>)Parameters["LocalSearch"]; }
    }

    [Storable]
    private TPopulationContext context;
    public TPopulationContext Context {
      get { return context; }
      protected set {
        if (context == value) return;
        context = value;
        OnPropertyChanged("State");
      }
    }

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [StorableConstructor]
    protected MemPRAlgorithm(bool deserializing) : base(deserializing) { }
    protected MemPRAlgorithm(MemPRAlgorithm<TProblem, TSolution, TPopulationContext, TSolutionContext> original, Cloner cloner) : base(original, cloner) {
      context = cloner.Clone(original.context);
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
      RegisterEventHandlers();
    }
    protected MemPRAlgorithm() {
      Parameters.Add(new ValueParameter<IAnalyzer>("Analyzer", "The analyzer to apply to the population.", new MultiAnalyzer()));
      Parameters.Add(new FixedValueParameter<IntValue>("MaximumPopulationSize", "The maximum size of the population that is evolved.", new IntValue(20)));
      Parameters.Add(new OptionalValueParameter<IntValue>("MaximumEvaluations", "The maximum number of solution evaluations."));
      Parameters.Add(new OptionalValueParameter<TimeSpanValue>("MaximumExecutionTime", "The maximum runtime.", new TimeSpanValue(TimeSpan.FromMinutes(1))));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("TargetQuality", "The target quality at which the algorithm terminates."));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "Whether each run of the algorithm should be conducted with a new random seed.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random number seed that is used in case SetSeedRandomly is false.", new IntValue(0)));
      Parameters.Add(new ConstrainedValueParameter<ISolutionModelTrainer<TPopulationContext>>("SolutionModelTrainer", "The object that creates a solution model that can be sampled."));
      Parameters.Add(new ConstrainedValueParameter<ILocalSearch<TSolutionContext>>("LocalSearch", "The local search operator to use."));

      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      MaximumPopulationSizeParameter.Value.ValueChanged += MaximumPopulationSizeOnChanged;
    }

    private void MaximumPopulationSizeOnChanged(object sender, EventArgs eventArgs) {
      if (ExecutionState == ExecutionState.Started || ExecutionState == ExecutionState.Paused)
        throw new InvalidOperationException("Cannot change maximum population size before algorithm finishes.");
      Prepare();
    }

    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      qualityAnalyzer.MaximizationParameter.ActualName = Problem.MaximizationParameter.Name;
      qualityAnalyzer.MaximizationParameter.Hidden = true;
      qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      qualityAnalyzer.QualityParameter.Depth = 1;
      qualityAnalyzer.QualityParameter.Hidden = true;
      qualityAnalyzer.BestKnownQualityParameter.ActualName = Problem.BestKnownQualityParameter.Name;
      qualityAnalyzer.BestKnownQualityParameter.Hidden = true;

      var multiAnalyzer = Analyzer as MultiAnalyzer;
      if (multiAnalyzer != null) {
        multiAnalyzer.Operators.Clear();
        if (Problem != null) {
          foreach (var analyzer in Problem.Operators.OfType<IAnalyzer>()) {
            foreach (var param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
              param.Depth = 1;
            multiAnalyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
          }
        }
        multiAnalyzer.Operators.Add(qualityAnalyzer, qualityAnalyzer.EnabledByDefault);
      }
    }

    public override void Prepare() {
      base.Prepare();
      Results.Clear();
      Context = null;
    }

    protected virtual TPopulationContext CreateContext() {
      return new TPopulationContext();
    }

    protected sealed override void Run(CancellationToken token) {
      if (Context == null) {
        Context = CreateContext();
        if (SetSeedRandomly) Seed = new System.Random().Next();
        Context.Random.Reset(Seed);
        Context.Scope.Variables.Add(new Variable("Results", Results));
        Context.Problem = Problem;
      }

      IExecutionContext context = null;
      foreach (var item in Problem.ExecutionContextItems)
        context = new Core.ExecutionContext(context, item, Context.Scope);
      context = new Core.ExecutionContext(context, this, Context.Scope);
      Context.Parent = context;

      if (!Context.Initialized) {
        // We initialize the population with two local optima
        while (Context.PopulationCount < 2) {
          var child = Create(token);
          Context.HcSteps += HillClimb(child, token);
          Context.AddToPopulation(child);
          Analyze(token);
          token.ThrowIfCancellationRequested();
          if (Terminate()) return;
        }
        Context.HcSteps /= 2;
        Context.Initialized = true;
      }

      while (!Terminate()) {
        Iterate(token);
        Analyze(token);
        token.ThrowIfCancellationRequested();
      }
    }

    private void Iterate(CancellationToken token) {
      var replaced = false;

      var i1 = Context.Random.Next(Context.PopulationCount);
      var i2 = Context.Random.Next(Context.PopulationCount);
      while (i1 == i2) i2 = Context.Random.Next(Context.PopulationCount);

      var p1 = Context.AtPopulation(i1);
      var p2 = Context.AtPopulation(i2);

      var parentDist = Dist(p1, p2);

      ISingleObjectiveSolutionScope<TSolution> offspring = null;
      int replPos = -1;

      if (Context.Random.NextDouble() > parentDist) {
        offspring = BreedAndImprove(p1, p2, token);
        replPos = Replace(offspring, token);
        if (replPos >= 0) {
          replaced = true;
          Context.ByBreeding++;
        }
      }

      if (Context.Random.NextDouble() < parentDist) {
        offspring = RelinkAndImprove(p1, p2, token);
        replPos = Replace(offspring, token);
        if (replPos >= 0) {
          replaced = true;
          Context.ByRelinking++;
        }
      }

      offspring = PerformSampling(token);
      replPos = Replace(offspring, token);
      if (replPos >= 0) {
        replaced = true;
        Context.BySampling++;
      }

      if (!replaced) {
        offspring = Create(token);
        if (HillclimbingSuited(offspring)) {
          HillClimb(offspring, token);
          replPos = Replace(offspring, token);
          if (replPos >= 0) {
            Context.ByHillclimbing++;
            replaced = true;
          }
        } else {
          offspring = (ISingleObjectiveSolutionScope<TSolution>)Context.AtPopulation(Context.Random.Next(Context.PopulationCount)).Clone();
          Mutate(offspring, token);
          PerformTabuWalk(offspring, Context.HcSteps, token);
          replPos = Replace(offspring, token);
          if (replPos >= 0) {
            Context.ByTabuwalking++;
            replaced = true;
          }
        }
      }
      Context.Iterations++;
    }

    protected void Analyze(CancellationToken token) {
      IResult res;
      if (!Results.TryGetValue("EvaluatedSolutions", out res))
        Results.Add(new Result("EvaluatedSolutions", new IntValue(Context.EvaluatedSolutions)));
      else ((IntValue)res.Value).Value = Context.EvaluatedSolutions;
      if (!Results.TryGetValue("Iterations", out res))
        Results.Add(new Result("Iterations", new IntValue(Context.Iterations)));
      else ((IntValue)res.Value).Value = Context.Iterations;
      if (!Results.TryGetValue("HcSteps", out res))
        Results.Add(new Result("HcSteps", new IntValue(Context.HcSteps)));
      else ((IntValue)res.Value).Value = Context.HcSteps;
      if (!Results.TryGetValue("ByBreeding", out res))
        Results.Add(new Result("ByBreeding", new IntValue(Context.ByBreeding)));
      else ((IntValue)res.Value).Value = Context.ByBreeding;
      if (!Results.TryGetValue("ByRelinking", out res))
        Results.Add(new Result("ByRelinking", new IntValue(Context.ByRelinking)));
      else ((IntValue)res.Value).Value = Context.ByRelinking;
      if (!Results.TryGetValue("BySampling", out res))
        Results.Add(new Result("BySampling", new IntValue(Context.BySampling)));
      else ((IntValue)res.Value).Value = Context.BySampling;
      if (!Results.TryGetValue("ByHillclimbing", out res))
        Results.Add(new Result("ByHillclimbing", new IntValue(Context.ByHillclimbing)));
      else ((IntValue)res.Value).Value = Context.ByHillclimbing;
      if (!Results.TryGetValue("ByTabuwalking", out res))
        Results.Add(new Result("ByTabuwalking", new IntValue(Context.ByTabuwalking)));
      else ((IntValue)res.Value).Value = Context.ByTabuwalking;

      var sp = new ScatterPlot("Parent1 vs Offspring", "");
      sp.Rows.Add(new ScatterPlotDataRow("corr", "", Context.BreedingStat.Select(x => new Point2D<double>(x.Item1, x.Item3))) { VisualProperties = { PointSize = 6 }});
      if (!Results.TryGetValue("BreedingStat1", out res)) {
        Results.Add(new Result("BreedingStat1", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Parent2 vs Offspring", "");
      sp.Rows.Add(new ScatterPlotDataRow("corr", "", Context.BreedingStat.Select(x => new Point2D<double>(x.Item2, x.Item3))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("BreedingStat2", out res)) {
        Results.Add(new Result("BreedingStat2", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Solution vs Local Optimum", "");
      sp.Rows.Add(new ScatterPlotDataRow("corr", "", Context.HillclimbingStat.Select(x => new Point2D<double>(x.Item1, x.Item2))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("HillclimbingStat", out res)) {
        Results.Add(new Result("HillclimbingStat", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Solution vs Tabu Walk", "");
      sp.Rows.Add(new ScatterPlotDataRow("corr", "", Context.TabuwalkingStat.Select(x => new Point2D<double>(x.Item1, x.Item2))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("TabuwalkingStat", out res)) {
        Results.Add(new Result("TabuwalkingStat", sp));
      } else res.Value = sp;

      RunOperator(Analyzer, Context.Scope, token);
    }

    protected int Replace(ISingleObjectiveSolutionScope<TSolution> child, CancellationToken token) {
      if (double.IsNaN(child.Fitness)) {
        Evaluate(child, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      if (IsBetter(child.Fitness, Context.BestQuality)) {
        Context.BestQuality = child.Fitness;
        Context.BestSolution = (TSolution)child.Solution.Clone();
      }

      var popSize = MaximumPopulationSize;
      if (Context.Population.All(p => !Eq(p, child))) {

        if (Context.PopulationCount < popSize) {
          Context.AddToPopulation(child);
          return Context.PopulationCount - 1;
        }
        
        // The set of replacement candidates consists of all solutions at least as good as the new one
        var candidates = Context.Population.Select((p, i) => new { Index = i, Individual = p })
                                         .Where(x => x.Individual.Fitness == child.Fitness
                                           || IsBetter(child, x.Individual)).ToList();
        if (candidates.Count == 0) return -1;

        var repCand = -1;
        var avgChildDist = 0.0;
        var minChildDist = double.MaxValue;
        var plateau = new List<int>();
        var worstPlateau = -1;
        var minAvgPlateauDist = double.MaxValue;
        var minPlateauDist = double.MaxValue;
        // If there are equally good solutions it is first tried to replace one of those
        // The criteria for replacement is that the new solution has better average distance
        // to all other solutions at this "plateau"
        foreach (var c in candidates.Where(x => x.Individual.Fitness == child.Fitness)) {
          var dist = Dist(c.Individual, child);
          avgChildDist += dist;
          if (dist < minChildDist) minChildDist = dist;
          plateau.Add(c.Index);
        }
        if (plateau.Count > 2) {
          avgChildDist /= plateau.Count;
          foreach (var p in plateau) {
            var avgDist = 0.0;
            var minDist = double.MaxValue;
            foreach (var q in plateau) {
              if (p == q) continue;
              var dist = Dist(Context.AtPopulation(p), Context.AtPopulation(q));
              avgDist += dist;
              if (dist < minDist) minDist = dist;
            }

            var d = Dist(Context.AtPopulation(p), child);
            avgDist += d;
            avgDist /= plateau.Count;
            if (d < minDist) minDist = d;

            if (minDist < minPlateauDist || (minDist == minPlateauDist && avgDist < avgChildDist)) {
              minAvgPlateauDist = avgDist;
              minPlateauDist = minDist;
              worstPlateau = p;
            }
          }
          if (minPlateauDist < minChildDist || (minPlateauDist == minChildDist && minAvgPlateauDist < avgChildDist))
            repCand = worstPlateau;
        }

        if (repCand < 0) {
          // If no solution at the same plateau were identified for replacement
          // a worse solution with smallest distance is chosen
          var minDist = double.MaxValue;
          foreach (var c in candidates.Where(x => IsBetter(child, x.Individual))) {
            var d = Dist(c.Individual, child);
            if (d < minDist) {
              minDist = d;
              repCand = c.Index;
            }
          }
        }

        // If no replacement was identified, this can only mean that there are
        // no worse solutions and those on the same plateau are all better
        // stretched out than the new one
        if (repCand < 0) return -1;
        
        Context.ReplaceAtPopulation(repCand, child);
        return repCand;
      }
      return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool IsBetter(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b) {
      return IsBetter(a.Fitness, b.Fitness);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool IsBetter(double a, double b) {
      return double.IsNaN(b) && !double.IsNaN(a)
        || Problem.Maximization && a > b
        || !Problem.Maximization && a < b;
    }
    
    protected abstract bool Eq(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b);
    protected abstract double Dist(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b);
    protected abstract ISingleObjectiveSolutionScope<TSolution> ToScope(TSolution code, double fitness = double.NaN);
    protected abstract ISolutionSubspace<TSolution> CalculateSubspace(IEnumerable<TSolution> solutions, bool inverse = false);
    protected virtual void Evaluate(ISingleObjectiveSolutionScope<TSolution> scope, CancellationToken token) {
      var prob = Problem as ISingleObjectiveProblemDefinition;
      if (prob != null) {
        var ind = new SingleEncodingIndividual(prob.Encoding, scope);
        scope.Fitness = prob.Evaluate(ind, Context.Random);
      } else RunOperator(Problem.Evaluator, scope, token);
    }

    #region Create
    protected virtual ISingleObjectiveSolutionScope<TSolution> Create(CancellationToken token) {
      var child = ToScope(null);
      RunOperator(Problem.SolutionCreator, child, token);
      return child;
    }
    #endregion

    #region Improve
    protected virtual int HillClimb(ISingleObjectiveSolutionScope<TSolution> scope, CancellationToken token, ISolutionSubspace<TSolution> subspace = null) {
      if (double.IsNaN(scope.Fitness)) {
        Evaluate(scope, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      var before = scope.Fitness;
      var lscontext = Context.CreateSingleSolutionContext(scope);
      LocalSearchParameter.Value.Optimize(lscontext);
      var after = scope.Fitness;
      Context.HillclimbingStat.Add(Tuple.Create(before, after));
      Context.IncrementEvaluatedSolutions(lscontext.EvaluatedSolutions);
      return lscontext.EvaluatedSolutions;
    }

    protected virtual void PerformTabuWalk(ISingleObjectiveSolutionScope<TSolution> scope, int steps, CancellationToken token, ISolutionSubspace<TSolution> subspace = null) {
      if (double.IsNaN(scope.Fitness)) {
        Evaluate(scope, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      var before = scope.Fitness;
      var newScope = (ISingleObjectiveSolutionScope<TSolution>)scope.Clone();
      var newSteps = TabuWalk(newScope, steps, token, subspace);
      Context.TabuwalkingStat.Add(Tuple.Create(before, newScope.Fitness));
      //Context.HcSteps = (int)Math.Ceiling(Context.HcSteps * (1.0 + Context.TabuwalkingStat.Count) / (2.0 + Context.TabuwalkingStat.Count) + newSteps / (2.0 + Context.TabuwalkingStat.Count));
      if (IsBetter(newScope, scope) || (newScope.Fitness == scope.Fitness && Dist(newScope, scope) > 0))
        scope.Adopt(newScope);
    }
    protected abstract int TabuWalk(ISingleObjectiveSolutionScope<TSolution> scope, int maxEvals, CancellationToken token, ISolutionSubspace<TSolution> subspace = null);
    protected virtual void TabuClimb(ISingleObjectiveSolutionScope<TSolution> scope, int steps, CancellationToken token, ISolutionSubspace<TSolution> subspace = null) {
      if (double.IsNaN(scope.Fitness)) {
        Evaluate(scope, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      var before = scope.Fitness;
      var newScope = (ISingleObjectiveSolutionScope<TSolution>)scope.Clone();
      var newSteps = TabuWalk(newScope, steps, token, subspace);
      Context.TabuwalkingStat.Add(Tuple.Create(before, newScope.Fitness));
      //Context.HcSteps = (int)Math.Ceiling(Context.HcSteps * (1.0 + Context.TabuwalkingStat.Count) / (2.0 + Context.TabuwalkingStat.Count) + newSteps / (2.0 + Context.TabuwalkingStat.Count));
      if (IsBetter(newScope, scope) || (newScope.Fitness == scope.Fitness && Dist(newScope, scope) > 0))
        scope.Adopt(newScope);
    }
    #endregion
    
    #region Breed
    protected virtual ISingleObjectiveSolutionScope<TSolution> PerformBreeding(CancellationToken token) {
      if (Context.PopulationCount < 2) throw new InvalidOperationException("Cannot breed from population with less than 2 individuals.");
      var i1 = Context.Random.Next(Context.PopulationCount);
      var i2 = Context.Random.Next(Context.PopulationCount);
      while (i1 == i2) i2 = Context.Random.Next(Context.PopulationCount);

      var p1 = Context.AtPopulation(i1);
      var p2 = Context.AtPopulation(i2);

      if (double.IsNaN(p1.Fitness)) {
        Evaluate(p1, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      if (double.IsNaN(p2.Fitness)) {
        Evaluate(p2, token);
        Context.IncrementEvaluatedSolutions(1);
      }

      return BreedAndImprove(p1, p2, token);
    }

    protected virtual ISingleObjectiveSolutionScope<TSolution> BreedAndImprove(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2, CancellationToken token) {
      var offspring = Cross(p1, p2, token);
      var subspace = CalculateSubspace(new[] { p1.Solution, p2.Solution });
      if (Context.Random.NextDouble() < MutationProbabilityMagicConst) {
        Mutate(offspring, token, subspace); // mutate the solutions, especially to widen the sub-space
      }
      if (double.IsNaN(offspring.Fitness)) {
        Evaluate(offspring, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      Context.BreedingStat.Add(Tuple.Create(p1.Fitness, p2.Fitness, offspring.Fitness));
      if ((IsBetter(offspring, p1) && IsBetter(offspring, p2))
        || Context.Population.Any(p => IsBetter(offspring, p))) return offspring;

      if (HillclimbingSuited(offspring))
        HillClimb(offspring, token, subspace); // perform hillclimb in the solution sub-space
      return offspring;
    }

    protected abstract ISingleObjectiveSolutionScope<TSolution> Cross(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2, CancellationToken token);
    protected abstract void Mutate(ISingleObjectiveSolutionScope<TSolution> offspring, CancellationToken token, ISolutionSubspace<TSolution> subspace = null);
    #endregion

    #region Relink
    protected virtual ISingleObjectiveSolutionScope<TSolution> PerformRelinking(CancellationToken token) {
      if (Context.PopulationCount < 2) throw new InvalidOperationException("Cannot breed from population with less than 2 individuals.");
      var i1 = Context.Random.Next(Context.PopulationCount);
      var i2 = Context.Random.Next(Context.PopulationCount);
      while (i1 == i2) i2 = Context.Random.Next(Context.PopulationCount);

      var p1 = Context.AtPopulation(i1);
      var p2 = Context.AtPopulation(i2);

      return RelinkAndImprove(p1, p2, token);
    }

    protected virtual ISingleObjectiveSolutionScope<TSolution> RelinkAndImprove(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, CancellationToken token) {
      var child = Relink(a, b, token);
      if (IsBetter(child, a) && IsBetter(child, b)) return child;

      var dist1 = Dist(child, a);
      var dist2 = Dist(child, b);
      if (dist1 > 0 && dist2 > 0) {
        var subspace = CalculateSubspace(new[] { a.Solution, b.Solution }, inverse: true);
        if (HillclimbingSuited(child)) {
          HillClimb(child, token, subspace); // perform hillclimb in solution sub-space
        }
      }
      return child;
    }

    protected abstract ISingleObjectiveSolutionScope<TSolution> Relink(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, CancellationToken token);
    #endregion

    #region Sample
    protected virtual ISingleObjectiveSolutionScope<TSolution> PerformSampling(CancellationToken token) {
      SolutionModelTrainerParameter.Value.TrainModel(Context);
      var sample = ToScope(Context.Model.Sample());
      Evaluate(sample, token);
      Context.IncrementEvaluatedSolutions(1);
      if (Context.Population.Any(p => IsBetter(sample, p) || sample.Fitness == p.Fitness)) return sample;

      if (HillclimbingSuited(sample)) {
        var subspace = CalculateSubspace(Context.Population.Select(x => x.Solution));
        HillClimb(sample, token, subspace);
      }
      return sample;
    }
    #endregion

    protected bool HillclimbingSuited(ISingleObjectiveSolutionScope<TSolution> scope) {
      return Context.Random.NextDouble() < ProbabilityAccept(scope, Context.HillclimbingStat);
    }
    protected bool HillclimbingSuited(double startingFitness) {
      return Context.Random.NextDouble() < ProbabilityAccept(startingFitness, Context.HillclimbingStat);
    }
    protected bool TabuwalkingSuited(ISingleObjectiveSolutionScope<TSolution> scope) {
      return Context.Random.NextDouble() < ProbabilityAccept(scope, Context.TabuwalkingStat);
    }
    protected bool TabuwalkingSuited(double startingFitness) {
      return Context.Random.NextDouble() < ProbabilityAccept(startingFitness, Context.TabuwalkingStat);
    }

    protected double ProbabilityAccept(ISingleObjectiveSolutionScope<TSolution> scope, IList<Tuple<double, double>> data) {
      if (double.IsNaN(scope.Fitness)) {
        Evaluate(scope, CancellationToken.None);
        Context.IncrementEvaluatedSolutions(1);
      }
      return ProbabilityAccept(scope.Fitness, data);
    }
    protected double ProbabilityAccept(double startingFitness, IList<Tuple<double, double>> data) {
      if (data.Count < 10) return 1.0;
      int[] clusterValues;
      var centroids = CkMeans1D.Cluster(data.Select(x => x.Item1).ToArray(), 2, out clusterValues);
      var cluster = Math.Abs(startingFitness - centroids.First().Key) < Math.Abs(startingFitness - centroids.Last().Key) ? centroids.First().Value : centroids.Last().Value;

      var samples = 0;
      double meanStart = 0, meanStartOld = 0, meanEnd = 0, meanEndOld = 0;
      double varStart = 0, varStartOld = 0, varEnd = 0, varEndOld = 0;
      for (var i = 0; i < data.Count; i++) {
        if (clusterValues[i] != cluster) continue;

        samples++;
        var x = data[i].Item1;
        var y = data[i].Item2;

        if (samples == 1) {
          meanStartOld = x;
          meanEndOld = y;
        } else {
          meanStart = meanStartOld + (x - meanStartOld) / samples;
          meanEnd = meanEndOld + (x - meanEndOld) / samples;
          varStart = varStartOld + (x - meanStartOld) * (x - meanStart) / (samples - 1);
          varEnd = varEndOld + (x - meanEndOld) * (x - meanEnd) / (samples - 1);

          meanStartOld = meanStart;
          meanEndOld = meanEnd;
          varStartOld = varStart;
          varEndOld = varEnd;
        }
      }
      if (samples < 5) return 1.0;
      var cov = data.Select((v, i) => new { Index = i, Value = v }).Where(x => clusterValues[x.Index] == cluster).Select(x => x.Value).Sum(x => (x.Item1 - meanStart) * (x.Item2 - meanEnd)) / data.Count;

      var biasedMean = meanEnd + cov / varStart * (startingFitness - meanStart);
      var biasedStdev = Math.Sqrt(varEnd - (cov * cov) / varStart);

      if (Problem.Maximization) {
        var goal = Context.Population.Min(x => x.Fitness);
        var z = (goal - biasedMean) / biasedStdev;
        return 1.0 - Phi(z); // P(X >= z)
      } else {
        var goal = Context.Population.Max(x => x.Fitness);
        var z = (goal - biasedMean) / biasedStdev;
        return Phi(z); // P(X <= z)
      }
    }

    protected virtual bool Terminate() {
      return MaximumEvaluations.HasValue && Context.EvaluatedSolutions >= MaximumEvaluations.Value
        || MaximumExecutionTime.HasValue && ExecutionTime >= MaximumExecutionTime.Value
        || TargetQuality.HasValue && (Problem.Maximization && Context.BestQuality >= TargetQuality.Value
                                  || !Problem.Maximization && Context.BestQuality <= TargetQuality.Value);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string property) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(property));
    }

    #region Engine Helper
    protected void RunOperator(IOperator op, IScope scope, CancellationToken cancellationToken) {
      var stack = new Stack<IOperation>();
      stack.Push(Context.CreateChildOperation(op, scope));

      while (stack.Count > 0) {
        cancellationToken.ThrowIfCancellationRequested();

        var next = stack.Pop();
        if (next is OperationCollection) {
          var coll = (OperationCollection)next;
          for (int i = coll.Count - 1; i >= 0; i--)
            if (coll[i] != null) stack.Push(coll[i]);
        } else if (next is IAtomicOperation) {
          var operation = (IAtomicOperation)next;
          try {
            next = operation.Operator.Execute((IExecutionContext)operation, cancellationToken);
          } catch (Exception ex) {
            stack.Push(operation);
            if (ex is OperationCanceledException) throw ex;
            else throw new OperatorExecutionException(operation.Operator, ex);
          }
          if (next != null) stack.Push(next);
        }
      }
    }
    #endregion

    #region Math Helper
    // normal distribution CDF (left of x) for N(0;1) standard normal distribution
    // from http://www.johndcook.com/blog/csharp_phi/
    // license: "This code is in the public domain. Do whatever you want with it, no strings attached."
    // added: 2016-11-19 21:46 CET
    protected static double Phi(double x) {
      // constants
      double a1 = 0.254829592;
      double a2 = -0.284496736;
      double a3 = 1.421413741;
      double a4 = -1.453152027;
      double a5 = 1.061405429;
      double p = 0.3275911;

      // Save the sign of x
      int sign = 1;
      if (x < 0)
        sign = -1;
      x = Math.Abs(x) / Math.Sqrt(2.0);

      // A&S formula 7.1.26
      double t = 1.0 / (1.0 + p * x);
      double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

      return 0.5 * (1.0 + sign * y);
    }
    #endregion
  }
}
