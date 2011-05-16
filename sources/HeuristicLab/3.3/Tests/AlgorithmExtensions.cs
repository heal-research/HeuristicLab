#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab_33.Tests {
  public static class AlgorithmExtensions {
    public static void StartSync(this IAlgorithm algorithm, CancellationToken cancellationToken) {
      var executor = new AlgorithmExecutor(algorithm, cancellationToken);
      executor.StartSync();
    }
  }

  /// <summary>
  /// Can execute an algorithm synchronously
  /// </summary>
  internal class AlgorithmExecutor {
    private IAlgorithm algorithm;
    private AutoResetEvent waitHandle = new AutoResetEvent(false);
    private CancellationToken cancellationToken;

    public AlgorithmExecutor(IAlgorithm algorithm, CancellationToken cancellationToken) {
      this.algorithm = algorithm;
      this.cancellationToken = cancellationToken;
    }

    public void StartSync() {
      algorithm.Stopped += new EventHandler(algorithm_Stopped);
      algorithm.Paused += new EventHandler(algorithm_Paused);

      using (CancellationTokenRegistration registration = cancellationToken.Register(new Action(cancellationToken_Canceled))) {
        algorithm.Start();
        waitHandle.WaitOne();
        waitHandle.Dispose();
      }

      algorithm.Stopped -= new EventHandler(algorithm_Stopped);
      algorithm.Paused -= new EventHandler(algorithm_Paused);
      if (algorithm.ExecutionState == ExecutionState.Started) {
        algorithm.Pause();
      }
      cancellationToken.ThrowIfCancellationRequested();
    }

    private void algorithm_Paused(object sender, EventArgs e) {
      waitHandle.Set();
    }

    private void algorithm_Stopped(object sender, EventArgs e) {
      waitHandle.Set();
    }

    private void cancellationToken_Canceled() {
      waitHandle.Set();
    }
  }
}
