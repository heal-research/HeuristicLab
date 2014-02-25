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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Scripting.Views {

  [View("Script View")]
  [Content(typeof(Script), true)]
  public partial class ScriptView : NamedItemView {
    private bool running;

    public new Script Content {
      get { return (Script)base.Content; }
      set { base.Content = (Script)value; }
    }

    public ScriptView() {
      InitializeComponent();
      errorListView.SmallImageList.Images.AddRange(new Image[] { VSImageLibrary.Warning, VSImageLibrary.Error });
      AdjustErrorListViewColumnSizes();
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
      if (InvokeRequired)
        Invoke((Action<object, EventArgs>)Content_ScriptExecutionStarted, sender, e);
      else {
        Locked = true;
        ReadOnly = true;
        startStopButton.Image = VSImageLibrary.Stop;
        infoTabControl.SelectedTab = outputTabPage;
      }
    }
    private void Content_ScriptExecutionFinished(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs<Exception>>)Content_ScriptExecutionFinished, sender, e);
      else {
        Locked = false;
        ReadOnly = false;
        startStopButton.Image = VSImageLibrary.Play;
        running = false;
        var ex = e.Value;
        if (ex != null)
          PluginInfrastructure.ErrorHandling.ShowErrorDialog(this, ex);
      }
    }
    private void Content_ConsoleOutputChanged(object sender, EventArgs<string> e) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs<string>>)Content_ConsoleOutputChanged, sender, e);
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
      compileButton.Enabled = Content != null && !Locked && !ReadOnly;
      startStopButton.Enabled = Content != null && (!Locked || running);
      codeEditor.Enabled = Content != null && !Locked && !ReadOnly;
    }

    #region Child Control event handlers
    private void compileButton_Click(object sender, EventArgs e) {
      Compile();
    }

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

    private void codeEditor_TextEditorTextChanged(object sender, EventArgs e) {
      Content.Code = codeEditor.UserCode;
    }
    #endregion

    #region global HotKeys
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      switch (keyData) {
        case Keys.F5:
          if (Content != null && !Locked) {
            if (Compile()) {
              outputTextBox.Clear();
              Content.Execute();
              running = true;
            } else
              infoTabControl.SelectedTab = errorListTabPage;
          }
          break;
        case Keys.F5 | Keys.Shift:
          if (running) Content.Kill();
          break;
        case Keys.F6:
          if (!Compile() || Content.CompileErrors.HasWarnings)
            infoTabControl.SelectedTab = errorListTabPage;
          break;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion

    #region Auxiliary functions
    private bool Compile() {
      ReadOnly = true;
      Locked = true;
      errorListView.Items.Clear();
      outputTextBox.Text = "Compiling ... ";
      try {
        Content.Compile();
        outputTextBox.AppendText("Compilation succeeded.");
        return true;
      } catch {
        outputTextBox.AppendText("Compilation failed.");
        return false;
      } finally {
        ShowCompilationResults();
        ReadOnly = false;
        Locked = false;
        OnContentChanged();
      }
    }
    #endregion

    private void ShowCompilationResults() {
      if (Content.CompileErrors.Count == 0) return;
      var msgs = Content.CompileErrors.OfType<CompilerError>()
                                      .OrderBy(x => x.IsWarning)
                                      .ThenBy(x => x.Line)
                                      .ThenBy(x => x.Column);
      foreach (var m in msgs) {
        var item = new ListViewItem();
        item.SubItems.AddRange(new[] {
          m.IsWarning ? "Warning" : "Error",
          m.ErrorNumber,
          m.Line.ToString(CultureInfo.InvariantCulture),
          m.Column.ToString(CultureInfo.InvariantCulture),
          m.ErrorText
        });
        item.ImageIndex = m.IsWarning ? 0 : 1;
        errorListView.Items.Add(item);
      }
      AdjustErrorListViewColumnSizes();
    }

    private void AdjustErrorListViewColumnSizes() {
      foreach (ColumnHeader ch in errorListView.Columns)
        // adjusts the column width to the width of the column
        // header or the column content, whichever is greater
        ch.Width = -2;
    }
  }
}