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
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Hive.Views {
  public partial class ProgressView : HeuristicLab.MainForm.WindowsForms.View {
    private ContentView parentView;
    private IProgress progress;
    public IProgress Progress {
      get {
        return this.progress;
      }
      set {
        if (this.progress != value) {
          if (this.progress != null) {
            DeregisterProgressEvents();
          }
          this.progress = value;
          RegisterProgressEvents();
          OnProgressChanged();
        }
      }
    }

    public bool CancelEnabled {
      get {
        return cancelButton.Visible;
      }
      set {
        if (InvokeRequired) {
          Invoke(new Action<bool>((ce) => { CancelEnabled = ce; }), value);
        } else {
          cancelButton.Visible = value;
        }
      }
    }

    /// <param name="parentView">This is the View which will be locked if lockParentView is true</param>
    public ProgressView(ContentView parentView, IProgress progress) {
      InitializeComponent();
      this.parentView = parentView;
      this.Progress = progress;

      this.CancelEnabled = false;

      progressBar.Style = ProgressBarStyle.Marquee;

      this.Left = (parentView.ClientRectangle.Width / 2) - (this.Width / 2);
      this.Top = (parentView.ClientRectangle.Height / 2) - (this.Height / 2);
      this.Anchor = AnchorStyles.Left | AnchorStyles.Top;

      LockBackground();

      parentView.Controls.Add(this);
      this.BringToFront();
    }

    private void RegisterProgressEvents() {
      progress.Finished += new EventHandler(progress_Finished);
      progress.StatusChanged += new EventHandler(progress_StatusChanged);
      progress.ProgressValueChanged += new EventHandler(progress_ProgressChanged);
    }

    private void DeregisterProgressEvents() {
      progress.Finished -= new EventHandler(progress_Finished);
      progress.StatusChanged -= new EventHandler(progress_StatusChanged);
      progress.ProgressValueChanged -= new EventHandler(progress_ProgressChanged);
    }

    void progress_Finished(object sender, EventArgs e) {
      Finish();
    }

    void progress_ProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(progress_ProgressChanged), sender, e);
      } else {
        progressBar.Style = ProgressBarStyle.Blocks;
        this.progressBar.Value = Math.Min(this.progressBar.Maximum, (int)(progress.ProgressValue * progressBar.Maximum));
      }
    }

    void progress_StatusChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(progress_StatusChanged), sender, e);
      } else {
        statusLabel.Text = progress.Status;
      }
    }

    private void LockBackground() {
      if (InvokeRequired) {
        Invoke(new Action(LockBackground));
      } else {
        this.parentView.Locked = true;
        foreach (Control c in this.parentView.Controls) {
          c.Enabled = false;
        }
        this.Enabled = true;
        this.ReadOnly = false;
      }
    }

    public void Finish() {
      if (InvokeRequired) {
        Invoke(new Action(Finish));
      } else {
        progressBar.Value = progressBar.Maximum;
        parentView.Locked = false;
        foreach (Control c in this.parentView.Controls) {
          c.Enabled = true;
        }
        parentView.Controls.Remove(this);
        this.Dispose();
      }
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      Finish();
    }

    private void OnProgressChanged() {
      progress_StatusChanged(this, EventArgs.Empty);
      progress_ProgressChanged(this, EventArgs.Empty);
    }

    public event EventHandler Canceled;
    protected virtual void OnCanceled() {
      var handler = Canceled;
      if (handler != null) Canceled(this, EventArgs.Empty);
    }
  }
}
