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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  [View("IOptimizer View")]
  [Content(typeof(IOptimizer), false)]
  public partial class IOptimizerView : NamedItemView {
    public IOptimizerView() {
      InitializeComponent();
    }

    public new IOptimizer Content {
      get { return (IOptimizer)base.Content; }
      set { base.Content = value; }
    }

    protected override void DeregisterContentEvents() {
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared -= new EventHandler(Content_Prepared);
      Content.Started -= new EventHandler(Content_Started);
      Content.Paused -= new EventHandler(Content_Paused);
      Content.Stopped -= new EventHandler(Content_Stopped);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared += new EventHandler(Content_Prepared);
      Content.Started += new EventHandler(Content_Started);
      Content.Paused += new EventHandler(Content_Paused);
      Content.Stopped += new EventHandler(Content_Stopped);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        executionTimeTextBox.Text = "-";
      } else {
        Locked = ReadOnly = Content.ExecutionState == ExecutionState.Started;
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      executionTimeTextBox.Enabled = Content != null;
      SetEnabledStateOfExecutableButtons();
    }

    #region Content Events
    protected virtual void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else
        startButton.Enabled = pauseButton.Enabled = stopButton.Enabled = resetButton.Enabled = false;
    }
    protected virtual void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else {
        nameTextBox.Enabled = infoLabel.Enabled = true;
        ReadOnly = Locked = false;
        SetEnabledStateOfExecutableButtons();
      }
    }
    protected virtual void Content_Started(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Started), sender, e);
      else {
        nameTextBox.Enabled = infoLabel.Enabled = false;
        ReadOnly = Locked = true;
        SetEnabledStateOfExecutableButtons();
      }
    }
    protected virtual void Content_Paused(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Paused), sender, e);
      else {
        nameTextBox.Enabled = infoLabel.Enabled = true;
        ReadOnly = Locked = false;
        SetEnabledStateOfExecutableButtons();
      }
    }
    protected virtual void Content_Stopped(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Stopped), sender, e);
      else {
        nameTextBox.Enabled = infoLabel.Enabled = true;
        ReadOnly = Locked = false;
        SetEnabledStateOfExecutableButtons();
      }
    }
    protected virtual void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionTimeChanged), sender, e);
      else
        executionTimeTextBox.Text = Content == null ? "-" : Content.ExecutionTime.ToString();
    }
    protected virtual void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        ErrorHandling.ShowErrorDialog(this, e.Value);
    }
    #endregion

    #region Control events
    protected virtual async void startButton_Click(object sender, EventArgs e) {
      await Content.StartAsync();
    }
    protected virtual void pauseButton_Click(object sender, EventArgs e) {
      Content.Pause();
    }
    protected virtual void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    protected virtual void resetButton_Click(object sender, EventArgs e) {
      Content.Prepare(false);
    }
    #endregion

    #region Helpers
    protected virtual void SetEnabledStateOfExecutableButtons() {
      if (Content == null) {
        startButton.Enabled = pauseButton.Enabled = stopButton.Enabled = resetButton.Enabled = false;
      } else {
        startButton.Enabled = (Content.ExecutionState == ExecutionState.Prepared) || (Content.ExecutionState == ExecutionState.Paused);
        pauseButton.Enabled = Content.ExecutionState == ExecutionState.Started;
        stopButton.Enabled = (Content.ExecutionState == ExecutionState.Started) || (Content.ExecutionState == ExecutionState.Paused);
        resetButton.Enabled = Content.ExecutionState != ExecutionState.Started;
      }
    }
    #endregion
  }
}
