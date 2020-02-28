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
namespace HeuristicLab.Common {
  public static class DoubleExtensions {
    /// <summary>
    /// Compares the similarity of value x and value y with a precision of 1.0E-12.
    /// </summary>
    /// <param name="x">First double value to be checked</param>
    /// <param name="y">Second double value to compare with</param>
    /// <returns>true if the difference is <= 1.0E-12</returns>
    public static bool IsAlmost(this double x, double y) {
      var epsilon = 1.0E-12;
      return IsAlmost(x, y, epsilon);
    }

    /// <summary> 
    /// Compares the similarity of value x and value y with a given precision (epsilon).
    /// </summary>
    /// <param name="x">First double value to be checked</param>
    /// <param name="y">Second double value to compare with</param>
    /// <param name="epsilon">Error term to specify the precision</param>
    /// <returns>true if the difference is <= epsilon</returns>
    public static bool IsAlmost(this double x, double y, double epsilon) {
      if (double.IsInfinity(x)) {
        if (x > 0) return double.IsPositiveInfinity(y);
        else return double.IsNegativeInfinity(y);
      } else {
        return Math.Abs(x - y) < epsilon;
      }
    }
  }
}
