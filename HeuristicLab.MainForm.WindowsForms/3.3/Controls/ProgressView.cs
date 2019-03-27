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
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  internal sealed partial class ProgressView : UserControl {
    private readonly Control control;
    public Control Control {
      get { return control; }
    }

    private readonly IProgress content;
    public IProgress Content {
      get { return content; }
    }

    public ProgressView(Control control, IProgress content)
      : base() {
      if (control == null) throw new ArgumentNullException("control");
      if (control.Parent == null) throw new InvalidOperationException("A Progress can only be shown on controls that have a Parent-control. Therefore, Dialogs and Forms cannot have an associated ProgressView.");
      if (content == null) throw new ArgumentNullException("content");
      InitializeComponent();

      this.control = control;
      this.content = content;

      if (content.ProgressState != ProgressState.Finished)
        ShowProgress();
      RegisterContentEvents();
    }

    protected override void Dispose(bool disposing) {
      DeregisterContentEvents();
      HideProgress();

      if (disposing && (components != null)) {
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
      if (Control.InvokeRequired) {
        Control.Invoke((Action)ShowProgress);
        return;
      }
      if (Parent != null) return;

      Left = (Control.ClientRectangle.Width / 2) - (Width / 2);
      Top = (Control.ClientRectangle.Height / 2) - (Height / 2);
      Anchor = AnchorStyles.None;

      UpdateProgressMessage();
      UpdateProgressValue();
      UpdateButtonsState();

      Control.SuspendRepaint();
      Control.Enabled = false;
      Parent = Control.Parent;
      BringToFront();
      Control.ResumeRepaint(true);
      Visible = true;
    }

    private void HideProgress() {
      if (Control.InvokeRequired) {
        Control.Invoke((Action)HideProgress);
        return;
      }
      if (Parent == null) return;

      Visible = false;
      Control.SuspendRepaint();
      Control.Enabled = true;
      Control.ResumeRepaint(true);
      Parent = null;
    }

    private void UpdateProgressState() {
      if (Control.InvokeRequired) {
        Control.Invoke((Action)UpdateProgressState);
        return;
      }

      if (Content.ProgressState != ProgressState.Finished)
        ShowProgress();
      else
        HideProgress();
    }

    private void UpdateProgressMessage() {
      if (Control.InvokeRequired) {
        Control.Invoke((Action)UpdateProgressMessage);
        return;
      }

      messageLabel.Text = content.Message;
    }

    private void UpdateProgressValue() {
      if (InvokeRequired) {
        Invoke((Action)UpdateProgressValue);
        return;
      }

      switch (Content.ProgressMode) {
        case ProgressMode.Determinate:
          progressBar.Style = ProgressBarStyle.Continuous;
          progressBar.Value = (int)Math.Round(progressBar.Minimum + content.ProgressValue * (progressBar.Maximum - progressBar.Minimum));
          break;
        case ProgressMode.Indeterminate:
          progressBar.Style = ProgressBarStyle.Marquee;
          progressBar.Value = 0;
          break;
        default:
          throw new NotImplementedException($"Invalid Progress Mode: {content.ProgressMode}");
      }
    }

    private void UpdateButtonsState() {
      if (Control.InvokeRequired) {
        Control.Invoke((Action)UpdateButtonsState);
        return;
      }

      stopButton.Visible = Content.CanBeStopped;
      stopButton.Enabled = Content.CanBeStopped && content.ProgressState == ProgressState.Started;

      cancelButton.Visible = Content.CanBeCanceled;
      cancelButton.Enabled = Content.CanBeCanceled && content.ProgressState == ProgressState.Started;
    }

    private void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    private void cancelButton_Click(object sender, EventArgs e) {
      Content.Cancel();
    }
  }
}
