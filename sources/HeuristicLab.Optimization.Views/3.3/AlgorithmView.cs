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
using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Algorithm View")]
  [Content(typeof(Algorithm), true)]
  [Content(typeof(IAlgorithm), false)]
  public partial class AlgorithmView : NamedItemView {
    private TypeSelectorDialog problemTypeSelectorDialog;

    public new IAlgorithm Content {
      get { return (IAlgorithm)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public AlgorithmView() {
      InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e) {
      // Set order of tab pages according to z order.
      // NOTE: This is required due to a bug in the VS designer.
      List<Control> tabPages = new List<Control>();
      for (int i = 0; i < tabControl.Controls.Count; i++) {
        tabPages.Add(tabControl.Controls[i]);
      }
      tabControl.Controls.Clear();
      foreach (Control control in tabPages)
        tabControl.Controls.Add(control);

      base.OnInitialized(e);
    }

    protected override void DeregisterContentEvents() {
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared -= new EventHandler(Content_Prepared);
      Content.ProblemChanged -= new EventHandler(Content_ProblemChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared += new EventHandler(Content_Prepared);
      Content.ProblemChanged += new EventHandler(Content_ProblemChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        parameterCollectionView.Content = null;
        problemViewHost.Content = null;
        resultsView.Content = null;
        runsView.Content = null;
        executionTimeTextBox.Text = "-";
      } else {
        parameterCollectionView.Content = Content.Parameters;
        problemViewHost.ViewType = null;
        problemViewHost.Content = Content.Problem;
        resultsView.Content = Content.Results.AsReadOnly();
        runsView.Content = Content.Runs;
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parameterCollectionView.Enabled = Content != null;
      newProblemButton.Enabled = Content != null && !ReadOnly;
      openProblemButton.Enabled = Content != null && !ReadOnly;
      problemViewHost.Enabled = Content != null;
      resultsView.Enabled = Content != null;
      runsView.Enabled = Content != null;
      executionTimeTextBox.Enabled = Content != null;
      SetEnabledStateOfExecutableButtons();
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if ((Content != null) && (Content.ExecutionState == ExecutionState.Started)) Content.Stop();
      base.OnClosed(e);
    }

    #region Content Events
    protected virtual void Content_ProblemChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ProblemChanged), sender, e);
      else {
        problemViewHost.ViewType = null;
        problemViewHost.Content = Content.Problem;
      }
    }
    protected virtual void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else
        resultsView.Content = Content.Results.AsReadOnly();
    }
    protected virtual void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else {
        ReadOnly = Content.ExecutionState == ExecutionState.Started;
        Locked = Content.ExecutionState == ExecutionState.Started;
        SetEnabledStateOfExecutableButtons();
      }
    }
    protected virtual void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionTimeChanged), sender, e);
      else
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
    }
    protected virtual void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        ErrorHandling.ShowErrorDialog(this, e.Value);
    }
    #endregion

    #region Control Events
    protected virtual void newProblemButton_Click(object sender, EventArgs e) {
      if (problemTypeSelectorDialog == null) {
        problemTypeSelectorDialog = new TypeSelectorDialog();
        problemTypeSelectorDialog.Caption = "Select Problem";
        problemTypeSelectorDialog.TypeSelector.Caption = "Available Problems";
        problemTypeSelectorDialog.TypeSelector.Configure(Content.ProblemType, false, true);
      }
      if (problemTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Problem = (IProblem)problemTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    protected virtual void openProblemButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Problem";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newProblemButton.Enabled = openProblemButton.Enabled = false;
        problemViewHost.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            IProblem problem = content as IProblem;
            if (problem == null)
              Invoke(new Action(() =>
                MessageBox.Show(this, "The selected file does not contain a problem.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            else if (!Content.ProblemType.IsInstanceOfType(problem))
              Invoke(new Action(() =>
                MessageBox.Show(this, "The selected file contains a problem type which is not supported by this algorithm.", "Invalid Problem Type", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            else
              Content.Problem = problem;
          }
          catch (Exception ex) {
            Invoke(new Action(() => ErrorHandling.ShowErrorDialog(this, ex)));
          }
          finally {
            Invoke(new Action(delegate() {
              problemViewHost.Enabled = true;
              newProblemButton.Enabled = openProblemButton.Enabled = true;
            }));
          }
        });
      }
    }
    protected virtual void startButton_Click(object sender, EventArgs e) {
      Content.Start();
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
    protected virtual void problemViewHost_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((type != null) && (Content.ProblemType.IsAssignableFrom(type))) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void problemViewHost_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IProblem problem = e.Data.GetData("Value") as IProblem;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) problem = (IProblem)problem.Clone();
        Content.Problem = problem;
      }
    }
    #endregion

    #region Helpers
    private void SetEnabledStateOfExecutableButtons() {
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
