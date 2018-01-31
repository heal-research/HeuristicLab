#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.QuadraticAssignment.Algorithms {
  [Item("RobustTabooSearchOperator", "Performs an iteration of the robust taboo search algorithm as descrbied in Taillard 1991.")]
  [StorableClass]
  public sealed class RobustTabooSeachOperator : SingleSuccessorOperator, IIterationBasedOperator, IStochasticOperator, ISingleObjectiveOperator {

    #region Parameter Properties
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleMatrix> WeightsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }
    public ILookupParameter<DoubleMatrix> DistancesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }
    public ILookupParameter<IntMatrix> ShortTermMemoryParameter {
      get { return (ILookupParameter<IntMatrix>)Parameters["ShortTermMemory"]; }
    }
    public ILookupParameter<DoubleMatrix> MoveQualityMatrixParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["MoveQualityMatrix"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ILookupParameter<Swap2Move> LastMoveParameter {
      get { return (ILookupParameter<Swap2Move>)Parameters["LastMove"]; }
    }
    public ILookupParameter<BoolValue> UseNewTabuTenureAdaptionSchemeParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["UseNewTabuTenureAdaptionScheme"]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }
    public IValueLookupParameter<IntValue> MinimumTabuTenureParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MinimumTabuTenure"]; }
    }
    public IValueLookupParameter<IntValue> MaximumTabuTenureParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumTabuTenure"]; }
    }
    public IValueLookupParameter<BoolValue> UseAlternativeAspirationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["UseAlternativeAspiration"]; }
    }
    public IValueLookupParameter<IntValue> AlternativeAspirationTenureParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["AlternativeAspirationTenure"]; }
    }

    private ILookupParameter<BoolValue> AllMovesTabuParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["AllMovesTabu"]; }
    }

    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }

    public ILookupParameter<DoubleValue> EvaluatedSolutionEquivalentsParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["EvaluatedSolutionEquivalents"]; }
    }
    #endregion

    [StorableConstructor]
    private RobustTabooSeachOperator(bool deserializing) : base(deserializing) { }
    private RobustTabooSeachOperator(RobustTabooSeachOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public RobustTabooSeachOperator() {
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The current iteration."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation solution."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The weights matrix."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distances matrix."));
      Parameters.Add(new LookupParameter<IntMatrix>("ShortTermMemory", "The table that stores the iteration at which a certain facility has been assigned to a certain location."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("MoveQualityMatrix", "The quality of all swap moves as evaluated on the current permutation."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "The best quality value."));
      Parameters.Add(new LookupParameter<Swap2Move>("LastMove", "The last move that was applied."));
      Parameters.Add(new LookupParameter<BoolValue>("UseNewTabuTenureAdaptionScheme", "True if the new tabu tenure adaption should be used or false otherwise."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "Collection that houses the results of the algorithm."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The number of iterations that the algorithm should run."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MinimumTabuTenure", "The minimum tabu tenure."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumTabuTenure", "The maximum tabu tenure."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("UseAlternativeAspiration", "True if the alternative aspiration condition should be used that takes moves that have not been made for some time above others."));
      Parameters.Add(new ValueLookupParameter<IntValue>("AlternativeAspirationTenure", "The time t that a move will be remembered for the alternative aspiration condition."));
      Parameters.Add(new LookupParameter<BoolValue>("AllMovesTabu", "Indicates that all moves are tabu."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new LookupParameter<DoubleValue>("EvaluatedSolutionEquivalents", "The number of evaluated solution equivalents."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RobustTabooSeachOperator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("AllMovesTabu")) {
        Parameters.Add(new LookupParameter<BoolValue>("AllMovesTabu", "Indicates that all moves are tabu."));
      }
      if (!Parameters.ContainsKey("EvaluatedSolutions")) {
        Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      }
      if (!Parameters.ContainsKey("EvaluatedSolutionEquivalents")) {
        Parameters.Add(new LookupParameter<DoubleValue>("EvaluatedSolutionEquivalents", "The number of evaluated solution equivalents."));
      }
      #endregion
    }

    public override IOperation Apply() {
      IRandom random = RandomParameter.ActualValue;
      int iteration = IterationsParameter.ActualValue.Value;
      IntMatrix shortTermMemory = ShortTermMemoryParameter.ActualValue;
      DoubleMatrix weights = WeightsParameter.ActualValue;
      DoubleMatrix distances = DistancesParameter.ActualValue;
      DoubleMatrix moveQualityMatrix = MoveQualityMatrixParameter.ActualValue;

      DoubleValue quality = QualityParameter.ActualValue;
      DoubleValue bestQuality = BestQualityParameter.ActualValue;
      if (bestQuality == null) {
        BestQualityParameter.ActualValue = (DoubleValue)quality.Clone();
        bestQuality = BestQualityParameter.ActualValue;
      }
      bool allMovesTabu = false;
      if (AllMovesTabuParameter.ActualValue == null)
        AllMovesTabuParameter.ActualValue = new BoolValue(false);
      else allMovesTabu = AllMovesTabuParameter.ActualValue.Value;

      int minTenure = MinimumTabuTenureParameter.ActualValue.Value;
      int maxTenure = MaximumTabuTenureParameter.ActualValue.Value;
      int alternativeAspirationTenure = AlternativeAspirationTenureParameter.ActualValue.Value;
      bool useAlternativeAspiration = UseAlternativeAspirationParameter.ActualValue.Value;
      Permutation solution = PermutationParameter.ActualValue;
      Swap2Move lastMove = LastMoveParameter.ActualValue;

      double bestMoveQuality = double.MaxValue;
      Swap2Move bestMove = null;
      bool already_aspired = false;

      double evaluations = EvaluatedSolutionEquivalentsParameter.ActualValue.Value;
      foreach (Swap2Move move in ExhaustiveSwap2MoveGenerator.Generate(solution)) {
        double moveQuality;
        if (lastMove == null) {
          moveQuality = QAPSwap2MoveEvaluator.Apply(solution, move, weights, distances);
          evaluations += 4.0 / solution.Length;
        } else if (allMovesTabu) moveQuality = moveQualityMatrix[move.Index1, move.Index2];
        else {
          moveQuality = QAPSwap2MoveEvaluator.Apply(solution, move, moveQualityMatrix[move.Index1, move.Index2], weights, distances, lastMove);
          if (move.Index1 == lastMove.Index1 || move.Index2 == lastMove.Index1 || move.Index1 == lastMove.Index2 || move.Index2 == lastMove.Index2)
            evaluations += 4.0 / solution.Length;
          else evaluations += 2.0 / (solution.Length * solution.Length);
        }

        moveQualityMatrix[move.Index1, move.Index2] = moveQuality;
        moveQualityMatrix[move.Index2, move.Index1] = moveQuality;

        bool autorized = shortTermMemory[move.Index1, solution[move.Index2]] < iteration
                      || shortTermMemory[move.Index2, solution[move.Index1]] < iteration;

        bool aspired = (shortTermMemory[move.Index1, solution[move.Index2]] < iteration - alternativeAspirationTenure
                     && shortTermMemory[move.Index2, solution[move.Index1]] < iteration - alternativeAspirationTenure)
                  || quality.Value + moveQuality < bestQuality.Value;

        if ((aspired && !already_aspired) // the first alternative move is aspired
          || (aspired && already_aspired && moveQuality < bestMoveQuality) // an alternative move was already aspired, but this is better
          || (autorized && !aspired && !already_aspired && moveQuality < bestMoveQuality)) { // a regular better move is found
          bestMove = move;
          bestMoveQuality = moveQuality;
          if (aspired) already_aspired = true;
        }
      }

      ResultCollection results = ResultsParameter.ActualValue;
      if (results != null) {
        IntValue aspiredMoves = null;
        if (!results.ContainsKey("AspiredMoves")) {
          aspiredMoves = new IntValue(already_aspired ? 1 : 0);
          results.Add(new Result("AspiredMoves", "Counts the number of moves that were selected because of the aspiration criteria.", aspiredMoves));
        } else if (already_aspired) {
          aspiredMoves = (IntValue)results["AspiredMoves"].Value;
          aspiredMoves.Value++;
        }
      }

      EvaluatedSolutionEquivalentsParameter.ActualValue.Value = evaluations;
      EvaluatedSolutionsParameter.ActualValue.Value = (int)Math.Ceiling(evaluations);

      allMovesTabu = bestMove == null;
      if (!allMovesTabu)
        LastMoveParameter.ActualValue = bestMove;
      AllMovesTabuParameter.ActualValue.Value = allMovesTabu;

      if (allMovesTabu) return base.Apply();

      bool useNewAdaptionScheme = UseNewTabuTenureAdaptionSchemeParameter.ActualValue.Value;
      if (useNewAdaptionScheme) {
        double r = random.NextDouble();
        if (r == 0) r = 1; // transform to (0;1]
        shortTermMemory[bestMove.Index1, solution[bestMove.Index1]] = (int)(iteration + r * r * r * maxTenure);
        r = random.NextDouble();
        if (r == 0) r = 1; // transform to (0;1]
        shortTermMemory[bestMove.Index2, solution[bestMove.Index2]] = (int)(iteration + r * r * r * maxTenure);
      } else {
        shortTermMemory[bestMove.Index1, solution[bestMove.Index1]] = iteration + random.Next(minTenure, maxTenure);
        shortTermMemory[bestMove.Index2, solution[bestMove.Index2]] = iteration + random.Next(minTenure, maxTenure);
      }
      Swap2Manipulator.Apply(solution, bestMove.Index1, bestMove.Index2);
      quality.Value += bestMoveQuality;

      if (quality.Value < bestQuality.Value) bestQuality.Value = quality.Value;

      return base.Apply();
    }
  }
}
