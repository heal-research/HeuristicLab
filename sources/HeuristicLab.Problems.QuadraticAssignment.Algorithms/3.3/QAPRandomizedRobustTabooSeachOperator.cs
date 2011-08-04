#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("QAPRandomizedRobustTabooSeachOperator", "Performs an iteration of a modified robust taboo search algorithm based on Taillard 1991.")]
  public sealed class QAPRandomizedRobustTabooSeachOperator : SingleSuccessorOperator, IIterationBasedOperator, IStochasticOperator {

    #region Parameter Properties
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }
    public ILookupParameter<IntValue> TabuTenureParameter {
      get { return (ILookupParameter<IntValue>)Parameters["TabuTenure"]; }
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
    public ILookupParameter<DoubleMatrix> ShortTermMemory2Parameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["ShortTermMemory2"]; }
    }
    public ILookupParameter<IntMatrix> LongTermMemoryParameter {
      get { return (ILookupParameter<IntMatrix>)Parameters["LongTermMemory"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public ILookupParameter<IntValue> LastGlobalImprovementParameter {
      get { return (ILookupParameter<IntValue>)Parameters["LastGlobalImprovement"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
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
    public IValueLookupParameter<IntValue> TabuTenureAdaptionIntervalParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["TabuTenureAdaptionInterval"]; }
    }
    #endregion

    [StorableConstructor]
    private QAPRandomizedRobustTabooSeachOperator(bool deserializing) : base(deserializing) { }
    private QAPRandomizedRobustTabooSeachOperator(QAPRandomizedRobustTabooSeachOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public QAPRandomizedRobustTabooSeachOperator() {
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The current iteration."));
      Parameters.Add(new LookupParameter<IntValue>("TabuTenure", "The current tabu tenure."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation solution."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The weights matrix."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distances matrix."));
      Parameters.Add(new LookupParameter<IntMatrix>("ShortTermMemory", "The table that stores the iteration at which a certain facility has been assigned to a certain location."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("ShortTermMemory2", "The table that stores the quality at which a certain facility has been assigned to a certain location."));
      Parameters.Add(new LookupParameter<IntMatrix>("LongTermMemory", "Same as the tabu table, but constantly updates the information given the current solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "The best quality value."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The number of iterations that the algorithm should run."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MinimumTabuTenure", "The minimum tabu tenure."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumTabuTenure", "The maximum tabu tenure."));
      Parameters.Add(new ValueLookupParameter<IntValue>("TabuTenureAdaptionInterval", "The amount of iterations that have to pass before the tabu tenure is adapted."));
      Parameters.Add(new LookupParameter<IntValue>("LastGlobalImprovement", "The iteration at which the best solution so far has been improved."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The best known quality is just used to store the iteration at which it was found."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The collection to store results to."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPRandomizedRobustTabooSeachOperator(this, cloner);
    }

    public override IOperation Apply() {
      ResultCollection results = ResultsParameter.ActualValue;
      IRandom random = RandomParameter.ActualValue;
      int iteration = IterationsParameter.ActualValue.Value;

      IntMatrix longTermMemory = LongTermMemoryParameter.ActualValue;
      IntMatrix shortTermMemory = ShortTermMemoryParameter.ActualValue;
      DoubleMatrix shortTermMemory2 = ShortTermMemory2Parameter.ActualValue;
      DoubleMatrix weights = WeightsParameter.ActualValue;
      DoubleMatrix distances = DistancesParameter.ActualValue;

      DoubleValue quality = QualityParameter.ActualValue;
      DoubleValue bestQuality = BestQualityParameter.ActualValue;
      if (bestQuality == null) {
        BestQualityParameter.ActualValue = (DoubleValue)quality.Clone();
        bestQuality = BestQualityParameter.ActualValue;
      }

      Permutation solution = PermutationParameter.ActualValue;

      for (int i = 0; i < solution.Length; i++)
        longTermMemory[i, solution[i]] = iteration;

      double minQuality = double.MaxValue, maxQuality = double.MinValue,
        minImprovement = double.MaxValue, maxImprovement = double.MinValue,
        minRevist = double.MaxValue, maxRevist = double.MinValue;
      Swap2Move[] moves = ExhaustiveSwap2MoveGenerator.Apply(solution);
      double[] moveQualities = new double[moves.Length];
      for (int i = 0; i < moves.Length; i++) {
        Swap2Move move = moves[i];
        double moveQuality = QAPSwap2MoveEvaluator.Apply(solution, move, weights, distances);
        moveQualities[i] = moveQuality;
        if (moveQuality < minQuality) minQuality = moveQuality;
        if (moveQuality > maxQuality) maxQuality = moveQuality;

        double improvement = 0;
        if (shortTermMemory[move.Index1, solution[move.Index2]] > 0)
          improvement += Math.Max(0, shortTermMemory2[move.Index1, solution[move.Index2]] - quality.Value + moveQuality);
        if (shortTermMemory[move.Index2, solution[move.Index1]] > 0)
          improvement += Math.Max(0, shortTermMemory2[move.Index2, solution[move.Index1]] - quality.Value + moveQuality);
        if (improvement > 0) {
          if (improvement < minImprovement) minImprovement = improvement;
          if (improvement > maxImprovement) maxImprovement = improvement;
        }

        double revisit = 0;
        revisit += Math.Max(0, quality.Value - shortTermMemory2[move.Index1, solution[move.Index2]]);
        revisit += Math.Max(0, quality.Value - shortTermMemory2[move.Index2, solution[move.Index1]]);
        if (revisit > 0) {
          if (revisit < minRevist) minRevist = revisit;
          if (revisit > maxRevist) maxRevist = revisit;
        }
      }

      Swap2Move selectedMove = null;
      double bestInterestingness = double.MinValue, selectedMoveQuality = 0;
      int equalInterestingCount = 0;
      for (int i = 0; i < moves.Length; i++) {
        Swap2Move move = moves[i];

        double interestingness = 0;

        if (maxQuality > minQuality)
          interestingness += 4 * (maxQuality - moveQualities[i]) / (maxQuality - minQuality);

        if (maxImprovement > minImprovement) {
          double improvement = 0;
          if (shortTermMemory[move.Index1, solution[move.Index2]] > 0)
            improvement += Math.Max(0, shortTermMemory2[move.Index1, solution[move.Index2]] - quality.Value + moveQualities[i]);
          if (shortTermMemory[move.Index2, solution[move.Index1]] > 0)
            improvement += Math.Max(0, shortTermMemory2[move.Index2, solution[move.Index1]] - quality.Value + moveQualities[i]);
          if (improvement > 0)
            interestingness += 2 * (improvement - minImprovement) / (maxImprovement - minImprovement);
        }

        if (iteration > 0) {
          interestingness += ((double)(iteration - longTermMemory[move.Index1, solution[move.Index2]]) / (double)iteration)
            + ((double)(iteration - longTermMemory[move.Index2, solution[move.Index1]]) / (double)iteration);
        }

        if (maxRevist > minRevist) {
          double revisit = 0;
          revisit += Math.Max(0, quality.Value - shortTermMemory2[move.Index1, solution[move.Index2]]);
          revisit += Math.Max(0, quality.Value - shortTermMemory2[move.Index2, solution[move.Index1]]);
          if (revisit > 0)
            interestingness += (revisit - minRevist) / (maxRevist - minRevist);
        }

        if (quality.Value + moveQualities[i] < bestQuality.Value) interestingness = double.MaxValue;
        if (interestingness > bestInterestingness) {
          bestInterestingness = interestingness;
          selectedMove = moves[i];
          selectedMoveQuality = moveQualities[i];
          equalInterestingCount = 1;
        } else if (interestingness == bestInterestingness) {
          equalInterestingCount++;
          if (random.NextDouble() < 1.0 / equalInterestingCount) {
            selectedMove = moves[i];
            selectedMoveQuality = moveQualities[i];
          }
        }
      }

      shortTermMemory[selectedMove.Index1, solution[selectedMove.Index1]] = iteration;
      shortTermMemory[selectedMove.Index2, solution[selectedMove.Index2]] = iteration;
      if (shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index2]] > 0)
        shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index2]] = Math.Min(quality.Value + selectedMoveQuality, shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index2]]);
      else shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index2]] = quality.Value + selectedMoveQuality;
      if (shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index1]] > 0)
        shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index1]] = Math.Min(quality.Value, shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index1]]);
      else shortTermMemory2[selectedMove.Index1, solution[selectedMove.Index1]] = quality.Value;
      if (shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index1]] > 0)
        shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index1]] = Math.Min(quality.Value + selectedMoveQuality, shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index1]]);
      else shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index1]] = quality.Value + selectedMoveQuality;
      if (shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index2]] > 0)
        shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index2]] = Math.Min(quality.Value, shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index2]]);
      else shortTermMemory2[selectedMove.Index2, solution[selectedMove.Index2]] = quality.Value;

      Swap2Manipulator.Apply(solution, selectedMove.Index1, selectedMove.Index2);
      quality.Value += selectedMoveQuality;

      if (quality.Value < bestQuality.Value) {
        bestQuality.Value = quality.Value;
        if (LastGlobalImprovementParameter.ActualValue == null)
          LastGlobalImprovementParameter.ActualValue = new IntValue(iteration);
        else LastGlobalImprovementParameter.ActualValue.Value = iteration;
      }
      if (!results.ContainsKey("GlobalBestFound")) results.Add(new Result("GlobalBestFound", new IntValue(-1)));
      if (BestKnownQualityParameter.ActualValue.Value == bestQuality.Value
        && ((IntValue)results["GlobalBestFound"].Value).Value < 0) {
        ((IntValue)results["GlobalBestFound"].Value).Value = iteration;
      }
      return base.Apply();
    }
  }
}
