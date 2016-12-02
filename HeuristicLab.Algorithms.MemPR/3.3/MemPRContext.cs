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
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR {
  [Item("MemPRContext", "Abstract base class for MemPR contexts.")]
  [StorableClass]
  public abstract class MemPRPopulationContext<TProblem, TSolution, TPopulationContext, TSolutionContext> : ParameterizedNamedItem,
    IPopulationBasedHeuristicAlgorithmContext<TProblem, TSolution>, ISolutionModelContext<TSolution>
      where TProblem : class, IItem, ISingleObjectiveProblemDefinition
      where TSolution : class, IItem
      where TPopulationContext : MemPRPopulationContext<TProblem, TSolution, TPopulationContext, TSolutionContext>
      where TSolutionContext : MemPRSolutionContext<TProblem, TSolution, TPopulationContext, TSolutionContext> {

    private IExecutionContext parent;
    public IExecutionContext Parent {
      get { return parent; }
      set { parent = value; }
    }

    [Storable]
    private IScope scope;
    public IScope Scope {
      get { return scope; }
      private set { scope = value; }
    }

    IKeyedItemCollection<string, IParameter> IExecutionContext.Parameters {
      get { return Parameters; }
    }

    [Storable]
    private IValueParameter<TProblem> problem;
    public TProblem Problem {
      get { return problem.Value; }
      set { problem.Value = value; }
    }

    [Storable]
    private IValueParameter<BoolValue> initialized;
    public bool Initialized {
      get { return initialized.Value.Value; }
      set { initialized.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> iterations;
    public int Iterations {
      get { return iterations.Value.Value; }
      set { iterations.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> evaluatedSolutions;
    public int EvaluatedSolutions {
      get { return evaluatedSolutions.Value.Value; }
      set { evaluatedSolutions.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<DoubleValue> bestQuality;
    public double BestQuality {
      get { return bestQuality.Value.Value; }
      set { bestQuality.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<TSolution> bestSolution;
    public TSolution BestSolution {
      get { return bestSolution.Value; }
      set { bestSolution.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> hcSteps;
    public int HcSteps {
      get { return hcSteps.Value.Value; }
      set { hcSteps.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> byBreeding;
    public int ByBreeding {
      get { return byBreeding.Value.Value; }
      set { byBreeding.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> byRelinking;
    public int ByRelinking {
      get { return byRelinking.Value.Value; }
      set { byRelinking.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> bySampling;
    public int BySampling {
      get { return bySampling.Value.Value; }
      set { bySampling.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> byHillclimbing;
    public int ByHillclimbing {
      get { return byHillclimbing.Value.Value; }
      set { byHillclimbing.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> byTabuwalking;
    public int ByTabuwalking {
      get { return byTabuwalking.Value.Value; }
      set { byTabuwalking.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IRandom> random;
    public IRandom Random {
      get { return random.Value; }
      set { random.Value = value; }
    }
    
    public IEnumerable<ISingleObjectiveSolutionScope<TSolution>> Population {
      get { return scope.SubScopes.OfType<ISingleObjectiveSolutionScope<TSolution>>(); }
    }
    public void AddToPopulation(ISingleObjectiveSolutionScope<TSolution> solScope) {
      scope.SubScopes.Add(solScope);
    }
    public void ReplaceAtPopulation(int index, ISingleObjectiveSolutionScope<TSolution> solScope) {
      scope.SubScopes[index] = solScope;
    }
    public ISingleObjectiveSolutionScope<TSolution> AtPopulation(int index) {
      return scope.SubScopes[index] as ISingleObjectiveSolutionScope<TSolution>;
    }
    public int PopulationCount {
      get { return scope.SubScopes.Count; }
    }

    [Storable]
    private List<Tuple<double, double, double>> breedingStat;
    public List<Tuple<double, double, double>> BreedingStat {
      get { return breedingStat; }
    }
    [Storable]
    private List<Tuple<double, double>> hillclimbingStat;
    public List<Tuple<double, double>> HillclimbingStat {
      get { return hillclimbingStat; }
    }
    [Storable]
    private List<Tuple<double, double>> tabuwalkingStat;
    public List<Tuple<double, double>> TabuwalkingStat {
      get { return tabuwalkingStat; }
    }

    [Storable]
    public ISolutionModel<TSolution> Model { get; set; }

    [StorableConstructor]
    protected MemPRPopulationContext(bool deserializing) : base(deserializing) { }
    protected MemPRPopulationContext(MemPRPopulationContext<TProblem, TSolution, TPopulationContext, TSolutionContext> original, Cloner cloner)
      : base(original, cloner) {
      scope = cloner.Clone(original.scope);
      problem = cloner.Clone(original.problem);
      initialized = cloner.Clone(original.initialized);
      iterations = cloner.Clone(original.iterations);
      evaluatedSolutions = cloner.Clone(original.evaluatedSolutions);
      bestQuality = cloner.Clone(original.bestQuality);
      bestSolution = cloner.Clone(original.bestSolution);
      hcSteps = cloner.Clone(original.hcSteps);
      byBreeding = cloner.Clone(original.byBreeding);
      byRelinking = cloner.Clone(original.byRelinking);
      bySampling = cloner.Clone(original.bySampling);
      byHillclimbing = cloner.Clone(original.byHillclimbing);
      byTabuwalking = cloner.Clone(original.byTabuwalking);
      random = cloner.Clone(original.random);
      breedingStat = original.breedingStat.Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3)).ToList();
      hillclimbingStat = original.hillclimbingStat.Select(x => Tuple.Create(x.Item1, x.Item2)).ToList();
      tabuwalkingStat = original.tabuwalkingStat.Select(x => Tuple.Create(x.Item1, x.Item2)).ToList();

      Model = cloner.Clone(original.Model);
    }
    public MemPRPopulationContext() : this("MemPRContext") { }
    public MemPRPopulationContext(string name) : base(name) {
      scope = new Scope("Global");

      Parameters.Add(problem = new ValueParameter<TProblem>("Problem"));
      Parameters.Add(initialized = new ValueParameter<BoolValue>("Initialized", new BoolValue(false)));
      Parameters.Add(iterations = new ValueParameter<IntValue>("Iterations", new IntValue(0)));
      Parameters.Add(evaluatedSolutions = new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      Parameters.Add(bestQuality = new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(double.NaN)));
      Parameters.Add(bestSolution = new ValueParameter<TSolution>("BestSolution"));
      Parameters.Add(hcSteps = new ValueParameter<IntValue>("HcSteps", new IntValue(0)));
      Parameters.Add(byBreeding = new ValueParameter<IntValue>("ByBreeding", new IntValue(0)));
      Parameters.Add(byRelinking = new ValueParameter<IntValue>("ByRelinking", new IntValue(0)));
      Parameters.Add(bySampling = new ValueParameter<IntValue>("BySampling", new IntValue(0)));
      Parameters.Add(byHillclimbing = new ValueParameter<IntValue>("ByHillclimbing", new IntValue(0)));
      Parameters.Add(byTabuwalking = new ValueParameter<IntValue>("ByTabuwalking", new IntValue(0)));
      Parameters.Add(random = new ValueParameter<IRandom>("Random", new MersenneTwister()));

      breedingStat = new List<Tuple<double, double, double>>();
      hillclimbingStat = new List<Tuple<double, double>>();
      tabuwalkingStat = new List<Tuple<double, double>>();
    }

    public abstract TSolutionContext CreateSingleSolutionContext(ISingleObjectiveSolutionScope<TSolution> solution);

    public void IncrementEvaluatedSolutions(int byEvaluations) {
      if (byEvaluations < 0) throw new ArgumentException("Can only increment and not decrement evaluated solutions.");
      EvaluatedSolutions += byEvaluations;
    }

    #region IExecutionContext members
    public IAtomicOperation CreateOperation(IOperator op) {
      return new ExecutionContext(this, op, Scope);
    }

    public IAtomicOperation CreateOperation(IOperator op, IScope s) {
      return new ExecutionContext(this, op, s);
    }

    public IAtomicOperation CreateChildOperation(IOperator op) {
      return new ExecutionContext(this, op, Scope);
    }

    public IAtomicOperation CreateChildOperation(IOperator op, IScope s) {
      return new ExecutionContext(this, op, s);
    }
    #endregion
  }

  [Item("SingleSolutionMemPRContext", "Abstract base class for single solution MemPR contexts.")]
  [StorableClass]
  public abstract class MemPRSolutionContext<TProblem, TSolution, TContext, TSolutionContext> : ParameterizedNamedItem,
    ISingleSolutionHeuristicAlgorithmContext<TProblem, TSolution>
      where TProblem : class, IItem, ISingleObjectiveProblemDefinition
      where TSolution : class, IItem
      where TContext : MemPRPopulationContext<TProblem, TSolution, TContext, TSolutionContext>
      where TSolutionContext : MemPRSolutionContext<TProblem, TSolution, TContext, TSolutionContext> {

    private TContext parent;
    public IExecutionContext Parent {
      get { return parent; }
      set { throw new InvalidOperationException("Cannot set the parent of a single-solution context."); }
    }

    [Storable]
    private ISingleObjectiveSolutionScope<TSolution> scope;
    public IScope Scope {
      get { return scope; }
    }

    IKeyedItemCollection<string, IParameter> IExecutionContext.Parameters {
      get { return Parameters; }
    }

    public TProblem Problem {
      get { return parent.Problem; }
    }

    public double BestQuality {
      get { return parent.BestQuality; }
      set { parent.BestQuality = value; }
    }

    public TSolution BestSolution {
      get { return parent.BestSolution; }
      set { parent.BestSolution = value; }
    }

    public IRandom Random {
      get { return parent.Random; }
    }

    [Storable]
    private IValueParameter<IntValue> evaluatedSolutions;
    public int EvaluatedSolutions {
      get { return evaluatedSolutions.Value.Value; }
      set { evaluatedSolutions.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> iterations;
    public int Iterations {
      get { return iterations.Value.Value; }
      set { iterations.Value.Value = value; }
    }

    ISingleObjectiveSolutionScope<TSolution> ISingleSolutionHeuristicAlgorithmContext<TProblem, TSolution>.Solution {
      get { return scope; }
    }

    [StorableConstructor]
    protected MemPRSolutionContext(bool deserializing) : base(deserializing) { }
    protected MemPRSolutionContext(MemPRSolutionContext<TProblem, TSolution, TContext, TSolutionContext> original, Cloner cloner)
      : base(original, cloner) {
      scope = cloner.Clone(original.scope);
      evaluatedSolutions = cloner.Clone(original.evaluatedSolutions);
      iterations = cloner.Clone(original.iterations);
    }
    public MemPRSolutionContext(TContext baseContext, ISingleObjectiveSolutionScope<TSolution> solution) {
      parent = baseContext;
      scope = solution;
      
      Parameters.Add(evaluatedSolutions = new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      Parameters.Add(iterations = new ValueParameter<IntValue>("Iterations", new IntValue(0)));
    }

    public void IncrementEvaluatedSolutions(int byEvaluations) {
      if (byEvaluations < 0) throw new ArgumentException("Can only increment and not decrement evaluated solutions.");
      EvaluatedSolutions += byEvaluations;
    }

    #region IExecutionContext members
    public IAtomicOperation CreateOperation(IOperator op) {
      return new ExecutionContext(this, op, Scope);
    }

    public IAtomicOperation CreateOperation(IOperator op, IScope s) {
      return new ExecutionContext(this, op, s);
    }

    public IAtomicOperation CreateChildOperation(IOperator op) {
      return new ExecutionContext(this, op, Scope);
    }

    public IAtomicOperation CreateChildOperation(IOperator op, IScope s) {
      return new ExecutionContext(this, op, s);
    }
    #endregion
  }
}
