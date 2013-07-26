#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Tests {
  public static class AlgorithmExtensions {
    public static void StartSync(this IExecutable executable, CancellationToken cancellationToken) {
      var executor = new AlgorithmExecutor(executable, cancellationToken);
      executor.StartSync();
    }
  }

  /// <summary>
  /// Can execute an algorithm synchronously
  /// </summary>
  internal class AlgorithmExecutor {
    private IExecutable executable;
    private AutoResetEvent waitHandle = new AutoResetEvent(false);
    private CancellationToken cancellationToken;
    private Exception occuredException;

    public AlgorithmExecutor(IExecutable executable, CancellationToken cancellationToken) {
      this.executable = executable;
      this.cancellationToken = cancellationToken;
      this.occuredException = null;
    }

    public void StartSync() {
      executable.Stopped += new EventHandler(executable_Stopped);
      executable.Paused += new EventHandler(executable_Paused);
      executable.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(executable_ExceptionOccurred);

      using (CancellationTokenRegistration registration = cancellationToken.Register(new Action(cancellationToken_Canceled))) {
        executable.Start();
        waitHandle.WaitOne(-1, false);
        waitHandle.Dispose();
      }

      executable.Stopped -= new EventHandler(executable_Stopped);
      executable.Paused -= new EventHandler(executable_Paused);
      if (executable.ExecutionState == ExecutionState.Started) {
        executable.Pause();
      }
      cancellationToken.ThrowIfCancellationRequested();
      if (occuredException != null) throw occuredException;
    }

    private void executable_Paused(object sender, EventArgs e) {
      waitHandle.Set();
    }

    private void executable_Stopped(object sender, EventArgs e) {
      waitHandle.Set();
    }

    private void executable_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      occuredException = e.Value; // after an exception occured the executable pauses
    }

    private void cancellationToken_Canceled() {
      waitHandle.Set();
    }
  }
}