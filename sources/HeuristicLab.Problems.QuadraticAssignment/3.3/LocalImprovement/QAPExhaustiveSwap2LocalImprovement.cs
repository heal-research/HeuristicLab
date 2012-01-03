#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("QAPExhaustiveSwap2LocalImprovement", "Takes a solution and finds the local optimum with respect to the swap2 neighborhood by decending along the steepest gradient.")]
  [StorableClass]
  public class QAPExhaustiveSwap2LocalImprovement : SingleSuccessorOperator, ILocalImprovementOperator {

    public Type ProblemType {
      get { return typeof(QuadraticAssignmentProblem); }
    }

    [Storable]
    private QuadraticAssignmentProblem problem;
    public IProblem Problem {
      get { return problem; }
      set { problem = (QuadraticAssignmentProblem)value; }
    }

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }

    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ILookupParameter<Permutation> AssignmentParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Assignment"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public ILookupParameter<DoubleMatrix> WeightsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }

    public ILookupParameter<DoubleMatrix> DistancesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }

    [StorableConstructor]
    protected QAPExhaustiveSwap2LocalImprovement(bool deserializing) : base(deserializing) { }
    protected QAPExhaustiveSwap2LocalImprovement(QAPExhaustiveSwap2LocalImprovement original, Cloner cloner)
      : base(original, cloner) {
      this.problem = cloner.Clone(original.problem);
    }
    public QAPExhaustiveSwap2LocalImprovement()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum amount of iterations that should be performed (note that this operator will abort earlier when a local optimum is reached.", new IntValue(10000)));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The amount of evaluated solutions (here a move is counted only as 4/n evaluated solutions with n being the length of the permutation)."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The collection where to store results."));
      Parameters.Add(new LookupParameter<Permutation>("Assignment", "The permutation that is to be locally optimized."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the assignment."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem should be maximized or minimized."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The weights matrix."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distances matrix."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPExhaustiveSwap2LocalImprovement(this, cloner);
    }

    public override IOperation Apply() {
      int maxIterations = MaximumIterationsParameter.ActualValue.Value;
      Permutation assignment = AssignmentParameter.ActualValue;
      bool maximization = MaximizationParameter.ActualValue.Value;
      DoubleMatrix weights = WeightsParameter.ActualValue;
      DoubleMatrix distances = DistancesParameter.ActualValue;

      double evaluatedSolutions = 0.0;
      double evalSolPerMove = 4.0 / assignment.Length;

      for (int i = 0; i < maxIterations; i++) {
        Swap2Move bestMove = null;
        double bestQuality = 0; // we have to make an improvement, so 0 is the baseline
        foreach (Swap2Move move in ExhaustiveSwap2MoveGenerator.Generate(assignment)) {
          double moveQuality = QAPSwap2MoveEvaluator.Apply(assignment, move, weights, distances);
          evaluatedSolutions += evalSolPerMove;
          if (maximization && moveQuality > bestQuality
            || !maximization && moveQuality < bestQuality) {
            bestQuality = moveQuality;
            bestMove = move;
          }
        }
        if (bestMove == null) break;
        Swap2Manipulator.Apply(assignment, bestMove.Index1, bestMove.Index2);
        QualityParameter.ActualValue.Value += bestQuality;
      }
      EvaluatedSolutionsParameter.ActualValue.Value += (int)Math.Ceiling(evaluatedSolutions);
      return base.Apply();
    }
  }
}
