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
namespace HeuristicLab.Problems.DataAnalysis {
  [Flags]
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
  public interface IOnlineCalculator {
    OnlineCalculatorError ErrorState { get; }
    double Value { get; }
    void Reset();
    void Add(double original, double estimated);
  }
}
