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
using System.ComponentModel;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  [View("ProgressView")]
  [Content(typeof(IProgress), true)]
  public partial class ProgressView : AsynchronousContentView {
    private const int DefaultCancelTimeoutMs = 3000;
    private readonly IView parentView;

    [Category("Custom"), Description("The time that the process is allowed to exit.")]
    [DefaultValue(DefaultCancelTimeoutMs)]
    public int CancelTimeoutMs { get; set; }
    private bool ShouldSerializeCancelTimeoutMs() { return CancelTimeoutMs != DefaultCancelTimeoutMs; }

    public new IProgress Content {
      get { return (IProgress)base.Content; }
      set { base.Content = value; }
    }

    private Control Control {
      get { return (Control)parentView; }
    }

    public bool DisposeOnFinish { get; set; }

    public ProgressView() {
      InitializeComponent();
    }
    public ProgressView(IProgress progress)
      : this() {
      Content = progress;
    }
    public ProgressView(IView parentView)
      : this() {
      if (parentView == null) throw new ArgumentNullException("parentView", "The parent view is null.");
      if (!(parentView is Control)) throw new ArgumentException("The parent view is not a control.", "parentView");
      this.parentView = parentView;
    }
    public ProgressView(IView parentView, IProgress progress)
      : this(parentView) {
      Content = progress;
    }

    public static ProgressView Attach(IView parentView, IProgress progress, bool disposeOnFinish = false) {
      return new ProgressView(parentView, progress) {
        DisposeOnFinish = disposeOnFinish
      };
    }

    protected override void RegisterContentEvents() {
      Content.StatusChanged += new EventHandler(progress_StatusChanged);
      Content.ProgressValueChanged += new EventHandler(progress_ProgressValueChanged);
      Content.ProgressStateChanged += new EventHandler(Content_ProgressStateChanged);
      Content.CanBeCanceledChanged += new EventHandler(Content_CanBeCanceledChanged);
      base.RegisterContentEvents();
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.StatusChanged -= new EventHandler(progress_StatusChanged);
      Content.ProgressValueChanged -= new EventHandler(progress_ProgressValueChanged);
      Content.ProgressStateChanged -= new EventHandler(Content_ProgressStateChanged);
      Content.CanBeCanceledChanged -= new EventHandler(Content_CanBeCanceledChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        HideProgress();
      } else {
        if (Content.ProgressState == ProgressState.Started)
          ShowProgress();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      cancelButton.Visible = Content != null && Content.CanBeCanceled;
      cancelButton.Enabled = Content != null && Content.CanBeCanceled && !ReadOnly;
    }

    private void ShowProgress() {
      if (InvokeRequired) Invoke((Action)ShowProgress);
      else {
        if (parentView != null) {
          this.Left = (Control.ClientRectangle.Width / 2) - (this.Width / 2);
          this.Top = (Control.ClientRectangle.Height / 2) - (this.Height / 2);
          this.Anchor = AnchorStyles.None;

          LockBackground();

          if (!Control.Controls.Contains(this))
            Control.Controls.Add(this);

          BringToFront();
        }
        UpdateProgressValue();
        UpdateProgressStatus();
        Visible = true;
      }
    }

    private void HideProgress() {
      if (InvokeRequired) Invoke((Action)HideProgress);
      else {
        if (parentView != null) {
          if (Control.Controls.Contains(this))
            Control.Controls.Remove(this);

          UnlockBackground();
        }
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
      switch (Content.ProgressState) {
        case ProgressState.Finished:
          HideProgress();
          if (DisposeOnFinish) {
            Content = null;
            Dispose();
          }
          break;
        case ProgressState.Canceled: HideProgress(); break;
        case ProgressState.Started: ShowProgress(); break;
      }
    }

    private void Content_CanBeCanceledChanged(object sender, EventArgs e) {
      SetEnabledStateOfControls();
    }

    private void LockBackground() {
      if (InvokeRequired) {
        Invoke((Action)LockBackground);
      } else {
        parentView.Enabled = false;
        Enabled = true;
      }
    }

    private void UnlockBackground() {
      if (InvokeRequired) Invoke((Action)UnlockBackground);
      else {
        parentView.Enabled = true;
        Enabled = false;
      }
    }

    private void UpdateProgressValue() {
      if (InvokeRequired) Invoke((Action)UpdateProgressValue);
      else {
        if (Content != null) {
          double progressValue = Content.ProgressValue;
          if (progressValue <= 0.0 || progressValue > 1.0) {
            if (progressBar.Style != ProgressBarStyle.Marquee)
              progressBar.Style = ProgressBarStyle.Marquee;
          } else {
            if (progressBar.Style != ProgressBarStyle.Blocks)
              progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = (int)Math.Round(progressBar.Minimum + progressValue * (progressBar.Maximum - progressBar.Minimum));
          }
        }
      }
    }

    private void UpdateProgressStatus() {
      if (InvokeRequired) Invoke((Action)UpdateProgressStatus);
      else if (Content != null)
        statusLabel.Text = Content.Status;
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      if (Content != null) {
        try {
          Content.Cancel(CancelTimeoutMs);
          ReadOnly = true;
          cancelButtonTimer.Interval = CancelTimeoutMs;
          cancelButtonTimer.Start();
        } catch (NotSupportedException nse) {
          PluginInfrastructure.ErrorHandling.ShowErrorDialog(nse);
        }
      }
    }

    private void cancelButtonTimer_Tick(object sender, EventArgs e) {
      cancelButtonTimer.Stop();
      if (Visible) ReadOnly = false;
    }
  }
}
