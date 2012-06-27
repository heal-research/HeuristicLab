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
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class ProgressView : HeuristicLab.MainForm.WindowsForms.View {
    private ContentView parentView;

    private IProgress progress;
    public IProgress Progress {
      get { return progress; }
      set {
        if (progress == value) return;
        DeregisterProgressEvents();
        progress = value;
        RegisterProgressEvents();
        OnProgressChanged();
      }
    }

    private bool cancelEnabled;
    public bool CancelEnabled {
      get { return cancelEnabled; }
      set {
        cancelEnabled = value;
        SetCancelButtonVisibility();
      }
    }

    /// <param name="parentView">This is the view which will be locked while progress is made.</param>
    public ProgressView(ContentView parentView, IProgress progress) {
      InitializeComponent();
      Progress = progress;
      CancelEnabled = false;

      if (parentView != null) {
        this.parentView = parentView;
        this.Left = (parentView.ClientRectangle.Width / 2) - (this.Width / 2);
        this.Top = (parentView.ClientRectangle.Height / 2) - (this.Height / 2);
        this.Anchor = AnchorStyles.Left | AnchorStyles.Top;

        LockBackground();

        parentView.Controls.Add(this);
        BringToFront();
      }
    }

    private void RegisterProgressEvents() {
      if (progress == null) return;
      progress.Finished += new EventHandler(progress_Finished);
      progress.StatusChanged += new EventHandler(progress_StatusChanged);
      progress.ProgressValueChanged += new EventHandler(progress_ProgressValueChanged);
    }

    private void DeregisterProgressEvents() {
      if (progress == null) return;
      progress.Finished -= new EventHandler(progress_Finished);
      progress.StatusChanged -= new EventHandler(progress_StatusChanged);
      progress.ProgressValueChanged -= new EventHandler(progress_ProgressValueChanged);
    }

    private void progress_Finished(object sender, EventArgs e) {
      Finish();
    }

    private void progress_StatusChanged(object sender, EventArgs e) {
      UpdateProgressStatus();
    }

    private void progress_ProgressValueChanged(object sender, EventArgs e) {
      UpdateProgressValue();
    }

    private void LockBackground() {
      if (InvokeRequired) {
        Invoke((Action)LockBackground);
      } else {
        parentView.Locked = true;
        parentView.ReadOnly = true;
        foreach (Control c in this.parentView.Controls)
          c.Enabled = false;
        Enabled = true;
        ReadOnly = false;
      }
    }

    private void UpdateProgressValue() {
      if (InvokeRequired) Invoke((Action)UpdateProgressValue);
      else {
        double progressValue = progress.ProgressValue;
        if (progressValue < progressBar.Minimum || progressValue > progressBar.Maximum) {
          progressBar.Style = ProgressBarStyle.Marquee;
          progressBar.Value = progressBar.Minimum;
        } else {
          progressBar.Style = ProgressBarStyle.Blocks;
          progressBar.Value = (int)Math.Round(progressBar.Minimum + progressValue * (progressBar.Maximum - progressBar.Minimum));
        }
      }
    }

    private void UpdateProgressStatus() {
      if (InvokeRequired) Invoke((Action)UpdateProgressStatus);
      else {
        string status = progress.Status;
        statusLabel.Text = progress.Status;
      }
    }

    public void Finish() {
      if (InvokeRequired) {
        Invoke(new Action(Finish));
      } else {
        progressBar.Value = progressBar.Maximum;
        parentView.Controls.Remove(this);
        parentView.Locked = false;
        parentView.ReadOnly = false;
        foreach (Control c in this.parentView.Controls)
          c.Enabled = true;
        DeregisterProgressEvents();
        Dispose();
      }
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      OnCanceled();
      Finish();
    }

    private void SetCancelButtonVisibility() {
      if (InvokeRequired) {
        Invoke((Action)SetCancelButtonVisibility);
      } else {
        cancelButton.Visible = cancelEnabled;
      }
    }

    private void OnProgressChanged() {
      UpdateProgressStatus();
      UpdateProgressValue();
    }

    public event EventHandler Canceled;
    protected virtual void OnCanceled() {
      var handler = Canceled;
      if (handler != null) Canceled(this, EventArgs.Empty);
    }
  }
}
