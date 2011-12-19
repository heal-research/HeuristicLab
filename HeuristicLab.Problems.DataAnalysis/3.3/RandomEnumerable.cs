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
using System.Collections.Generic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis {
  public class RandomEnumerable {
    public static IEnumerable<int> SampleRandomNumbers(int maxElement, int count) {
      return SampleRandomNumbers(Environment.TickCount, 0, maxElement, count);
    }

    public static IEnumerable<int> SampleRandomNumbers(int start, int end, int count) {
      return SampleRandomNumbers(Environment.TickCount, start, end, count);
    }

    //algorithm taken from progamming pearls page 127
    //IMPORTANT because IEnumerables with yield are used the seed must best be specified to return always 
    //the same sequence of numbers without caching the values.
    public static IEnumerable<int> SampleRandomNumbers(int seed, int start, int end, int count) {
      int remaining = end - start;
      var mt = new FastRandom(seed);
      for (int i = start; i < end && count > 0; i++) {
        double probability = mt.NextDouble();
        if (probability < ((double)count) / remaining) {
          count--;
          yield return i;
        }
        remaining--;
      }
    }
  }
}
