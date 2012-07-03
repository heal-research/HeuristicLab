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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public static class ValueGenerator {
    private static FastRandom rand = new FastRandom();

    public static IEnumerable<double> GenerateSteps(double start, double end, double stepWidth) {
      int steps = (int)Math.Round(((end - start) / stepWidth) + 1);
      for (int i = 0; i < steps; i++)
        yield return start + i * stepWidth;
    }

    public static IEnumerable<double> GenerateUniformDistributedValues(int amount, double start, double end) {
      for (int i = 0; i < amount; i++)
        yield return rand.NextDouble() * (end - start) + start;
    }

    public static IEnumerable<double> GenerateNormalDistributedValues(int amount, double mu, double sigma) {
      for (int i = 0; i < amount; i++)
        yield return NormalDistributedRandom.NextDouble(rand, mu, sigma);
    }

    // iterative approach
    public static IEnumerable<IEnumerable<double>> GenerateAllCombinationsOfValuesInLists(List<List<double>> lists) {
      List<List<double>> allCombinations = new List<List<double>>();
      if (lists.Count < 1) {
        return allCombinations;
      }

      List<IEnumerator<double>> enumerators = new List<IEnumerator<double>>();
      foreach (var list in lists) {
        allCombinations.Add(new List<double>());
        enumerators.Add(list.GetEnumerator());
      }

      bool finished = !enumerators.All(x => x.MoveNext());

      while (!finished) {
        GetCurrentCombination(enumerators, allCombinations);
        finished = MoveNext(enumerators, lists);
      }
      return allCombinations;
    }

    private static bool MoveNext(List<IEnumerator<double>> enumerators, List<List<double>> lists) {
      int cur = enumerators.Count - 1;
      while (cur >= 0 && !enumerators[cur].MoveNext()) {
        enumerators[cur] = lists[cur].GetEnumerator();
        enumerators[cur].MoveNext();
        cur--;
      }
      return cur < 0;
    }

    private static void GetCurrentCombination(List<IEnumerator<double>> enumerators, List<List<double>> allCombinations) {
      for (int i = 0; i < enumerators.Count(); i++) {
        allCombinations[i].Add(enumerators[i].Current);
      }
    }

    //recursive approach
    /*public static IEnumerable<IEnumerable<double>> GenerateAllCombinationsOfValuesInLists(List<List<double>> lists) {
      int cur = 0;
      List<double> curCombination = new List<double>();
      List<List<double>> allCombinations = new List<List<double>>();
      for (int i = 0; i < lists.Count; i++) {
        allCombinations.Add(new List<double>());
      }
      if (lists.Count() > cur) {
        foreach (var item in lists[cur]) {
          curCombination.Clear();
          curCombination.Add(item);
          GetCombination(lists, cur + 1, curCombination, allCombinations);
        }
      }
      return allCombinations;
    }

    private static void GetCombination(List<List<double>> lists, int cur, List<double> curCombinations, List<List<double>> allCombinations) {
      if (lists.Count > cur) {
        foreach (var item in lists[cur]) {
          if (curCombinations.Count > cur) {
            curCombinations.RemoveAt(cur);
          }
          curCombinations.Add(item);
          GetCombination(lists, cur + 1, curCombinations, allCombinations);
        }
      } else {
        for (int i = 0; i < curCombinations.Count; i++) {
          allCombinations[i].Add(curCombinations[i]);
        }
      }
    }         */

    //original
    /*public static IEnumerable<IEnumerable<double>> GenerateAllCombinationsOfValuesInLists(List<List<double>> sets) {

      var combinations = new List<List<double>>();

      foreach (var value in sets[0])
        combinations.Add(new List<double> { value });

      foreach (var set in sets.Skip(1))
        combinations = AddListToCombinations(combinations, set);

      IEnumerable<IEnumerable<double>> res = (from i in Enumerable.Range(0, sets.Count)
                                              select (from list in combinations
                                                      select list.ElementAt(i)));

      return res;
    }

    private static List<List<double>> AddListToCombinations
         (List<List<double>> combinations, List<double> set) {
      var newCombinations = from value in set
                            from combination in combinations
                            select new List<double>(combination) { value };

      return newCombinations.ToList();
    }    */
  }
}
