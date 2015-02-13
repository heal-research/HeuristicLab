#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

  [View("C# Script View")]
  [Content(typeof(CSharpScript), true)]
  public partial class CSharpScriptView : ScriptView {
    private const string ScriptExecutionStartedMessage = "Script execution started";
    private const string ScriptExecutionCanceledMessage = "Script execution canceled";
    private const string ScriptExecutionSuccessfulMessage = "Script execution successful";
    private const string ScriptExecutionFailedMessage = "Script execution failed";

    protected bool Running { get; set; }

    public new CSharpScript Content {
      get { return (CSharpScript)base.Content; }
      set { base.Content = value; }
    }

    public CSharpScriptView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ScriptExecutionStarted += ContentOnScriptExecutionStarted;
      Content.ScriptExecutionFinished += ContentOnScriptExecutionFinished;
      Content.ConsoleOutputChanged += ContentOnConsoleOutputChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.ScriptExecutionStarted -= ContentOnScriptExecutionStarted;
      Content.ScriptExecutionFinished -= ContentOnScriptExecutionFinished;
      Content.ConsoleOutputChanged -= ContentOnConsoleOutputChanged;
      base.DeregisterContentEvents();
    }

    #region Content event handlers
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

        Running = false;
      }
    }
    protected virtual void ContentOnConsoleOutputChanged(object sender, EventArgs<string> e) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs<string>>)ContentOnConsoleOutputChanged, sender, e);
      else {
        outputTextBox.AppendText(e.Value);
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        variableStoreView.Content = null;
      } else {
        variableStoreView.Content = Content.VariableStore;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      startStopButton.Enabled = Content != null && (!Locked || Running);
    }

    protected virtual void StartStopButtonOnClick(object sender, EventArgs e) {
      if (Running) {
        Content.Kill();
      } else
        if (Compile()) {
          outputTextBox.Clear();
          Running = true;
          Content.ExecuteAsync();
        }
    }

    #region global HotKeys
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      switch (keyData) {
        case Keys.F5:
          if (Content != null && !Locked && !Running) {
            if (Compile()) {
              outputTextBox.Clear();
              Content.ExecuteAsync();
              Running = true;
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
    #endregion
  }
}