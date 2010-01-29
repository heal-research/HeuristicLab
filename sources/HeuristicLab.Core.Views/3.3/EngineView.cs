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
    public new Engine Content {
      get { return (Engine)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBaseEditor"/>.
    /// </summary>
    public EngineView() {
      InitializeComponent();
    }
    public EngineView(Engine engine)
      : this() {
      Content = engine;
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.OperatorGraphChanged -= new EventHandler(Content_OperatorGraphChanged);
      Content.Initialized -= new EventHandler(Content_Initialized);
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
      Content.Initialized += new EventHandler(Content_Initialized);
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
        operatorGraphView.Enabled = false;
        scopeView.Enabled = false;
        startButton.Enabled = false;
        resetButton.Enabled = false;
        executionTimeTextBox.Enabled = false;
      } else {
        operatorGraphView.Content = Content.OperatorGraph;
        scopeView.Content = Content.GlobalScope;
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
    protected virtual void Content_Initialized(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Initialized), sender, e);
      else {
        operatorGraphView.Enabled = true;
        scopeView.Enabled = true;
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
        operatorGraphView.Enabled = false;
        scopeView.Enabled = false;
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
        operatorGraphView.Enabled = true;
        scopeView.Enabled = true;
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
    protected virtual void startButton_Click(object sender, EventArgs e) {
      Content.Start();
    }
    protected virtual void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    protected virtual void resetButton_Click(object sender, EventArgs e) {
      Content.Initialize();
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
