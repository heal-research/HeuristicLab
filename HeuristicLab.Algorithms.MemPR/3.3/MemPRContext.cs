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
      scope.SubScopes.Replace(scope.SubScopes.OfType<ISingleObjectiveSolutionScope<TSolution>>().OrderBy(x => Problem.Maximization ? -x.Fitness : x.Fitness).ToList());
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
    private List<Tuple<double, double, double>> breedingStat;
    public List<Tuple<double, double, double>> BreedingStat {
      get { return breedingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel relinkingPerformanceModel;
    public IConfidenceRegressionModel RelinkingPerformanceModel {
      get { return relinkingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double, double>> relinkingStat;
    public List<Tuple<double, double, double>> RelinkingStat {
      get { return relinkingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel delinkingPerformanceModel;
    public IConfidenceRegressionModel DelinkingPerformanceModel {
      get { return delinkingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double, double>> delinkingStat;
    public List<Tuple<double, double, double>> DelinkingStat {
      get { return delinkingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel samplingPerformanceModel;
    public IConfidenceRegressionModel SamplingPerformanceModel {
      get { return samplingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double>> samplingStat;
    public List<Tuple<double, double>> SamplingStat {
      get { return samplingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel hillclimbingPerformanceModel;
    public IConfidenceRegressionModel HillclimbingPerformanceModel {
      get { return hillclimbingPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double>> hillclimbingStat;
    public List<Tuple<double, double>> HillclimbingStat {
      get { return hillclimbingStat; }
    }
    [Storable]
    private IConfidenceRegressionModel adaptiveWalkPerformanceModel;
    public IConfidenceRegressionModel AdaptiveWalkPerformanceModel {
      get { return adaptiveWalkPerformanceModel; }
    }
    [Storable]
    private List<Tuple<double, double>> adaptivewalkingStat;
    public List<Tuple<double, double>> AdaptivewalkingStat {
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
      breedingStat = original.breedingStat.Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3)).ToList();
      relinkingPerformanceModel = cloner.Clone(original.relinkingPerformanceModel);
      relinkingStat = original.relinkingStat.Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3)).ToList();
      delinkingPerformanceModel = cloner.Clone(original.delinkingPerformanceModel);
      delinkingStat = original.delinkingStat.Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3)).ToList();
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
      Parameters.Add(bestSolution = new ValueParameter<TSolution>("BestSolution"));
      Parameters.Add(localSearchEvaluations = new ValueParameter<IntValue>("LocalSearchEvaluations", new IntValue(0)));
      Parameters.Add(localOptimaLevel = new ValueParameter<DoubleValue>("LocalOptimaLevel", new DoubleValue(0)));
      Parameters.Add(byBreeding = new ValueParameter<IntValue>("ByBreeding", new IntValue(0)));
      Parameters.Add(byRelinking = new ValueParameter<IntValue>("ByRelinking", new IntValue(0)));
      Parameters.Add(byDelinking = new ValueParameter<IntValue>("ByDelinking", new IntValue(0)));
      Parameters.Add(bySampling = new ValueParameter<IntValue>("BySampling", new IntValue(0)));
      Parameters.Add(byHillclimbing = new ValueParameter<IntValue>("ByHillclimbing", new IntValue(0)));
      Parameters.Add(byAdaptivewalking = new ValueParameter<IntValue>("ByAdaptivewalking", new IntValue(0)));
      Parameters.Add(random = new ValueParameter<IRandom>("Random", new MersenneTwister()));

      breedingStat = new List<Tuple<double, double, double>>();
      relinkingStat = new List<Tuple<double, double, double>>();
      delinkingStat = new List<Tuple<double, double, double>>();
      samplingStat = new List<Tuple<double, double>>();
      hillclimbingStat = new List<Tuple<double, double>>();
      adaptivewalkingStat = new List<Tuple<double, double>>();
    }

    public abstract TSolutionContext CreateSingleSolutionContext(ISingleObjectiveSolutionScope<TSolution> solution);

    public void IncrementEvaluatedSolutions(int byEvaluations) {
      if (byEvaluations < 0) throw new ArgumentException("Can only increment and not decrement evaluated solutions.");
      EvaluatedSolutions += byEvaluations;
    }

    public void RelearnBreedingPerformanceModel() {
      breedingPerformanceModel = RunRegression(PrepareRegression(BreedingStat), breedingPerformanceModel).Model;
    }
    public bool BreedingSuited(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2) {
      if (breedingPerformanceModel == null) return true;
      double minI1 = double.MaxValue, minI2 = double.MaxValue, maxI1 = double.MinValue, maxI2 = double.MinValue;
      foreach (var d in BreedingStat) {
        if (d.Item1 < minI1) minI1 = d.Item1;
        if (d.Item1 > maxI1) maxI1 = d.Item1;
        if (d.Item2 < minI2) minI2 = d.Item2;
        if (d.Item2 > maxI2) maxI2 = d.Item2;
      }
      if (IsBetter(p1, p2)) {
        if (p1.Fitness < minI1 || p1.Fitness > maxI1 || p2.Fitness < minI2 || p2.Fitness > maxI2)
          return true;
        return Random.NextDouble() < ProbabilityAccept3dModel(p1.Fitness, p2.Fitness, breedingPerformanceModel);
      }
      if (p1.Fitness < minI2 || p1.Fitness > maxI2 || p2.Fitness < minI1 || p2.Fitness > maxI1)
        return true;
      return Random.NextDouble() < ProbabilityAccept3dModel(p2.Fitness, p1.Fitness, breedingPerformanceModel);
    }

    public void RelearnRelinkingPerformanceModel() {
      relinkingPerformanceModel = RunRegression(PrepareRegression(RelinkingStat), relinkingPerformanceModel).Model;
    }
    public bool RelinkSuited(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2) {
      if (relinkingPerformanceModel == null) return true;
      double minI1 = double.MaxValue, minI2 = double.MaxValue, maxI1 = double.MinValue, maxI2 = double.MinValue;
      foreach (var d in RelinkingStat) {
        if (d.Item1 < minI1) minI1 = d.Item1;
        if (d.Item1 > maxI1) maxI1 = d.Item1;
        if (d.Item2 < minI2) minI2 = d.Item2;
        if (d.Item2 > maxI2) maxI2 = d.Item2;
      }
      if (IsBetter(p1, p2)) {
        if (p1.Fitness < minI1 || p1.Fitness > maxI1 || p2.Fitness < minI2 || p2.Fitness > maxI2)
          return true;
        return Random.NextDouble() < ProbabilityAccept3dModel(p1.Fitness, p2.Fitness, relinkingPerformanceModel);
      }
      if (p1.Fitness < minI2 || p1.Fitness > maxI2 || p2.Fitness < minI1 || p2.Fitness > maxI1)
        return true;
      return Random.NextDouble() < ProbabilityAccept3dModel(p2.Fitness, p1.Fitness, relinkingPerformanceModel);
    }

    public void RelearnDelinkingPerformanceModel() {
      delinkingPerformanceModel = RunRegression(PrepareRegression(DelinkingStat), delinkingPerformanceModel).Model;
    }
    public bool DelinkSuited(ISingleObjectiveSolutionScope<TSolution> p1, ISingleObjectiveSolutionScope<TSolution> p2) {
      if (delinkingPerformanceModel == null) return true;
      double minI1 = double.MaxValue, minI2 = double.MaxValue, maxI1 = double.MinValue, maxI2 = double.MinValue;
      foreach (var d in DelinkingStat) {
        if (d.Item1 < minI1) minI1 = d.Item1;
        if (d.Item1 > maxI1) maxI1 = d.Item1;
        if (d.Item2 < minI2) minI2 = d.Item2;
        if (d.Item2 > maxI2) maxI2 = d.Item2;
      }
      if (IsBetter(p1, p2)) {
        if (p1.Fitness < minI1 || p1.Fitness > maxI1 || p2.Fitness < minI2 || p2.Fitness > maxI2)
          return true;
        return Random.NextDouble() < ProbabilityAccept3dModel(p1.Fitness, p2.Fitness, delinkingPerformanceModel);
      }
      if (p1.Fitness < minI2 || p1.Fitness > maxI2 || p2.Fitness < minI1 || p2.Fitness > maxI1)
        return true;
      return Random.NextDouble() < ProbabilityAccept3dModel(p2.Fitness, p1.Fitness, delinkingPerformanceModel);
    }

    public void RelearnSamplingPerformanceModel() {
      samplingPerformanceModel = RunRegression(PrepareRegression(SamplingStat), samplingPerformanceModel).Model;
    }
    public bool SamplingSuited() {
      if (samplingPerformanceModel == null) return true;
      return Random.NextDouble() < ProbabilityAccept2dModel(Population.Average(x => x.Fitness), samplingPerformanceModel);
    }

    public void RelearnHillclimbingPerformanceModel() {
      hillclimbingPerformanceModel = RunRegression(PrepareRegression(HillclimbingStat), hillclimbingPerformanceModel).Model;
    }
    public bool HillclimbingSuited(ISingleObjectiveSolutionScope<TSolution> scope) {
      if (hillclimbingPerformanceModel == null) return true;
      if (scope.Fitness < HillclimbingStat.Min(x => x.Item1) || scope.Fitness > HillclimbingStat.Max(x => x.Item1))
        return true;
      return Random.NextDouble() < ProbabilityAccept2dModel(scope.Fitness, hillclimbingPerformanceModel);
    }
    public bool HillclimbingSuited(double startingFitness) {
      if (hillclimbingPerformanceModel == null) return true;
      if (startingFitness < HillclimbingStat.Min(x => x.Item1) || startingFitness > HillclimbingStat.Max(x => x.Item1))
        return true;
      return Random.NextDouble() < ProbabilityAccept2dModel(startingFitness, hillclimbingPerformanceModel);
    }

    public void RelearnAdaptiveWalkPerformanceModel() {
      adaptiveWalkPerformanceModel = RunRegression(PrepareRegression(AdaptivewalkingStat), adaptiveWalkPerformanceModel).Model;
    }
    public bool AdaptivewalkingSuited(ISingleObjectiveSolutionScope<TSolution> scope) {
      if (adaptiveWalkPerformanceModel == null) return true;
      if (scope.Fitness < AdaptivewalkingStat.Min(x => x.Item1) || scope.Fitness > AdaptivewalkingStat.Max(x => x.Item1))
        return true;
      return Random.NextDouble() < ProbabilityAccept2dModel(scope.Fitness, adaptiveWalkPerformanceModel);
    }
    public bool AdaptivewalkingSuited(double startingFitness) {
      if (adaptiveWalkPerformanceModel == null) return true;
      if (startingFitness < AdaptivewalkingStat.Min(x => x.Item1) || startingFitness > AdaptivewalkingStat.Max(x => x.Item1))
        return true;
      return Random.NextDouble() < ProbabilityAccept2dModel(startingFitness, adaptiveWalkPerformanceModel);
    }

    public IConfidenceRegressionSolution GetSolution(IConfidenceRegressionModel model, List<Tuple<double, double>> data) {
      return new ConfidenceRegressionSolution(model, PrepareRegression(data));
    }
    public IConfidenceRegressionSolution GetSolution(IConfidenceRegressionModel model, List<Tuple<double, double, double>> data) {
      return new ConfidenceRegressionSolution(model, PrepareRegression(data));
    }

    protected RegressionProblemData PrepareRegression(List<Tuple<double, double>> sample) {
      var inCol = new List<double>();
      var outCol = new List<double>();
      foreach (var next in sample.Shuffle(Random)) {
        inCol.Add(next.Item1);
        outCol.Add(next.Item2);
      }
      var ds = new Dataset(new[] { "in", "out" }, new[] { inCol, outCol });
      var regPrb = new RegressionProblemData(ds, new[] { "in" }, "out") {
        TrainingPartition = { Start = 0, End = Math.Min(50, sample.Count) },
        TestPartition = { Start = Math.Min(50, sample.Count), End = sample.Count }
      };
      return regPrb;
    }

    protected RegressionProblemData PrepareRegression(List<Tuple<double, double, double>> sample) {
      var in1Col = new List<double>();
      var in2Col = new List<double>();
      var outCol = new List<double>();
      foreach (var next in sample.Shuffle(Random)) {
        in1Col.Add(next.Item1);
        in2Col.Add(next.Item2);
        outCol.Add(next.Item3);
      }
      var ds = new Dataset(new[] { "in1", "in2", "out" }, new[] { in1Col, in2Col, outCol });
      var regPrb = new RegressionProblemData(ds, new[] { "in1", "in2" }, "out") {
        TrainingPartition = { Start = 0, End = Math.Min(50, sample.Count) },
        TestPartition = { Start = Math.Min(50, sample.Count), End = sample.Count }
      };
      return regPrb;
    }

    protected static IConfidenceRegressionSolution RunRegression(RegressionProblemData trainingData, IConfidenceRegressionModel baseLineModel = null) {
      var baseline = baseLineModel != null ? new ConfidenceRegressionSolution(baseLineModel, trainingData) : null;
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
      if (baseline == null) return solution;
      if (trainingData.Dataset.Rows < 60)
        return solution.TrainingMeanAbsoluteError < baseline.TrainingMeanAbsoluteError ? solution : baseline;
      return solution.TestMeanAbsoluteError < baseline.TestMeanAbsoluteError ? solution : baseline;
    }

    protected static void ExecuteAlgorithm(IAlgorithm algorithm) {
      using (var evt = new AutoResetEvent(false)) {
        EventHandler exeStateChanged = (o, args) => {
          if (algorithm.ExecutionState == ExecutionState.Paused || algorithm.ExecutionState == ExecutionState.Stopped)
            evt.Set();
        };
        algorithm.ExecutionStateChanged += exeStateChanged;
        algorithm.Prepare(true);
        algorithm.Start();
        evt.WaitOne();
        algorithm.ExecutionStateChanged -= exeStateChanged;
      }
    }

    private double ProbabilityAccept2dModel(double a, IConfidenceRegressionModel model) {
      var ds = new Dataset(new[] { "in", "out" }, new[] { new List<double> { a }, new List<double> { double.NaN } });
      var mean = model.GetEstimatedValues(ds, new[] { 0 }).Single();
      var sdev = Math.Sqrt(model.GetEstimatedVariances(ds, new[] { 0 }).Single());

      var goal = Problem.Maximization ? Population.Min(x => x.Fitness) : Population.Max(x => x.Fitness);
      var z = (goal - mean) / sdev;
      return Problem.Maximization ? 1.0 - Phi(z) /* P(X >= z) */ : Phi(z); // P(X <= z)
    }

    private double ProbabilityAccept3dModel(double a, double b, IConfidenceRegressionModel model) {
      var ds = new Dataset(new[] { "in1", "in2", "out" }, new[] { new List<double> { a }, new List<double> { b }, new List<double> { double.NaN } });
      var mean = model.GetEstimatedValues(ds, new[] { 0 }).Single();
      var sdev = Math.Sqrt(model.GetEstimatedVariances(ds, new[] { 0 }).Single());

      var goal = Problem.Maximization ? Population.Min(x => x.Fitness) : Population.Max(x => x.Fitness);
      var z = (goal - mean) / sdev;
      return Problem.Maximization ? 1.0 - Phi(z) /* P(X >= z) */ : Phi(z); // P(X <= z)
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsBetter(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b) {
      return IsBetter(a.Fitness, b.Fitness);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsBetter(double a, double b) {
      return double.IsNaN(b) && !double.IsNaN(a)
        || Problem.Maximization && a > b
        || !Problem.Maximization && a < b;
    }

    public void AddBreedingResult(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, ISingleObjectiveSolutionScope<TSolution> child) {
      if (IsBetter(a, b))
        BreedingStat.Add(Tuple.Create(a.Fitness, b.Fitness, child.Fitness));
      else BreedingStat.Add(Tuple.Create(b.Fitness, a.Fitness, child.Fitness));
    }

    public void AddRelinkingResult(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, ISingleObjectiveSolutionScope<TSolution> child) {
      if (IsBetter(a, b))
        RelinkingStat.Add(Tuple.Create(a.Fitness, b.Fitness, child.Fitness));
      else RelinkingStat.Add(Tuple.Create(b.Fitness, a.Fitness, child.Fitness));
    }

    public void AddDelinkingResult(ISingleObjectiveSolutionScope<TSolution> a, ISingleObjectiveSolutionScope<TSolution> b, ISingleObjectiveSolutionScope<TSolution> child) {
      if (IsBetter(a, b))
        DelinkingStat.Add(Tuple.Create(a.Fitness, b.Fitness, child.Fitness));
      else DelinkingStat.Add(Tuple.Create(b.Fitness, a.Fitness, child.Fitness));
    }

    public void AddSamplingResult(ISingleObjectiveSolutionScope<TSolution> sample) {
      SamplingStat.Add(Tuple.Create(Population.Average(x => x.Fitness), sample.Fitness));
    }

    public void AddHillclimbingResult(ISingleObjectiveSolutionScope<TSolution> input, ISingleObjectiveSolutionScope<TSolution> outcome) {
      HillclimbingStat.Add(Tuple.Create(input.Fitness, outcome.Fitness));
    }

    public void AddTabuwalkingResult(ISingleObjectiveSolutionScope<TSolution> input, ISingleObjectiveSolutionScope<TSolution> outcome) {
      AdaptivewalkingStat.Add(Tuple.Create(input.Fitness, outcome.Fitness));
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
