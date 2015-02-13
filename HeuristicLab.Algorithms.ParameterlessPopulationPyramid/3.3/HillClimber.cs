#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.Binary;
using HeuristicLab.Random;


namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [Item("Hill Climber", "Binary Hill Climber.")]
  [StorableClass]
  [Creatable("Algorithms")]
  public class HillClimber : BasicAlgorithm {
    [Storable]
    private IRandom random;

    private const string IterationsParameterName = "Iterations";

    public override Type ProblemType {
      get { return typeof(BinaryProblem); }
    }
    public new BinaryProblem Problem {
      get { return (BinaryProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }

    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected HillClimber(bool deserializing) : base(deserializing) { }
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
    protected override void Run(CancellationToken cancellationToken) {
      var BestQuality = new DoubleValue(double.NaN);
      Results.Add(new Result("Best quality", BestQuality));
      for (int iteration = 0; iteration < Iterations; iteration++) {
        var solution = new BinaryVector(Problem.Length);
        for (int i = 0; i < solution.Length; i++) {
          solution[i] = random.Next(2) == 1;
        }

        var fitness = Problem.Evaluate(solution, random);

        fitness = ImproveToLocalOptimum(Problem, solution, fitness, random);
        if (double.IsNaN(BestQuality.Value) || Problem.IsBetter(fitness, BestQuality.Value)) {
          BestQuality.Value = fitness;
        }
      }
    }
    // In the GECCO paper, Section 2.1
    public static double ImproveToLocalOptimum(BinaryProblem problem, BinaryVector solution, double fitness, IRandom rand) {
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
