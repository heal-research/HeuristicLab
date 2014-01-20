#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.CodeEditor;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.HLScript.Views {

  [View("HLScript View")]
  [Content(typeof(HLScript), true)]
  public partial class HLScriptView : NamedItemView {
    private bool running;

    public new HLScript Content {
      get { return (HLScript)base.Content; }
      set { base.Content = (HLScript)value; }
    }

    public HLScriptView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CodeChanged += Content_CodeChanged;
      Content.ScriptExecutionStarted += Content_ScriptExecutionStarted;
      Content.ScriptExecutionFinished += Content_ScriptExecutionFinished;
      Content.ConsoleOutputChanged += Content_ConsoleOutputChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.CodeChanged -= Content_CodeChanged;
      Content.ScriptExecutionStarted -= Content_ScriptExecutionStarted;
      Content.ScriptExecutionFinished -= Content_ScriptExecutionFinished;
      Content.ConsoleOutputChanged -= Content_ConsoleOutputChanged;
      base.DeregisterContentEvents();
    }

    #region Content event handlers
    private void Content_CodeChanged(object sender, EventArgs e) {
      codeEditor.UserCode = Content.Code;
    }
    private void Content_ScriptExecutionStarted(object sender, EventArgs e) {
      Locked = true;
      startStopButton.Image = VSImageLibrary.Stop;
    }
    private void Content_ScriptExecutionFinished(object sender, EventArgs e) {
      Locked = false;
      startStopButton.Image = VSImageLibrary.Play;
      running = false;
    }
    private void Content_ConsoleOutputChanged(object sender, EventArgs<string> e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs<string>>)Content_ConsoleOutputChanged, sender, e);
      else {
        outputTextBox.AppendText(e.Value);
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        codeEditor.UserCode = string.Empty;
        variableStoreView.Content = null;
      } else {
        codeEditor.UserCode = Content.Code;
        foreach (var asm in Content.GetAssemblies())
          codeEditor.AddAssembly(asm);
        variableStoreView.Content = Content.VariableStore;
        if (Content.CompileErrors == null) {
          compilationLabel.ForeColor = SystemColors.ControlDarkDark;
          compilationLabel.Text = "Not compiled";
        } else if (Content.CompileErrors.HasErrors) {
          compilationLabel.ForeColor = Color.DarkRed;
          compilationLabel.Text = "Compilation failed";
        } else {
          compilationLabel.ForeColor = Color.DarkGreen;
          compilationLabel.Text = "Compilation successful";
        }
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      startStopButton.Enabled = Content != null && !Locked;
      showCodeButton.Enabled = Content != null && !string.IsNullOrEmpty(Content.CompilationUnitCode);
      codeEditor.Enabled = Content != null && !Locked && !ReadOnly;
    }

    #region Child Control event handlers
    private void startStopButton_Click(object sender, EventArgs e) {
      if (running) {
        Content.Kill();
      } else
        if (Compile()) {
          outputTextBox.Clear();
          Content.Execute();
          running = true;
        }
    }

    private void showCodeButton_Click(object sender, EventArgs e) {
      new CodeViewer(Content.CompilationUnitCode).ShowDialog(this);
    }

    private void codeEditor_TextEditorTextChanged(object sender, EventArgs e) {
      Content.Code = codeEditor.UserCode;
    }
    #endregion

    #region global HotKeys
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      if (keyData == Keys.F5) {
        if (Content == null || Locked)
          return base.ProcessCmdKey(ref msg, keyData);
        outputTextBox.Clear();
        bool result = Compile();
        if (result) {
          outputTextBox.Clear();
          Content.Execute();
          running = true;
        }
        return true;
      } else if (keyData == (Keys.F5 | Keys.Shift)) {
        Content.Kill();
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion

    #region Auxiliary functions
    private bool Compile() {
      ReadOnly = true;
      Locked = true;
      errorListView.Items.Clear();
      outputTextBox.Clear();
      outputTextBox.AppendText("Compiling ... ");
      try {
        Content.Compile();
        outputTextBox.AppendText("Compilation succeeded.");
        return true;
      } catch {
        outputTextBox.AppendText("Compilation failed.");
        ShowCompilationErrors();
        return false;
      } finally {
        OnContentChanged();
        ReadOnly = false;
        Locked = false;
      }
    }
    #endregion

    private void ShowCompilationErrors() {
      if (Content.CompileErrors.Count == 0) return;
      var warnings = new List<CompilerError>();
      var errors = new List<CompilerError>();
      foreach (CompilerError ce in Content.CompileErrors) {
        if (ce.IsWarning) warnings.Add(ce);
        else errors.Add(ce);
      }
      var msgs = warnings.OrderBy(x => x.Line)
                         .ThenBy(x => x.Column)
                   .Concat(errors.OrderBy(x => x.Line)
                                 .ThenBy(x => x.Column));
      outputTextBox.AppendText(Environment.NewLine);
      outputTextBox.AppendText("---");
      outputTextBox.AppendText(Environment.NewLine);
      foreach (var m in msgs) {
        var item = new ListViewItem(new[] {
          m.IsWarning ? "Warning" : "Error",
          m.ErrorNumber,
          m.Line.ToString(),
          m.Column.ToString(),
          m.ErrorText
        });
        errorListView.Items.Add(item);
        outputTextBox.AppendText(string.Format("{0} {1} ({2}:{3}): {4}", item.SubItems[0].Text, item.SubItems[1].Text, item.SubItems[2].Text, item.SubItems[3].Text, item.SubItems[4].Text));
        outputTextBox.AppendText(Environment.NewLine);
      }
    }
  }
}