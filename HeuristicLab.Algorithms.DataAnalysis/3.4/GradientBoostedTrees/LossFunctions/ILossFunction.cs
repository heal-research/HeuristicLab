#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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

using System.Collections.Generic;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("588270d5-61ee-4906-b30f-841f64cd6724")]
  // represents an interface for loss functions used by gradient boosting
  // target represents the target vector  (original targets from the problem data, never changed)
  // pred   represents the current vector of predictions (a weighted combination of models learned so far, this vector is updated after each step)
  public interface ILossFunction : IItem {
    // returns the loss of the current prediction vector 
    double GetLoss(IEnumerable<double> target, IEnumerable<double> pred);

    // returns an enumerable of the loss gradient for each row
    IEnumerable<double> GetLossGradient(IEnumerable<double> target, IEnumerable<double> pred);

    // returns the optimal value for the partition of rows stored in idx[startIdx] .. idx[endIdx] inclusive
    double LineSearch(double[] targetArr, double[] predArr, int[] idx, int startIdx, int endIdx);
  }
}


