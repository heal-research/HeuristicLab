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
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  public static class ScopeUtil {
    private const string EvaluationResultName = "EvaluationResult";

    // TODO: Create SolutionContexts that derive from IScope to have a unified datastructure (e.g. #1614)
    public static TEncodedSolution CopyEncodedSolutionToScope<TEncodedSolution>(IScope scope, IEncoding<TEncodedSolution> encoding, TEncodedSolution solution)
      where TEncodedSolution : class, IEncodedSolution {
      return CopyEncodedSolutionToScope(scope, encoding.Name, solution);
    }

    public static TEncodedSolution CopyEncodedSolutionToScope<TEncodedSolution>(IScope scope, string name, TEncodedSolution solution)
      where TEncodedSolution : class, IEncodedSolution {
      var copy = (TEncodedSolution)solution.Clone();
      if (!scope.Variables.ContainsKey(name)) scope.Variables.Add(new Variable(name, copy));
      else scope.Variables[name].Value = copy;
      return copy;
    }

    public static TEncodedSolution GetEncodedSolution<TEncodedSolution>(IScope scope, IEncoding<TEncodedSolution> encoding)
      where TEncodedSolution : class, IEncodedSolution {
      var name = encoding.Name;
      if (!scope.Variables.ContainsKey(name)) throw new ArgumentException(string.Format(" {0} cannot be found in the provided scope.", name));
      var value = scope.Variables[name].Value as TEncodedSolution;
      if (value == null) throw new InvalidOperationException(string.Format("Value of {0} is null or not of type {1}.", name, typeof(TEncodedSolution).GetPrettyName()));
      return value;
    }

    public static IEncodedSolution GetEncodedSolution(IScope scope, IEncoding encoding) {
      return GetEncodedSolution(scope, encoding.Name);
    }

    public static IEncodedSolution GetEncodedSolution(IScope scope, string name) {
      IVariable variable;
      if (!scope.Variables.TryGetValue(name, out variable)) throw new ArgumentException(string.Format("{0} cannot be found in the provided scope.", name));
      var solution = variable.Value as IEncodedSolution;
      if (solution == null) throw new InvalidOperationException(string.Format("{0} is null or not of type ISolution.", name));
      return solution;
    }

    public static ISingleObjectiveSolutionContext<TEncodedSolution> CreateSolutionContext<TEncodedSolution>(IScope scope, IEncoding<TEncodedSolution> encoding)
      where TEncodedSolution : class, IEncodedSolution {
      var solution = GetEncodedSolution(scope, encoding);
      var context = new SingleObjectiveSolutionContext<TEncodedSolution>(solution);
      foreach (var variable in scope.Variables) {
        if (variable.Name != encoding.Name)
          context.SetAdditionalData(variable.Name, variable.Value);
      }
      if (scope.Variables.TryGetValue(EvaluationResultName, out var variable2)) {
        context.EvaluationResult = (ISingleObjectiveEvaluationResult)variable2.Value;
      }
      return context;
    }

    public static void CopyToScope<TEncodedSolution>(IScope scope, ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext)
      where TEncodedSolution : class, IEncodedSolution {
      foreach (var item in solutionContext.AdditionalData) {
        if (item.Value is IItem i) {
          if (!scope.Variables.TryGetValue(item.Key, out var variable))
            scope.Variables.Add(new Variable(item.Key, i));
          else variable.Value = i;
        }
      }
      if (!scope.Variables.TryGetValue(EvaluationResultName, out var variable2)) {
        scope.Variables.Add(new Variable(EvaluationResultName, solutionContext.EvaluationResult));
      } else variable2.Value = solutionContext.EvaluationResult;
    }
  }
}
