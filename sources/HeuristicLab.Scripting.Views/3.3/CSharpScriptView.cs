#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm;

namespace HeuristicLab.Scripting.Views {

  [View("C# Script View")]
  [Content(typeof(CSharpScript), true)]
  public partial class CSharpScriptView : ScriptView {
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
        Running = false;
        var ex = e.Value;
        if (ex != null)
          PluginInfrastructure.ErrorHandling.ShowErrorDialog(this, ex);
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
          Content.Execute();
          Running = true;
        }
    }

    #region global HotKeys
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      switch (keyData) {
        case Keys.F5:
          if (Content != null && !Locked && !Running) {
            if (Compile()) {
              outputTextBox.Clear();
              Content.Execute();
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