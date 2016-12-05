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
using System.Threading;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Algorithms.MemPR.Util;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR.Permutation {
  [Item("MemPR (permutation)", "MemPR implementation for permutations.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 999)]
  public class PermutationMemPR : MemPRAlgorithm<SingleObjectiveBasicProblem<PermutationEncoding>, Encodings.PermutationEncoding.Permutation, PermutationMemPRPopulationContext, PermutationMemPRSolutionContext> {
    private const double UncommonBitSubsetMutationProbabilityMagicConst = 0.05;

#if DEBUG
    private const bool VALIDATE = true;
#else
    private const bool VALIDATE = false;
#endif

    [StorableConstructor]
    protected PermutationMemPR(bool deserializing) : base(deserializing) { }
    protected PermutationMemPR(PermutationMemPR original, Cloner cloner) : base(original, cloner) { }
    public PermutationMemPR() {
      foreach (var trainer in ApplicationManager.Manager.GetInstances<ISolutionModelTrainer<PermutationMemPRPopulationContext>>())
        SolutionModelTrainerParameter.ValidValues.Add(trainer);
      
      foreach (var localSearch in ApplicationManager.Manager.GetInstances<ILocalSearch<PermutationMemPRSolutionContext>>()) {
        LocalSearchParameter.ValidValues.Add(localSearch);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PermutationMemPR(this, cloner);
    }

    protected override bool Eq(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> a, ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> b) {
      return new PermutationEqualityComparer().Equals(a.Solution, b.Solution);
    }

    protected override double Dist(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> a, ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> b) {
      return Dist(a.Solution, b.Solution);
    }

    private static double Dist(Encodings.PermutationEncoding.Permutation a, Encodings.PermutationEncoding.Permutation b) {
      return 1.0 - PermutationSimilarityCalculator.CalculateSimilarity(a, b);
    }

    protected override ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> ToScope(Encodings.PermutationEncoding.Permutation code, double fitness = double.NaN) {
      var creator = Problem.SolutionCreator as IPermutationCreator;
      if (creator == null) throw new InvalidOperationException("Can only solve binary encoded problems with MemPR (binary)");
      return new SingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation>(code, creator.PermutationParameter.ActualName, fitness, Problem.Evaluator.QualityParameter.ActualName) {
        Parent = Context.Scope
      };
    }

    protected override ISolutionSubspace<Encodings.PermutationEncoding.Permutation> CalculateSubspace(IEnumerable<Encodings.PermutationEncoding.Permutation> solutions, bool inverse = false) {
      var subspace = new bool[Problem.Encoding.Length, Problem.Encoding.PermutationTypeParameter.Value.Value == PermutationTypes.Absolute ? 1 : Problem.Encoding.Length];

      switch (Problem.Encoding.PermutationTypeParameter.Value.Value) {
        case PermutationTypes.Absolute: {
            if (inverse) {
              for (var i = 0; i < subspace.GetLength(0); i++)
                subspace[i, 0] = true;
            }
            var first = solutions.First();
            foreach (var s in solutions.Skip(1)) {
              for (var i = 0; i < s.Length; i++) {
                if (first[i] != s[i]) subspace[i, 0] = !inverse;
              }
            }
          }
          break;
        case PermutationTypes.RelativeDirected: {
            if (inverse) {
              for (var i = 0; i < subspace.GetLength(0); i++)
                for (var j = 0; j < subspace.GetLength(1); j++)
                  subspace[i, j] = true;
            }
            var first = solutions.First();
            var placedFirst = new int[first.Length];
            for (var i = 0; i < first.Length; i++) {
              placedFirst[first[i]] = i;
            }
            foreach (var s in solutions.Skip(1)) {
              for (var i = 0; i < s.Length; i++) {
                if (placedFirst[s[i]] - placedFirst[s.GetCircular(i + 1)] != -1)
                  subspace[i, 0] = !inverse;
              }
            }
          }
          break;
        case PermutationTypes.RelativeUndirected: {
            if (inverse) {
              for (var i = 0; i < subspace.GetLength(0); i++)
                for (var j = 0; j < subspace.GetLength(1); j++)
                  subspace[i, j] = true;
            }
            var first = solutions.First();
            var placedFirst = new int[first.Length];
            for (var i = 0; i < first.Length; i++) {
              placedFirst[first[i]] = i;
            }
            foreach (var s in solutions.Skip(1)) {
              for (var i = 0; i < s.Length; i++) {
                if (Math.Abs(placedFirst[s[i]] - placedFirst[s.GetCircular(i + 1)]) != 1)
                  subspace[i, 0] = !inverse;
              }
            }
          }
          break;
        default:
          throw new ArgumentException(string.Format("Unknown permutation type {0}", Problem.Encoding.PermutationTypeParameter.Value.Value));
      }
      return new PermutationSolutionSubspace(subspace);
    }

    protected override void TabuWalk(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> scope, int steps, CancellationToken token, ISolutionSubspace<Encodings.PermutationEncoding.Permutation> subspace = null) {
      var wrapper = new EvaluationWrapper<Encodings.PermutationEncoding.Permutation>(Context.Problem, scope);
      var quality = scope.Fitness;
      try {
        TabuWalk(Context.Random, scope.Solution, wrapper.Evaluate, ref quality, Context.HcSteps, subspace != null ? ((PermutationSolutionSubspace)subspace).Subspace : null);
      } finally {
        scope.Fitness = quality;
      }
    }

    public void TabuWalk(IRandom random, Encodings.PermutationEncoding.Permutation perm, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, ref double quality, int maxIterations = int.MaxValue, bool[,] noTouch = null) {
      switch (perm.PermutationType) {
        case PermutationTypes.Absolute:
          TabuWalkSwap(random, perm, eval, ref quality, maxIterations, noTouch);
          break;
        case PermutationTypes.RelativeDirected:
          TabuWalkShift(random, perm, eval, ref quality, maxIterations, noTouch);
          break;
        case PermutationTypes.RelativeUndirected:
          TabuWalkOpt(random, perm, eval, ref quality, maxIterations, noTouch);
          break;
        default: throw new ArgumentException(string.Format("Permutation type {0} is not known", perm.PermutationType));
      }
      if (VALIDATE && !perm.Validate()) throw new ArgumentException("TabuWalk produced invalid child");
    }

    public void TabuWalkSwap(IRandom random, Encodings.PermutationEncoding.Permutation perm, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, ref double quality, int maxIterations = int.MaxValue, bool[,] noTouch = null) {
      var evaluations = 0;
      if (double.IsNaN(quality)) {
        quality = eval(perm, random);
        evaluations++;
      }
      Encodings.PermutationEncoding.Permutation bestOfTheWalk = null;
      double bestOfTheWalkF = double.NaN;
      var current = (Encodings.PermutationEncoding.Permutation)perm.Clone();
      var currentF = quality;
      var overallImprovement = false;
      var tabu = new double[current.Length, current.Length];
      for (var i = 0; i < current.Length; i++) {
        for (var j = i; j < current.Length; j++) {
          tabu[i, j] = tabu[j, i] = double.MaxValue;
        }
        tabu[i, current[i]] = currentF;
      }

      for (var iter = 0; iter < maxIterations; iter++) {
        var allTabu = true;
        var bestOfTheRestF = double.NaN;
        Swap2Move bestOfTheRest = null;
        var improved = false;
        foreach (var swap in ExhaustiveSwap2MoveGenerator.Generate(current).Shuffle(random)) {
          if (noTouch != null && (noTouch[swap.Index1, 0] || noTouch[swap.Index2, 0]))
            continue;

          var h = current[swap.Index1];
          current[swap.Index1] = current[swap.Index2];
          current[swap.Index2] = h;
          var q = eval(current, random);
          evaluations++;
          if (q < quality) {
            overallImprovement = true;
            quality = q;
            for (var i = 0; i < current.Length; i++) perm[i] = current[i];
          }
          // check if it would not be an improvement to swap these into their positions
          var isTabu = q >= tabu[swap.Index1, current[swap.Index1]] && q >= tabu[swap.Index2, current[swap.Index2]];
          if (!isTabu) allTabu = false;
          if (q < currentF && !isTabu) {
            if (double.IsNaN(bestOfTheWalkF) || bestOfTheWalkF > q) {
              bestOfTheWalk = (Encodings.PermutationEncoding.Permutation)current.Clone();
              bestOfTheWalkF = q;
            }

            improved = true;
            // perform the move
            currentF = q;
            // mark that to move them to their previous position requires to make an improvement
            tabu[swap.Index1, current[swap.Index2]] = Math.Min(q, tabu[swap.Index1, current[swap.Index2]]);
            //LogAlways("Making Tabu [{0},{1}] = {2}", swap.Index1, current[swap.Index2], tabu[swap.Index1, current[swap.Index2]]);
            tabu[swap.Index2, current[swap.Index1]] = Math.Min(q, tabu[swap.Index2, current[swap.Index1]]);
            //LogAlways("Making Tabu [{0},{1}] = {2}", swap.Index2, current[swap.Index1], tabu[swap.Index2, current[swap.Index1]]);
          } else { // undo the move
            if (!isTabu && (double.IsNaN(bestOfTheRestF) || q < bestOfTheRestF)) {
              bestOfTheRest = swap;
              bestOfTheRestF = q;
            }
            current[swap.Index2] = current[swap.Index1];
            current[swap.Index1] = h;
          }
        }
        if (!allTabu && !improved) {
          tabu[bestOfTheRest.Index1, current[bestOfTheRest.Index1]] = Math.Min(currentF, tabu[bestOfTheRest.Index1, current[bestOfTheRest.Index1]]);
          tabu[bestOfTheRest.Index2, current[bestOfTheRest.Index2]] = Math.Min(currentF, tabu[bestOfTheRest.Index2, current[bestOfTheRest.Index2]]);

          var h = current[bestOfTheRest.Index1];
          current[bestOfTheRest.Index1] = current[bestOfTheRest.Index2];
          current[bestOfTheRest.Index2] = h;

          currentF = bestOfTheRestF;
        } else if (allTabu) break;
      }
      Context.IncrementEvaluatedSolutions(evaluations);
      if (!overallImprovement && bestOfTheWalk != null) {
        quality = bestOfTheWalkF;
        for (var i = 0; i < current.Length; i++) perm[i] = bestOfTheWalk[i];
      }
    }

    public void TabuWalkShift(IRandom random, Encodings.PermutationEncoding.Permutation perm, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, ref double quality, int maxIterations = int.MaxValue, bool[,] noTouch = null) {
      return;
    }

    public void TabuWalkOpt(IRandom random, Encodings.PermutationEncoding.Permutation perm, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, ref double quality, int maxIterations = int.MaxValue, bool[,] noTouch = null) {
      var evaluations = 0;
      if (double.IsNaN(quality)) {
        quality = eval(perm, random);
        evaluations++;
      }
      Encodings.PermutationEncoding.Permutation bestOfTheWalk = null;
      double bestOfTheWalkF = double.NaN;
      var current = (Encodings.PermutationEncoding.Permutation)perm.Clone();
      var currentF = quality;
      var overallImprovement = false;
      //Console.WriteLine("Current    {0}", string.Join(", ", current));
      var tabu = new double[current.Length, current.Length];
      for (var i = 0; i < current.Length; i++) {
        for (var j = i; j < current.Length; j++) {
          tabu[i, j] = tabu[j, i] = double.MaxValue;
        }
        tabu[current[i], current.GetCircular(i + 1)] = currentF;
        tabu[current.GetCircular(i + 1), current[i]] = currentF;
      }

      for (var iter = 0; iter < maxIterations; iter++) {
        var allTabu = true;
        var bestOfTheRestF = double.NaN;
        InversionMove bestOfTheRest = null;
        var improved = false;

        foreach (var opt in ExhaustiveInversionMoveGenerator.Generate(current).Shuffle(random)) {
          var prev = opt.Index1 - 1;
          var next = (opt.Index2 + 1) % current.Length;
          if (prev < 0) prev += current.Length;
          if (noTouch != null && ((noTouch[current[prev], current[opt.Index1]]) || (noTouch[current[opt.Index2], current[next]])))
            continue;

          InversionManipulator.Apply(current, opt.Index1, opt.Index2);

          var q = eval(current, random);
          evaluations++;
          if (q < quality) {
            overallImprovement = true;
            quality = q;
            for (var i = 0; i < current.Length; i++) perm[i] = current[i];
          }
          // check if it would not be an improvement to opt these into their positions
          var isTabu = q >= tabu[current[prev], current[opt.Index1]] && q >= tabu[current[opt.Index2], current[next]];
          if (!isTabu) allTabu = false;
          if (q < currentF && !isTabu) {
            if (double.IsNaN(bestOfTheWalkF) || bestOfTheWalkF > q) {
              bestOfTheWalk = (Encodings.PermutationEncoding.Permutation)current.Clone();
              bestOfTheWalkF = q;
            }

            improved = true;
            // perform the move
            currentF = q;
            // mark that to move them to their previous position requires to make an improvement
            tabu[current[prev], current[opt.Index2]] = Math.Min(q, tabu[current[prev], current[opt.Index2]]);
            tabu[current[opt.Index2], current[prev]] = Math.Min(q, tabu[current[opt.Index2], current[prev]]);
            tabu[current[opt.Index1], current[next]] = Math.Min(q, tabu[current[opt.Index1], current[next]]);
            tabu[current[next], current[opt.Index1]] = Math.Min(q, tabu[current[next], current[opt.Index1]]);
          } else { // undo the move
            if (!isTabu && (double.IsNaN(bestOfTheRestF) || q < bestOfTheRestF)) {
              bestOfTheRest = opt;
              bestOfTheRestF = q;
            }

            InversionManipulator.Apply(current, opt.Index1, opt.Index2);
          }
        }
        if (!allTabu && !improved) {
          var prev = bestOfTheRest.Index1 - 1;
          var next = (bestOfTheRest.Index2 + 1) % current.Length;
          if (prev < 0) prev += current.Length;

          tabu[current[prev], current[bestOfTheRest.Index1]] = Math.Min(currentF, tabu[current[prev], current[bestOfTheRest.Index1]]);
          tabu[current[bestOfTheRest.Index1], current[prev]] = Math.Min(currentF, tabu[current[bestOfTheRest.Index1], current[prev]]);
          tabu[current[bestOfTheRest.Index2], current[next]] = Math.Min(currentF, tabu[current[bestOfTheRest.Index2], current[next]]);
          tabu[current[next], current[bestOfTheRest.Index2]] = Math.Min(currentF, tabu[current[next], current[bestOfTheRest.Index2]]);

          InversionManipulator.Apply(current, bestOfTheRest.Index1, bestOfTheRest.Index2);

          currentF = bestOfTheRestF;
        } else if (allTabu) break;
      }
      Context.IncrementEvaluatedSolutions(evaluations);
      if (!overallImprovement && bestOfTheWalk != null) {
        quality = bestOfTheWalkF;
        for (var i = 0; i < current.Length; i++) perm[i] = bestOfTheWalk[i];
      }
    }

    protected override ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> Cross(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> p1, ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> p2, CancellationToken token) {
      Encodings.PermutationEncoding.Permutation child = null;

      if (p1.Solution.PermutationType == PermutationTypes.Absolute) {
        child = CyclicCrossover.Apply(Context.Random, p1.Solution, p2.Solution);
      } else if (p1.Solution.PermutationType == PermutationTypes.RelativeDirected) {
        child = PartiallyMatchedCrossover.Apply(Context.Random, p1.Solution, p2.Solution);
      } else if (p1.Solution.PermutationType == PermutationTypes.RelativeUndirected) {
        child = EdgeRecombinationCrossover.Apply(Context.Random, p1.Solution, p2.Solution);
      } else throw new ArgumentException(string.Format("Unknown permutation type {0}", p1.Solution.PermutationType));

      if (VALIDATE && !child.Validate()) throw new ArgumentException("Cross produced invalid child");
      return ToScope(child);
    }

    protected override void Mutate(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> offspring, CancellationToken token, ISolutionSubspace<Encodings.PermutationEncoding.Permutation> subspace = null) {
      Mutate(Context.Random, offspring.Solution, UncommonBitSubsetMutationProbabilityMagicConst, subspace != null ? ((PermutationSolutionSubspace)subspace).Subspace : null);
    }

    public static void Mutate(IRandom random, Encodings.PermutationEncoding.Permutation perm, double p, bool[,] subspace) {
      switch (perm.PermutationType) {
        case PermutationTypes.Absolute:
          MutateSwap(random, perm, p, subspace);
          break;
        case PermutationTypes.RelativeDirected:
          MutateShift(random, perm, p, subspace);
          break;
        case PermutationTypes.RelativeUndirected:
          MutateOpt(random, perm, p, subspace);
          break;
        default: throw new ArgumentException(string.Format("Permutation type {0} is not known", perm.PermutationType));
      }
      if (VALIDATE && !perm.Validate()) throw new ArgumentException("Mutate produced invalid child");
    }

    public static void MutateSwap(IRandom random, Encodings.PermutationEncoding.Permutation perm, double p, bool[,] subspace) {
      //Log("BEFOR: {0}", string.Join(", ", lle));
      // The goal of the mutation is to disrupt crossover when it's in an agreeing position
      var options = new List<int>(Enumerable.Range(0, perm.Length).Where(x => subspace == null || !subspace[x, 0]));
      if (options.Count < 1) return;

      for (var i = options.Count - 1; i > 0; i--) {
        if (random.NextDouble() < p) {
          var j = random.Next(0, i);
          var h = perm[options[i]];
          perm[options[i]] = perm[options[j]];
          perm[options[j]] = h;
          if (subspace != null) {
            subspace[options[i], 0] = true;
            subspace[options[j], 0] = true;
          }
        }
      }
    }

    public static void MutateShift(IRandom random, Encodings.PermutationEncoding.Permutation perm, double p, bool[,] subspace) {
      //Log("BEFOR: {0}", string.Join(", ", lle));
      // The goal of the mutation is to disrupt crossover when it's in an agreeing position
      foreach (var shift in ExhaustiveInsertionMoveGenerator.Generate(perm).Shuffle(random)) {
        var prev1 = shift.Index1 - 1;
        var next1 = (shift.Index1 + 1) % perm.Length;
        if (prev1 < 0) prev1 += perm.Length;
        var prev3 = shift.Index3 - 1;
        var next3 = (shift.Index3 + 1) % perm.Length;
        if (prev3 < 0) prev3 += perm.Length;
        if (subspace == null || !(subspace[perm[prev1], perm[shift.Index1]] && subspace[perm[shift.Index1], perm[next1]]
            && subspace[perm[prev3], perm[shift.Index3]] && subspace[perm[shift.Index3], perm[next3]])) {
          if (random.NextDouble() < p) {
            if (subspace != null) {
              subspace[perm[prev1], perm[shift.Index1]] = true;
              subspace[perm[shift.Index1], perm[next1]] = true;
              subspace[perm[prev3], perm[shift.Index3]] = true;
              subspace[perm[shift.Index3], perm[next3]] = true;
            }
            TranslocationManipulator.Apply(perm, shift.Index1, shift.Index2, shift.Index3);
            return;
          }
        }
      }
    }

    public static void MutateOpt(IRandom random, Encodings.PermutationEncoding.Permutation perm, double p, bool[,] subspace) {
      //Log("BEFOR: {0}", string.Join(", ", lle));
      // The goal of the mutation is to disrupt crossover when it's in an agreeing position
      foreach (var opt in ExhaustiveInversionMoveGenerator.Generate(perm).Shuffle(random)) {
        var prev = opt.Index1 - 1;
        var next = (opt.Index2 + 1) % perm.Length;
        if (prev < 0) prev += perm.Length;
        if (subspace == null || !(subspace[perm[prev], perm[opt.Index1]] && subspace[perm[opt.Index2], perm[next]])) {
          if (random.NextDouble() < p) {
            if (subspace != null) {
              subspace[perm[prev], perm[opt.Index1]] = true;
              subspace[perm[opt.Index1], perm[prev]] = true;
              subspace[perm[opt.Index2], perm[next]] = true;
              subspace[perm[next], perm[opt.Index2]] = true;
            }
            InversionManipulator.Apply(perm, opt.Index1, opt.Index2);
            return;
          }
        }
      }
    }

    protected override ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> Relink(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> a, ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> b, CancellationToken token) {
      if (double.IsNaN(a.Fitness)) Evaluate(a, token);
      if (double.IsNaN(b.Fitness)) Evaluate(b, token);
      if (Context.Random.NextDouble() < 0.5)
        return IsBetter(a, b) ? Relink(a, b, token, false) : Relink(b, a, token, true);
      else return IsBetter(a, b) ? Relink(b, a, token, true) : Relink(a, b, token, false);
    }

    protected virtual ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> Relink(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> betterScope, ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> worseScope, CancellationToken token, bool fromWorseToBetter) {
      var wrapper = new EvaluationWrapper<Encodings.PermutationEncoding.Permutation>(Problem, betterScope);
      double quality;
      return ToScope(Relink(Context.Random, betterScope.Solution, worseScope.Solution, wrapper.Evaluate, out quality));
    }

    public Encodings.PermutationEncoding.Permutation Relink(IRandom random, Encodings.PermutationEncoding.Permutation p1, Encodings.PermutationEncoding.Permutation p2, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, out double best) {
      if (p1.PermutationType != p2.PermutationType) throw new ArgumentException(string.Format("Unequal permutation types {0} and {1}", p1.PermutationType, p2.PermutationType));
      switch (p1.PermutationType) {
        case PermutationTypes.Absolute:
          return RelinkSwap(random, p1, p2, eval, out best);
        case PermutationTypes.RelativeDirected:
          return RelinkShift(random, p1, p2, eval, out best);
        case PermutationTypes.RelativeUndirected:
          return RelinkOpt(random, p1, p2, eval, out best);
        default: throw new ArgumentException(string.Format("Unknown permutation type {0}", p1.PermutationType));
      }
    }

    public Encodings.PermutationEncoding.Permutation RelinkSwap(IRandom random, Encodings.PermutationEncoding.Permutation p1, Encodings.PermutationEncoding.Permutation p2, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, out double best) {
      var evaluations = 0;
      var child = (Encodings.PermutationEncoding.Permutation)p1.Clone();

      best = double.NaN;
      Encodings.PermutationEncoding.Permutation bestChild = null;

      var options = Enumerable.Range(0, child.Length).Where(x => child[x] != p2[x]).ToList();
      var invChild = new int[child.Length];
      for (var i = 0; i < child.Length; i++) invChild[child[i]] = i;

      //Log(string.Join(", ", child));
      while (options.Count > 0) {
        int bestOption = -1;
        var bestChange = double.NaN;
        for (var j = 0; j < options.Count; j++) {
          var idx = options[j];
          if (child[idx] == p2[idx]) {
            options.RemoveAt(j);
            j--;
            continue;
          }
          Swap(child, invChild[p2[idx]], idx);
          var moveF = eval(child, random);
          evaluations++;
          if (double.IsNaN(bestChange) || moveF < bestChange) {
            bestChange = moveF;
            bestOption = j;
          }
          // undo
          Swap(child, invChild[p2[idx]], idx);
        }
        if (!double.IsNaN(bestChange)) {
          var idx1 = options[bestOption];
          var idx2 = invChild[p2[idx1]];
          Swap(child, idx1, idx2);
          invChild[child[idx1]] = idx1;
          invChild[child[idx2]] = idx2;
          //Log(string.Join(", ", child));
          if (double.IsNaN(best) || bestChange < best) {
            if (Dist(child, p2) > 0) {
              best = bestChange;
              bestChild = (Encodings.PermutationEncoding.Permutation)child.Clone();
            }
          }
          options.RemoveAt(bestOption);
        }
      }
      Context.IncrementEvaluatedSolutions(evaluations);

      if (VALIDATE && bestChild != null && !bestChild.Validate()) throw new ArgumentException("Relinking produced invalid child");
      if (VALIDATE && Dist(child, p2) > 0) throw new InvalidOperationException("Child is not equal to p2 after relinking");
      
      if (bestChild == null) best = eval(child, random);
      return bestChild ?? child;
    }

    public Encodings.PermutationEncoding.Permutation RelinkShift(IRandom random, Encodings.PermutationEncoding.Permutation p1, Encodings.PermutationEncoding.Permutation p2, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, out double best) {
      var evaluations = 0;
      var child = (Encodings.PermutationEncoding.Permutation)p1.Clone();

      best = double.NaN;
      Encodings.PermutationEncoding.Permutation bestChild = null;

      var invChild = new int[child.Length];
      for (var i = 0; i < child.Length; i++) invChild[child[i]] = i;

      var bestChange = double.NaN;
      do {
        int bestFrom = -1, bestTo = -1;
        bestChange = double.NaN;
        for (var j = 0; j < child.Length; j++) {
          var c = invChild[p2[j]];
          var n = invChild[p2.GetCircular(j + 1)];
          if (n - c == 1 || c == child.Length - 1 && n == 0) continue;

          if (c < n) Shift(child, from: n, to: c + 1);
          else Shift(child, from: c, to: n);
          var moveF = eval(child, random);
          evaluations++;
          if (double.IsNaN(bestChange) || moveF < bestChange) {
            bestChange = moveF;
            bestFrom = c < n ? n : c;
            bestTo = c < n ? c + 1 : n;
          }
          // undo
          if (c < n) Shift(child, from: c + 1, to: n);
          else Shift(child, from: n, to: c);
        }
        if (!double.IsNaN(bestChange)) {
          Shift(child, bestFrom, bestTo);
          for (var i = Math.Min(bestFrom, bestTo); i < Math.Max(bestFrom, bestTo); i++) invChild[child[i]] = i;
          if (double.IsNaN(best) || bestChange < best) {
            best = bestChange;
            bestChild = (Encodings.PermutationEncoding.Permutation)child.Clone();
          }
        }
      } while (!double.IsNaN(bestChange));

      Context.IncrementEvaluatedSolutions(evaluations);
      if (VALIDATE && !bestChild.Validate()) throw new ArgumentException("Relinking produced invalid child");
      if (VALIDATE && Dist(child, p2) > 0) throw new InvalidOperationException("Child is not equal to p2 after relinking");
      return bestChild;
    }

    public Encodings.PermutationEncoding.Permutation RelinkOpt(IRandom random, Encodings.PermutationEncoding.Permutation p1, Encodings.PermutationEncoding.Permutation p2, Func<Encodings.PermutationEncoding.Permutation, IRandom, double> eval, out double best) {
      var evaluations = 0;
      var child = (Encodings.PermutationEncoding.Permutation)p1.Clone();

      best = double.NaN;
      Encodings.PermutationEncoding.Permutation bestChild = null;

      var invChild = new int[child.Length];
      var invP2 = new int[child.Length];
      for (var i = 0; i < child.Length; i++) {
        invChild[child[i]] = i;
        invP2[p2[i]] = i;
      }

      var bestChange = double.NaN;
      var moveQueue = new Queue<Tuple<int, int>>();
      var undoStack = new Stack<Tuple<int, int>>();
      do {
        Queue<Tuple<int, int>> bestQueue = null;
        bestChange = double.NaN;
        //Log("{0}", string.Join(" ", child));
        for (var j = 0; j < p2.Length; j++) {
          if (IsUndirectedEdge(invChild, p2[j], p2.GetCircular(j + 1))) continue;

          var a = invChild[p2[j]];
          var b = invChild[p2.GetCircular(j + 1)];
          if (a > b) { var h = a; a = b; b = h; }
          var aprev = a - 1;
          var bprev = b - 1;
          while (IsUndirectedEdge(invP2, child.GetCircular(aprev), child.GetCircular(aprev + 1))) {
            aprev--;
          }
          while (IsUndirectedEdge(invP2, child.GetCircular(bprev), child.GetCircular(bprev + 1))) {
            bprev--;
          }
          var bnext = b + 1;
          var anext = a + 1;
          while (IsUndirectedEdge(invP2, child.GetCircular(bnext - 1), child.GetCircular(bnext))) {
            bnext++;
          }
          while (IsUndirectedEdge(invP2, child.GetCircular(anext - 1), child.GetCircular(anext))) {
            anext++;
          }
          aprev++; bprev++; anext--; bnext--;

          if (aprev < a && bnext > b) {
            if (aprev < 0) {
              moveQueue.Enqueue(Tuple.Create(a + 1, bnext));
              moveQueue.Enqueue(Tuple.Create(a + 1, a + 1 + (bnext - b)));
            } else {
              moveQueue.Enqueue(Tuple.Create(aprev, b - 1));
              moveQueue.Enqueue(Tuple.Create(b - 1 - (a - aprev), b - 1));
            }
          } else if (aprev < a && bnext == b && bprev == b) {
            moveQueue.Enqueue(Tuple.Create(a + 1, b));
          } else if (aprev < a && bprev < b) {
            moveQueue.Enqueue(Tuple.Create(a + 1, b));
          } else if (aprev == a && anext == a && bnext > b) {
            moveQueue.Enqueue(Tuple.Create(a, b - 1));
          } else if (aprev == a && anext == a && bnext == b && bprev == b) {
            moveQueue.Enqueue(Tuple.Create(a, b - 1));
          } else if (aprev == a && anext == a && bprev < b) {
            moveQueue.Enqueue(Tuple.Create(a + 1, b));
          } else if (anext > a && bnext > b) {
            moveQueue.Enqueue(Tuple.Create(a, b - 1));
          } else if (anext > a && bnext == b && bprev == b) {
            moveQueue.Enqueue(Tuple.Create(a, b - 1));
          } else /*if (anext > a && bprev < b)*/ {
            moveQueue.Enqueue(Tuple.Create(a, bprev - 1));
            moveQueue.Enqueue(Tuple.Create(bprev, b));
          }

          while (moveQueue.Count > 0) {
            var m = moveQueue.Dequeue();
            Opt(child, m.Item1, m.Item2);
            undoStack.Push(m);
          }
          var moveF = eval(child, random);
          evaluations++;
          if (double.IsNaN(bestChange) || moveF < bestChange) {
            bestChange = moveF;
            bestQueue = new Queue<Tuple<int, int>>(undoStack.Reverse());
          }
          // undo
          while (undoStack.Count > 0) {
            var m = undoStack.Pop();
            Opt(child, m.Item1, m.Item2);
          }
        }
        if (!double.IsNaN(bestChange)) {
          while (bestQueue.Count > 0) {
            var m = bestQueue.Dequeue();
            Opt(child, m.Item1, m.Item2);
          }
          for (var i = 0; i < child.Length; i++) invChild[child[i]] = i;
          if (double.IsNaN(best) || bestChange < best) {
            best = bestChange;
            bestChild = (Encodings.PermutationEncoding.Permutation)child.Clone();
          }
        }
      } while (!double.IsNaN(bestChange));

      Context.IncrementEvaluatedSolutions(evaluations);
      
      if (VALIDATE && !bestChild.Validate()) throw new ArgumentException("Relinking produced invalid child");
      if (VALIDATE && Dist(child, p2) > 0) throw new InvalidOperationException("Child is not equal to p2 after relinking");
      return bestChild;
    }

    private static bool IsUndirectedEdge(int[] invP, int a, int b) {
      var d = Math.Abs(invP[a] - invP[b]);
      return d == 1 || d == invP.Length - 1;
    }

    private static void Swap(Encodings.PermutationEncoding.Permutation child, int from, int to) {
      Swap2Manipulator.Apply(child, from, to);
    }

    private static void Shift(Encodings.PermutationEncoding.Permutation child, int from, int to) {
      TranslocationManipulator.Apply(child, from, from, to);
    }

    private static void Opt(Encodings.PermutationEncoding.Permutation child, int from, int to) {
      if (from > to) {
        var h = from;
        from = to;
        to = h;
      }
      InversionManipulator.Apply(child, from, to);
    }
  }
}
