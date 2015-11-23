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
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  public static class ScopeUtil {

    public static TSolution CopySolutionToScope<TSolution>(IScope scope, IEncoding<TSolution> encoding, TSolution solution)
      where TSolution : class,ISolution {
      var name = encoding.Name;
      var copy = (TSolution)solution.Clone();
      if (!scope.Variables.ContainsKey(name)) scope.Variables.Add(new Variable(name, copy));
      else scope.Variables[name].Value = copy;
      return copy;
    }

    public static TSolution GetSolution<TSolution>(IScope scope, IEncoding<TSolution> encoding)
      where TSolution : class,ISolution {
      var name = encoding.Name;
      if (!scope.Variables.ContainsKey(name)) throw new ArgumentException(string.Format(" {0} cannot be found in the provided scope.", name));
      var value = scope.Variables[name].Value as TSolution;
      if (value == null) throw new InvalidOperationException(string.Format("Value of {0} is null.", name));
      return value;
    }

    public static ISolution GetSolution(IScope scope, IEncoding encoding) {
      var name = encoding.Name;
      if (!scope.Variables.ContainsKey(name)) throw new ArgumentException(string.Format(" {0} cannot be found in the provided scope.", name));
      var value = scope.Variables[name].Value as ISolution;
      if (value == null) throw new InvalidOperationException(string.Format("Value of {0} is null.", name));
      return value;
    }

  }
}
