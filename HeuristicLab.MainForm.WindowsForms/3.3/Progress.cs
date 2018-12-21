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
using System.Windows.Forms;
using HeuristicLab.Common;

namespace HeuristicLab.MainForm {
  public class Progress : IProgress {
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

    private string message;
    public string Message {
      get { return message; }
      set {
        if (message != value) {
          message = value;
          OnMessageChanged();
        }
      }
    }

    private ProgressMode progressMode;
    public ProgressMode ProgressMode {
      get { return progressMode; }
      set {
        if (progressMode != value) {
          progressMode = value;
          OnProgressBarModeChanged();
        }
      }
    }

    private double progressValue;
    public double ProgressValue {
      get { return progressValue; }
      set {
        if (progressMode == ProgressMode.Indeterminate)
          throw new InvalidOperationException("Cannot set ProgressValue while ProgressBar is in Indeterminate-Mode");
        if (progressValue != value) {
          progressValue = Math.Max(Math.Min(value, 1.0), 0.0);
          OnProgressChanged();
        }
      }
    }

    private bool canBeStopped;
    public bool CanBeStopped {
      get { return canBeStopped; }
      set {
        if (canBeStopped != value) {
          canBeStopped = value;
          OnCanBeStoppedChanged();
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
      canBeStopped = false;
      canBeCanceled = false;
      progressMode = ProgressMode.Indeterminate;
      progressValue = 0.0;
    }

    public void Start(string message, ProgressMode mode = ProgressMode.Determinate) {
      ProgressState = ProgressState.Started;
      ProgressMode = mode;
      if (mode == ProgressMode.Determinate)
        ProgressValue = 0.0;
      Message = message;
    }

    public void Finish() {
      if (ProgressMode == ProgressMode.Determinate && ProgressValue != 1.0)
        ProgressValue = 1.0;
      ProgressState = ProgressState.Finished;
    }

    public void Stop() {
      if (canBeStopped) {
        ProgressState = ProgressState.StopRequested;
        OnStopRequested();
      } else throw new NotSupportedException("This progress cannot be stopped.");
    }
    public void Cancel() {
      if (canBeCanceled) {
        ProgressState = ProgressState.CancelRequested;
        OnCancelRequested();
      } else throw new NotSupportedException("This progress cannot be canceled.");
    }

    #region Show and Hide Progress
    /// <summary>
    /// Shows a started Progress on all Views of the specified content.
    /// </summary>
    public static IProgress Show(IContent content, string progressMessage, ProgressMode mode = ProgressMode.Determinate, Action stopRequestHandler = null, Action cancelRequestHandler = null, bool addToObjectGraphObjects = true) {
      var progress = CreateAndStartProgress(progressMessage, mode, stopRequestHandler, cancelRequestHandler);
      Show(content, progress, addToObjectGraphObjects);
      return progress;
    }

    /// <summary>
    /// Shows a started Progress on the specified view.
    /// </summary>
    public static IProgress Show(IView view, string progressMessage, ProgressMode mode = ProgressMode.Determinate, Action stopRequestHandler = null, Action cancelRequestHandler = null) {
      var progress = CreateAndStartProgress(progressMessage, mode, stopRequestHandler, cancelRequestHandler);
      Show(view, progress);
      return progress;
    }
    /// <summary>
    /// Shows a started Progress on the specified control.
    /// </summary>
    /// <remarks>For backwards compatibility. Use Progress.Show(IView, ...) if possible.</remarks>
    public static IProgress ShowOnControl(Control control, string progressMessage, ProgressMode mode = ProgressMode.Determinate, Action stopRequestHandler = null, Action cancelRequestHandler = null) {
      var progress = CreateAndStartProgress(progressMessage, mode, stopRequestHandler, cancelRequestHandler);
      ShowOnControl(control, progress);
      return progress;
    }

    private static IProgress CreateAndStartProgress(string progressMessage, ProgressMode mode, Action stopRequestHandler, Action cancelRequestHandler) {
      var progress = new Progress();
      if (stopRequestHandler != null) {
        progress.CanBeStopped = true;
        progress.StopRequested += (s, a) => stopRequestHandler();
      }
      if (cancelRequestHandler != null) {
        progress.CanBeCanceled = true;
        progress.CancelRequested += (s, a) => cancelRequestHandler();
      }
      progress.Start(progressMessage, mode);
      return progress;
    }

    /// <summary>
    /// Shows an existing progress on all Views of the specified content.
    /// </summary>
    public static IProgress Show(IContent content, IProgress progress, bool addToObjectGraphObjects = true) {
      MainFormManager.GetMainForm<WindowsForms.MainForm>().AddProgressToContent(content, progress, addToObjectGraphObjects);
      return progress;
    }
    /// <summary>
    /// Shows an existing progress on the specified View.
    /// </summary>
    public static IProgress Show(IView view, IProgress progress) {
      return ShowOnControl((Control)view, progress);
    }
    /// <summary>
    /// Shows an existing progress on the specified control.
    /// </summary>
    /// <remarks>For backwards compatibility. Use Progress.Show(IView, ...) if possible.</remarks>
    public static IProgress ShowOnControl(Control control, IProgress progress) {
      MainFormManager.GetMainForm<WindowsForms.MainForm>().AddProgressToControl(control, progress);
      return progress;
    }

    /// <summary>
    /// Hides the Progress from all Views of the specified content.
    /// </summary>
    public static void Hide(IContent content, bool finishProgress = true) {
      MainFormManager.GetMainForm<WindowsForms.MainForm>().RemoveProgressFromContent(content, finishProgress);
    }
    /// <summary>
    /// Hides the Progress from the specified view.
    /// </summary>
    public static void Hide(IView view, bool finishProgress = true) {
      HideFromControl((Control)view, finishProgress);
    }
    /// <summary>
    /// Hides the Progress from the specified control.
    /// </summary>
    /// <remarks>For backwards compatibility. Use Progress.Hide(IView) if possible.</remarks>
    public static void HideFromControl(Control control, bool finishProgress = true) {
      MainFormManager.GetMainForm<WindowsForms.MainForm>().RemoveProgressFromControl(control, finishProgress);
    }
    #endregion

    #region Event Handler
    public event EventHandler ProgressStateChanged;
    private void OnProgressStateChanged() {
      var handler = ProgressStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler MessageChanged;
    private void OnMessageChanged() {
      var handler = MessageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ProgressBarModeChanged;
    private void OnProgressBarModeChanged() {
      var handler = ProgressBarModeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ProgressValueChanged;
    private void OnProgressChanged() {
      var handler = ProgressValueChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CanBeStoppedChanged;
    private void OnCanBeStoppedChanged() {
      var handler = CanBeStoppedChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CanBeCanceledChanged;
    private void OnCanBeCanceledChanged() {
      var handler = CanBeCanceledChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler StopRequested;
    private void OnStopRequested() {
      var handler = StopRequested;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CancelRequested;
    private void OnCancelRequested() {
      var handler = CancelRequested;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
