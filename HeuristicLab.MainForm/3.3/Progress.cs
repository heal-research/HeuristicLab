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

using System;
using HeuristicLab.Common;

namespace HeuristicLab.MainForm {
  public class Progress : IProgress {
    private string status;
    public string Status {
      get { return status; }
      set {
        if (status != value) {
          status = value;
          OnStatusChanged();
        }
      }
    }

    private double progressValue;
    public double ProgressValue {
      get { return progressValue; }
      set {
        if (progressValue != value) {
          progressValue = value;
          OnProgressChanged();
        }
      }
    }

    private ProgressState progressState;
    public ProgressState ProgressState {
      get { return progressState; }
      set {
        if (progressState != value) {
          progressState = value;
          OnProgressStateChanged();
        }
      }
    }

    private bool canBeCanceled;
    public bool CanBeCanceled {
      get { return canBeCanceled; }
      set {
        if (canBeCanceled != value) {
          canBeCanceled = value;
          OnCanBeCanceledChanged();
        }
      }
    }

    public Progress() {
      progressState = ProgressState.Started;
    }
    public Progress(string status)
      : this() {
      this.status = status;
    }
    public Progress(string status, double progressValue)
      : this(status) {
      this.progressValue = progressValue;
    }

    public void Cancel(int timeoutMs) {
      if (canBeCanceled)
        OnCancelRequested(timeoutMs);
    }

    /// <summary>
    /// Sets the ProgressValue to 1 and the ProgressState to Finished.
    /// </summary>
    public void Finish() {
      if (ProgressValue != 1.0) ProgressValue = 1.0;
      ProgressState = ProgressState.Finished;
    }

    #region Event Handler
    public event EventHandler StatusChanged;
    private void OnStatusChanged() {
      var handler = StatusChanged;
      try {
        if (handler != null) handler(this, EventArgs.Empty);
      } catch { }
    }

    public event EventHandler ProgressValueChanged;
    private void OnProgressChanged() {
      var handler = ProgressValueChanged;
      try {
        if (handler != null) handler(this, EventArgs.Empty);
      } catch { }
    }

    public event EventHandler ProgressStateChanged;
    private void OnProgressStateChanged() {
      var handler = ProgressStateChanged;
      try {
        if (handler != null) handler(this, EventArgs.Empty);
      } catch { }
    }

    public event EventHandler CanBeCanceledChanged;
    private void OnCanBeCanceledChanged() {
      var handler = CanBeCanceledChanged;
      try {
        if (handler != null) handler(this, EventArgs.Empty);
      } catch { }
    }

    public event EventHandler<EventArgs<int>> CancelRequested;
    private void OnCancelRequested(int timeoutMs) {
      var handler = CancelRequested;
      try {
        if (handler == null) throw new NotSupportedException("Cancel request was ignored.");
        else handler(this, new EventArgs<int>(timeoutMs));
      } catch { }
    }
    #endregion
  }
}
