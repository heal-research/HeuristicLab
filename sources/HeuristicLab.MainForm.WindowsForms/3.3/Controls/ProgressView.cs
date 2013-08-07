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
    private readonly IView view;

    [Category("Custom"), Description("The time that the process is allowed to exit.")]
    [DefaultValue(DefaultCancelTimeoutMs)]
    public int CancelTimeoutMs { get; set; }
    private bool ShouldSerializeCancelTimeoutMs() { return CancelTimeoutMs != DefaultCancelTimeoutMs; }

    public new IProgress Content {
      get { return (IProgress)base.Content; }
      set { base.Content = value; }
    }

    private Control Control {
      get { return (Control)view; }
    }

    public bool DisposeOnFinish { get; set; }

    public ProgressView() {
      InitializeComponent();
    }

    public ProgressView(IView view)
      : this() {
      if (view == null) throw new ArgumentNullException("view", "The view is null.");
      if (!(view is Control)) throw new ArgumentException("The view is not a control.", "view");
      this.view = view;
    }
    public ProgressView(IView view, IProgress progress)
      : this(view) {
      Content = progress;
    }

    public static ProgressView Attach(IView view, IProgress progress, bool disposeOnFinish = false) {
      return new ProgressView(view, progress) {
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
        if (view != null) {
          Left = (Control.ClientRectangle.Width / 2) - (Width / 2);
          Top = (Control.ClientRectangle.Height / 2) - (Height / 2);
          Anchor = AnchorStyles.None;

          LockBackground();
          Parent = Control.Parent;
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
        if (view != null) {
          Parent = null;
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
      if (InvokeRequired) Invoke((Action)LockBackground);
      else {
        view.Enabled = false;
      }
    }

    private void UnlockBackground() {
      if (InvokeRequired) Invoke((Action)UnlockBackground);
      else {
        view.Enabled = true;
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
