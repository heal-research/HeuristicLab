#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text.RegularExpressions;

namespace HeuristicLab.Common {
  public class NaturalStringComparer : IComparer<string> {
    public int Compare(string x, string y) {
      if (x == y) return 0;

      string[] first = Regex.Split(x, "([0-9]+)");
      string[] second = Regex.Split(y, "([0-9]+)");

      for (int i = 0; i < first.Length && i < second.Length; i++) {
        if (first[i] != second[i]) return CompareWithParsing(first[i], second[i]);
      }

      if (first.Length < second.Length) return -1;
      if (second.Length < first.Length) return 1;
      return 0;
    }

    private int CompareWithParsing(string x, string y) {
      int first, second;
      if (int.TryParse(x, out first) && int.TryParse(y, out second))
        return first.CompareTo(second);
      return x.CompareTo(y);
    }
  }
}
