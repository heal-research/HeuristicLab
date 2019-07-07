#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.MainForm {
  public enum ProgressState { Started, Finished, StopRequested, CancelRequested }
  public enum ProgressMode { Determinate, Indeterminate }

  public interface IProgress : IContent {
    ProgressState ProgressState { get; }

    string Message { get; set; }

    ProgressMode ProgressMode { get; set; }
    /// <summary>
    /// Gets or sets the currently associated progress value in the range [0;1] (values outside the range are truncated).
    /// Changing the ProgressValue when <c>ProgressMode</c> is <c>Indeterminate</c> raises an Exception.
    /// </summary>
    /// <exception cref="InvalidOperationException">Setting the ProgressValue-property while in the Indeterminate state is invalid.</exception>
    double ProgressValue { get; set; }

    bool CanBeStopped { get; set; }
    bool CanBeCanceled { get; set; }

    void Start(string message, ProgressMode mode = ProgressMode.Determinate);
    void Finish();
    void Stop();
    void Cancel();

    event EventHandler ProgressStateChanged;
    event EventHandler MessageChanged;
    event EventHandler ProgressBarModeChanged;
    event EventHandler ProgressValueChanged;
    event EventHandler CanBeStoppedChanged;
    event EventHandler CanBeCanceledChanged;
    event EventHandler StopRequested;
    event EventHandler CancelRequested;
  }
}
