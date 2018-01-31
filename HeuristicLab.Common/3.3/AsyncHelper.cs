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

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace HeuristicLab.Common {
  public static class AsyncHelper {
    public static async Task DoAsync(Action<CancellationToken> startAction, CancellationToken cancellationToken) {
      try {
        await Task.Factory.StartNew(
          ct => startAction((CancellationToken)ct),
          cancellationToken,
          cancellationToken,
          TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning,
          TaskScheduler.Default);
      } catch (OperationCanceledException) {
      } catch (AggregateException ae) {
        ae.FlattenAndHandle(new[] { typeof(OperationCanceledException) }, e => ExceptionDispatchInfo.Capture(e).Throw());
      }
    }
  }
}
