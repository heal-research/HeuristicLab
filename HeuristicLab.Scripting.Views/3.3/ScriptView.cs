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
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Scripting.Views {

  [View("Script View")]
  [Content(typeof(Script), true)]
  public partial class ScriptView : NamedItemView {
    public new Script Content {
      get { return (Script)base.Content; }
      set { base.Content = value; }
    }

    public ScriptView() {
      InitializeComponent();
      errorListView.SmallImageList.Images.AddRange(new Image[] { VSImageLibrary.Warning, VSImageLibrary.Error });
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CodeChanged += ContentOnCodeChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.CodeChanged -= ContentOnCodeChanged;
      base.DeregisterContentEvents();
    }

    protected virtual void ContentOnCodeChanged(object sender, EventArgs e) {
      codeEditor.UserCode = Content.Code;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        codeEditor.UserCode = string.Empty;
      } else {
        codeEditor.UserCode = Content.Code;
        foreach (var asm in Content.GetAssemblies())
          codeEditor.AddAssembly(asm);
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
      codeEditor.Enabled = Content != null && !Locked && !ReadOnly;
    }

    protected virtual void CompileButtonOnClick(object sender, EventArgs e) {
      Compile();
    }

    protected virtual void CodeEditorOnTextEditorTextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      Content.Code = codeEditor.UserCode;
    }
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      switch (keyData) {
        case Keys.F6:
          if (Content != null && !Locked)
            Compile();
          return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    public virtual bool Compile() {
      ReadOnly = true;
      Locked = true;
      errorListView.Items.Clear();
      outputTextBox.Text = "Compiling ... ";
      try {
        Content.Compile();
        outputTextBox.AppendText("Compilation succeeded.");
        return true;
      } catch {
        if (Content.CompileErrors.HasErrors) {
          outputTextBox.AppendText("Compilation failed.");
          return false;
        } else {
          outputTextBox.AppendText("Compilation succeeded.");
          return true;
        }
      } finally {
        ShowCompilationResults();
        if (Content.CompileErrors.Count > 0)
          infoTabControl.SelectedTab = errorListTabPage;
        ReadOnly = false;
        Locked = false;
        OnContentChanged();
      }
    }

    protected virtual void ShowCompilationResults() {
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

    protected virtual void AdjustErrorListViewColumnSizes() {
      foreach (ColumnHeader ch in errorListView.Columns)
        // adjusts the column width to the width of the column header
        ch.Width = -2;
    }
  }
}