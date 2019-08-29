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

namespace HeuristicLab.Problems.DataAnalysis {
  public class ConfusionMatrixCalculator {
    public static double[,] Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      if (!originalValues.Any() || !estimatedValues.Any()) {
        errorState = OnlineCalculatorError.InsufficientElementsAdded;
        return null;
      }

      Dictionary<double, int> classValueIndexMapping = new Dictionary<double, int>();
      int index = 0;
      foreach (double classValue in originalValues.OrderBy(x => x)) {
        if (!classValueIndexMapping.ContainsKey(classValue)) {
          classValueIndexMapping.Add(classValue, index);
          index++;
        }
      }

      int classes = classValueIndexMapping.Count;
      double[,] confusionMatrix = new double[classes, classes];


      IEnumerator<double> originalEnumerator = originalValues.GetEnumerator();
      IEnumerator<double> estimatedEnumerator = estimatedValues.GetEnumerator();
      int originalIndex;
      int estimatedIndex;
      while (originalEnumerator.MoveNext() & estimatedEnumerator.MoveNext()) {
        if (!classValueIndexMapping.TryGetValue(originalEnumerator.Current, out originalIndex)) {
          errorState = OnlineCalculatorError.InvalidValueAdded;
          return null;
        }
        if (!classValueIndexMapping.TryGetValue(estimatedEnumerator.Current, out estimatedIndex)) {
          errorState = OnlineCalculatorError.InvalidValueAdded;
          return null;
        }

        confusionMatrix[estimatedIndex, originalIndex] += 1;
      }

      if (originalEnumerator.MoveNext() || estimatedEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in originalValues and estimatedValues enumerations doesn't match.");

      }

      errorState = OnlineCalculatorError.None;
      return confusionMatrix;
    }
  }
}
