#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Base class for editors of engines.
  /// </summary>
  [Content(typeof(EngineBase), true)]
  public partial class EngineBaseView : ItemViewBase {
    private int executionTimeCounter;

    /// <summary>
    /// Gets or sets the current engine.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>.</remarks>
    public IEngine Engine {
      get { return (IEngine)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBaseEditor"/>.
    /// </summary>
    public EngineBaseView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      Engine.Initialized -= new EventHandler(Engine_Initialized);
      Engine.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      Engine.ExecutionTimeChanged -= new EventHandler(Engine_ExecutionTimeChanged);
      Engine.Finished -= new EventHandler(Engine_Finished);
      operatorGraphView.OperatorGraph = null;
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds event handlers to the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Engine.Initialized += new EventHandler(Engine_Initialized);
      Engine.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      Engine.ExecutionTimeChanged += new EventHandler(Engine_ExecutionTimeChanged);
      Engine.Finished += new EventHandler(Engine_Finished);
      operatorGraphView.OperatorGraph = Engine.OperatorGraph;
      scopeView.Scope = Engine.GlobalScope;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="EditorBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      abortButton.Enabled = false;
      if (Engine == null) {
        executeButton.Enabled = false;
        resetButton.Enabled = false;
        executionTimeTextBox.Enabled = false;
      } else {
        executeButton.Enabled = true;
        resetButton.Enabled = true;
        executionTimeCounter = 0;
        executionTimeTextBox.Text = Engine.ExecutionTime.ToString();
        executionTimeTextBox.Enabled = true;
      }
    }

    #region Engine Events
    private void Engine_Initialized(object sender, EventArgs e) {
      Refresh();
    }
    private delegate void OnExceptionEventDelegate(object sender, EventArgs<Exception> e);
    private void Engine_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new OnExceptionEventDelegate(Engine_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Value);
    }
    private void Engine_ExecutionTimeChanged(object sender, EventArgs e) {
      executionTimeCounter++;
      if (executionTimeCounter == 1000)
        UpdateExecutionTimeTextBox();
    }
    private void Engine_Finished(object sender, EventArgs e) {
      UpdateExecutionTimeTextBox();
      if (globalScopeGroupBox.Controls.Count > 0) {
        ScopeView scopeEditor = (ScopeView)globalScopeGroupBox.Controls[0];
        if (!scopeEditor.AutomaticUpdating)
          scopeEditor.Refresh();
      }
      EnableDisableControls();
    }
    private void UpdateExecutionTimeTextBox() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(UpdateExecutionTimeTextBox));
      else {
        executionTimeCounter = 0;
        executionTimeTextBox.Text = Engine.ExecutionTime.ToString();
      }
    }
    private void EnableDisableControls() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(EnableDisableControls));
      else {
        executeButton.Enabled = true;
        abortButton.Enabled = false;
        resetButton.Enabled = true;
        operatorGraphGroupBox.Enabled = true;
        globalScopeGroupBox.Enabled = true;
      }
    }
    #endregion

    #region Button events
    private void executeButton_Click(object sender, EventArgs e) {
      executeButton.Enabled = false;
      abortButton.Enabled = true;
      resetButton.Enabled = false;
      operatorGraphGroupBox.Enabled = false;
      globalScopeGroupBox.Enabled = false;
      Engine.Execute();
    }
    private void abortButton_Click(object sender, EventArgs e) {
      Engine.Abort();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      Engine.Reset();
      UpdateExecutionTimeTextBox();
      if (globalScopeGroupBox.Controls.Count > 0) {
        ScopeView scopeEditor = (ScopeView)globalScopeGroupBox.Controls[0];
        if (!scopeEditor.AutomaticUpdating)
          scopeEditor.Refresh();
      }
    }
    #endregion
  }
}
