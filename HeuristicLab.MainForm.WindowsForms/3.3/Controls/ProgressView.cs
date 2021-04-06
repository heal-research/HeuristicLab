#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.MainForm.WindowsForms {
  internal sealed partial class ProgressView : UserControl {
    public Control TargetControl { get; }
    public IProgress Content { get; }

    public ProgressView(Control targetControl, IProgress content)
      : base() {
      if (targetControl == null) throw new ArgumentNullException(nameof(targetControl));
      if (targetControl.Parent == null) throw new InvalidOperationException("A Progress can only be shown on controls that have a Parent-control. Therefore, Dialogs and Forms cannot have an associated ProgressView.");
      if (content == null) throw new ArgumentNullException(nameof(content));
      InitializeComponent();

      this.TargetControl = targetControl;
      this.Content = content;

      if (content.ProgressState != ProgressState.Finished)
        ShowProgress();
      RegisterContentEvents();
    }

    protected override void Dispose(bool disposing) {
      DeregisterContentEvents();

      if (!TargetControl.IsDisposed)
        HideProgress();

      if (disposing && components != null) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void RegisterContentEvents() {
      Content.ProgressStateChanged += new EventHandler(Content_ProgressStateChanged);
      Content.MessageChanged += new EventHandler(Content_MessageChanged);
      Content.ProgressBarModeChanged += new EventHandler(Content_ProgressBarModeChanged);
      Content.ProgressValueChanged += new EventHandler(Content_ProgressValueChanged);
      Content.CanBeStoppedChanged += new EventHandler(Content_CanBeStoppedChanged);
      Content.CanBeCanceledChanged += new EventHandler(Content_CanBeCanceledChanged);
    }
    private void DeregisterContentEvents() {
      Content.ProgressStateChanged -= new EventHandler(Content_ProgressStateChanged);
      Content.MessageChanged -= new EventHandler(Content_MessageChanged);
      Content.ProgressBarModeChanged -= new EventHandler(Content_ProgressBarModeChanged);
      Content.ProgressValueChanged -= new EventHandler(Content_ProgressValueChanged);
      Content.CanBeStoppedChanged -= new EventHandler(Content_CanBeStoppedChanged);
      Content.CanBeCanceledChanged -= new EventHandler(Content_CanBeCanceledChanged);
    }

    private void Content_ProgressStateChanged(object sender, EventArgs e) {
      UpdateProgressState();
      UpdateButtonsState();
    }

    private void Content_MessageChanged(object sender, EventArgs e) {
      UpdateProgressMessage();
    }

    private void Content_ProgressBarModeChanged(object sender, EventArgs e) {
      UpdateProgressValue();
    }
    private void Content_ProgressValueChanged(object sender, EventArgs e) {
      UpdateProgressValue();
    }

    private void Content_CanBeStoppedChanged(object sender, EventArgs e) {
      UpdateButtonsState();
    }
    private void Content_CanBeCanceledChanged(object sender, EventArgs e) {
      UpdateButtonsState();
    }

    private void ShowProgress() {
      if (TargetControl.InvokeRequired) {
        TargetControl.Invoke((Action)ShowProgress);
        return;
      }
      if (Parent != null) return;

      Left = (TargetControl.ClientRectangle.Width / 2) - (Width / 2);
      Top = (TargetControl.ClientRectangle.Height / 2) - (Height / 2);
      Anchor = AnchorStyles.None;

      UpdateProgressMessage();
      UpdateProgressValue();
      UpdateButtonsState();

      TargetControl.SuspendRepaint();
      TargetControl.Enabled = false;
      RegisterTargetControlEvents();
      Parent = TargetControl.Parent;
      BringToFront();
      TargetControl.ResumeRepaint(true);
      Visible = true;
    }

    private void HideProgress() {
      if (TargetControl.InvokeRequired) {
        TargetControl.Invoke((Action)HideProgress);
        return;
      }
      if (Parent == null) return;

      Visible = false;
      TargetControl.SuspendRepaint();
      TargetControl.Enabled = true;
      DeregisterTargetControlEvents();
      Parent = null;
      TargetControl.ResumeRepaint(TargetControl.Visible);
    }


    private void RegisterTargetControlEvents() {
      TargetControl.Disposed += TargetControl_Disposed;
      TargetControl.VisibleChanged += TargetControl_VisibleChanged;
      TargetControl.ParentChanged += TargetControl_ParentChanged;
    }

    private void DeregisterTargetControlEvents() {
      TargetControl.Disposed -= TargetControl_Disposed;
      TargetControl.VisibleChanged -= TargetControl_VisibleChanged;
      TargetControl.ParentChanged -= TargetControl_ParentChanged;
    }

    private void TargetControl_Disposed(object sender, EventArgs e) {
      Dispose();
    }
    private void TargetControl_VisibleChanged(object sender, EventArgs e) {
      Visible = TargetControl.Visible;
    }
    private void TargetControl_ParentChanged(object sender, EventArgs e) {
      Parent = TargetControl.Parent;
    }

    private void UpdateProgressState() {
      if (TargetControl.InvokeRequired) {
        TargetControl.Invoke((Action)UpdateProgressState);
        return;
      }

      if (Content.ProgressState != ProgressState.Finished)
        ShowProgress();
      else
        HideProgress();
    }

    private void UpdateProgressMessage() {
      if (TargetControl.InvokeRequired) {
        TargetControl.Invoke((Action)UpdateProgressMessage);
        return;
      }

      messageLabel.Text = Content.Message;
    }

    private void UpdateProgressValue() {
      if (Disposing || IsDisposed) return;
      if (InvokeRequired) {
        Invoke((Action)UpdateProgressValue);
        return;
      }

      switch (Content.ProgressMode) {
        case ProgressMode.Determinate:
          progressBar.Style = ProgressBarStyle.Continuous;
          progressBar.Value = (int)Math.Round(progressBar.Minimum + Content.ProgressValue * (progressBar.Maximum - progressBar.Minimum));
          break;
        case ProgressMode.Indeterminate:
          progressBar.Style = ProgressBarStyle.Marquee;
          progressBar.Value = 0;
          break;
        default:
          throw new NotImplementedException($"Invalid Progress Mode: {Content.ProgressMode}");
      }
    }

    private void UpdateButtonsState() {
      if (TargetControl.InvokeRequired) {
        TargetControl.Invoke((Action)UpdateButtonsState);
        return;
      }

      stopButton.Visible = Content.CanBeStopped;
      stopButton.Enabled = Content.CanBeStopped && Content.ProgressState == ProgressState.Started;

      cancelButton.Visible = Content.CanBeCanceled;
      cancelButton.Enabled = Content.CanBeCanceled && Content.ProgressState == ProgressState.Started;
    }

    private void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    private void cancelButton_Click(object sender, EventArgs e) {
      Content.Cancel();
    }
  }
}
