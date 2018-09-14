#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Persistence.Auxiliary {
  /// <summary>
  /// Extension methods for the <see cref="System.String"/> class.
  /// </summary>
  public static class StringExtensions {

    /// <summary>
    /// Enumeration over the substrings when split with a certain delimiter.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>An enumeration over the delimited substrings.</returns>
    public static IEnumerable<string> EnumerateSplit(this string s, char delimiter) {
      int startIdx = 0;
      for (int i = 0; i < s.Length; i++) {
        if (s[i] == delimiter) {
          if (i > startIdx) {
            yield return s.Substring(startIdx, i - startIdx);
          }
          startIdx = i + 1;
        }
      }
      if (startIdx < s.Length)
        yield return s.Substring(startIdx, s.Length - startIdx);
    }

    /// <summary>
    /// Enumeration over the substrings when split with a certain delimiter..
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns>An enumerator over the delimited substrings.</returns>
    public static IEnumerator<string> GetSplitEnumerator(this string s, char delimiter) {
      int startIdx = 0;
      for (int i = 0; i < s.Length; i++) {
        if (s[i] == delimiter) {
          if (i > startIdx) {
            yield return s.Substring(startIdx, i - startIdx);
          }
          startIdx = i + 1;
        }
      }
      if (startIdx < s.Length)
        yield return s.Substring(startIdx, s.Length - startIdx);
    }
  }
}
