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
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Selection;
using HeuristicLab.Optimization.Algorithms.SingleObjective;
using HeuristicLab.Optimization.Crossover;
using HeuristicLab.Optimization.Manipulation;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  public class CanonicalGeneticAlgorithm<TProblem, TEncoding, TSolution> : HeuristicAlgorithm<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>, TProblem, TEncoding, TSolution>
      where TProblem : class, ISingleObjectiveProblem<TEncoding, TSolution>, ISingleObjectiveProblemDefinition<TEncoding, TSolution>
      where TEncoding : class, IEncoding<TSolution>
      where TSolution : class, ISolution {

    [Storable]
    private IValueParameter<IntValue> populationSize;
    public int PopulationSize {
      get { return populationSize.Value.Value; }
      set {
        if (value < 2) throw new ArgumentException("PopulationSize cannot be smaller than 2");
        populationSize.Value.Value = value;
      }
    }

    [Storable]
    private IConstrainedValueParameter<ISelector<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>> selector;
    public IConstrainedValueParameter<ISelector<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>> SelectorParameter {
      get { return selector; }
    }
    public ISelector<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>> Selector {
      get { return selector.Value; }
      set {
        if (!selector.ValidValues.Contains(value))
          selector.ValidValues.Add(value);
        selector.Value = value;
      }
    }

    [Storable]
    private IConstrainedValueParameter<ICrossover<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>> crossover;
    public IConstrainedValueParameter<ICrossover<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>> CrossoverParameter {
      get { return crossover; }
    }
    public ICrossover<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>> Crossover {
      get { return crossover.Value; }
      set {
        if (!crossover.ValidValues.Contains(value))
          crossover.ValidValues.Add(value);
        crossover.Value = value;
      }
    }

    [Storable]
    private IConstrainedValueParameter<IManipulator<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>> mutator;
    public IConstrainedValueParameter<IManipulator<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>> MutatorParameter {
      get { return mutator; }
    }
    public IManipulator<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>> Mutator {
      get { return mutator.Value; }
      set {
        if (!mutator.ValidValues.Contains(value))
          mutator.ValidValues.Add(value);
        mutator.Value = value;
      }
    }

    [Storable]
    private IValueParameter<PercentValue> mutationProbability;
    public double MutationProbability {
      get { return mutationProbability.Value.Value; }
      set { mutationProbability.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> elitism;
    public int Elitism {
      get { return elitism.Value.Value; }
      set {
        if (value < 0) throw new ArgumentException("Elitism cannot be negative");
        elitism.Value.Value = value;
      }
    }

    [Storable]
    protected BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [StorableConstructor]
    protected CanonicalGeneticAlgorithm(bool deserializing) : base(deserializing) { }
    protected CanonicalGeneticAlgorithm(CanonicalGeneticAlgorithm<TProblem, TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) {
      populationSize = cloner.Clone(original.populationSize);
      selector = cloner.Clone(original.selector);
      crossover = cloner.Clone(original.crossover);
      mutator = cloner.Clone(original.mutator);
      mutationProbability = cloner.Clone(original.mutationProbability);
      elitism = cloner.Clone(original.elitism);
    }
    public CanonicalGeneticAlgorithm() {
      ProblemAnalyzer = new MultiAnalyzer();
      AlgorithmAnalyzer = new MultiAnalyzer();
      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      AlgorithmAnalyzer.Operators.Add(qualityAnalyzer, true);

      Parameters.Add(populationSize = new ValueParameter<IntValue>("PopulationSize", "The number of individuals in the population", new IntValue(100)));
      Parameters.Add(selector = new ConstrainedValueParameter<ISelector<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>>("Selector", "The selection heuristic that creates the mating pool."));
      Parameters.Add(crossover = new ConstrainedValueParameter<ICrossover<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>>("Crossover", "The crossover heuristic that takes two parents and produces a child."));
      Parameters.Add(mutator = new ConstrainedValueParameter<IManipulator<EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>>>("Mutator", "The mutation heuristic that slightly alters an individual."));
      Parameters.Add(mutationProbability = new ValueParameter<PercentValue>("MutationProbability", "The probability with which mutation will be applied to an individual after mating.", new PercentValue(0.05)));
      Parameters.Add(elitism = new ValueParameter<IntValue>("Elitism", "The number of best solutions from the old population that will live in the next generation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CanonicalGeneticAlgorithm<TProblem, TEncoding, TSolution>(this, cloner);
    }

    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      qualityAnalyzer.QualityParameter.Depth = 1;
      qualityAnalyzer.QualityParameter.Hidden = true;
    }

    protected override void PerformInitialize(CancellationToken token) {
      for (var i = 0; i < PopulationSize; i++) {
        var solutionScope = CreateEmptySolutionScope();
        Context.Scope.SubScopes.Add(solutionScope);
        RunOperator(Problem.Encoding.SolutionCreator, solutionScope, token);
      }
      EvaluatePopulation(Context.Scope.SubScopes.OfType<ISingleObjectiveSolutionScope<TSolution>>().ToList(), token);
    }

    protected override void PerformIterate(CancellationToken token) {
      Selector.Select(Context, 2 * (PopulationSize - Elitism), true);
      var pool = Context.MatingPool.ToList();

      var nextGen = new List<ISingleObjectiveSolutionScope<TSolution>>(PopulationSize);
      for (var i = 0; i < PopulationSize - Elitism; i++) {
        Context.Parents = Tuple.Create(pool[2 * i], pool[2 * i + 1]);
        Context.Child = CreateEmptySolutionScope();
        Crossover.Cross(Context);
        if (Context.Random.NextDouble() < MutationProbability)
          Mutator.Manipulate(Context);
        nextGen.Add(Context.Child);
      }
      EvaluatePopulation(nextGen, token);
      if (Elitism > 0) {
        nextGen.AddRange(Problem.Maximization
          ? Context.Population.OrderByDescending(x => x.Fitness).Take(Elitism)
          : Context.Population.OrderBy(x => x.Fitness).Take(Elitism));
      }
      
      Context.Scope.SubScopes.Replace(nextGen);
      Context.Iterations++;
    }

    private void EvaluatePopulation(ICollection<ISingleObjectiveSolutionScope<TSolution>> solutions, CancellationToken token) {
      Parallel.ForEach(solutions, p => Evaluate(p, token));
      Context.EvaluatedSolutions += solutions.Count;
      var best = Problem.Maximization ? solutions.MaxItems(x => x.Fitness).First() : solutions.MinItems(x => x.Fitness).First();
      if (IsBetter(Problem.Maximization, best.Fitness, Context.BestQuality)) {
        Context.BestQuality = best.Fitness;
        Context.BestSolution = (TSolution)best.Solution.Clone();
      }
    }

    protected override void PerformAnalyze(CancellationToken token) {
      base.PerformAnalyze(token);

      IResult res;
      if (!Results.TryGetValue("Generations", out res)) {
        res = new Result("Generations", new IntValue(Context.Iterations));
        Results.Add(res);
      } else ((IntValue)res.Value).Value = Context.Iterations;
    }
  }
}
