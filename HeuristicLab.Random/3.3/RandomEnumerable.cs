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
using HeuristicLab.Core;

namespace HeuristicLab.Random {
  public static class RandomEnumerable {
    public static IEnumerable<int> SampleRandomNumbers(int maxElement, int count) {
      return SampleRandomNumbers(Environment.TickCount, 0, maxElement, count);
    }

    public static IEnumerable<int> SampleRandomNumbers(int start, int end, int count) {
      return SampleRandomNumbers(Environment.TickCount, start, end, count);
    }

    //algorithm taken from progamming pearls page 127
    //IMPORTANT because IEnumerables with yield are used the seed must be specified to return always 
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

    /// <summary>
    /// Chooses one elements from a sequence giving each element an equal chance.
    /// </summary>
    /// <remarks>
    /// Runtime complexity is O(1) for sequences that are of type <see cref="IList{T}"/> and
    /// O(N) for all other.
    /// </remarks>
    /// <exception cref="ArgumentException">If the sequence is empty.</exception>
    /// <typeparam name="T">The type of the items to be selected.</typeparam>
    /// <param name="source">The sequence of elements.</param>
    /// <param name="random">The random number generator to use, its NextDouble() method must produce values in the range [0;1)</param>
    /// <param name="count">The number of items to be selected.</param>
    /// <returns>An element that has been chosen randomly from the sequence.</returns>
    public static T SampleRandom<T>(this IEnumerable<T> source, IRandom random) {
      if (!source.Any()) throw new ArgumentException("sequence is empty.", "source");
      return source.SampleRandom(random, 1).First();
    }

    /// <summary>
    /// Chooses <paramref name="count"/> elements from a sequence with repetition with equal chances for each element.
    /// </summary>
    /// <remarks>
    /// Runtime complexity is O(count) for sequences that are <see cref="IList{T}"/> and
    /// O(N * count) for all other. No exception is thrown if the sequence is empty.
    /// 
    /// The method is online.
    /// </remarks>
    /// <typeparam name="T">The type of the items to be selected.</typeparam>
    /// <param name="source">The sequence of elements.</param>
    /// <param name="random">The random number generator to use, its NextDouble() method must produce values in the range [0;1)</param>
    /// <param name="count">The number of items to be selected.</param>
    /// <returns>A sequence of elements that have been chosen randomly.</returns>
    public static IEnumerable<T> SampleRandom<T>(this IEnumerable<T> source, IRandom random, int count) {
      var listSource = source as IList<T>;
      if (listSource != null) {
        while (count > 0) {
          yield return listSource[random.Next(listSource.Count)];
          count--;
        }
      } else {
        while (count > 0) {
          var enumerator = source.GetEnumerator();
          enumerator.MoveNext();
          T selectedItem = enumerator.Current;
          int counter = 1;
          while (enumerator.MoveNext()) {
            counter++;
            if (counter * random.NextDouble() < 1.0)
              selectedItem = enumerator.Current;
          }
          yield return selectedItem;
          count--;
        }
      }
    }

    /// <summary>
    /// Chooses <paramref name="count"/> elements from a sequence without repetition with equal chances for each element.
    /// The items are returned in the same order as they appear in the sequence.
    /// </summary>
    /// <remarks>
    /// Runtime complexity is O(N) for all sequences.
    /// No exception is thrown if the sequence contains less items than there are to be selected.
    /// 
    /// The method is online.
    /// </remarks>
    /// <typeparam name="T">The type of the items to be selected.</typeparam>
    /// <param name="source">The sequence of elements.</param>
    /// <param name="random">The random number generator to use, its NextDouble() method must produce values in the range [0;1)</param>
    /// <param name="count">The number of items to be selected.</param>
    /// <param name="sourceCount">Optional parameter specifying the number of elements in the source enumerations</param>
    /// <returns>A sequence of elements that have been chosen randomly.</returns>
    public static IEnumerable<T> SampleRandomWithoutRepetition<T>(this IEnumerable<T> source, IRandom random, int count, int sourceCount = -1) {
      if (sourceCount == -1) sourceCount = source.Count();
      int remaining = sourceCount;
      foreach (var item in source) {
        if (random.NextDouble() * remaining < count) {
          count--;
          yield return item;
          if (count <= 0) break;
        }
        remaining--;
      }
    }

    /// <summary>
    /// Chooses elements out of a sequence with repetition. The chance that an item is selected is proportional or inverse-proportional
    /// to the <paramref name="weights"/>.
    /// </summary>
    /// <remarks>
    /// In case both <paramref name="inverseProportional"/> and <paramref name="windowing"/> are false values must be &gt; 0,
    /// otherwise an InvalidOperationException is thrown.
    /// 
    /// The method internally holds two arrays: One that is the sequence itself and another one for the values.
    /// </remarks>
    /// <typeparam name="T">The type of the items to be selected.</typeparam>
    /// <param name="source">The sequence of elements.</param>
    /// <param name="random">The random number generator to use, its NextDouble() method must produce values in the range [0;1)</param>
    /// <param name="count">The number of items to be selected.</param>
    /// <param name="weights">The weight values for the items.</param>
    /// <param name="windowing">Whether to scale the proportional values or not.</param>
    /// <param name="inverseProportional">Determines whether to choose proportionally (false) or inverse-proportionally (true).</param>
    /// <returns>A sequence of selected items. The sequence might contain the same item more than once.</returns>
    public static IEnumerable<T> SampleProportional<T>(this IEnumerable<T> source, IRandom random, int count, IEnumerable<double> weights, bool windowing = true, bool inverseProportional = false) {
      return source.SampleProportional(random, weights, windowing, inverseProportional).Take(count);
    }

