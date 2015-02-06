#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.BinaryVector;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  public class EvaluationTracker : IBinaryVectorProblem {
    private readonly IBinaryVectorProblem problem;

    private int maxEvaluations;

    #region Properties
    public double BestQuality {
      get;
      private set;
    }

    public int Evaluations {
      get;
      private set;
    }

    public int BestFoundOnEvaluation {
      get;
      private set;
    }

    public bool[] BestSolution {
      get;
      private set;
    }
    #endregion

    public EvaluationTracker(IBinaryVectorProblem problem, int maxEvaluations) {
      this.problem = problem;
      this.maxEvaluations = maxEvaluations;
      BestSolution = new bool[0];
      BestQuality = double.NaN;
      Evaluations = 0;
      BestFoundOnEvaluation = 0;
    }

    public double Evaluate(bool[] individual) {
      if (Evaluations >= maxEvaluations) throw new OperationCanceledException("Maximum Evaluation Limit Reached");
      Evaluations++;
      double fitness = problem.Evaluate(individual);
      if (double.IsNaN(BestQuality) || problem.IsBetter(fitness, BestQuality)) {
        BestQuality = fitness;
        BestSolution = (bool[])individual.Clone();
        BestFoundOnEvaluation = Evaluations;
      }
      return fitness;
    }

    #region ForwardedInteraface
    public int Length {
      get { return problem.Length; }
    }
    public bool Maximization {
      get { return problem.Maximization; }
    }
    public bool IsBetter(double quality, double bestQuality) {
      return problem.IsBetter(quality, bestQuality);
    }
    #endregion
  }
}
