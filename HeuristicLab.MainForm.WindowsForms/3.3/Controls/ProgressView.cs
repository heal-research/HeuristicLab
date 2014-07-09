#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private const int defaultControlHeight = 88;
    private const int collapsedControlHeight = 55;

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
      if (control == null) throw new ArgumentNullException("control", "The control is null.");
      if (content == null) throw new ArgumentNullException("content", "The passed progress is null.");
      InitializeComponent();

      this.control = control;
      this.content = content;
      if (content.ProgressState == ProgressState.Started)
        ShowProgress();
      RegisterContentEvents();
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      DeregisterContentEvents();
      HideProgress();

      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void RegisterContentEvents() {
      content.StatusChanged += new EventHandler(progress_StatusChanged);
      content.ProgressValueChanged += new EventHandler(progress_ProgressValueChanged);
      content.ProgressStateChanged += new EventHandler(Content_ProgressStateChanged);
      content.CanBeCanceledChanged += new EventHandler(Content_CanBeCanceledChanged);
    }
    private void DeregisterContentEvents() {
      content.StatusChanged -= new EventHandler(progress_StatusChanged);
      content.ProgressValueChanged -= new EventHandler(progress_ProgressValueChanged);
      content.ProgressStateChanged -= new EventHandler(Content_ProgressStateChanged);
      content.CanBeCanceledChanged -= new EventHandler(Content_CanBeCanceledChanged);
    }

    private void ShowProgress() {
      if (Control.InvokeRequired) {
        Control.Invoke((Action)ShowProgress);
        return;
      }
      int height = Content.CanBeCanceled ? Height : collapsedControlHeight;

      Left = (Control.ClientRectangle.Width / 2) - (Width / 2);
      Top = (Control.ClientRectangle.Height / 2) - (height / 2);
      Anchor = AnchorStyles.None;

      control.Enabled = false;
      Parent = Control.Parent;
      BringToFront();

      UpdateProgressValue();
      UpdateProgressStatus();
      UpdateCancelButton();
      Visible = true;
    }

    private void HideProgress() {
      if (InvokeRequired) Invoke((Action)HideProgress);
      else {
        control.Enabled = true;
        Parent = null;
        Visible = false;
      }
    }

    private void progress_StatusChanged(object sender, EventArgs e) {
      UpdateProgressStatus();
    }

    private void progress_ProgressValueChanged(object sender, EventArgs e) {
      UpdateProgressValue();
    }

    private void Content_ProgressStateChanged(object sender, EventArgs e) {
      switch (content.ProgressState) {
        case ProgressState.Finished: HideProgress(); break;
        case ProgressState.Canceled: HideProgress(); break;
        case ProgressState.Started: ShowProgress(); break;
        default: throw new NotSupportedException("The progress state " + content.ProgressState + " is not supported by the ProgressView.");
      }
    }

    private void Content_CanBeCanceledChanged(object sender, EventArgs e) {
      UpdateCancelButton();
    }

    private void UpdateCancelButton() {
      cancelButton.Visible = content != null && content.CanBeCanceled;
      cancelButton.Enabled = content != null && content.CanBeCanceled;

      if (content != null && content.CanBeCanceled) {
        Height = defaultControlHeight;
      } else if (content != null && !content.CanBeCanceled) {
        Height = collapsedControlHeight;
      }
    }

    private void UpdateProgressValue() {
      if (InvokeRequired) Invoke((Action)UpdateProgressValue);
      else {
        if (content != null) {
          double progressValue = content.ProgressValue;
          if (progressValue <= 0.0 || progressValue > 1.0) {
            progressBar.Style = ProgressBarStyle.Marquee;
          } else {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = (int)Math.Round(progressBar.Minimum + progressValue * (progressBar.Maximum - progressBar.Minimum));
          }
        }
      }
    }

    private void UpdateProgressStatus() {
      if (InvokeRequired) Invoke((Action)UpdateProgressStatus);
      else if (content != null)
        statusLabel.Text = content.Status;
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      content.Cancel();
    }
  }
}
