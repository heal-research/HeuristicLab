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
using System.Threading;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR {
  [Item("MemPR Algorithm", "Base class for MemPR algorithms")]
  [StorableClass]
  public abstract class MemPRAlgorithm<TProblem, TSolution, TPopulationContext, TSolutionContext> : BasicAlgorithm, INotifyPropertyChanged
      where TProblem : class, IItem, ISingleObjectiveHeuristicOptimizationProblem
      where TSolution : class, IItem
      where TPopulationContext : MemPRPopulationContext<TProblem, TSolution, TPopulationContext, TSolutionContext>, new()
      where TSolutionContext : MemPRSolutionContext<TProblem, TSolution, TPopulationContext, TSolutionContext> {

    public override Type ProblemType {
      get { return typeof(TProblem); }
    }

    public new TProblem Problem {
      get { return (TProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public override bool SupportsPause {
      get { return true; }
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

      if (MaximumExecutionTime.HasValue)
        CancellationTokenSource.CancelAfter(MaximumExecutionTime.Value);

      IExecutionContext context = null;
      foreach (var item in Problem.ExecutionContextItems)
        context = new Core.ExecutionContext(context, item, Context.Scope);
      context = new Core.ExecutionContext(context, this, Context.Scope);
      Context.Parent = context;

      if (!Context.Initialized) {
        // We initialize the population with two local optima
        while (Context.PopulationCount < 2) {
          var child = Create(token);
          Context.LocalSearchEvaluations += HillClimb(child, token);
          Context.LocalOptimaLevel += child.Fitness;
          Context.AddToPopulation(child);
          Context.BestQuality = child.Fitness;
          Analyze(token);
          token.ThrowIfCancellationRequested();
          if (Terminate()) return;
        }
        Context.LocalSearchEvaluations /= 2;
        Context.LocalOptimaLevel /= 2;
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
      ISingleObjectiveSolutionScope<TSolution> offspring = null;
      
      offspring = Breed(token);
      if (offspring != null) {
        var replNew = Replace(offspring, token);
        if (replNew) {
          replaced = true;
          Context.ByBreeding++;
        }
      }

      offspring = Relink(token);
      if (offspring != null) {
        if (Replace(offspring, token)) {
          replaced = true;
          Context.ByRelinking++;
        }
      }

      offspring = Delink(token);
      if (offspring != null) {
        if (Replace(offspring, token)) {
          replaced = true;
          Context.ByDelinking++;
        }
      }

      offspring = Sample(token);
      if (offspring != null) {
        if (Replace(offspring, token)) {
          replaced = true;
          Context.BySampling++;
        }
      }

      if (!replaced && offspring != null) {
        if (Context.HillclimbingSuited(offspring)) {
          HillClimb(offspring, token, CalculateSubspace(Context.Population.Select(x => x.Solution)));
          if (Replace(offspring, token)) {
            Context.ByHillclimbing++;
            replaced = true;
          }
        }
      }

      if (!replaced) {
        offspring = (ISingleObjectiveSolutionScope<TSolution>)Context.Population.SampleRandom(Context.Random).Clone();
        var before = offspring.Fitness;
        AdaptiveWalk(offspring, Context.LocalSearchEvaluations * 2, token);
        Context.AdaptivewalkingStat.Add(Tuple.Create(before, offspring.Fitness));
        if (Context.AdaptivewalkingStat.Count % 10 == 0) Context.RelearnAdaptiveWalkPerformanceModel();
        if (Replace(offspring, token)) {
          Context.ByAdaptivewalking++;
          replaced = true;
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
      if (!Results.TryGetValue("LocalSearch Evaluations", out res))
        Results.Add(new Result("LocalSearch Evaluations", new IntValue(Context.LocalSearchEvaluations)));
      else ((IntValue)res.Value).Value = Context.LocalSearchEvaluations;
      if (!Results.TryGetValue("ByBreeding", out res))
        Results.Add(new Result("ByBreeding", new IntValue(Context.ByBreeding)));
      else ((IntValue)res.Value).Value = Context.ByBreeding;
      if (!Results.TryGetValue("ByRelinking", out res))
        Results.Add(new Result("ByRelinking", new IntValue(Context.ByRelinking)));
      else ((IntValue)res.Value).Value = Context.ByRelinking;
      if (!Results.TryGetValue("ByDelinking", out res))
        Results.Add(new Result("ByDelinking", new IntValue(Context.ByDelinking)));
      else ((IntValue)res.Value).Value = Context.ByDelinking;
      if (!Results.TryGetValue("BySampling", out res))
        Results.Add(new Result("BySampling", new IntValue(Context.BySampling)));
      else ((IntValue)res.Value).Value = Context.BySampling;
      if (!Results.TryGetValue("ByHillclimbing", out res))
        Results.Add(new Result("ByHillclimbing", new IntValue(Context.ByHillclimbing)));
      else ((IntValue)res.Value).Value = Context.ByHillclimbing;
      if (!Results.TryGetValue("ByAdaptivewalking", out res))
        Results.Add(new Result("ByAdaptivewalking", new IntValue(Context.ByAdaptivewalking)));
      else ((IntValue)res.Value).Value = Context.ByAdaptivewalking;

      var sp = new ScatterPlot("Breeding Correlation", "");
      sp.Rows.Add(new ScatterPlotDataRow("Parent1 vs Offspring", "", Context.BreedingStat.Select(x => new Point2D<double>(x.Item1, x.Item3))) { VisualProperties = { PointSize = 6 }});
      sp.Rows.Add(new ScatterPlotDataRow("Parent2 vs Offspring", "", Context.BreedingStat.Select(x => new Point2D<double>(x.Item2, x.Item3))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("BreedingStat", out res)) {
        Results.Add(new Result("BreedingStat", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Relinking Correlation", "");
      sp.Rows.Add(new ScatterPlotDataRow("A vs Relink", "", Context.RelinkingStat.Select(x => new Point2D<double>(x.Item1, x.Item3))) { VisualProperties = { PointSize = 6 } });
      sp.Rows.Add(new ScatterPlotDataRow("B vs Relink", "", Context.RelinkingStat.Select(x => new Point2D<double>(x.Item2, x.Item3))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("RelinkingStat", out res)) {
        Results.Add(new Result("RelinkingStat", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Delinking Correlation", "");
      sp.Rows.Add(new ScatterPlotDataRow("A vs Delink", "", Context.DelinkingStat.Select(x => new Point2D<double>(x.Item1, x.Item3))) { VisualProperties = { PointSize = 6 } });
      sp.Rows.Add(new ScatterPlotDataRow("B vs Delink", "", Context.DelinkingStat.Select(x => new Point2D<double>(x.Item2, x.Item3))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("DelinkingStat", out res)) {
        Results.Add(new Result("DelinkingStat", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Sampling Correlation", "");
      sp.Rows.Add(new ScatterPlotDataRow("AvgFitness vs Sample", "", Context.SamplingStat.Select(x => new Point2D<double>(x.Item1, x.Item2))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("SampleStat", out res)) {
        Results.Add(new Result("SampleStat", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Hillclimbing Correlation", "");
      sp.Rows.Add(new ScatterPlotDataRow("Start vs End", "", Context.HillclimbingStat.Select(x => new Point2D<double>(x.Item1, x.Item2))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("HillclimbingStat", out res)) {
        Results.Add(new Result("HillclimbingStat", sp));
      } else res.Value = sp;

      sp = new ScatterPlot("Adaptivewalking Correlation", "");
      sp.Rows.Add(new ScatterPlotDataRow("Start vs Best", "", Context.AdaptivewalkingStat.Select(x => new Point2D<double>(x.Item1, x.Item2))) { VisualProperties = { PointSize = 6 } });
      if (!Results.TryGetValue("AdaptivewalkingStat", out res)) {
        Results.Add(new Result("AdaptivewalkingStat", sp));
      } else res.Value = sp;

      if (Context.BreedingPerformanceModel != null) {
        var sol = Context.GetSolution(Context.BreedingPerformanceModel, Context.BreedingStat);
        if (!Results.TryGetValue("Breeding Performance", out res)) {
          Results.Add(new Result("Breeding Performance", sol));
        } else res.Value = sol;
      }
      if (Context.RelinkingPerformanceModel != null) {
        var sol = Context.GetSolution(Context.RelinkingPerformanceModel, Context.RelinkingStat);
        if (!Results.TryGetValue("Relinking Performance", out res)) {
          Results.Add(new Result("Relinking Performance", sol));
        } else res.Value = sol;
      }
      if (Context.DelinkingPerformanceModel != null) {
        var sol = Context.GetSolution(Context.DelinkingPerformanceModel, Context.DelinkingStat);
        if (!Results.TryGetValue("Delinking Performance", out res)) {
          Results.Add(new Result("Delinking Performance", sol));
        } else res.Value = sol;
      }
      if (Context.SamplingPerformanceModel != null) {
        var sol = Context.GetSolution(Context.SamplingPerformanceModel, Context.SamplingStat);
        if (!Results.TryGetValue("Sampling Performance", out res)) {
          Results.Add(new Result("Sampling Performance", sol));
        } else res.Value = sol;
      }
      if (Context.HillclimbingPerformanceModel != null) {
        var sol = Context.GetSolution(Context.HillclimbingPerformanceModel, Context.HillclimbingStat);
        if (!Results.TryGetValue("Hillclimbing Performance", out res)) {
          Results.Add(new Result("Hillclimbing Performance", sol));
        } else res.Value = sol;
      }
      if (Context.AdaptiveWalkPerformanceModel != null) {
        var sol = Context.GetSolution(Context.AdaptiveWalkPerformanceModel, Context.AdaptivewalkingStat);
        if (!Results.TryGetValue("Adaptivewalk Performance", out res)) {
          Results.Add(new Result("Adaptivewalk Performance", sol));
        } else res.Value = sol;
      }

      Context.RunOperator(Analyzer, Context.Scope, token);
    }

    protected bool Replace(ISingleObjectiveSolutionScope<TSolution> child, CancellationToken token) {
      if (double.IsNaN(child.Fitness)) {
        Context.Evaluate(child, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      if (Context.IsBetter(child.Fitness, Context.BestQuality)) {
        Context.BestQuality = child.Fitness;
        Context.BestSolution = (TSolution)child.Solution.Clone();
      }

      var popSize = MaximumPopulationSize;
      if (Context.Population.All(p => !Eq(p, child))) {

        if (Context.PopulationCount < popSize) {
          Context.AddToPopulation(child);
          return true;// Context.PopulationCount - 1;
        }
        
        // The set of replacement candidates consists of all solutions at least as good as the new one
        var candidates = Context.Population.Select((p, i) => new { Index = i, Individual = p })
                                         .Where(x => x.Individual.Fitness == child.Fitness
                                           || Context.IsBetter(child, x.Individual)).ToList();
        if (candidates.Count == 0) return false;// -1;

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
          foreach (var c in candidates.Where(x => Context.IsBetter(child, x.Individual))) {
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
        if (repCand < 0) return false;// -1;
        
        Context.ReplaceAtPopulation(repCand, child);
        return true;// repCand;
      }
      return false;// -1;
    }

    protected bool Eq(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b) {
      return Eq(a.Solution, b.Solution);
    }
    protected abstract bool Eq(TSolution a, TSolution b);
    protected abstract double Dist(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b);
    protected abstract ISolutionSubspace<TSolution> CalculateSubspace(IEnumerable<TSolution> solutions, bool inverse = false);

    #region Create
    protected virtual ISingleObjectiveSolutionScope<TSolution> Create(CancellationToken token) {
      var child = Context.ToScope(null);
      Context.RunOperator(Problem.SolutionCreator, child, token);
      return child;
    }
    #endregion

    #region Improve
    protected virtual int HillClimb(ISingleObjectiveSolutionScope<TSolution> scope, CancellationToken token, ISolutionSubspace<TSolution> subspace = null) {
      if (double.IsNaN(scope.Fitness)) {
        Context.Evaluate(scope, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      var before = scope.Fitness;
      var lscontext = Context.CreateSingleSolutionContext(scope);
      LocalSearchParameter.Value.Optimize(lscontext);
      var after = scope.Fitness;
      Context.HillclimbingStat.Add(Tuple.Create(before, after));
      if (Context.HillclimbingStat.Count % 10 == 0) Context.RelearnHillclimbingPerformanceModel();
      Context.IncrementEvaluatedSolutions(lscontext.EvaluatedSolutions);
      return lscontext.EvaluatedSolutions;
    }

    protected virtual void AdaptiveClimb(ISingleObjectiveSolutionScope<TSolution> scope, int maxEvals, CancellationToken token, ISolutionSubspace<TSolution> subspace = null) {
      if (double.IsNaN(scope.Fitness)) {
        Context.Evaluate(scope, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      var before = scope.Fitness;
      var newScope = (ISingleObjectiveSolutionScope<TSolution>)scope.Clone();
      AdaptiveWalk(newScope, maxEvals, token, subspace);
      Context.AdaptivewalkingStat.Add(Tuple.Create(before, newScope.Fitness));
      if (Context.AdaptivewalkingStat.Count % 10 == 0) Context.RelearnAdaptiveWalkPerformanceModel();
      if (Context.IsBetter(newScope, scope))
        scope.Adopt(newScope);
    }
    protected abstract void AdaptiveWalk(ISingleObjectiveSolutionScope<TSolution> scope, int maxEvals, CancellationToken token, ISolutionSubspace<TSolution> subspace = null);
    
    #endregion

    #region Breed
    protected virtual ISingleObjectiveSolutionScope<TSolution> Breed(CancellationToken token) {
      var i1 = Context.Random.Next(Context.PopulationCount);
      var i2 = Context.Random.Next(Context.PopulationCount);
      while (i1 == i2) i2 = Context.Random.Next(Context.PopulationCount);

      var p1 = Context.AtPopulation(i1);
      var p2 = Context.AtPopulation(i2);

      if (double.IsNaN(p1.Fitness)) {
        Context.Evaluate(p1, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      if (double.IsNaN(p2.Fitness)) {
        Context.Evaluate(p2, token);
        Context.IncrementEvaluatedSolutions(1);
      }

      if (Context.BreedingSuited(p1, p2)) {
        var offspring = Breed(p1, p2, token);

        if (double.IsNaN(offspring.Fitness)) {
          Context.Evaluate(offspring, token);
          Context.IncrementEvaluatedSolutions(1);
        }

        // new best solutions are improved using hill climbing in full solution space
        if (Context.Population.All(p => Context.IsBetter(offspring, p)))
          HillClimb(offspring, token);
        else if (!Eq(offspring, p1) && !Eq(offspring, p2) && Context.HillclimbingSuited(offspring.Fitness))
          HillClimb(offspring, token, CalculateSubspace(new[] { p1.Solution, p2.Solution }, inverse: false));

        Context.AddBreedingResult(p1, p2, offspring);
        if (Context.BreedingStat.Count % 10 == 0) Context.RelearnBreedingPerformanceModel();
        return offspring;
      }
      return null;
    }

    protected abstract ISingleObjectiveSolutionScope<TSolution> Breed(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2, CancellationToken token);
    #endregion

    #region Relink/Delink
    protected virtual ISingleObjectiveSolutionScope<TSolution> Relink(CancellationToken token) {
      var i1 = Context.Random.Next(Context.PopulationCount);
      var i2 = Context.Random.Next(Context.PopulationCount);
      while (i1 == i2) i2 = Context.Random.Next(Context.PopulationCount);

      var p1 = Context.AtPopulation(i1);
      var p2 = Context.AtPopulation(i2);

      if (!Context.RelinkSuited(p1, p2)) return null;

      var link = PerformRelinking(p1, p2, token, delink: false);
      if (double.IsNaN(link.Fitness)) {
        Context.Evaluate(link, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      // new best solutions are improved using hill climbing in full solution space
      if (Context.Population.All(p => Context.IsBetter(link, p)))
        HillClimb(link, token);
      else if (!Eq(link, p1) && !Eq(link, p2) && Context.HillclimbingSuited(link.Fitness))
        HillClimb(link, token, CalculateSubspace(new[] { p1.Solution, p2.Solution }, inverse: true));

      return link;
    }

    protected virtual ISingleObjectiveSolutionScope<TSolution> Delink(CancellationToken token) {
      var i1 = Context.Random.Next(Context.PopulationCount);
      var i2 = Context.Random.Next(Context.PopulationCount);
      while (i1 == i2) i2 = Context.Random.Next(Context.PopulationCount);

      var p1 = Context.AtPopulation(i1);
      var p2 = Context.AtPopulation(i2);
      
      if (!Context.DelinkSuited(p1, p2)) return null;

      var link = PerformRelinking(p1, p2, token, delink: true);
      if (double.IsNaN(link.Fitness)) {
        Context.Evaluate(link, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      // new best solutions are improved using hill climbing in full solution space
      if (Context.Population.All(p => Context.IsBetter(link, p)))
        HillClimb(link, token);
      // intentionally not making hill climbing after delinking in sub-space
      return link;
    }

    protected virtual ISingleObjectiveSolutionScope<TSolution> PerformRelinking(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, CancellationToken token, bool delink = false) {
      var relink = Link(a, b, token, delink);

      if (double.IsNaN(relink.Fitness)) {
        Context.Evaluate(relink, token);
        Context.IncrementEvaluatedSolutions(1);
      }

      // new best solutions are improved using hill climbing
      if (Context.Population.All(p => Context.IsBetter(relink, p)))
        HillClimb(relink, token);

      if (delink) {
        Context.AddDelinkingResult(a, b, relink);
        if (Context.DelinkingStat.Count % 10 == 0) Context.RelearnDelinkingPerformanceModel();
      } else {
        Context.AddRelinkingResult(a, b, relink);
        if (context.RelinkingStat.Count % 10 == 0) Context.RelearnRelinkingPerformanceModel();
      }
      return relink;
    }

    protected abstract ISingleObjectiveSolutionScope<TSolution> Link(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, CancellationToken token, bool delink = false);
    #endregion

    #region Sample
    protected virtual ISingleObjectiveSolutionScope<TSolution> Sample(CancellationToken token) {
      if (Context.PopulationCount == MaximumPopulationSize) {
        SolutionModelTrainerParameter.Value.TrainModel(Context);
        ISingleObjectiveSolutionScope<TSolution> bestSample = null;
        var tries = 1;
        for (; tries < 100; tries++) {
          var sample = Context.ToScope(Context.Model.Sample());
          Context.Evaluate(sample, token);
          if (bestSample == null || Context.IsBetter(sample, bestSample)) {
            bestSample = sample;
            if (Context.Population.Any(x => !Context.IsBetter(x, bestSample))) break;
          }
          if (!Context.SamplingSuited()) break;
        }
        Context.IncrementEvaluatedSolutions(tries);
        Context.AddSamplingResult(bestSample);
        if (Context.SamplingStat.Count % 10 == 0) Context.RelearnSamplingPerformanceModel();
        return bestSample;
      }
      return null;
    }
    #endregion

    protected virtual bool Terminate() {
      var maximization = ((IValueParameter<BoolValue>)Problem.MaximizationParameter).Value.Value;
      return MaximumEvaluations.HasValue && Context.EvaluatedSolutions >= MaximumEvaluations.Value
        || MaximumExecutionTime.HasValue && ExecutionTime >= MaximumExecutionTime.Value
        || TargetQuality.HasValue && (maximization && Context.BestQuality >= TargetQuality.Value
                                  || !maximization && Context.BestQuality <= TargetQuality.Value);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string property) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(property));
    }
  }
}
