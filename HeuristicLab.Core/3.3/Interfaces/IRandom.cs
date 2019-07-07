using HEAL.Attic;
#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core {
  [StorableType("a25f7f50-5838-4312-862d-9ad950b616c5")]
  /// <summary>
  /// Represents an interface for random number generators.
  /// </summary>
  public interface IRandom : IItem {
    /// <summary>
    /// Resets the random number generator.
    /// </summary>
    void Reset();
    /// <summary>
    /// Resets the random number generator with the given <paramref name="seed"/>.
    /// </summary>
    /// <param name="seed">The new seed.</param>
    void Reset(int seed);

    /// <summary>
    /// Gets a new random number.
    /// </summary>
    /// <returns>A random integer number.</returns>
    int Next();
    /// <summary>
    /// Gets a new random number between 0 and <paramref name="maxVal"/>.
    /// </summary>
    /// <param name="maxVal">The maximal value of the random number (exclusive).</param>
    /// <returns>A random integer number smaller than <paramref name="maxVal"/>.</returns>
    int Next(int maxVal);
    /// <summary>
    /// Gets a new random number between <paramref name="minVal"/> and <paramref name="maxVal"/>.
    /// </summary>
    /// <param name="maxVal">The maximal value of the random number (exclusive).</param>
    /// <param name="minVal">The minimal value of the random number (inclusive).</param>
    /// <returns>A random integer number. (<paramref name="minVal"/> &lt;= x &lt; <paramref name="maxVal"/>).</returns>
    int Next(int minVal, int maxVal);
    /// <summary>
    /// Gets a new double random number.
    /// </summary>
    /// <returns>A random double number.</returns>
    double NextDouble();
  }
}
