#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Experiment View")]
  [Content(typeof(Experiment), true)]
  public sealed partial class ExperimentView : NamedItemView {
    public new Experiment Content {
      get { return (Experiment)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public ExperimentView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public ExperimentView(Experiment content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        optimizerListView.Content = null;
        tabControl.Enabled = false;
        startButton.Enabled = pauseButton.Enabled = stopButton.Enabled = resetButton.Enabled = false;
        executionTimeTextBox.Text = "-";
        executionTimeTextBox.Enabled = false;
      } else {
        optimizerListView.Content = Content.Optimizers;
        tabControl.Enabled = true;
        EnableDisableButtons();
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
        executionTimeTextBox.Enabled = true;
      }
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if ((Content != null) && (Content.ExecutionState == ExecutionState.Started)) Content.Stop();
      base.OnClosed(e);
    }

    #region Content Events
    private void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else {
        nameTextBox.Enabled = Content.ExecutionState != ExecutionState.Started;
        descriptionTextBox.Enabled = Content.ExecutionState != ExecutionState.Started;
        SaveEnabled = Content.ExecutionState != ExecutionState.Started;
        EnableDisableButtons();
      }
    }
    private void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionTimeChanged), sender, e);
      else
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
    }
    private void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Value);
    }
    #endregion

    #region Control events
    private void startButton_Click(object sender, EventArgs e) {
      Content.Start();
    }
    private void pauseButton_Click(object sender, EventArgs e) {
      Content.Pause();
    }
    private void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      if (MessageBox.Show(this, "Clear all runs executed so far?", "Clear All Runs?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
        Content.Prepare(true);
      else
        Content.Prepare(false);
    }
    #endregion

    #region Helpers
    private void EnableDisableButtons() {
      startButton.Enabled = (Content.ExecutionState == ExecutionState.Prepared) || (Content.ExecutionState == ExecutionState.Paused);
      pauseButton.Enabled = Content.ExecutionState == ExecutionState.Started;
      stopButton.Enabled = (Content.ExecutionState == ExecutionState.Started) || (Content.ExecutionState == ExecutionState.Paused);
      resetButton.Enabled = Content.ExecutionState != ExecutionState.Started;
    }
    #endregion
  }
}
