#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm;

namespace HeuristicLab.Scripting.Views {
  [View("Executable Script View")]
  [Content(typeof(ExecutableScript), true)]
  public partial class ExecutableScriptView : ScriptView {
    private const string ScriptExecutionStartedMessage = "Script execution started";
    private const string ScriptExecutionCanceledMessage = "Script execution canceled";
    private const string ScriptExecutionSuccessfulMessage = "Script execution successful";
    private const string ScriptExecutionFailedMessage = "Script execution failed";

    protected bool Running {
      get { return Content.Running; }
    }

    public new ExecutableScript Content {
      get { return (ExecutableScript)base.Content; }
      set { base.Content = value; }
    }

    public ExecutableScriptView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ScriptExecutionStarted += ContentOnScriptExecutionStarted;
      Content.ScriptExecutionFinished += ContentOnScriptExecutionFinished;
      Content.ExecutionTimeChanged += ContentOnExecutionTimeChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.ScriptExecutionStarted -= ContentOnScriptExecutionStarted;
      Content.ScriptExecutionFinished -= ContentOnScriptExecutionFinished;
      Content.ExecutionTimeChanged -= ContentOnExecutionTimeChanged;
      base.DeregisterContentEvents();
    }

    #region Event Handlers
    protected virtual void ContentOnScriptExecutionStarted(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs>)ContentOnScriptExecutionStarted, sender, e);
      else {
        Locked = true;
        ReadOnly = true;
        startStopButton.Image = VSImageLibrary.Stop;
        toolTip.SetToolTip(startStopButton, "Stop (Shift+F5)");
        UpdateInfoTextLabel(ScriptExecutionStartedMessage, SystemColors.ControlText);
        infoTabControl.SelectedTab = outputTabPage;
      }
    }
    protected virtual void ContentOnScriptExecutionFinished(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs<Exception>>)ContentOnScriptExecutionFinished, sender, e);
      else {
        Locked = false;
        ReadOnly = false;
        startStopButton.Image = VSImageLibrary.Play;
        toolTip.SetToolTip(startStopButton, "Run (F5)");

        var ex = e.Value;
        if (ex == null) {
          UpdateInfoTextLabel(ScriptExecutionSuccessfulMessage, Color.DarkGreen);
        } else if (ex is ThreadAbortException) {
          // the execution was canceled by the user
          UpdateInfoTextLabel(ScriptExecutionCanceledMessage, Color.DarkOrange);
        } else {
          UpdateInfoTextLabel(ScriptExecutionFailedMessage, Color.DarkRed);
          PluginInfrastructure.ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    protected virtual void ContentOnExecutionTimeChanged(object sender, EventArgs eventArgs) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs>)ContentOnExecutionTimeChanged, sender, eventArgs);
      else {
        executionTimeTextBox.Text = Content == null ? "-" : Content.ExecutionTime.ToString();
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        executionTimeTextBox.Text = "-";
      } else {
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      startStopButton.Enabled = Content != null && (!Locked || Running);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      switch (keyData) {
        case Keys.F5:
          if (Content != null && !Locked && !Running) {
            if (Compile()) {
              outputTextBox.Clear();
              Content.ExecuteAsync();
            }
          }
          return true;
        case Keys.F5 | Keys.Shift:
          if (Running) Content.Kill();
          return true;
        case Keys.F6:
          if (!Running) base.ProcessCmdKey(ref msg, keyData);
          return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void startStopButton_Click(object sender, EventArgs e) {
      if (Running) {
        Content.Kill();
      } else if (Compile()) {
        outputTextBox.Clear();
        Content.ExecuteAsync();
      }
    }
  }
}
