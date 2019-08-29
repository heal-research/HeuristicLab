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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
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

    private const string IterationsParameterName = "Iterations";
    private const string BestQualityResultName = "Best quality";
    private const string IterationsResultName = "Iterations";

    public override Type ProblemType {
      get { return typeof(ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector>); }
    }
    public new ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector> Problem {
      get { return (ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector>)base.Problem; }
      set { base.Problem = (IProblem)value; }
    }

    public override bool SupportsPause { get { return false; } }

    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }

    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }

    #region ResultsProperties
    private double ResultsBestQuality {
      get { return ((DoubleValue)Results[BestQualityResultName].Value).Value; }
      set { ((DoubleValue)Results[BestQualityResultName].Value).Value = value; }
    }
    private int ResultsIterations {
      get { return ((IntValue)Results[IterationsResultName].Value).Value; }
      set { ((IntValue)Results[IterationsResultName].Value).Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected HillClimber(StorableConstructorFlag _) : base(_) { }
    protected HillClimber(HillClimber original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HillClimber(this, cloner);
    }

    public HillClimber()
      : base() {
      random = new MersenneTwister();
      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "", new IntValue(100)));
    }


    protected override void Initialize(CancellationToken cancellationToken) {
      Results.Add(new Result(BestQualityResultName, new DoubleValue(double.NaN)));
      Results.Add(new Result(IterationsResultName, new IntValue(0)));
      base.Initialize(cancellationToken);
    }
    protected override void Run(CancellationToken cancellationToken) {
      while (ResultsIterations < Iterations) {
        cancellationToken.ThrowIfCancellationRequested();

        var solution = new BinaryVector(Problem.Encoding.Length);
        for (int i = 0; i < solution.Length; i++) {
          solution[i] = random.Next(2) == 1;
        }

        var fitness = Problem.Evaluate(solution, random);

        fitness = ImproveToLocalOptimum(Problem, solution, fitness, random);
        if (double.IsNaN(ResultsBestQuality) || Problem.IsBetter(fitness, ResultsBestQuality)) {
          ResultsBestQuality = fitness;
        }

        ResultsIterations++;
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
          double newFitness = problem.Evaluate(solution, rand);
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
