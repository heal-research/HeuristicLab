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
using System.Collections.Generic;

namespace HeuristicLab.Problems.DataAnalysis {
  public class OnlineAccuracyEvaluator : IOnlineEvaluator {

    private int correctlyClassified;
    private int n;
    public double Accuracy {
      get {
        if (n < 1)
          throw new InvalidOperationException("No elements");
        else
          return correctlyClassified / (double)n;
      }
    }

    public OnlineAccuracyEvaluator() {
      Reset();
    }

    #region IOnlineEvaluator Members
    public double Value {
      get { return Accuracy; }
    }
    public void Reset() {
      n = 0;
      correctlyClassified = 0;
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original)) {
        throw new ArgumentException("Accuracy is not defined for NaN or infinity elements");
      } else {
        if (original.IsAlmost(estimated)) correctlyClassified++;
        n++;
      }
    }
    #endregion

    public static double Calculate(IEnumerable<double> first, IEnumerable<double> second) {
      IEnumerator<double> firstEnumerator = first.GetEnumerator();
      IEnumerator<double> secondEnumerator = second.GetEnumerator();
      OnlineAccuracyEvaluator accuracyEvaluator = new OnlineAccuracyEvaluator();

      // always move forward both enumerators (do not use short-circuit evaluation!)
      while (firstEnumerator.MoveNext() & secondEnumerator.MoveNext()) {
        double estimated = secondEnumerator.Current;
        double original = firstEnumerator.Current;
        accuracyEvaluator.Add(original, estimated);
      }

      // check if both enumerators are at the end to make sure both enumerations have the same length
      if (secondEnumerator.MoveNext() || firstEnumerator.MoveNext()) {
        throw new ArgumentException("Number of elements in first and second enumeration doesn't match.");
      } else {
        return accuracyEvaluator.Accuracy;
      }
    }
  }
}
