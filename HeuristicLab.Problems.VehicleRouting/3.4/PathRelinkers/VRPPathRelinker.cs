#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which relinks paths between VRP solutions.
  /// </summary>
  [Item("VRPPathRelinker", "An operator which relinks paths between VRP solutions.")]
  [StorableType("C0C17982-BC36-4DF9-8C33-2B6F9A19CA53")]
  public sealed class VRPPathRelinker : SingleObjectivePathRelinker, IGeneralVRPOperator, IStochasticOperator {
    #region Parameter properties
    public IValueParameter<IntValue> IterationsParameter {
      get { return (IValueParameter<IntValue>)Parameters["Iterations"]; }
    }
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueParameter<IntValue> SampleSizeParameter {
      get { return (IValueParameter<IntValue>)Parameters["SampleSize"]; }
    }
    #endregion

    [StorableConstructor]
    private VRPPathRelinker(StorableConstructorFlag _) : base(_) { }
    private VRPPathRelinker(VRPPathRelinker original, Cloner cloner) : base(original, cloner) { }
    public VRPPathRelinker()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Iterations", "The number of iterations the operator should perform.", new IntValue(50)));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ValueParameter<IntValue>("SampleSize", "The number of moves that should be executed.", new IntValue(10)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPPathRelinker(this, cloner);
    }

    public static ItemArray<IItem> Apply(PotvinEncoding initiator, PotvinEncoding guide, PercentValue n, int sampleSize, int iterations, IRandom rand, IVRPProblemInstance problemInstance) {
      if (initiator == null || guide == null)
        throw new ArgumentException("Cannot relink path because one of the provided solutions or both are null.");

      double sigma = 1.5;
      double minPenalty = 0.001;
      double maxPenalty = 1000000000;

      var originalOverloadPenalty = new DoubleValue();
      if (problemInstance is IHomogenousCapacitatedProblemInstance)
        originalOverloadPenalty.Value = (problemInstance as IHomogenousCapacitatedProblemInstance).OverloadPenalty.Value;
      var originalTardinessPenalty = new DoubleValue();
      if (problemInstance is ITimeWindowedProblemInstance)
        originalTardinessPenalty.Value = (problemInstance as ITimeWindowedProblemInstance).TardinessPenalty.Value;

      PotvinEncoding current = MatchTours(initiator, guide, problemInstance);
      double currentSimilarity = VRPSimilarityCalculator.CalculateSimilarity(current, guide);

      IList<PotvinEncoding> solutions = new List<PotvinEncoding>();
      int i = 0;
      while (i < iterations && !currentSimilarity.IsAlmost(1.0)) {
        var currentEval = problemInstance.Evaluate(current);
        currentSimilarity = VRPSimilarityCalculator.CalculateSimilarity(current, guide);

        if (currentSimilarity < 1.0) {
          for (int sample = 0; sample < sampleSize; sample++) {
            var next = current.Clone() as PotvinEncoding;

            int neighborhood = rand.Next(3);
            switch (neighborhood) {
              case 0: next = RouteBasedXOver(next, guide, rand,
                problemInstance);
                break;
              case 1: next = SequenceBasedXOver(next, guide, rand,
                problemInstance);
                break;
              case 2: GuidedRelocateMove(next, guide, rand);
                break;
            }

            next = MatchTours(next, guide, problemInstance);

            var nextEval = problemInstance.Evaluate(next);
            if ((nextEval.Quality < currentEval.Quality)) {
              current = next;
              solutions.Add(current);
              break;
            }
          }

          if (problemInstance is IHomogenousCapacitatedProblemInstance) {
            if (((CVRPEvaluation)currentEval).Overload > 0) {
              (problemInstance as IHomogenousCapacitatedProblemInstance).OverloadPenalty.Value =
                Math.Min(maxPenalty,
                (problemInstance as IHomogenousCapacitatedProblemInstance).OverloadPenalty.Value * sigma);
            } else {
              (problemInstance as IHomogenousCapacitatedProblemInstance).OverloadPenalty.Value =
                Math.Max(minPenalty,
                (problemInstance as IHomogenousCapacitatedProblemInstance).OverloadPenalty.Value * sigma);
            }
          }


          if (problemInstance is ITimeWindowedProblemInstance) {
            if (((CVRPTWEvaluation)currentEval).Tardiness > 0) {
              (problemInstance as ITimeWindowedProblemInstance).TardinessPenalty.Value =
                Math.Min(maxPenalty,
              (problemInstance as ITimeWindowedProblemInstance).TardinessPenalty.Value * sigma);
            } else {
              (problemInstance as ITimeWindowedProblemInstance).TardinessPenalty.Value =
                Math.Max(minPenalty,
                (problemInstance as ITimeWindowedProblemInstance).TardinessPenalty.Value / sigma);
            }
          }

          i++;
        }
      }

      if (problemInstance is IHomogenousCapacitatedProblemInstance)
        (problemInstance as IHomogenousCapacitatedProblemInstance).OverloadPenalty.Value = originalOverloadPenalty.Value;
      if (problemInstance is ITimeWindowedProblemInstance)
        (problemInstance as ITimeWindowedProblemInstance).TardinessPenalty.Value = originalTardinessPenalty.Value;

      return new ItemArray<IItem>(ChooseSelection(solutions, n));
    }

    private static IList<IItem> ChooseSelection(IList<PotvinEncoding> solutions, PercentValue n) {
      IList<IItem> selection = new List<IItem>();
      if (solutions.Count > 0) {
        int noSol = (int)(solutions.Count * n.Value);
        if (noSol <= 0) noSol++;
        double stepSize = (double)solutions.Count / (double)noSol;
        for (int i = 0; i < noSol; i++)
          selection.Add(solutions.ElementAt((int)((i + 1) * stepSize - stepSize * 0.5)));
      }

      return selection;
    }

    protected override ItemArray<IItem> Relink(ItemArray<IItem> parents, PercentValue n) {
      if (parents.Length != 2)
        throw new ArgumentException("The number of parents is not equal to 2.");

      if (!(parents[0] is PotvinEncoding))
        parents[0] = PotvinEncoding.ConvertFrom(parents[0] as IVRPEncoding, ProblemInstanceParameter.ActualValue);
      if (!(parents[1] is PotvinEncoding))
        parents[1] = PotvinEncoding.ConvertFrom(parents[1] as IVRPEncoding, ProblemInstanceParameter.ActualValue);

      return Apply(parents[0] as PotvinEncoding, parents[1] as PotvinEncoding, n,
        SampleSizeParameter.Value.Value, IterationsParameter.Value.Value, RandomParameter.ActualValue, ProblemInstanceParameter.ActualValue);
    }

    private static int MatchingCities(Tour tour1, Tour tour2) {
      return tour1.Stops.Intersect(tour2.Stops).Count();
    }

    private static PotvinEncoding MatchTours(PotvinEncoding initiator, PotvinEncoding guide, IVRPProblemInstance problemInstance) {
      var result = new PotvinEncoding(problemInstance);

      var used = new List<bool>();
      for (int i = 0; i < initiator.Tours.Count; i++) {
        used.Add(false);
      }

      for (int i = 0; i < guide.Tours.Count; i++) {
        int bestMatch = -1;
        int bestTour = -1;

        for (int j = 0; j < initiator.Tours.Count; j++) {
          if (!used[j]) {
            int match = MatchingCities(guide.Tours[i], initiator.Tours[j]);
            if (match > bestMatch) {
              bestMatch = match;
              bestTour = j;
            }
          }
        }

        if (bestTour != -1) {
          result.Tours.Add(initiator.Tours[bestTour]);
          used[bestTour] = true;
        }
      }

      for (int i = 0; i < initiator.Tours.Count; i++) {
        if (!used[i])
          result.Tours.Add(initiator.Tours[i]);
      }

      return result;
    }

    #region moves
    public static PotvinEncoding RouteBasedXOver(PotvinEncoding initiator, PotvinEncoding guide, IRandom random, IVRPProblemInstance problemInstance) {
      return PotvinRouteBasedCrossover.Apply(random, initiator, guide, problemInstance, false);
    }

    public static PotvinEncoding SequenceBasedXOver(PotvinEncoding initiator, PotvinEncoding guide, IRandom random, IVRPProblemInstance problemInstance) {
      return PotvinSequenceBasedCrossover.Apply(random, initiator, guide, problemInstance, false);
    }

    public static void GuidedRelocateMove(PotvinEncoding initiator, PotvinEncoding guide, IRandom random) {
      List<int> cities = new List<int>();
      foreach (Tour tour in initiator.Tours) {
        foreach (int city in tour.Stops) {
          Tour guideTour = guide.Tours.First(t => t.Stops.Contains(city));
          if (guide.Tours.IndexOf(guideTour) != initiator.Tours.IndexOf(tour)) {
            cities.Add(city);
          }
        }
      }

      if (cities.Count == 0) {
        RelocateMove(initiator, random);
      } else {
        int city = cities[random.Next(cities.Count)];
        Tour tour = initiator.Tours.First(t => t.Stops.Contains(city));
        tour.Stops.Remove(city);

        Tour guideTour = guide.Tours.First(t => t.Stops.Contains(city));
        int guideTourIndex = guide.Tours.IndexOf(guideTour);

        if (guideTourIndex < initiator.Tours.Count) {
          Tour tour2 = initiator.Tours[guideTourIndex];

          int guideIndex = guideTour.Stops.IndexOf(city);
          if (guideIndex == 0) {
            tour2.Stops.Insert(0, city);
          } else {
            int predecessor = guideTour.Stops[guideIndex - 1];
            int initIndex = tour2.Stops.IndexOf(predecessor);
            if (initIndex != -1) {
              tour2.Stops.Insert(initIndex + 1, city);
            } else {
              if (guideIndex == guideTour.Stops.Count - 1) {
                tour2.Stops.Insert(tour2.Stops.Count, city);
              } else {
                int sucessor = guideTour.Stops[guideIndex + 1];
                initIndex = tour2.Stops.IndexOf(sucessor);
                if (initIndex != -1) {
                  tour2.Stops.Insert(initIndex, city);
                } else {
                  tour2.Stops.Insert(random.Next(tour2.Stops.Count + 1), city);
                }
              }
            }
          }
        } else {
          Tour tour2 = new Tour();
          tour2.Stops.Add(city);
          initiator.Tours.Add(tour2);
        }

        if (tour.Stops.Count == 0)
          initiator.Tours.Remove(tour);
      }
    }

    public static void RelocateMove(PotvinEncoding individual, IRandom random) {
      int cities = individual.Cities;
      int city = 1 + random.Next(cities);
      Tour originalTour = individual.Tours.Find(t => t.Stops.Contains(city));
      //consider creating new route
      individual.Tours.Add(new Tour());

      int position = 1 + random.Next(cities + individual.Tours.Count - 1);
      if (position >= city) {
        position++;
      }

      int originalPosition = originalTour.Stops.IndexOf(city);
      originalTour.Stops.RemoveAt(originalPosition);

      Tour insertionTour;
      int insertionPosition;
      if (position <= cities) {
        insertionTour = individual.Tours.Find(t => t.Stops.Contains(position));
        insertionPosition = insertionTour.Stops.IndexOf(position) + 1;
      } else {
        insertionTour = individual.Tours[position - cities - 1];
        insertionPosition = 0;
      }

      insertionTour.Stops.Insert(insertionPosition, city);

      individual.Tours.RemoveAll(t => t.Stops.Count == 0);
    }
    #endregion
  }
}