    /// <summary>
    /// Same as <seealso cref="SampleProportional<T>"/>, but chooses an item exactly once.
    /// </summary>
    /// <remarks>
    /// In case both <paramref name="inverseProportional"/> and <paramref name="windowing"/> are false values must be &gt; 0,
    /// otherwise an InvalidOperationException is thrown.
    /// 
    /// The method internally holds two arrays: One that is the sequence itself and another one for the values.
    /// 
    /// The method does not check if the number of elements in source and weights are the same.
    /// </remarks>
    /// <typeparam name="T">The type of the items to be selected.</typeparam>
    /// <param name="source">The sequence of elements.</param>
    /// <param name="random">The random number generator to use, its NextDouble() method must produce values in the range [0;1)</param>
    /// <param name="count">The number of items to be selected.</param>
    /// <param name="weights">The weight values for the items.</param>
    /// <param name="windowing">Whether to scale the proportional values or not.</param>
    /// <param name="inverseProportional">Determines whether to choose proportionally (true) or inverse-proportionally (false).</param>
    /// <returns>A sequence of selected items. Might actually be shorter than <paramref name="count"/> elements if source has less than <paramref name="count"/> elements.</returns>
    public static IEnumerable<T> SampleProportionalWithoutRepetition<T>(this IEnumerable<T> source, IRandom random, int count, IEnumerable<double> weights, bool windowing = true, bool inverseProportional = false) {
      return source.SampleProportionalWithoutRepetition(random, weights, windowing, inverseProportional).Take(count);
    }
    #region Proportional Helpers
    private static IEnumerable<T> SampleProportional<T>(this IEnumerable<T> source, IRandom random, IEnumerable<double> weights, bool windowing, bool inverseProportional) {
      var sourceArray = source.ToArray();
      var valueArray = PrepareProportional(weights, windowing, inverseProportional);
      double total = valueArray.Sum();

      while (true) {
        int index = 0;
        double ball = valueArray[index], sum = random.NextDouble() * total;
        while (ball < sum)
          ball += valueArray[++index];
        yield return sourceArray[index];
      }
    }
    private static IEnumerable<T> SampleProportionalWithoutRepetition<T>(this IEnumerable<T> source, IRandom random, IEnumerable<double> weights, bool windowing, bool inverseProportional) {
      var valueArray = PrepareProportional(weights, windowing, inverseProportional);
      var list = new LinkedList<Tuple<T, double>>(source.Zip(valueArray, Tuple.Create));
      double total = valueArray.Sum();

      while (list.Count > 0) {
        var cur = list.First;
        double ball = cur.Value.Item2, sum = random.NextDouble() * total; // assert: sum < total. When there is only one item remaining: sum < ball
        while (ball < sum) {
          cur = cur.Next;
          ball += cur.Value.Item2;
        }
        yield return cur.Value.Item1;
        list.Remove(cur);
        total -= cur.Value.Item2;
      }
    }

    private static double[] PrepareProportional(IEnumerable<double> weights, bool windowing, bool inverseProportional) {
      double maxValue = double.MinValue, minValue = double.MaxValue;
      double[] valueArray = weights.ToArray();

      for (int i = 0; i < valueArray.Length; i++) {
        if (valueArray[i] > maxValue) maxValue = valueArray[i];
        if (valueArray[i] < minValue) minValue = valueArray[i];
      }
      if (minValue == maxValue) {  // all values are equal
        for (int i = 0; i < valueArray.Length; i++) {
          valueArray[i] = 1.0;
        }
      } else {
        if (windowing) {
          if (inverseProportional) InverseProportionalScale(valueArray, maxValue);
          else ProportionalScale(valueArray, minValue);
        } else {
          if (minValue < 0.0) throw new InvalidOperationException("Proportional selection without windowing does not work with values < 0.");
          if (inverseProportional) InverseProportionalScale(valueArray, 2 * maxValue);
        }
      }
      return valueArray;
    }
    private static void ProportionalScale(double[] values, double minValue) {
      for (int i = 0; i < values.Length; i++) {
        values[i] = values[i] - minValue;
      }
    }
    private static void InverseProportionalScale(double[] values, double maxValue) {
      for (int i = 0; i < values.Length; i++) {
        values[i] = maxValue - values[i];
      }
    }
    #endregion

    /// <summary>
    /// Shuffles an enumerable and returns a new enumerable according to the Fisher-Yates shuffle.
    /// </summary>
    /// <remarks>
    /// Note that the source enumerable is transformed into an array.
    /// 
    /// The implementation is described in http://stackoverflow.com/questions/1287567/c-is-using-random-and-orderby-a-good-shuffle-algorithm.
    /// </remarks>
    /// <typeparam name="T">The type of the items that are to be shuffled.</typeparam>
    /// <param name="source">The enumerable that contains the items.</param>
    /// <param name="random">The random number generator, its Next(n) method must deliver uniformly distributed random numbers in the range [0;n).</param>
    /// <returns>An enumerable with the elements shuffled.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, IRandom random) {
      T[] elements = source.ToArray();
      for (int i = elements.Length - 1; i > 0; i--) {
        // Swap element "i" with a random earlier element (including itself)
        int swapIndex = random.Next(i + 1);
        yield return elements[swapIndex];
        elements[swapIndex] = elements[i];
        // we don't actually perform the swap, we can forget about the
        // swapped element because we already returned it.
      }
      yield return elements[0];
    }
  }
}

