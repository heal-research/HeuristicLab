#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text.RegularExpressions;

namespace HeuristicLab.PluginInfrastructure {
  public static class ArgumentHandling {
    public const string StartToken = "start";

    public static IArgument[] GetArguments(string[] args) {
      var arguments = new HashSet<IArgument>();
      foreach (var entry in args) {
        var argument = ParseArgument(entry);
        if (argument != null && argument.Valid) arguments.Add(argument);
      }
      return arguments.ToArray();
    }

    private static Argument ParseArgument(string entry) {
      var regex = new Regex("^/[a-z]+(:[A-Za-z0-9 ]+)?$");
      if (!regex.IsMatch(entry)) return null;
      entry = entry.Remove(0, 1);
      var parts = entry.Split(':');
      string key = parts[0];
      string value = parts.Length == 2 ? parts[1].Trim() : string.Empty;
      switch (key) {
        case StartToken: return new StartArgument(value);
        default: return null;
      }
    }
  }
}
