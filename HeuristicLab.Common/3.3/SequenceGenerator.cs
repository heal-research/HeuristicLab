#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public static class SequenceGenerator {
    /// <summary>
    /// Generates a sequence of evenly spaced points by returning the start value and adding the stepwidth until the end is reached or surpassed.
    /// </summary>
    /// <param name="start">The smallest and first value of the sequence.</param>
    /// <param name="end">The largest and last value of the sequence.</param>
    /// <param name="stepWidth">The step size between subsequent values.</param>
    /// <param name="includeEnd">Determines if the end should be included in the sequence regardless if the end is divisible by the stepwidth.</param>
    /// <returns>A sequence of values from start to end (inclusive)</returns>
    [Obsolete("It is recommended to use the decimal overload to achieve a higher numerical accuracy.")]
    public static IEnumerable<double> GenerateSteps(double start, double end, double stepWidth, bool includeEnd = false) {
      //mkommend: IEnumerable.Cast fails due to boxing and unboxing of the involved types
      // http://referencesource.microsoft.com/#System.Core/System/Linq/Enumerable.cs#27bb217a6d5457ec
      // http://blogs.msdn.com/b/ericlippert/archive/2009/03/19/representation-and-identity.aspx     

      return GenerateSteps((decimal)start, (decimal)end, (decimal)stepWidth, includeEnd).Select<decimal, double>(x => (double)x);
    }

    /// <summary>
    /// Generates a sequence of evenly spaced points by returning the start value and adding the stepwidth until the end is reached or surpassed.
    /// </summary>
    /// <param name="start">The smallest and first value of the sequence.</param>
    /// <param name="end">The largest and last value of the sequence.</param>
    /// <param name="stepWidth">The step size between subsequent values.</param>
    /// /// <param name="includeEnd">Determines if the end should be included in the sequence regardless if the end is divisible by the stepwidth.</param>
    /// <returns>A sequence of values from start to end</returns>
    public static IEnumerable<decimal> GenerateSteps(decimal start, decimal end, decimal stepWidth, bool includeEnd = false) {
      if (stepWidth == 0)
        throw new ArgumentException("The step width cannot be zero.");
      if (start < end && stepWidth < 0)
        throw new ArgumentException("The step width must be larger than zero for increasing sequences (start < end).");
      if (start > end && stepWidth > 0)
        throw new ArgumentException("The step width must be smaller than zero for decreasing sequences (start > end).");

      decimal x = start;
      while (x <= end) {
        yield return x;
        x += stepWidth;
      }
      if (x - stepWidth < end && includeEnd) yield return end;
    }
  }
}