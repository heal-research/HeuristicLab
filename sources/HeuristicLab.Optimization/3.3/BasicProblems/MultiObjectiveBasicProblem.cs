#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class MultiObjectiveBasicProblem<TEncoding> : BasicProblem<TEncoding, MultiObjectiveEvaluator>, IMultiObjectiveHeuristicOptimizationProblem, IMultiObjectiveProblemDefinition
  where TEncoding : class, IEncoding {
    [StorableConstructor]
    protected MultiObjectiveBasicProblem(bool deserializing) : base(deserializing) { }

    protected MultiObjectiveBasicProblem(MultiObjectiveBasicProblem<TEncoding> original, Cloner cloner)
      : base(original, cloner) {
      ParameterizeOperators();
    }

    protected MultiObjectiveBasicProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolArray>("Maximization", "Set to false if the problem should be minimized.", (BoolArray)new BoolArray(Maximization).AsReadOnly()));

      Operators.Add(Evaluator);
      Operators.Add(new MultiObjectiveAnalyzer());

      ParameterizeOperators();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ParameterizeOperators();
    }

    public abstract bool[] Maximization { get; }
    public abstract double[] Evaluate(Individual individual, IRandom random);
    public virtual void Analyze(Individual[] individuals, double[][] qualities, ResultCollection results, IRandom random) { }

    protected List<List<Tuple<Individual, double[]>>> GetParetoFronts(Individual[] individuals, double[][] qualities, bool dominateOnEqualQualities = true) {
      return GetParetoFronts(individuals, qualities, Maximization, dominateOnEqualQualities);
    }
    public static List<List<Tuple<Individual, double[]>>> GetParetoFronts(Individual[] individuals, double[][] qualities, bool[] maximization, bool dominateOnEqualQualities = true) {
      int populationSize = individuals.Length;

      var fronts = new List<List<Tuple<Individual, double[]>>>();
      fronts.Add(new List<Tuple<Individual, double[]>>());
      Dictionary<Individual, List<int>> dominatedIndividuals = new Dictionary<Individual, List<int>>();
      int[] dominationCounter = new int[populationSize];
      ItemArray<IntValue> rank = new ItemArray<IntValue>(populationSize);

      for (int pI = 0; pI < populationSize - 1; pI++) {
        var p = individuals[pI];
        List<int> dominatedIndividualsByp;
        if (!dominatedIndividuals.TryGetValue(p, out dominatedIndividualsByp))
          dominatedIndividuals[p] = dominatedIndividualsByp = new List<int>();
        for (int qI = pI + 1; qI < populationSize; qI++) {
          var test = Dominates(qualities[pI], qualities[qI], maximization, dominateOnEqualQualities);
          if (test == 1) {
            dominatedIndividualsByp.Add(qI);
            dominationCounter[qI] += 1;
          } else if (test == -1) {
            dominationCounter[pI] += 1;
            if (!dominatedIndividuals.ContainsKey(individuals[qI]))
              dominatedIndividuals.Add(individuals[qI], new List<int>());
            dominatedIndividuals[individuals[qI]].Add(pI);
          }
          if (pI == populationSize - 2
            && qI == populationSize - 1
            && dominationCounter[qI] == 0) {
            rank[qI] = new IntValue(0);
            fronts[0].Add(Tuple.Create(individuals[qI], qualities[qI]));
          }
        }
        if (dominationCounter[pI] == 0) {
          rank[pI] = new IntValue(0);
          fronts[0].Add(Tuple.Create(p, qualities[pI]));
        }
      }
      int i = 0;
      while (i < fronts.Count && fronts[i].Count > 0) {
        var nextFront = new List<Tuple<Individual, double[]>>();
        foreach (var p in fronts[i]) {
          List<int> dominatedIndividualsByp;
          if (dominatedIndividuals.TryGetValue(p.Item1, out dominatedIndividualsByp)) {
            for (int k = 0; k < dominatedIndividualsByp.Count; k++) {
              int dominatedIndividual = dominatedIndividualsByp[k];
              dominationCounter[dominatedIndividual] -= 1;
              if (dominationCounter[dominatedIndividual] == 0) {
                rank[dominatedIndividual] = new IntValue(i + 1);
                nextFront.Add(Tuple.Create(individuals[dominatedIndividual], qualities[dominatedIndividual]));
              }
            }
          }
        }
        i += 1;
        fronts.Add(nextFront);
      }
      return fronts;
    }

    private static int Dominates(double[] left, double[] right, bool[] maximizations, bool dominateOnEqualQualities) {
      //mkommend Caution: do not use LINQ.SequenceEqual for comparing the two quality arrays (left and right) due to performance reasons
      if (dominateOnEqualQualities) {
        var equal = true;
        for (int i = 0; i < left.Length; i++) {
          if (left[i] != right[i]) {
            equal = false;
            break;
          }
        }
        if (equal) return 1;
      }

      bool leftIsBetter = false, rightIsBetter = false;
      for (int i = 0; i < left.Length; i++) {
        if (IsDominated(left[i], right[i], maximizations[i])) rightIsBetter = true;
        else if (IsDominated(right[i], left[i], maximizations[i])) leftIsBetter = true;
        if (leftIsBetter && rightIsBetter) break;
      }

      if (leftIsBetter && !rightIsBetter) return 1;
      if (!leftIsBetter && rightIsBetter) return -1;
      return 0;
    }

    private static bool IsDominated(double left, double right, bool maximization) {
      return maximization && left < right
        || !maximization && left > right;
    }

    protected override void OnOperatorsChanged() {
      base.OnOperatorsChanged();
      if (Encoding != null) {
        PruneSingleObjectiveOperators(Encoding);
        var multiEncoding = Encoding as MultiEncoding;
        if (multiEncoding != null) {
          foreach (var encoding in multiEncoding.Encodings.ToList()) {
            PruneSingleObjectiveOperators(encoding);
          }
        }
      }
    }

    private void PruneSingleObjectiveOperators(IEncoding encoding) {
      if (encoding != null && encoding.Operators.Any(x => x is ISingleObjectiveOperator && !(x is IMultiObjectiveOperator)))
        encoding.Operators = encoding.Operators.Where(x => !(x is ISingleObjectiveOperator) || x is IMultiObjectiveOperator).ToList();
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<IMultiObjectiveEvaluationOperator>())
        op.EvaluateFunc = Evaluate;
      foreach (var op in Operators.OfType<IMultiObjectiveAnalysisOperator>())
        op.AnalyzeAction = Analyze;
    }


    #region IMultiObjectiveHeuristicOptimizationProblem Members
    IParameter IMultiObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return Parameters["Maximization"]; }
    }
    IMultiObjectiveEvaluator IMultiObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
    #endregion
  }
}
