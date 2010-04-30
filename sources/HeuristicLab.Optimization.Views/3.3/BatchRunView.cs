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
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("BatchRun View")]
  [Content(typeof(BatchRun), true)]
  public sealed partial class BatchRunView : NamedItemView {
    private TypeSelectorDialog algorithmTypeSelectorDialog;

    public new BatchRun Content {
      get { return (BatchRun)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public BatchRunView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.AlgorithmChanged -= new EventHandler(Content_AlgorithmChanged);
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.RepetitionsChanged -= new EventHandler(Content_RepetitionsChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.AlgorithmChanged += new EventHandler(Content_AlgorithmChanged);
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.RepetitionsChanged += new EventHandler(Content_RepetitionsChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        repetitionsNumericUpDown.Value = 1;
        algorithmViewHost.Content = null;
        runsView.Content = null;
        executionTimeTextBox.Text = "-";
      } else {
        repetitionsNumericUpDown.Value = Content.Repetitions;
        algorithmViewHost.ViewType = null;
        algorithmViewHost.Content = Content.Algorithm;
        runsView.Content = Content.Runs;
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
      }
      SetEnableStateOfControls();
    }
    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnableStateOfControls();
    }
    private void SetEnableStateOfControls() {
      repetitionsNumericUpDown.Enabled = Content != null && !ReadOnly;
      newAlgorithmButton.Enabled = Content != null && !ReadOnly;
      openAlgorithmButton.Enabled = Content != null && !ReadOnly;
      algorithmViewHost.Enabled = Content != null;
      runsView.Enabled = Content != null;
      executionTimeTextBox.Enabled = Content != null;
      SetEnabledStateOfExecutableButtons();
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
        this.ReadOnly = Content.ExecutionState == ExecutionState.Started;
        Locked = Content.ExecutionState == ExecutionState.Started;
        SetEnabledStateOfExecutableButtons();
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
    private void Content_AlgorithmChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmChanged), sender, e);
      else {
        algorithmViewHost.ViewType = null;
        algorithmViewHost.Content = Content.Algorithm;
      }
    }
    private void Content_RepetitionsChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_RepetitionsChanged), sender, e);
      else
        repetitionsNumericUpDown.Value = Content.Repetitions;
    }
    #endregion

    #region Control events
    private void repetitionsNumericUpDown_Validated(object sender, System.EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
    }
    private void repetitionsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      Content.Repetitions = (int)repetitionsNumericUpDown.Value;
    }
    private void newAlgorithmButton_Click(object sender, EventArgs e) {
      if (algorithmTypeSelectorDialog == null) {
        algorithmTypeSelectorDialog = new TypeSelectorDialog();
        algorithmTypeSelectorDialog.Caption = "Select Algorithm";
        algorithmTypeSelectorDialog.TypeSelector.Caption = "Available Algorithms";
        algorithmTypeSelectorDialog.TypeSelector.Configure(typeof(IAlgorithm), false, false);
      }
      if (algorithmTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Algorithm = (IAlgorithm)algorithmTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
    private void openAlgorithmButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Algorithm";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = false;
        algorithmViewHost.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            IAlgorithm algorithm = content as IAlgorithm;
            if (algorithm == null)
              MessageBox.Show(this, "The selected file does not contain an algorithm.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
              Content.Algorithm = algorithm;
          }
          catch (Exception ex) {
            Auxiliary.ShowErrorMessageBox(ex);
          }
          finally {
            Invoke(new Action(delegate() {
              algorithmViewHost.Enabled = true;
              newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = true;
            }));
          }
        });
      }
    }
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
      if (Content.Runs.Count > 0) {
        if (MessageBox.Show(this, "Clear all runs executed so far?", "Clear All Runs?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
          Content.Prepare(true);
        else
          Content.Prepare(false);
      } else {
        Content.Prepare();
      }
    }
    private void algorithmPanel_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (ReadOnly)
        return;
      Type type = e.Data.GetData("Type") as Type;
      if ((type != null) && (typeof(IAlgorithm).IsAssignableFrom(type))) {
        if ((e.KeyState & 8) == 8) e.Effect = DragDropEffects.Link;  // CTRL key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
      }
    }
    private void algorithmPanel_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IAlgorithm algorithm = e.Data.GetData("Value") as IAlgorithm;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) algorithm = (IAlgorithm)algorithm.Clone();
        Content.Algorithm = algorithm;
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
