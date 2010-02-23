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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [Content(typeof(Algorithm), true)]
  [Content(typeof(IAlgorithm), false)]
  public partial class AlgorithmView : NamedItemView {
    private TypeSelectorDialog problemTypeSelectorDialog;
    private int executionTimeCounter;

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
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public AlgorithmView(IAlgorithm content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared -= new EventHandler(Content_Prepared);
      Content.ProblemChanged -= new EventHandler(Content_ProblemChanged);
      Content.Started -= new EventHandler(Content_Started);
      Content.Stopped -= new EventHandler(Content_Stopped);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared += new EventHandler(Content_Prepared);
      Content.ProblemChanged += new EventHandler(Content_ProblemChanged);
      Content.Started += new EventHandler(Content_Started);
      Content.Stopped += new EventHandler(Content_Stopped);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      stopButton.Enabled = false;
      if (Content == null) {
        parameterCollectionView.Content = null;
        problemViewHost.Content = null;
        tabControl.Enabled = false;
        startButton.Enabled = resetButton.Enabled = false;
        executionTimeTextBox.Text = "-";
        executionTimeTextBox.Enabled = false;
      } else {
        parameterCollectionView.Content = Content.Parameters;
        saveProblemButton.Enabled = Content.Problem != null;
        problemViewHost.Content = Content.Problem;
        tabControl.Enabled = true;
        startButton.Enabled = !Content.Finished;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
        executionTimeTextBox.Enabled = true;
      }
    }

    #region Content Events
    protected virtual void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else {
        parameterCollectionView.Enabled = true;
        newProblemButton.Enabled = openProblemButton.Enabled = saveProblemButton.Enabled = true;
        problemViewHost.Enabled = true;
        startButton.Enabled = !Content.Finished;
        stopButton.Enabled = false;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
      }
    }
    protected void Content_ProblemChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ProblemChanged), sender, e);
      else {
        problemViewHost.Content = Content.Problem;
        saveProblemButton.Enabled = Content.Problem != null;
      }
    }
    protected virtual void Content_Started(object sender, EventArgs e) {
      executionTimeCounter = 0;
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Started), sender, e);
      else {
        parameterCollectionView.Enabled = false;
        newProblemButton.Enabled = openProblemButton.Enabled = saveProblemButton.Enabled = false;
        problemViewHost.Enabled = false;
        startButton.Enabled = false;
        stopButton.Enabled = true;
        resetButton.Enabled = false;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Content_Stopped(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Stopped), sender, e);
      else {
        parameterCollectionView.Enabled = true;
        newProblemButton.Enabled = openProblemButton.Enabled = saveProblemButton.Enabled = true;
        problemViewHost.Enabled = true;
        startButton.Enabled = !Content.Finished;
        stopButton.Enabled = false;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      executionTimeCounter++;
      if ((executionTimeCounter == 100) || !Content.Running) {
        executionTimeCounter = 0;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Value);
    }
    #endregion

    #region Button events
    protected void newProblemButton_Click(object sender, EventArgs e) {
      if (problemTypeSelectorDialog == null) {
        problemTypeSelectorDialog = new TypeSelectorDialog();
        problemTypeSelectorDialog.Caption = "Select Problem";
        problemTypeSelectorDialog.TypeSelector.Configure(Content.ProblemType, false, false);
      }
      if (problemTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        Content.Problem = (IProblem)problemTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
      }
    }
    protected void openProblemButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Problem";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        IProblem problem = null;
        try {
          problem = XmlParser.Deserialize(openFileDialog.FileName) as IProblem;
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
        if (problem == null)
          MessageBox.Show(this, "The selected file does not contain a problem.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
        else if (!Content.ProblemType.IsInstanceOfType(problem))
          MessageBox.Show(this, "The selected file contains a problem type which is not supported by this algorithm.", "Invalid Problem Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
        else
          Content.Problem = problem;
      }
    }
    protected void saveProblemButton_Click(object sender, EventArgs e) {
      saveFileDialog.Title = "Save Problem";
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          if (saveFileDialog.FilterIndex == 1)
            XmlGenerator.Serialize(Content.Problem, saveFileDialog.FileName, 0);
          else
            XmlGenerator.Serialize(Content.Problem, saveFileDialog.FileName, 9);
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
    protected virtual void startButton_Click(object sender, EventArgs e) {
      Content.Start();
    }
    protected virtual void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    protected virtual void resetButton_Click(object sender, EventArgs e) {
      Content.Prepare();
    }
    #endregion

    #region Helpers
    protected virtual void UpdateExecutionTimeTextBox() {
      if (InvokeRequired)
        Invoke(new Action(UpdateExecutionTimeTextBox));
      else
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
    }
    #endregion
  }
}
