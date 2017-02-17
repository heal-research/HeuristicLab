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
using System.Runtime.CompilerServices;
using System.Threading;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;
using ExecutionContext = HeuristicLab.Core.ExecutionContext;

namespace HeuristicLab.Algorithms.MemPR {
  [Item("MemPRContext", "Abstract base class for MemPR contexts.")]
  [StorableClass]
  public abstract class MemPRPopulationContext<TProblem, TSolution, TPopulationContext, TSolutionContext> : ParameterizedNamedItem,
    IPopulationBasedHeuristicAlgorithmContext<TProblem, TSolution>, ISolutionModelContext<TSolution>, IEvaluationServiceContext<TSolution>
      where TProblem : class, IItem, ISingleObjectiveHeuristicOptimizationProblem
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
    public bool Maximization {
      get { return ((IValueParameter<BoolValue>)Problem.MaximizationParameter).Value.Value; }
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
    private IValueParameter<IntValue> localSearchEvaluations;
    public int LocalSearchEvaluations {
      get { return localSearchEvaluations.Value.Value; }
      set { localSearchEvaluations.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<DoubleValue> localOptimaLevel;
    public double LocalOptimaLevel {
      get { return localOptimaLevel.Value.Value; }
      set { localOptimaLevel.Value.Value = value; }
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
    private IValueParameter<IntValue> byDelinking;
    public int ByDelinking {
      get { return byDelinking.Value.Value; }
      set { byDelinking.Value.Value = value; }
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
    private IValueParameter<IntValue> byAdaptivewalking;
    public int ByAdaptivewalking {
      get { return byAdaptivewalking.Value.Value; }
      set { byAdaptivewalking.Value.Value = value; }
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
    public void SortPopulation() {
      scope.SubScopes.Replace(scope.SubScopes.OfType<ISingleObjectiveSolutionScope<TSolution>>().OrderBy(x => Maximization ? -x.Fitness : x.Fitness).ToList());
    }
    public int PopulationCount {
      get { return scope.SubScopes.Count; }
    }

    [Storable]
    private IConfidenceRegressionModel breedingPerformanceModel;
    public IConfidenceRegressionModel BreedingPerformanceModel {
      get { return breedingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double, double, double>> breedingStat;
    public IEnumerable<Tuple<double, double, double, double>> BreedingStat {
      get { return breedingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel relinkingPerformanceModel;
    public IConfidenceRegressionModel RelinkingPerformanceModel {
      get { return relinkingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double, double, double>> relinkingStat;
    public IEnumerable<Tuple<double, double, double, double>> RelinkingStat {
      get { return relinkingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel delinkingPerformanceModel;
    public IConfidenceRegressionModel DelinkingPerformanceModel {
      get { return delinkingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double, double, double>> delinkingStat;
    public IEnumerable<Tuple<double, double, double, double>> DelinkingStat {
      get { return delinkingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel samplingPerformanceModel;
    public IConfidenceRegressionModel SamplingPerformanceModel {
      get { return samplingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double>> samplingStat;
    public IEnumerable<Tuple<double, double>> SamplingStat {
      get { return samplingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel hillclimbingPerformanceModel;
    public IConfidenceRegressionModel HillclimbingPerformanceModel {
      get { return hillclimbingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double>> hillclimbingStat;
    public IEnumerable<Tuple<double, double>> HillclimbingStat {
      get { return hillclimbingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel adaptiveWalkPerformanceModel;
    public IConfidenceRegressionModel AdaptiveWalkPerformanceModel {
      get { return adaptiveWalkPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double>> adaptivewalkingStat;
    public IEnumerable<Tuple<double, double>> AdaptivewalkingStat {
      get { return adaptivewalkingStat; }
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
      localSearchEvaluations = cloner.Clone(original.localSearchEvaluations);
      localOptimaLevel = cloner.Clone(original.localOptimaLevel);
      byBreeding = cloner.Clone(original.byBreeding);
      byRelinking = cloner.Clone(original.byRelinking);
      byDelinking = cloner.Clone(original.byDelinking);
      bySampling = cloner.Clone(original.bySampling);
      byHillclimbing = cloner.Clone(original.byHillclimbing);
      byAdaptivewalking = cloner.Clone(original.byAdaptivewalking);
      random = cloner.Clone(original.random);
      breedingPerformanceModel = cloner.Clone(original.breedingPerformanceModel);
      breedingStat = original.breedingStat.Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4)).ToList();
      relinkingPerformanceModel = cloner.Clone(original.relinkingPerformanceModel);
      relinkingStat = original.relinkingStat.Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4)).ToList();
      delinkingPerformanceModel = cloner.Clone(original.delinkingPerformanceModel);
      delinkingStat = original.delinkingStat.Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4)).ToList();
      samplingPerformanceModel = cloner.Clone(original.samplingPerformanceModel);
      samplingStat = original.samplingStat.Select(x => Tuple.Create(x.Item1, x.Item2)).ToList();
      hillclimbingPerformanceModel = cloner.Clone(original.hillclimbingPerformanceModel);
      hillclimbingStat = original.hillclimbingStat.Select(x => Tuple.Create(x.Item1, x.Item2)).ToList();
      adaptiveWalkPerformanceModel = cloner.Clone(original.adaptiveWalkPerformanceModel);
      adaptivewalkingStat = original.adaptivewalkingStat.Select(x => Tuple.Create(x.Item1, x.Item2)).ToList();
      
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
      Parameters.Add(bestSolution = new ValueParameter<TSolution>("BestFoundSolution"));
      Parameters.Add(localSearchEvaluations = new ValueParameter<IntValue>("LocalSearchEvaluations", new IntValue(0)));
      Parameters.Add(localOptimaLevel = new ValueParameter<DoubleValue>("LocalOptimaLevel", new DoubleValue(0)));
      Parameters.Add(byBreeding = new ValueParameter<IntValue>("ByBreeding", new IntValue(0)));
      Parameters.Add(byRelinking = new ValueParameter<IntValue>("ByRelinking", new IntValue(0)));
      Parameters.Add(byDelinking = new ValueParameter<IntValue>("ByDelinking", new IntValue(0)));
      Parameters.Add(bySampling = new ValueParameter<IntValue>("BySampling", new IntValue(0)));
      Parameters.Add(byHillclimbing = new ValueParameter<IntValue>("ByHillclimbing", new IntValue(0)));
      Parameters.Add(byAdaptivewalking = new ValueParameter<IntValue>("ByAdaptivewalking", new IntValue(0)));
      Parameters.Add(random = new ValueParameter<IRandom>("Random", new MersenneTwister()));

      breedingStat = new List<Tuple<double, double, double, double>>();
      relinkingStat = new List<Tuple<double, double, double, double>>();
      delinkingStat = new List<Tuple<double, double, double, double>>();
      samplingStat = new List<Tuple<double, double>>();
      hillclimbingStat = new List<Tuple<double, double>>();
      adaptivewalkingStat = new List<Tuple<double, double>>();
    }

    public abstract ISingleObjectiveSolutionScope<TSolution> ToScope(TSolution code, double fitness = double.NaN);

    public virtual double Evaluate(TSolution solution, CancellationToken token) {
      var solScope = ToScope(solution);
      Evaluate(solScope, token);
      return solScope.Fitness;
    }

    public virtual void Evaluate(ISingleObjectiveSolutionScope<TSolution> solScope, CancellationToken token) {
      var pdef = Problem as ISingleObjectiveProblemDefinition;
      if (pdef != null) {
        var ind = new SingleEncodingIndividual(pdef.Encoding, solScope);
        solScope.Fitness = pdef.Evaluate(ind, Random);
      } else {
        RunOperator(Problem.Evaluator, solScope, token);
      }
    }

    public abstract TSolutionContext CreateSingleSolutionContext(ISingleObjectiveSolutionScope<TSolution> solution);

    public void IncrementEvaluatedSolutions(int byEvaluations) {
      if (byEvaluations < 0) throw new ArgumentException("Can only increment and not decrement evaluated solutions.");
      EvaluatedSolutions += byEvaluations;
    }

    #region Breeding Performance
    public void AddBreedingResult(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, double parentDist, ISingleObjectiveSolutionScope<TSolution> child) {
      return;
      if (IsBetter(a, b))
        breedingStat.Add(Tuple.Create(a.Fitness, b.Fitness, parentDist, child.Fitness));
      else breedingStat.Add(Tuple.Create(b.Fitness, a.Fitness, parentDist, child.Fitness));
      if (breedingStat.Count % 10 == 0) RelearnBreedingPerformanceModel();
    }
    public void RelearnBreedingPerformanceModel() {
      return;
      breedingPerformanceModel = RunRegression(PrepareRegression(ToListRow(breedingStat)), breedingPerformanceModel).Model;
    }
    public bool BreedingSuited(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2, double dist) {
      return true;
      if (breedingPerformanceModel == null) return true;
      double minI1 = double.MaxValue, minI2 = double.MaxValue, maxI1 = double.MinValue, maxI2 = double.MinValue;
      foreach (var d in BreedingStat) {
        if (d.Item1 < minI1) minI1 = d.Item1;
        if (d.Item1 > maxI1) maxI1 = d.Item1;
        if (d.Item2 < minI2) minI2 = d.Item2;
        if (d.Item2 > maxI2) maxI2 = d.Item2;
      }
      if (p1.Fitness < minI1 || p1.Fitness > maxI1 || p2.Fitness < minI2 || p2.Fitness > maxI2)
        return true;
      
      return Random.NextDouble() < ProbabilityAcceptAbsolutePerformanceModel(new List<double> { p1.Fitness, p2.Fitness, dist }, breedingPerformanceModel);
    }
    #endregion

    #region Relinking Performance
    public void AddRelinkingResult(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, double parentDist, ISingleObjectiveSolutionScope<TSolution> child) {
      return;
      if (IsBetter(a, b))
        relinkingStat.Add(Tuple.Create(a.Fitness, b.Fitness, parentDist, Maximization ? child.Fitness - a.Fitness : a.Fitness - child.Fitness));
      else relinkingStat.Add(Tuple.Create(a.Fitness, b.Fitness, parentDist, Maximization ? child.Fitness - b.Fitness : b.Fitness - child.Fitness));
      if (relinkingStat.Count % 10 == 0) RelearnRelinkingPerformanceModel();
    }
    public void RelearnRelinkingPerformanceModel() {
      return;
      relinkingPerformanceModel = RunRegression(PrepareRegression(ToListRow(relinkingStat)), relinkingPerformanceModel).Model;
    }
    public bool RelinkSuited(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2, double dist) {
      return true;
      if (relinkingPerformanceModel == null) return true;
      double minI1 = double.MaxValue, minI2 = double.MaxValue, maxI1 = double.MinValue, maxI2 = double.MinValue;
      foreach (var d in RelinkingStat) {
        if (d.Item1 < minI1) minI1 = d.Item1;
        if (d.Item1 > maxI1) maxI1 = d.Item1;
        if (d.Item2 < minI2) minI2 = d.Item2;
        if (d.Item2 > maxI2) maxI2 = d.Item2;
      }
      if (p1.Fitness < minI1 || p1.Fitness > maxI1 || p2.Fitness < minI2 || p2.Fitness > maxI2)
        return true;

      if (IsBetter(p1, p2)) {
        return Random.NextDouble() < ProbabilityAcceptRelativePerformanceModel(p1.Fitness, new List<double> { p1.Fitness, p2.Fitness, dist }, relinkingPerformanceModel);
      }
      return Random.NextDouble() < ProbabilityAcceptRelativePerformanceModel(p2.Fitness, new List<double> { p1.Fitness, p2.Fitness, dist }, relinkingPerformanceModel);
    }
    #endregion

    #region Delinking Performance
    public void AddDelinkingResult(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, double parentDist, ISingleObjectiveSolutionScope<TSolution> child) {
      return;
      if (IsBetter(a, b))
        delinkingStat.Add(Tuple.Create(a.Fitness, b.Fitness, parentDist, Maximization ? child.Fitness - a.Fitness : a.Fitness - child.Fitness));
      else delinkingStat.Add(Tuple.Create(a.Fitness, b.Fitness, parentDist, Maximization ? child.Fitness - b.Fitness : b.Fitness - child.Fitness));
      if (delinkingStat.Count % 10 == 0) RelearnDelinkingPerformanceModel();
    }
    public void RelearnDelinkingPerformanceModel() {
      return;
      delinkingPerformanceModel = RunRegression(PrepareRegression(ToListRow(delinkingStat)), delinkingPerformanceModel).Model;
    }
    public bool DelinkSuited(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2, double dist) {
      return true;
      if (delinkingPerformanceModel == null) return true;
      double minI1 = double.MaxValue, minI2 = double.MaxValue, maxI1 = double.MinValue, maxI2 = double.MinValue;
      foreach (var d in DelinkingStat) {
        if (d.Item1 < minI1) minI1 = d.Item1;
        if (d.Item1 > maxI1) maxI1 = d.Item1;
        if (d.Item2 < minI2) minI2 = d.Item2;
        if (d.Item2 > maxI2) maxI2 = d.Item2;
      }
      if (p1.Fitness < minI1 || p1.Fitness > maxI1 || p2.Fitness < minI2 || p2.Fitness > maxI2)
        return true;
      if (IsBetter(p1, p2)) {
        return Random.NextDouble() < ProbabilityAcceptRelativePerformanceModel(p1.Fitness, new List<double> { p1.Fitness, p2.Fitness, dist }, delinkingPerformanceModel);
      }
      return Random.NextDouble() < ProbabilityAcceptRelativePerformanceModel(p2.Fitness, new List<double> { p1.Fitness, p2.Fitness, dist }, delinkingPerformanceModel);
    }
    #endregion

    #region Sampling Performance
    public void AddSamplingResult(ISingleObjectiveSolutionScope<TSolution> sample, double avgDist) {
      return;
      samplingStat.Add(Tuple.Create(avgDist, sample.Fitness));
      if (samplingStat.Count % 10 == 0) RelearnSamplingPerformanceModel();
    }
    public void RelearnSamplingPerformanceModel() {
      return;
      samplingPerformanceModel = RunRegression(PrepareRegression(ToListRow(samplingStat)), samplingPerformanceModel).Model;
    }
    public bool SamplingSuited(double avgDist) {
      return true;
      if (samplingPerformanceModel == null) return true;
      if (avgDist < samplingStat.Min(x => x.Item1) || avgDist > samplingStat.Max(x => x.Item1)) return true;
      return Random.NextDouble() < ProbabilityAcceptAbsolutePerformanceModel(new List<double> { avgDist }, samplingPerformanceModel);
    }
    #endregion

    #region Hillclimbing Performance
    public void AddHillclimbingResult(ISingleObjectiveSolutionScope<TSolution> input, ISingleObjectiveSolutionScope<TSolution> outcome) {
      return;
      hillclimbingStat.Add(Tuple.Create(input.Fitness, Maximization ? outcome.Fitness - input.Fitness : input.Fitness - outcome.Fitness));
      if (hillclimbingStat.Count % 10 == 0) RelearnHillclimbingPerformanceModel();
    }
    public void RelearnHillclimbingPerformanceModel() {
      return;
      hillclimbingPerformanceModel = RunRegression(PrepareRegression(ToListRow(hillclimbingStat)), hillclimbingPerformanceModel).Model;
    }
    public bool HillclimbingSuited(double startingFitness) {
      return true;
      if (hillclimbingPerformanceModel == null) return true;
      if (startingFitness < HillclimbingStat.Min(x => x.Item1) || startingFitness > HillclimbingStat.Max(x => x.Item1))
        return true;
      return Random.NextDouble() < ProbabilityAcceptRelativePerformanceModel(startingFitness, new List<double> { startingFitness }, hillclimbingPerformanceModel);
    }
    #endregion

    #region Adaptivewalking Performance
    public void AddAdaptivewalkingResult(ISingleObjectiveSolutionScope<TSolution> input, ISingleObjectiveSolutionScope<TSolution> outcome) {
      return;
      adaptivewalkingStat.Add(Tuple.Create(input.Fitness, Maximization ? outcome.Fitness - input.Fitness : input.Fitness - outcome.Fitness));
      if (adaptivewalkingStat.Count % 10 == 0) RelearnAdaptiveWalkPerformanceModel();
    }
    public void RelearnAdaptiveWalkPerformanceModel() {
      return;
      adaptiveWalkPerformanceModel = RunRegression(PrepareRegression(ToListRow(adaptivewalkingStat)), adaptiveWalkPerformanceModel).Model;
    }
    public bool AdaptivewalkingSuited(double startingFitness) {
      return true;
      if (adaptiveWalkPerformanceModel == null) return true;
      if (startingFitness < AdaptivewalkingStat.Min(x => x.Item1) || startingFitness > AdaptivewalkingStat.Max(x => x.Item1))
        return true;
      return Random.NextDouble() < ProbabilityAcceptRelativePerformanceModel(startingFitness, new List<double> { startingFitness }, adaptiveWalkPerformanceModel);
    }
    #endregion

    public IConfidenceRegressionSolution GetSolution(IConfidenceRegressionModel model, IEnumerable<Tuple<double, double>> data) {
      return new ConfidenceRegressionSolution(model, PrepareRegression(ToListRow(data.ToList())));
    }
    public IConfidenceRegressionSolution GetSolution(IConfidenceRegressionModel model, IEnumerable<Tuple<double, double, double>> data) {
      return new ConfidenceRegressionSolution(model, PrepareRegression(ToListRow(data.ToList())));
    }
    public IConfidenceRegressionSolution GetSolution(IConfidenceRegressionModel model, IEnumerable<Tuple<double, double, double, double>> data) {
      return new ConfidenceRegressionSolution(model, PrepareRegression(ToListRow(data.ToList())));
    }

    protected RegressionProblemData PrepareRegression(List<List<double>> data) {
      var columns = data.First().Select(y => new List<double>()).ToList();
      foreach (var next in data.Shuffle(Random)) {
        for (var i = 0; i < next.Count; i++) {
          columns[i].Add(next[i]);
        }
      }
      var ds = new Dataset(columns.Select((v, i) => i < columns.Count - 1 ? "in" + i : "out").ToList(), columns);
      var regPrb = new RegressionProblemData(ds, Enumerable.Range(0, columns.Count - 1).Select(x => "in" + x), "out") {
        TrainingPartition = { Start = 0, End = Math.Min(50, data.Count) },
        TestPartition = { Start = Math.Min(50, data.Count), End = data.Count }
      };
      return regPrb;
    }

    protected static IConfidenceRegressionSolution RunRegression(RegressionProblemData trainingData, IConfidenceRegressionModel baseLineModel = null) {
      var targetValues = trainingData.Dataset.GetDoubleValues(trainingData.TargetVariable, trainingData.TrainingIndices).ToList();
      var baseline = baseLineModel != null ? new ConfidenceRegressionSolution(baseLineModel, trainingData) : null;
      var constantSolution = new ConfidenceRegressionSolution(new ConfidenceConstantModel(targetValues.Average(), targetValues.Variance(), trainingData.TargetVariable), trainingData);
      var gpr = new GaussianProcessRegression { Problem = { ProblemData = trainingData } };
      if (trainingData.InputVariables.CheckedItems.Any(x => alglib.pearsoncorr2(trainingData.Dataset.GetDoubleValues(x.Value.Value).ToArray(), trainingData.TargetVariableValues.ToArray()) > 0.8)) {
        gpr.MeanFunction = new MeanZero();
        var cov1 = new CovarianceSum();
        cov1.Terms.Add(new CovarianceLinearArd());
        cov1.Terms.Add(new CovarianceConst());
        gpr.CovarianceFunction = cov1;
      }
      IConfidenceRegressionSolution solution = null;
      var cnt = 0;
      do {
        ExecuteAlgorithm(gpr);
        solution = (IConfidenceRegressionSolution)gpr.Results["Solution"].Value;
        cnt++;
      } while (cnt < 10 && (solution == null || solution.TrainingRSquared.IsAlmost(0)));

      return GetBestRegressionSolution(constantSolution, baseline, solution);
    }

    private static IConfidenceRegressionSolution GetBestRegressionSolution(IConfidenceRegressionSolution constant, IConfidenceRegressionSolution baseline, IConfidenceRegressionSolution solution) {
      if (baseline == null)
        return constant.TrainingMeanAbsoluteError < solution.TrainingMeanAbsoluteError ? constant : solution;

      double a, b, c;
      if (constant.ProblemData.Dataset.Rows < 60) {
        c = constant.TrainingMeanAbsoluteError;
        b = baseline.TrainingMeanAbsoluteError;
        a = solution.TrainingMeanAbsoluteError;
      } else {
        c = constant.TestMeanAbsoluteError;
        b = baseline.TestMeanAbsoluteError;
        a = solution.TestMeanAbsoluteError;
      }
      if (c < b && (c < a || b < a)) return constant;
      if (b < c && (b < a || c < a)) return baseline;
      return solution;
    }

    protected static void ExecuteAlgorithm(IAlgorithm algorithm) {
      using (var evt = new AutoResetEvent(false)) {
        EventHandler exeStateChanged = (o, args) => {
          if (algorithm.ExecutionState != ExecutionState.Started)
            evt.Set();
        };
        algorithm.ExecutionStateChanged += exeStateChanged;
        if (algorithm.ExecutionState != ExecutionState.Prepared) {
          algorithm.Prepare(true);
          evt.WaitOne();
        }
        algorithm.Start();
        evt.WaitOne();
        algorithm.ExecutionStateChanged -= exeStateChanged;
      }
    }

    private double ProbabilityAcceptAbsolutePerformanceModel(List<double> inputs, IConfidenceRegressionModel model) {
      var inputVariables = inputs.Select((v, i) => "in" + i);
      var ds = new Dataset(inputVariables.Concat( new [] { "out" }), inputs.Select(x => new List<double> { x }).Concat(new [] { new List<double> { double.NaN } }));
      var mean = model.GetEstimatedValues(ds, new[] { 0 }).Single();
      var sdev = Math.Sqrt(model.GetEstimatedVariances(ds, new[] { 0 }).Single());

      // calculate the fitness goal
      var goal = Maximization ? Population.Min(x => x.Fitness) : Population.Max(x => x.Fitness);
      var z = (goal - mean) / sdev;
      // return the probability of achieving or surpassing that goal
      var y = alglib.invnormaldistribution(z);
      return Maximization ? 1.0 - y /* P(X >= z) */ : y; // P(X <= z)
    }

    private double ProbabilityAcceptRelativePerformanceModel(double basePerformance, List<double> inputs, IConfidenceRegressionModel model) {
      var inputVariables = inputs.Select((v, i) => "in" + i);
      var ds = new Dataset(inputVariables.Concat(new[] { "out" }), inputs.Select(x => new List<double> { x }).Concat(new[] { new List<double> { double.NaN } }));
      var mean = model.GetEstimatedValues(ds, new[] { 0 }).Single();
      var sdev = Math.Sqrt(model.GetEstimatedVariances(ds, new[] { 0 }).Single());

      // calculate the improvement goal
      var goal = Maximization ? Population.Min(x => x.Fitness) - basePerformance : basePerformance - Population.Max(x => x.Fitness);
      var z = (goal - mean) / sdev;
      // return the probability of achieving or surpassing that goal
      return 1.0 - alglib.invnormaldistribution(z); /* P(X >= z) */
    }

    private static List<List<double>> ToListRow(List<Tuple<double, double>> rows) {
      return rows.Select(x => new List<double> { x.Item1, x.Item2 }).ToList();
    }
    private static List<List<double>> ToListRow(List<Tuple<double, double, double>> rows) {
      return rows.Select(x => new List<double> { x.Item1, x.Item2, x.Item3 }).ToList();
    }
    private static List<List<double>> ToListRow(List<Tuple<double, double, double, double>> rows) {
      return rows.Select(x => new List<double> { x.Item1, x.Item2, x.Item3, x.Item4 }).ToList();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsBetter(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b) {
      return IsBetter(a.Fitness, b.Fitness);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsBetter(double a, double b) {
      return double.IsNaN(b) && !double.IsNaN(a)
        || Maximization && a > b
        || !Maximization && a < b;
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

    #region Engine Helper
    public void RunOperator(IOperator op, IScope scope, CancellationToken cancellationToken) {
      var stack = new Stack<IOperation>();
      stack.Push(CreateChildOperation(op, scope));

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
  }

  [Item("SingleSolutionMemPRContext", "Abstract base class for single solution MemPR contexts.")]
  [StorableClass]
  public abstract class MemPRSolutionContext<TProblem, TSolution, TContext, TSolutionContext> : ParameterizedNamedItem,
    ISingleSolutionHeuristicAlgorithmContext<TProblem, TSolution>, IEvaluationServiceContext<TSolution>
      where TProblem : class, IItem, ISingleObjectiveHeuristicOptimizationProblem
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
    public bool Maximization {
      get { return parent.Maximization; }
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
    public virtual double Evaluate(TSolution solution, CancellationToken token) {
      return parent.Evaluate(solution, token);
    }

    public virtual void Evaluate(ISingleObjectiveSolutionScope<TSolution> solScope, CancellationToken token) {
      parent.Evaluate(solScope, token);
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
