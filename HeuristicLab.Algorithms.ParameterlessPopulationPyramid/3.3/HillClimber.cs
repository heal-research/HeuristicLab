#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;

using HeuristicLab.Parameters;
using HeuristicLab.Random;


namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [Item("Hill Climber (HC)", "Binary Hill Climber.")]
  [StorableType("BA349010-6295-406E-8989-B271FB96ED86")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 150)]
  public class HillClimber : BasicAlgorithm {
    [Storable]
    private IRandom random;

    [Storable] public IFixedValueParameter<IntValue> MaximumIterationsParameter { get; private set; }

    [Storable] public IResult<DoubleValue> BestQualityResult { get; private set; }
    [Storable] public IResult<IntValue> IterationsResult { get; private set; }

    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector>); }
    }
    public new ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector> Problem {
      get { return (ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector>)base.Problem; }
      set { base.Problem = (IProblem)value; }
    }

    public override bool SupportsPause { get { return false; } }

    public int MaximumIterations {
      get { return MaximumIterationsParameter.Value.Value; }
      set { MaximumIterationsParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected HillClimber(StorableConstructorFlag _) : base(_) { }
    protected HillClimber(HillClimber original, Cloner cloner)
      : base(original, cloner) {
      MaximumIterationsParameter = cloner.Clone(original.MaximumIterationsParameter);
      BestQualityResult = cloner.Clone(original.BestQualityResult);
      IterationsResult = cloner.Clone(original.IterationsResult);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HillClimber(this, cloner);
    }

    public HillClimber()
      : base() {
      random = new MersenneTwister();
      Parameters.Add(MaximumIterationsParameter = new FixedValueParameter<IntValue>("Maximum Iterations", "", new IntValue(100)));

      Results.Add(BestQualityResult = new Result<DoubleValue>("Best Quality"));
      Results.Add(IterationsResult = new Result<IntValue>("Iterations"));
    }



    protected override void Run(CancellationToken cancellationToken) {
      IterationsResult.Value = new IntValue();
      BestQualityResult.Value = new DoubleValue(double.NaN);

      while (IterationsResult.Value.Value < MaximumIterations) {
        cancellationToken.ThrowIfCancellationRequested();

        var solution = new BinaryVector(Problem.Encoding.Length);
        for (int i = 0; i < solution.Length; i++) {
          solution[i] = random.Next(2) == 1;
        }

        var evaluationResult = Problem.Evaluate(solution, random);
        var fitness = evaluationResult.Quality;

        fitness = ImproveToLocalOptimum(Problem, solution, fitness, random);
        var bestSoFar = BestQualityResult.Value.Value;
        if (double.IsNaN(bestSoFar) || Problem.IsBetter(fitness, bestSoFar)) {
          BestQualityResult.Value.Value = fitness;
        }

        IterationsResult.Value.Value++;
      }
    }
    // In the GECCO paper, Section 2.1
    public static double ImproveToLocalOptimum(ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector> problem, BinaryVector solution, double fitness, IRandom rand) {
      var tried = new HashSet<int>();
      do {
        var options = Enumerable.Range(0, solution.Length).Shuffle(rand);
        foreach (var option in options) {
          if (tried.Contains(option)) continue;
          solution[option] = !solution[option];
          var newEvaluationResult = problem.Evaluate(solution, rand);
          double newFitness = newEvaluationResult.Quality;
          if (problem.IsBetter(newFitness, fitness)) {
            fitness = newFitness;
            tried.Clear();
          } else {
            solution[option] = !solution[option];
          }
          tried.Add(option);
        }
      } while (tried.Count != solution.Length);
      return fitness;
    }
  }
}
