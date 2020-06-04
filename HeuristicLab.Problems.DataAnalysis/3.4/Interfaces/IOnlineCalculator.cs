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
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [Flags]
  [StorableType("8A28DDA1-4814-4B77-9457-0EE930BE9C73")]
  public enum OnlineCalculatorError {
    /// <summary>
    /// No error occurred
    /// </summary>
    None = 0,
    /// <summary>
    /// An invalid value has been added (often +/- Infinity and NaN are invalid values)
    /// </summary>
    InvalidValueAdded = 1,
    /// <summary>
    /// The number of elements added to the evaluator is not sufficient to calculate the result value
    /// </summary>
    InsufficientElementsAdded = 2
  }

  [StorableType("119C8242-3EE7-4C34-A7AC-68ABF76EB11B")]
  public interface IOnlineCalculator {
    OnlineCalculatorError ErrorState { get; }
    double Value { get; }
    void Reset();
    void Add(double original, double estimated);
  }
}
