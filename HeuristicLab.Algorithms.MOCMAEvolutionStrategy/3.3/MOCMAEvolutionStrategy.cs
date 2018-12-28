#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.TestFunctions.MultiObjective;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MOCMAEvolutionStrategy {
  [Item("Multi-Objective CMA Evolution Strategy (MOCMAES)", "A multi objective evolution strategy based on covariance matrix adaptation. Code is based on 'Covariance Matrix Adaptation for Multi - objective Optimization' by Igel, Hansen and Roth")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 210)]
  [StorableType("C10264E3-E4C6-4735-8E94-0DC116E8908D")]
  public class MOCMAEvolutionStrategy : BasicAlgorithm {
    public override Type ProblemType {
      get { return typeof(MultiObjectiveBasicProblem<RealVectorEncoding>); }
    }
    public new MultiObjectiveBasicProblem<RealVectorEncoding> Problem {
      get { return (MultiObjectiveBasicProblem<RealVectorEncoding>)base.Problem; }
      set { base.Problem = value; }
    }
    public override bool SupportsPause {
      get { return true; }
    }

    #region Storable fields
    [Storable]
    private IRandom random = new MersenneTwister();
    [Storable]
    private NormalDistributedRandom gauss;
    [Storable]
    private Individual[] solutions;
    [Storable]
    private double stepSizeLearningRate; //=cp learning rate in [0,1]
    [Storable]
    private double stepSizeDampeningFactor; //d
    [Storable]
    private double targetSuccessProbability;// p^target_succ
    [Storable]
    private double evolutionPathLearningRate;//cc
    [Storable]
    private double covarianceMatrixLearningRate;//ccov
    [Storable]
    private double covarianceMatrixUnlearningRate;
    [Storable]
    private double successThreshold; //ptresh

    #endregion

    #region ParameterNames
    private const string MaximumRuntimeName = "Maximum Runtime";
    private const string SeedName = "Seed";
    private const string SetSeedRandomlyName = "SetSeedRandomly";
    private const string PopulationSizeName = "PopulationSize";
    private const string MaximumGenerationsName = "MaximumGenerations";
    private const string MaximumEvaluatedSolutionsName = "MaximumEvaluatedSolutions";
    private const string InitialSigmaName = "InitialSigma";
    private const string IndicatorName = "Indicator";

    private const string EvaluationsResultName = "Evaluations";
    private const string IterationsResultName = "Generations";
    private const string TimetableResultName = "Timetable";
    private const string HypervolumeResultName = "Hypervolume";
    private const string GenerationalDistanceResultName = "Generational Distance";
    private const string InvertedGenerationalDistanceResultName = "Inverted Generational Distance";
    private const string CrowdingResultName = "Crowding";
    private const string SpacingResultName = "Spacing";
    private const string CurrentFrontResultName = "Pareto Front";
    private const string BestHypervolumeResultName = "Best Hypervolume";
    private const string BestKnownHypervolumeResultName = "Best known hypervolume";
    private const string DifferenceToBestKnownHypervolumeResultName = "Absolute Distance to BestKnownHypervolume";
    private const string ScatterPlotResultName = "ScatterPlot";
    #endregion

    #region ParameterProperties
    public IFixedValueParameter<IntValue> MaximumRuntimeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumRuntimeName]; }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedName]; }
    }
    public FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyName]; }
    }
    public IFixedValueParameter<IntValue> PopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[PopulationSizeName]; }
    }
    public IFixedValueParameter<IntValue> MaximumGenerationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumGenerationsName]; }
    }
    public IFixedValueParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumEvaluatedSolutionsName]; }
    }
    public IValueParameter<DoubleArray> InitialSigmaParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[InitialSigmaName]; }
    }
    public IConstrainedValueParameter<IIndicator> IndicatorParameter {
      get { return (IConstrainedValueParameter<IIndicator>)Parameters[IndicatorName]; }
    }
    #endregion

    #region Properties
    public int MaximumRuntime {
      get { return MaximumRuntimeParameter.Value.Value; }
      set { MaximumRuntimeParameter.Value.Value = value; }
    }
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
    public int MaximumGenerations {
      get { return MaximumGenerationsParameter.Value.Value; }
      set { MaximumGenerationsParameter.Value.Value = value; }
    }
    public int MaximumEvaluatedSolutions {
      get { return MaximumEvaluatedSolutionsParameter.Value.Value; }
      set { MaximumEvaluatedSolutionsParameter.Value.Value = value; }
    }
    public DoubleArray InitialSigma {
      get { return InitialSigmaParameter.Value; }
      set { InitialSigmaParameter.Value = value; }
    }
    public IIndicator Indicator {
      get { return IndicatorParameter.Value; }
      set { IndicatorParameter.Value = value; }
    }

    public double StepSizeLearningRate { get { return stepSizeLearningRate; } }
    public double StepSizeDampeningFactor { get { return stepSizeDampeningFactor; } }
    public double TargetSuccessProbability { get { return targetSuccessProbability; } }
    public double EvolutionPathLearningRate { get { return evolutionPathLearningRate; } }
    public double CovarianceMatrixLearningRate { get { return covarianceMatrixLearningRate; } }
    public double CovarianceMatrixUnlearningRate { get { return covarianceMatrixUnlearningRate; } }
    public double SuccessThreshold { get { return successThreshold; } }
    #endregion

    #region ResultsProperties
    private int ResultsEvaluations {
      get { return ((IntValue)Results[EvaluationsResultName].Value).Value; }
      set { ((IntValue)Results[EvaluationsResultName].Value).Value = value; }
    }
    private int ResultsIterations {
      get { return ((IntValue)Results[IterationsResultName].Value).Value; }
      set { ((IntValue)Results[IterationsResultName].Value).Value = value; }
    }
    #region Datatable
    private DataTable ResultsQualities {
      get { return (DataTable)Results[TimetableResultName].Value; }
    }
    private DataRow ResultsBestHypervolumeDataLine {
      get { return ResultsQualities.Rows[BestHypervolumeResultName]; }
    }
    private DataRow ResultsHypervolumeDataLine {
      get { return ResultsQualities.Rows[HypervolumeResultName]; }
    }
    private DataRow ResultsGenerationalDistanceDataLine {
      get { return ResultsQualities.Rows[GenerationalDistanceResultName]; }
    }
    private DataRow ResultsInvertedGenerationalDistanceDataLine {
      get { return ResultsQualities.Rows[InvertedGenerationalDistanceResultName]; }
    }
    private DataRow ResultsCrowdingDataLine {
      get { return ResultsQualities.Rows[CrowdingResultName]; }
    }
    private DataRow ResultsSpacingDataLine {
      get { return ResultsQualities.Rows[SpacingResultName]; }
    }
    private DataRow ResultsHypervolumeDifferenceDataLine {
      get { return ResultsQualities.Rows[DifferenceToBestKnownHypervolumeResultName]; }
    }
    #endregion
    //QualityIndicators
    private double ResultsHypervolume {
      get { return ((DoubleValue)Results[HypervolumeResultName].Value).Value; }
      set { ((DoubleValue)Results[HypervolumeResultName].Value).Value = value; }
    }
    private double ResultsGenerationalDistance {
      get { return ((DoubleValue)Results[GenerationalDistanceResultName].Value).Value; }
      set { ((DoubleValue)Results[GenerationalDistanceResultName].Value).Value = value; }
    }
    private double ResultsInvertedGenerationalDistance {
      get { return ((DoubleValue)Results[InvertedGenerationalDistanceResultName].Value).Value; }
      set { ((DoubleValue)Results[InvertedGenerationalDistanceResultName].Value).Value = value; }
    }
    private double ResultsCrowding {
      get { return ((DoubleValue)Results[CrowdingResultName].Value).Value; }
      set { ((DoubleValue)Results[CrowdingResultName].Value).Value = value; }
    }
    private double ResultsSpacing {
      get { return ((DoubleValue)Results[SpacingResultName].Value).Value; }
      set { ((DoubleValue)Results[SpacingResultName].Value).Value = value; }
    }
    private double ResultsBestHypervolume {
      get { return ((DoubleValue)Results[BestHypervolumeResultName].Value).Value; }
      set { ((DoubleValue)Results[BestHypervolumeResultName].Value).Value = value; }
    }
    private double ResultsBestKnownHypervolume {
      get { return ((DoubleValue)Results[BestKnownHypervolumeResultName].Value).Value; }
      set { ((DoubleValue)Results[BestKnownHypervolumeResultName].Value).Value = value; }
    }
    private double ResultsDifferenceBestKnownHypervolume {
      get { return ((DoubleValue)Results[DifferenceToBestKnownHypervolumeResultName].Value).Value; }
      set { ((DoubleValue)Results[DifferenceToBestKnownHypervolumeResultName].Value).Value = value; }

    }
    //Solutions
    private DoubleMatrix ResultsSolutions {
      get { return (DoubleMatrix)Results[CurrentFrontResultName].Value; }
      set { Results[CurrentFrontResultName].Value = value; }
    }
    private ParetoFrontScatterPlot ResultsScatterPlot {
      get { return (ParetoFrontScatterPlot)Results[ScatterPlotResultName].Value; }
      set { Results[ScatterPlotResultName].Value = value; }
    }
    #endregion

    #region Constructors
    public MOCMAEvolutionStrategy() {
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumRuntimeName, "The maximum runtime in seconds after which the algorithm stops. Use -1 to specify no limit for the runtime", new IntValue(3600)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(PopulationSizeName, "λ (lambda) - the size of the offspring population.", new IntValue(20)));
      Parameters.Add(new ValueParameter<DoubleArray>(InitialSigmaName, "The initial sigma can be a single value or a value for each dimension. All values need to be > 0.", new DoubleArray(new[] { 0.5 })));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumGenerationsName, "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumEvaluatedSolutionsName, "The maximum number of evaluated solutions that should be computed.", new IntValue(int.MaxValue)));
      var set = new ItemSet<IIndicator> { new HypervolumeIndicator(), new CrowdingIndicator(), new MinimalDistanceIndicator() };
      Parameters.Add(new ConstrainedValueParameter<IIndicator>(IndicatorName, "The selection mechanism on non-dominated solutions", set, set.First()));
    }

    [StorableConstructor]
    protected MOCMAEvolutionStrategy(StorableConstructorFlag _) : base(_) { }

    protected MOCMAEvolutionStrategy(MOCMAEvolutionStrategy original, Cloner cloner) : base(original, cloner) {
      random = cloner.Clone(original.random);
      gauss = cloner.Clone(original.gauss);
      solutions = original.solutions != null ? original.solutions.Select(cloner.Clone).ToArray() : null;
      stepSizeLearningRate = original.stepSizeLearningRate;
      stepSizeDampeningFactor = original.stepSizeDampeningFactor;
      targetSuccessProbability = original.targetSuccessProbability;
      evolutionPathLearningRate = original.evolutionPathLearningRate;
      covarianceMatrixLearningRate = original.covarianceMatrixLearningRate;
      covarianceMatrixUnlearningRate = original.covarianceMatrixUnlearningRate;
      successThreshold = original.successThreshold;
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new MOCMAEvolutionStrategy(this, cloner); }
    #endregion

    #region Initialization
    protected override void Initialize(CancellationToken cancellationToken) {
      if (SetSeedRandomly) Seed = RandomSeedGenerator.GetSeed();
      random.Reset(Seed);
      gauss = new NormalDistributedRandom(random, 0, 1);

      InitResults();
      InitStrategy();
      InitSolutions();
      Analyze();

      ResultsIterations = 1;
    }
    private Individual InitializeIndividual(RealVector x) {
      var zeros = new RealVector(x.Length);
      var c = new double[x.Length, x.Length];
      var sigma = InitialSigma.Max();
      for (var i = 0; i < x.Length; i++) {
        var d = InitialSigma[i % InitialSigma.Length] / sigma;
        c[i, i] = d * d;
      }
      return new Individual(x, targetSuccessProbability, sigma, zeros, c, this);
    }
    private void InitSolutions() {
      solutions = new Individual[PopulationSize];
      for (var i = 0; i < PopulationSize; i++) {
        var x = new RealVector(Problem.Encoding.Length); // Uniform distibution in all dimensions assumed.
        var bounds = Problem.Encoding.Bounds;
        for (var j = 0; j < Problem.Encoding.Length; j++) {
          var dim = j % bounds.Rows;
          x[j] = random.NextDouble() * (bounds[dim, 1] - bounds[dim, 0]) + bounds[dim, 0];
        }
        solutions[i] = InitializeIndividual(x);
        PenalizeEvaluate(solutions[i]);
      }
      ResultsEvaluations += solutions.Length;
    }
    private void InitStrategy() {
      const int lambda = 1;
      double n = Problem.Encoding.Length;
      targetSuccessProbability = 1.0 / (5.0 + Math.Sqrt(lambda) / 2.0);
      stepSizeDampeningFactor = 1.0 + n / (2.0 * lambda);
      stepSizeLearningRate = targetSuccessProbability * lambda / (2.0 + targetSuccessProbability * lambda);
      evolutionPathLearningRate = 2.0 / (n + 2.0);
      covarianceMatrixLearningRate = 2.0 / (n * n + 6.0);
      covarianceMatrixUnlearningRate = 0.4 / (Math.Pow(n, 1.6) + 1);
      successThreshold = 0.44;
    }
    private void InitResults() {
      Results.Add(new Result(IterationsResultName, "The number of gererations evaluated", new IntValue(0)));
      Results.Add(new Result(EvaluationsResultName, "The number of function evaltions performed", new IntValue(0)));
      Results.Add(new Result(HypervolumeResultName, "The hypervolume of the current front considering the Referencepoint defined in the Problem", new DoubleValue(0.0)));
      Results.Add(new Result(BestHypervolumeResultName, "The best hypervolume of the current run considering the Referencepoint defined in the Problem", new DoubleValue(0.0)));
      Results.Add(new Result(BestKnownHypervolumeResultName, "The best knwon hypervolume considering the Referencepoint defined in the Problem", new DoubleValue(double.NaN)));
      Results.Add(new Result(DifferenceToBestKnownHypervolumeResultName, "The difference between the current and the best known hypervolume", new DoubleValue(double.NaN)));
      Results.Add(new Result(GenerationalDistanceResultName, "The generational distance to an optimal pareto front defined in the Problem", new DoubleValue(double.NaN)));
      Results.Add(new Result(InvertedGenerationalDistanceResultName, "The inverted generational distance to an optimal pareto front defined in the Problem", new DoubleValue(double.NaN)));
      Results.Add(new Result(CrowdingResultName, "The average crowding value for the current front (excluding infinities)", new DoubleValue(0.0)));
      Results.Add(new Result(SpacingResultName, "The spacing for the current front (excluding infinities)", new DoubleValue(0.0)));

      var table = new DataTable("QualityIndicators");
      table.Rows.Add(new DataRow(BestHypervolumeResultName));
      table.Rows.Add(new DataRow(HypervolumeResultName));
      table.Rows.Add(new DataRow(CrowdingResultName));
      table.Rows.Add(new DataRow(GenerationalDistanceResultName));
      table.Rows.Add(new DataRow(InvertedGenerationalDistanceResultName));
      table.Rows.Add(new DataRow(DifferenceToBestKnownHypervolumeResultName));
      table.Rows.Add(new DataRow(SpacingResultName));
      Results.Add(new Result(TimetableResultName, "Different quality meassures in a timeseries", table));
      Results.Add(new Result(CurrentFrontResultName, "The current front", new DoubleMatrix()));
      Results.Add(new Result(ScatterPlotResultName, "A scatterplot displaying the evaluated solutions and (if available) the analytically optimal front", new ParetoFrontScatterPlot()));

      var problem = Problem as MultiObjectiveTestFunctionProblem;
      if (problem == null) return;
      if (problem.BestKnownFront != null) {
        ResultsBestKnownHypervolume = Hypervolume.Calculate(problem.BestKnownFront.ToJaggedArray(), problem.TestFunction.ReferencePoint(problem.Objectives), Problem.Maximization);
        ResultsDifferenceBestKnownHypervolume = ResultsBestKnownHypervolume;
      }
      ResultsScatterPlot = new ParetoFrontScatterPlot(new double[0][], new double[0][], problem.BestKnownFront.ToJaggedArray(), problem.Objectives, problem.ProblemSize);
    }
    #endregion

    #region Mainloop
    protected override void Run(CancellationToken cancellationToken) {
      while (ResultsIterations < MaximumGenerations && ResultsEvaluations < MaximumEvaluatedSolutions) {
        try {
          Iterate();
          ResultsIterations++;
          cancellationToken.ThrowIfCancellationRequested();
        } finally {
          Analyze();
        }
      }
    }
    private void Iterate() {
      var offspring = solutions.Select(i => {
        var o = new Individual(i);
        o.Mutate(gauss);
        PenalizeEvaluate(o);
        return o;
      });
      ResultsEvaluations += solutions.Length;
      var parents = solutions.Concat(offspring).ToArray();
      SelectParents(parents, solutions.Length);
      UpdatePopulation(parents);
    }
    protected override void OnExecutionTimeChanged() {
      base.OnExecutionTimeChanged();
      if (CancellationTokenSource == null) return;
      if (MaximumRuntime == -1) return;
      if (ExecutionTime.TotalSeconds > MaximumRuntime) CancellationTokenSource.Cancel();
    }
    #endregion

    #region Evaluation
    private void PenalizeEvaluate(Individual individual) {
      if (IsFeasable(individual.Mean)) {
        individual.Fitness = Evaluate(individual.Mean);
        individual.PenalizedFitness = individual.Fitness;
      } else {
        var t = ClosestFeasible(individual.Mean);
        individual.Fitness = Evaluate(t);
        individual.PenalizedFitness = Penalize(individual.Mean, t, individual.Fitness);
      }
    }
    private double[] Evaluate(RealVector x) {
      var res = Problem.Evaluate(new SingleEncodingIndividual(Problem.Encoding, new Scope { Variables = { new Variable(Problem.Encoding.Name, x) } }), random);
      return res;
    }
    private double[] Penalize(RealVector x, RealVector t, IEnumerable<double> fitness) {
      var penalty = x.Zip(t, (a, b) => (a - b) * (a - b)).Sum() * 1E-6;
      return fitness.Select((v, i) => Problem.Maximization[i] ? v - penalty : v + penalty).ToArray();
    }
    private RealVector ClosestFeasible(RealVector x) {
      var bounds = Problem.Encoding.Bounds;
      var r = new RealVector(x.Length);
      for (var i = 0; i < x.Length; i++) {
        var dim = i % bounds.Rows;
        r[i] = Math.Min(Math.Max(bounds[dim, 0], x[i]), bounds[dim, 1]);
      }
      return r;
    }
    private bool IsFeasable(RealVector offspring) {
      var bounds = Problem.Encoding.Bounds;
      for (var i = 0; i < offspring.Length; i++) {
        var dim = i % bounds.Rows;
        if (bounds[dim, 0] > offspring[i] || offspring[i] > bounds[dim, 1]) return false;
      }
      return true;
    }
    #endregion

    private void SelectParents(IReadOnlyList<Individual> parents, int length) {
      //perform a nondominated sort to assign the rank to every element
      int[] ranks;
      var fronts = DominationCalculator<Individual>.CalculateAllParetoFronts(parents.ToArray(), parents.Select(i => i.PenalizedFitness).ToArray(), Problem.Maximization, out ranks);

      //deselect the highest rank fronts until we would end up with less or equal mu elements
      var rank = fronts.Count - 1;
      var popSize = parents.Count;
      while (popSize - fronts[rank].Count >= length) {
        var front = fronts[rank];
        foreach (var i in front) i.Item1.Selected = false;
        popSize -= front.Count;
        rank--;
      }

      //now use the indicator to deselect the approximatingly worst elements of the last selected front
      var front1 = fronts[rank].OrderBy(x => x.Item1.PenalizedFitness[0]).ToList();
      for (; popSize > length; popSize--) {
        var lc = Indicator.LeastContributer(front1.Select(i => i.Item1).ToArray(), Problem);
        front1[lc].Item1.Selected = false;
        front1.Swap(lc, front1.Count - 1);
        front1.RemoveAt(front1.Count - 1);
      }
    }

    private void UpdatePopulation(IReadOnlyList<Individual> parents) {
      foreach (var p in parents.Skip(solutions.Length).Where(i => i.Selected))
        p.UpdateAsOffspring();
      for (var i = 0; i < solutions.Length; i++)
        if (parents[i].Selected)
          parents[i].UpdateAsParent(parents[i + solutions.Length].Selected);
      solutions = parents.Where(p => p.Selected).ToArray();
    }

    private void Analyze() {
      ResultsScatterPlot = new ParetoFrontScatterPlot(solutions.Select(x => x.Fitness).ToArray(), solutions.Select(x => x.Mean.ToArray()).ToArray(), ResultsScatterPlot.ParetoFront, ResultsScatterPlot.Objectives, ResultsScatterPlot.ProblemSize);
      ResultsSolutions = solutions.Select(x => x.Mean.ToArray()).ToMatrix();

      var problem = Problem as MultiObjectiveTestFunctionProblem;
      if (problem == null) return;

      var front = NonDominatedSelect.GetDominatingVectors(solutions.Select(x => x.Fitness), problem.ReferencePoint.CloneAsArray(), Problem.Maximization, true).ToArray();
      if (front.Length == 0) return;
      var bounds = problem.Bounds.CloneAsMatrix();
      ResultsCrowding = Crowding.Calculate(front, bounds);
      ResultsSpacing = Spacing.Calculate(front);
      ResultsGenerationalDistance = problem.BestKnownFront != null ? GenerationalDistance.Calculate(front, problem.BestKnownFront.ToJaggedArray(), 1) : double.NaN;
      ResultsInvertedGenerationalDistance = problem.BestKnownFront != null ? InvertedGenerationalDistance.Calculate(front, problem.BestKnownFront.ToJaggedArray(), 1) : double.NaN;
      ResultsHypervolume = Hypervolume.Calculate(front, problem.ReferencePoint.CloneAsArray(), Problem.Maximization);
      ResultsBestHypervolume = Math.Max(ResultsHypervolume, ResultsBestHypervolume);
      ResultsDifferenceBestKnownHypervolume = ResultsBestKnownHypervolume - ResultsBestHypervolume;

      ResultsBestHypervolumeDataLine.Values.Add(ResultsBestHypervolume);
      ResultsHypervolumeDataLine.Values.Add(ResultsHypervolume);
      ResultsCrowdingDataLine.Values.Add(ResultsCrowding);
      ResultsGenerationalDistanceDataLine.Values.Add(ResultsGenerationalDistance);
      ResultsInvertedGenerationalDistanceDataLine.Values.Add(ResultsInvertedGenerationalDistance);
      ResultsSpacingDataLine.Values.Add(ResultsSpacing);
      ResultsHypervolumeDifferenceDataLine.Values.Add(ResultsDifferenceBestKnownHypervolume);

      Problem.Analyze(
        solutions.Select(x => (Optimization.Individual)new SingleEncodingIndividual(Problem.Encoding, new Scope { Variables = { new Variable(Problem.Encoding.Name, x.Mean) } })).ToArray(),
        solutions.Select(x => x.Fitness).ToArray(),
        Results,
        random);
    }
  }
}
