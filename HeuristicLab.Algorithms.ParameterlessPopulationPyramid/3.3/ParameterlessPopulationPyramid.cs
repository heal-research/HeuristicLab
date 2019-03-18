#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [Item("Parameter-less Population Pyramid (P3)", "Binary value optimization algorithm which requires no configuration. B. W. Goldman and W. F. Punch, Parameter-less Population Pyramid, GECCO, pp. 785–792, 2014")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 400)]
  public class ParameterlessPopulationPyramid : BasicAlgorithm {
    public override Type ProblemType {
      get { return typeof(SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>); }
    }
    public new SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector> Problem {
      get { return (SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>)base.Problem; }
      set { base.Problem = value; }
    }

    [Storable]
    private readonly IRandom random = new MersenneTwister();
    [Storable]
    private List<Population> pyramid = new List<Population>();
    [Storable]
    private EvaluationTracker tracker;

    // Tracks all solutions in Pyramid for quick membership checks

    private HashSet<BinaryVector> seen = new HashSet<BinaryVector>(new EnumerableBoolEqualityComparer());
    [Storable]
    private IEnumerable<BinaryVector> StorableSeen {
      get { return seen; }
      set { seen = new HashSet<BinaryVector>(value, new EnumerableBoolEqualityComparer()); }
    }

    #region ParameterNames
    private const string MaximumIterationsParameterName = "Maximum Iterations";
    private const string MaximumEvaluationsParameterName = "Maximum Evaluations";
    private const string MaximumRuntimeParameterName = "Maximum Runtime";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    #endregion

    #region ParameterProperties
    public IFixedValueParameter<IntValue> MaximumIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumIterationsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumEvaluationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumEvaluationsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumRuntimeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumRuntimeParameterName]; }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    #endregion

    #region Properties
    public int MaximumIterations {
      get { return MaximumIterationsParameter.Value.Value; }
      set { MaximumIterationsParameter.Value.Value = value; }
    }
    public int MaximumEvaluations {
      get { return MaximumEvaluationsParameter.Value.Value; }
      set { MaximumEvaluationsParameter.Value.Value = value; }
    }
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
    #endregion

    #region ResultsProperties
    private double ResultsBestQuality {
      get { return ((DoubleValue)Results["Best Quality"].Value).Value; }
      set { ((DoubleValue)Results["Best Quality"].Value).Value = value; }
    }

    private BinaryVector ResultsBestSolution {
      get { return (BinaryVector)Results["Best Solution"].Value; }
      set { Results["Best Solution"].Value = value; }
    }

    private int ResultsBestFoundOnEvaluation {
      get { return ((IntValue)Results["Evaluation Best Solution Was Found"].Value).Value; }
      set { ((IntValue)Results["Evaluation Best Solution Was Found"].Value).Value = value; }
    }

    private int ResultsEvaluations {
      get { return ((IntValue)Results["Evaluations"].Value).Value; }
      set { ((IntValue)Results["Evaluations"].Value).Value = value; }
    }
    private int ResultsIterations {
      get { return ((IntValue)Results["Iterations"].Value).Value; }
      set { ((IntValue)Results["Iterations"].Value).Value = value; }
    }

    private DataTable ResultsQualities {
      get { return ((DataTable)Results["Qualities"].Value); }
    }
    private DataRow ResultsQualitiesBest {
      get { return ResultsQualities.Rows["Best Quality"]; }
    }

    private DataRow ResultsQualitiesIteration {
      get { return ResultsQualities.Rows["Iteration Quality"]; }
    }


    private DataRow ResultsLevels {
      get { return ((DataTable)Results["Pyramid Levels"].Value).Rows["Levels"]; }
    }

    private DataRow ResultsSolutions {
      get { return ((DataTable)Results["Stored Solutions"].Value).Rows["Solutions"]; }
    }
    #endregion

    public override bool SupportsPause { get { return true; } }

    [StorableConstructor]
    protected ParameterlessPopulationPyramid(bool deserializing) : base(deserializing) { }

    protected ParameterlessPopulationPyramid(ParameterlessPopulationPyramid original, Cloner cloner)
      : base(original, cloner) {
      random = cloner.Clone(original.random);
      pyramid = original.pyramid.Select(cloner.Clone).ToList();
      tracker = cloner.Clone(original.tracker);
      seen = new HashSet<BinaryVector>(original.seen.Select(cloner.Clone), new EnumerableBoolEqualityComparer());
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParameterlessPopulationPyramid(this, cloner);
    }

    public ParameterlessPopulationPyramid() : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumIterationsParameterName, "", new IntValue(Int32.MaxValue)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumEvaluationsParameterName, "", new IntValue(Int32.MaxValue)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumRuntimeParameterName, "The maximum runtime in seconds after which the algorithm stops. Use -1 to specify no limit for the runtime", new IntValue(3600)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
    }

    protected override void OnExecutionTimeChanged() {
      base.OnExecutionTimeChanged();
      if (CancellationTokenSource == null) return;
      if (MaximumRuntime == -1) return;
      if (ExecutionTime.TotalSeconds > MaximumRuntime) CancellationTokenSource.Cancel();
    }

    private void AddIfUnique(BinaryVector solution, int level) {
      // Don't add things you have seen
      if (seen.Contains(solution)) return;
      if (level == pyramid.Count) {
        pyramid.Add(new Population(Problem.Encoding.Length, random));
      }
      var copied = (BinaryVector)solution.Clone();
      pyramid[level].Add(copied);
      seen.Add(copied);
    }

    // In the GECCO paper, Figure 1
    private double iterate() {
      // Create a random solution
      BinaryVector solution = new BinaryVector(Problem.Encoding.Length);
      for (int i = 0; i < solution.Length; i++) {
        solution[i] = random.Next(2) == 1;
      }
      double fitness = tracker.Evaluate(solution, random);
      fitness = HillClimber.ImproveToLocalOptimum(tracker, solution, fitness, random);
      AddIfUnique(solution, 0);

      for (int level = 0; level < pyramid.Count; level++) {
        var current = pyramid[level];
        double newFitness = LinkageCrossover.ImproveUsingTree(current.Tree, current.Solutions, solution, fitness, tracker, random);
        // add it to the next level if its a strict fitness improvement
        if (tracker.IsBetter(newFitness, fitness)) {
          fitness = newFitness;
          AddIfUnique(solution, level + 1);
        }
      }
      return fitness;
    }

    protected override void Initialize(CancellationToken cancellationToken) {
      // Set up the algorithm
      if (SetSeedRandomly) Seed = new System.Random().Next();
      pyramid = new List<Population>();
      seen.Clear();
      random.Reset(Seed);
      tracker = new EvaluationTracker(Problem, MaximumEvaluations);

      // Set up the results display
      Results.Add(new Result("Iterations", new IntValue(0)));
      Results.Add(new Result("Evaluations", new IntValue(0)));
      Results.Add(new Result("Best Solution", new BinaryVector(tracker.BestSolution)));
      Results.Add(new Result("Best Quality", new DoubleValue(tracker.BestQuality)));
      Results.Add(new Result("Evaluation Best Solution Was Found", new IntValue(tracker.BestFoundOnEvaluation)));
      var table = new DataTable("Qualities");
      table.Rows.Add(new DataRow("Best Quality"));
      var iterationRows = new DataRow("Iteration Quality");
      iterationRows.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.Dot;
      table.Rows.Add(iterationRows);
      Results.Add(new Result("Qualities", table));

      table = new DataTable("Pyramid Levels");
      table.Rows.Add(new DataRow("Levels"));
      Results.Add(new Result("Pyramid Levels", table));

      table = new DataTable("Stored Solutions");
      table.Rows.Add(new DataRow("Solutions"));
      Results.Add(new Result("Stored Solutions", table));

      base.Initialize(cancellationToken);
    }

    protected override void Run(CancellationToken cancellationToken) {
      // Loop until iteration limit reached or canceled.
      while (ResultsIterations < MaximumIterations) {
        double fitness = double.NaN;

        try {
          fitness = iterate();
          ResultsIterations++;
          cancellationToken.ThrowIfCancellationRequested();
        }
        finally {
          ResultsEvaluations = tracker.Evaluations;
          ResultsBestSolution = new BinaryVector(tracker.BestSolution);
          ResultsBestQuality = tracker.BestQuality;
          ResultsBestFoundOnEvaluation = tracker.BestFoundOnEvaluation;
          ResultsQualitiesBest.Values.Add(tracker.BestQuality);
          ResultsQualitiesIteration.Values.Add(fitness);
          ResultsLevels.Values.Add(pyramid.Count);
          ResultsSolutions.Values.Add(seen.Count);
        }
      }
    }
  }
}
