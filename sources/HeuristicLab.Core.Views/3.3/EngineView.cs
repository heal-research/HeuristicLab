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
  [Content(typeof(Engine), true)]
  public partial class EngineView : ItemView {
    private int executionTimeCounter;

    /// <summary>
    /// Gets or sets the current engine.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>.</remarks>
    public Engine Engine {
      get { return (Engine)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBaseEditor"/>.
    /// </summary>
    public EngineView() {
      InitializeComponent();
    }
    public EngineView(Engine engine)
      : this() {
      Engine = engine;
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterObjectEvents() {
      Engine.OperatorGraphChanged -= new EventHandler(Engine_OperatorGraphChanged);
      Engine.Initialized -= new EventHandler(Engine_Initialized);
      Engine.Started -= new EventHandler(Engine_Started);
      Engine.Stopped -= new EventHandler(Engine_Stopped);
      Engine.ExecutionTimeChanged -= new EventHandler(Engine_ExecutionTimeChanged);
      Engine.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      base.DeregisterObjectEvents();
    }

    /// <summary>
    /// Adds event handlers to the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      Engine.OperatorGraphChanged += new EventHandler(Engine_OperatorGraphChanged);
      Engine.Initialized += new EventHandler(Engine_Initialized);
      Engine.Started += new EventHandler(Engine_Started);
      Engine.Stopped += new EventHandler(Engine_Stopped);
      Engine.ExecutionTimeChanged += new EventHandler(Engine_ExecutionTimeChanged);
      Engine.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="EditorBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      stopButton.Enabled = false;
      if (Engine == null) {
        operatorGraphView.Enabled = false;
        scopeView.Enabled = false;
        startButton.Enabled = false;
        resetButton.Enabled = false;
        executionTimeTextBox.Enabled = false;
      } else {
        operatorGraphView.OperatorGraph = Engine.OperatorGraph;
        scopeView.Scope = Engine.GlobalScope;
        startButton.Enabled = !Engine.Finished;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
        executionTimeTextBox.Enabled = true;
      }
    }

    #region Engine Events
    protected virtual void Engine_OperatorGraphChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Engine_OperatorGraphChanged), sender, e);
      else
        operatorGraphView.OperatorGraph = Engine.OperatorGraph;
    }
    protected virtual void Engine_Initialized(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Engine_Initialized), sender, e);
      else {
        operatorGraphView.Enabled = true;
        scopeView.Enabled = true;
        startButton.Enabled = !Engine.Finished;
        stopButton.Enabled = false;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Engine_Started(object sender, EventArgs e) {
      executionTimeCounter = 0;
      if (InvokeRequired)
        Invoke(new EventHandler(Engine_Started), sender, e);
      else {
        operatorGraphView.Enabled = false;
        scopeView.Enabled = false;
        startButton.Enabled = false;
        stopButton.Enabled = true;
        resetButton.Enabled = false;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Engine_Stopped(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Engine_Stopped), sender, e);
      else {
        operatorGraphView.Enabled = true;
        scopeView.Enabled = true;
        startButton.Enabled = !Engine.Finished;
        stopButton.Enabled = false;
        resetButton.Enabled = true;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Engine_ExecutionTimeChanged(object sender, EventArgs e) {
      executionTimeCounter++;
      if ((executionTimeCounter == 100) || !Engine.Running) {
        executionTimeCounter = 0;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Engine_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Value);
    }
    #endregion

    #region Button events
    protected virtual void startButton_Click(object sender, EventArgs e) {
      Engine.Start();
    }
    protected virtual void stopButton_Click(object sender, EventArgs e) {
      Engine.Stop();
    }
    protected virtual void resetButton_Click(object sender, EventArgs e) {
      Engine.Initialize();
    }
    #endregion

    #region Helpers
    protected virtual void UpdateExecutionTimeTextBox() {
      if (InvokeRequired)
        Invoke(new Action(UpdateExecutionTimeTextBox));
      else
        executionTimeTextBox.Text = Engine.ExecutionTime.ToString();
    }
    #endregion
  }
}
