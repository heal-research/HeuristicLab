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
using HeuristicLab.MainForm;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Base class for editors of engines.
  /// </summary>
  [Content(typeof(Engine), true)]
  [Content(typeof(IEngine), false)]
  public partial class EngineView : ItemView {
    protected TypeSelectorDialog typeSelectorDialog;
    private int executionTimeCounter;

    /// <summary>
    /// Gets or sets the current engine.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>.</remarks>
    public new IEngine Content {
      get { return (IEngine)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBaseEditor"/>.
    /// </summary>
    public EngineView() {
      InitializeComponent();
    }
    public EngineView(IEngine content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.OperatorGraphChanged -= new EventHandler(Content_OperatorGraphChanged);
      Content.ProblemChanged -= new EventHandler(Content_ProblemChanged);
      Content.Prepared -= new EventHandler(Content_Prepared);
      Content.Started -= new EventHandler(Content_Started);
      Content.Stopped -= new EventHandler(Content_Stopped);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds event handlers to the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.OperatorGraphChanged += new EventHandler(Content_OperatorGraphChanged);
      Content.ProblemChanged += new EventHandler(Content_ProblemChanged);
      Content.Prepared += new EventHandler(Content_Prepared);
      Content.Started += new EventHandler(Content_Started);
      Content.Stopped += new EventHandler(Content_Stopped);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="EditorBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void OnContentChanged() {
      base.OnContentChanged();
      stopButton.Enabled = false;
      if (Content == null) {
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = false;
        operatorGraphView.Enabled = false;
        scopeView.Enabled = false;
        newProblemButton.Enabled = openProblemButton.Enabled = saveProblemButton.Enabled = false;
        problemViewHost.Enabled = false;
        startButton.Enabled = resetButton.Enabled = false;
        executionTimeTextBox.Enabled = false;
      } else {
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = true;
        operatorGraphView.Content = Content.OperatorGraph;
        operatorGraphView.Enabled = true;
        scopeView.Content = Content.GlobalScope;
        scopeView.Enabled = true;
        newProblemButton.Enabled = openProblemButton.Enabled = true;
        saveProblemButton.Enabled = Content.Problem != null;
        problemViewHost.Content = Content.Problem;
        problemViewHost.Enabled = true;
        startButton.Enabled = !Content.Finished;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
        executionTimeTextBox.Enabled = true;
      }
    }

    #region Content Events
    protected virtual void Content_OperatorGraphChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_OperatorGraphChanged), sender, e);
      else
        operatorGraphView.Content = Content.OperatorGraph;
    }
    protected void Content_ProblemChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ProblemChanged), sender, e);
      else {
        saveProblemButton.Enabled = Content.Problem != null;
        problemViewHost.Content = Content.Problem;
      }
    }
    protected virtual void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else {
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = true;
        operatorGraphView.Enabled = true;
        scopeView.Enabled = true;
        newProblemButton.Enabled = openProblemButton.Enabled = true;
        saveProblemButton.Enabled = Content.Problem != null;
        problemViewHost.Enabled = true;
        startButton.Enabled = !Content.Finished;
        stopButton.Enabled = false;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Content_Started(object sender, EventArgs e) {
      executionTimeCounter = 0;
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Started), sender, e);
      else {
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = false;
        operatorGraphView.Enabled = false;
        scopeView.Enabled = false;
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
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = true;
        operatorGraphView.Enabled = true;
        scopeView.Enabled = true;
        newProblemButton.Enabled = openProblemButton.Enabled = true;
        saveProblemButton.Enabled = Content.Problem != null;
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
    protected void newOperatorGraphButton_Click(object sender, EventArgs e) {
      Content.OperatorGraph = new OperatorGraph();
    }
    protected void openOperatorGraphButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Operator Graph";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        OperatorGraph operatorGraph = null;
        try {
          operatorGraph = XmlParser.Deserialize(openFileDialog.FileName) as OperatorGraph;
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
        if (operatorGraph == null)
          MessageBox.Show(this, "Selected file does not contain an operator graph.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
        else
          Content.OperatorGraph = operatorGraph;
      }
    }
    protected void saveOperatorGraphButton_Click(object sender, EventArgs e) {
      saveFileDialog.Title = "Save Operator Graph";
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          if (saveFileDialog.FilterIndex == 1)
            XmlGenerator.Serialize(Content.OperatorGraph, saveFileDialog.FileName, 0);
          else
            XmlGenerator.Serialize(Content.OperatorGraph, saveFileDialog.FileName, 9);
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
    protected void newProblemButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
      }
      typeSelectorDialog.Caption = "Select Problem";
      typeSelectorDialog.TypeSelector.Configure(typeof(IProblem), false, false);

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        Content.Problem = (IProblem)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
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
          MessageBox.Show(this, "Selected file does not contain a problem.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
