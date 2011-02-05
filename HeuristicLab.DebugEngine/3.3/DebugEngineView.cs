#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using System.ComponentModel;
namespace HeuristicLab.DebugEngine {

  /// <summary>
  /// Engine espcially for debugging
  /// </summary>
  [View("DebugEngine View")]
  [Content(typeof(DebugEngine), true)]
  public partial class DebugEngineView : ItemView {

    #region Basics

    /// <summary>
    /// Gets or sets the current engine.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>.</remarks>
    public new DebugEngine Content {
      get { return (DebugEngine)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DebugEngineView"/>.
    /// </summary>
    public DebugEngineView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds event handlers to the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="EditorBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        executionStackView.Content = null;
        operatorTraceView.Content = null;
        operationContentView.Content = null;
      } else {
        executionStackView.Content = Content.ExecutionStack;
        operatorTraceView.Content = Content.OperatorTrace;
        operationContentView.Content = new OperationContent(Content.CurrentOperation);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        stepButton.Enabled = false;
        refreshButton.Enabled = false;
      } else {
        stepButton.Enabled = Content.CanContinue && Content.ExecutionState != ExecutionState.Started;
        refreshButton.Enabled = Content.CurrentAtomicOperation != null && Content.ExecutionState != ExecutionState.Started;
      }
    }

    #endregion

    void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      } else {
        SetEnabledStateOfControls();
        switch (Content.ExecutionState) {
          case ExecutionState.Started:
            if (!stepping) {
              executionStackView.Content = null;
              operatorTraceView.Content = null;
              operationContentView.Content = null;
            }
            break;
          default:
            executionStackView.Content = Content.ExecutionStack;
            operatorTraceView.Content = Content.OperatorTrace;
            operationContentView.Content = new OperationContent(Content.CurrentOperation);
            break;
        }
      }
    }

    private bool stepping = false;
    private void stepButton_Click(object sender, EventArgs e) {
      BackgroundWorker worker = new BackgroundWorker();
      bool skipStackops = skipStackOpsCheckBox.Checked;
      worker.DoWork += (s, a) => {
        Content.Step(skipStackops);
      };
      worker.RunWorkerCompleted += (s, a) => {
        stepping = false;
        SetEnabledStateOfControls();
      };
      stepping = true;
      stepButton.Enabled = false;
      worker.RunWorkerAsync();
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      var content = Content;
      Content = null;
      Content = content;
    }

  }
}
