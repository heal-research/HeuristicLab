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
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public BatchRunView(BatchRun content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared -= new EventHandler(Content_Prepared);
      Content.AlgorithmChanged -= new EventHandler(Content_AlgorithmChanged);
      Content.Started -= new EventHandler(Content_Started);
      Content.Stopped -= new EventHandler(Content_Stopped);
      Content.RepetitionsChanged -= new EventHandler(Content_RepetitionsChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared += new EventHandler(Content_Prepared);
      Content.AlgorithmChanged += new EventHandler(Content_AlgorithmChanged);
      Content.Started += new EventHandler(Content_Started);
      Content.Stopped += new EventHandler(Content_Stopped);
      Content.RepetitionsChanged += new EventHandler(Content_RepetitionsChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      stopButton.Enabled = false;
      if (Content == null) {
        repetitionsNumericUpDown.Value = 1;
        repetitionsNumericUpDown.Enabled = false;
        algorithmViewHost.Content = null;
        runsView.Content = null;
        tabControl.Enabled = false;
        startButton.Enabled = resetButton.Enabled = false;
        executionTimeTextBox.Text = "-";
        executionTimeTextBox.Enabled = false;
      } else {
        repetitionsNumericUpDown.Value = Content.Repetitions;
        repetitionsNumericUpDown.Enabled = true;
        saveAlgorithmButton.Enabled = Content.Algorithm != null;
        algorithmViewHost.ViewType = null;
        algorithmViewHost.Content = Content.Algorithm;
        runsView.Content = Content.Runs;
        tabControl.Enabled = true;
        startButton.Enabled = !Content.Finished;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
        executionTimeTextBox.Enabled = true;
      }
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if (Content != null) Content.Stop();
      base.OnClosed(e);
    }

    #region Content Events
    private void Content_AlgorithmChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmChanged), sender, e);
      else {
        algorithmViewHost.ViewType = null;
        algorithmViewHost.Content = Content.Algorithm;
        saveAlgorithmButton.Enabled = Content.Algorithm != null;
      }
    }
    private void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else {
        startButton.Enabled = !Content.Finished;
        UpdateExecutionTimeTextBox();
      }
    }
    private void Content_Started(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Started), sender, e);
      else {
        SaveEnabled = false;
        repetitionsNumericUpDown.Enabled = false;
        newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = saveAlgorithmButton.Enabled = false;
        startButton.Enabled = false;
        stopButton.Enabled = true;
        resetButton.Enabled = false;
        UpdateExecutionTimeTextBox();
      }
    }
    private void Content_Stopped(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Stopped), sender, e);
      else {
        SaveEnabled = true;
        repetitionsNumericUpDown.Enabled = true;
        newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = saveAlgorithmButton.Enabled = true;
        startButton.Enabled = !Content.Finished;
        stopButton.Enabled = false;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
      }
    }
    private void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      UpdateExecutionTimeTextBox();
    }
    private void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Value);
    }
    private void Content_RepetitionsChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_RepetitionsChanged), sender, e);
      else {
        repetitionsNumericUpDown.Value = Content.Repetitions;
        startButton.Enabled = !Content.Finished;
      }
    }
    #endregion

    #region Control events
    private void repetitionsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      Content.Repetitions = (int)repetitionsNumericUpDown.Value;
    }
    private void newAlgorithmButton_Click(object sender, EventArgs e) {
      if (algorithmTypeSelectorDialog == null) {
        algorithmTypeSelectorDialog = new TypeSelectorDialog();
        algorithmTypeSelectorDialog.Caption = "Select Algorithm";
        algorithmTypeSelectorDialog.TypeSelector.Configure(typeof(IAlgorithm), false, false);
      }
      if (algorithmTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        Content.Algorithm = (IAlgorithm)algorithmTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
      }
    }
    private void openAlgorithmButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Algorithm";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        this.Cursor = Cursors.AppStarting;
        newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = saveAlgorithmButton.Enabled = false;
        algorithmViewHost.Enabled = false;

        var call = new Func<string, object>(XmlParser.Deserialize);
        call.BeginInvoke(openFileDialog.FileName, delegate(IAsyncResult a) {
          IAlgorithm algorithm = null;
          try {
            algorithm = call.EndInvoke(a) as IAlgorithm;
          } catch (Exception ex) {
            Auxiliary.ShowErrorMessageBox(ex);
          }
          Invoke(new Action(delegate() {
            if (algorithm == null)
              MessageBox.Show(this, "The selected file does not contain an algorithm.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
              Content.Algorithm = algorithm;
            algorithmViewHost.Enabled = true;
            newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = saveAlgorithmButton.Enabled = true;
            this.Cursor = Cursors.Default;
          }));
        }, null);
      }
    }
    private void saveAlgorithmButton_Click(object sender, EventArgs e) {
      saveFileDialog.Title = "Save Algorithm";
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        this.Cursor = Cursors.AppStarting;
        newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = saveAlgorithmButton.Enabled = false;
        algorithmViewHost.Enabled = false;

        var call = new Action<IAlgorithm, string, int>(XmlGenerator.Serialize);
        int compression = 9;
        if (saveFileDialog.FilterIndex == 1) compression = 0;
        call.BeginInvoke(Content.Algorithm, saveFileDialog.FileName, compression, delegate(IAsyncResult a) {
          try {
            call.EndInvoke(a);
          }
          catch (Exception ex) {
            Auxiliary.ShowErrorMessageBox(ex);
          }
          Invoke(new Action(delegate() {
            algorithmViewHost.Enabled = true;
            newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = saveAlgorithmButton.Enabled = true;
            this.Cursor = Cursors.Default;
          }));
        }, null);
      }
    }
    private void startButton_Click(object sender, EventArgs e) {
      Content.Start();
    }
    private void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      Content.Prepare();
    }
    #endregion

    #region Helpers
    private void UpdateExecutionTimeTextBox() {
      if (InvokeRequired)
        Invoke(new Action(UpdateExecutionTimeTextBox));
      else
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
    }
    #endregion
  }
}
