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
      private set {
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
      progressState = ProgressState.Finished;
      canBeCanceled = false;
    }
    public Progress(string status)
      : this() {
      this.status = status;
    }
    public Progress(string status, ProgressState state)
      : this() {
      this.status = status;
      this.progressState = state;
    }

    public void Cancel() {
      if (canBeCanceled)
        OnCancelRequested();
    }

    public void Finish() {
      if (ProgressValue != 1.0) ProgressValue = 1.0;
      ProgressState = ProgressState.Finished;
    }

    public void Start() {
      ProgressValue = 0.0;
      ProgressState = ProgressState.Started;
    }

    public void Start(string status) {
      Start();
      Status = status;
    }

    #region Event Handler
    public event EventHandler StatusChanged;
    private void OnStatusChanged() {
      var handler = StatusChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ProgressValueChanged;
    private void OnProgressChanged() {
      var handler = ProgressValueChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ProgressStateChanged;
    private void OnProgressStateChanged() {
      var handler = ProgressStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CanBeCanceledChanged;
    private void OnCanBeCanceledChanged() {
      var handler = CanBeCanceledChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CancelRequested;
    private void OnCancelRequested() {
      var handler = CancelRequested;
      if (handler != null) throw new NotSupportedException("Cancel request was ignored.");
      else handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
