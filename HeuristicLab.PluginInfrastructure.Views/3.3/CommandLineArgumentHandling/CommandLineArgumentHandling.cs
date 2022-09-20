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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HeuristicLab.PluginInfrastructure {
  public static class CommandLineArgumentHandling {
    public static ICommandLineArgument[] GetArguments(string[] args) {
      var arguments = new HashSet<ICommandLineArgument>();
      var exceptions = new List<Exception>();

      foreach (var entry in args) {
        var argument = ParseArgument(entry);
        if (argument != null && argument.Valid) arguments.Add(argument);
        else exceptions.Add(new ArgumentException(string.Format("The argument \"{0}\" is invalid.", entry)));
      }

      if (exceptions.Any()) throw new AggregateException("One or more arguments are invalid.", exceptions);
      return arguments.ToArray();
    }

    private static ICommandLineArgument ParseArgument(string entry) {
      var regex = new Regex(@"^/[A-Za-z]+(:\w[\w\s]*)?$");
      bool isFile = File.Exists(entry);
      if (!regex.IsMatch(entry) && !isFile) return null;
      if (!isFile) {
        entry = entry.Remove(0, 1);
        var parts = entry.Split(':');
        string key = parts[0].Trim();
        string value = parts.Length == 2 ? parts[1].Trim() : string.Empty;
        switch (key) {
          case StartArgument.TOKEN: return new StartArgument(value);
          case HideStarterArgument.TOKEN: return new HideStarterArgument(value);
          default: return null;
        }
      } else return new OpenArgument(entry);
    }
  }
}
