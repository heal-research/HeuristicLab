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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.Binary;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  internal sealed class EvaluationTracker : BinaryProblem {
    private readonly BinaryProblem problem;

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

    public BinaryVector BestSolution {
      get;
      private set;
    }
    #endregion

    [StorableConstructor]
    private EvaluationTracker(bool deserializing) : base(deserializing) { }
    private EvaluationTracker(EvaluationTracker original, Cloner cloner)
      : base(original, cloner) {
      problem = cloner.Clone(original.problem);
      maxEvaluations = original.maxEvaluations;
      BestQuality = original.BestQuality;
      Evaluations = original.Evaluations;
      BestFoundOnEvaluation = original.BestFoundOnEvaluation;
      BestSolution = cloner.Clone(BestSolution);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvaluationTracker(this, cloner);
    }
    public EvaluationTracker(BinaryProblem problem, int maxEvaluations) {
      this.problem = problem;
      this.maxEvaluations = maxEvaluations;
      BestSolution = new BinaryVector(Length);
      BestQuality = double.NaN;
      Evaluations = 0;
      BestFoundOnEvaluation = 0;

      if (Parameters.ContainsKey("Maximization")) Parameters.Remove("Maximization");
      Parameters.Add(new FixedValueParameter<BoolValue>("Maximization", "Set to false if the problem should be minimized.", (BoolValue)new BoolValue(Maximization).AsReadOnly()) { Hidden = true });
    }

    public override double Evaluate(BinaryVector vector, IRandom random) {
      if (Evaluations >= maxEvaluations) throw new OperationCanceledException("Maximum Evaluation Limit Reached");
      Evaluations++;
      double fitness = problem.Evaluate(vector, random);
      if (double.IsNaN(BestQuality) || problem.IsBetter(fitness, BestQuality)) {
        BestQuality = fitness;
        BestSolution = (BinaryVector)vector.Clone();
        BestFoundOnEvaluation = Evaluations;
      }
      return fitness;
    }

    public override int Length {
      get { return problem.Length; }
      set { problem.Length = value; }
    }

    public override bool Maximization {
      get {
        if (problem == null) return false;
        return problem.Maximization;
      }
    }

    public bool IsBetter(double quality, double bestQuality) {
      return problem.IsBetter(quality, bestQuality);
    }

  }
}
