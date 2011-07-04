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

namespace HeuristicLab.Problems.DataAnalysis.Classification {
  public class OnlineAccuracyEvaluator : IOnlineEvaluator {
    private int n;
    private int correctClassified; // TP + TN necessary for multi class classification

    public OnlineAccuracyEvaluator() {
      Reset();
    }

    public void Add(double original, double estimated) {
      if (double.IsNaN(estimated) || double.IsInfinity(estimated) ||
          double.IsNaN(original) || double.IsInfinity(original))
        throw new ArgumentException("Accuracy is not defined for NaN or infinity elements");

      if (original.IsAlmost(estimated))
        correctClassified++;
      n++;
    }

    public void Reset() {
      n = 0;
      correctClassified = 0;
    }

    public double Accuracy {
      get {
        if (n < 1)
          throw new InvalidOperationException("No elements");
        else
          return ((double)correctClassified) / n;
      }
    }
    public double Value {
      get { return Accuracy; }
    }

  }
}
