#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public static class ExceptionExtensions {
    public static void FlattenAndHandle(this AggregateException exception, IEnumerable<Type> exceptionsToHandle, Action<Exception> unhandledCallback) {
      try {
        var toHandle = new HashSet<Type>(exceptionsToHandle);
        exception.Flatten().Handle(x => toHandle.Contains(x.GetType()));
      } catch (AggregateException remaining) {
        if (remaining.InnerExceptions.Count == 1) unhandledCallback(remaining.InnerExceptions[0]);
        else unhandledCallback(remaining);
      }
    }
  }
}
