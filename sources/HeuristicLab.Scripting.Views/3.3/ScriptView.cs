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
    #region Properties
    public new Script Content {
      get { return (Script)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get { return codeEditor.ReadOnly || base.ReadOnly; }
      set { base.ReadOnly = codeEditor.ReadOnly = value; }
    }

    public override bool Locked {
      get { return codeEditor.ReadOnly || base.Locked; }
      set { base.Locked = codeEditor.ReadOnly = value; }
    }
    #endregion

    public ScriptView() {
      InitializeComponent();
      errorListView.SmallImageList.Images.AddRange(new Image[] { VSImageLibrary.Warning, VSImageLibrary.Error });
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CodeChanged += Content_CodeChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.CodeChanged -= Content_CodeChanged;
      base.DeregisterContentEvents();
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        codeEditor.UserCode = string.Empty;
      } else {
        if (codeEditor.UserCode != Content.Code)
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
      codeEditor.Enabled = Content != null;
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
    #endregion

    public virtual bool Compile() {
      ReadOnly = true;
      Locked = true;
      errorListView.Items.Clear();
      outputTextBox.Text = "Compiling ... ";
      try {
        Content.Compile();
        outputTextBox.AppendText("Compilation succeeded.");
        return true;
      } catch (InvalidOperationException) {
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
        codeEditor.Focus();
        OnContentChanged();
      }
    }

    #region Helpers
    protected virtual void ShowCompilationResults() {
      if (Content.CompileErrors.Count == 0) return;

      var messages = Content.CompileErrors.OfType<CompilerError>()
                                      .OrderBy(x => x.IsWarning)
                                      .ThenBy(x => x.Line)
                                      .ThenBy(x => x.Column);

      foreach (var m in messages) {
        var item = new ListViewItem { Tag = m };
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

      codeEditor.ShowCompileErrors(Content.CompileErrors, ".cs");

      AdjustErrorListViewColumnSizes();
    }

    protected virtual void AdjustErrorListViewColumnSizes() {
      foreach (ColumnHeader ch in errorListView.Columns)
        // adjusts the column width to the width of the column header
        ch.Width = -2;
    }
    #endregion

    #region Event Handlers
    private void Content_CodeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_CodeChanged), sender, e);
      else {
        codeEditor.UserCode = Content.Code;
      }
    }

    private void compileButton_Click(object sender, EventArgs e) {
      Compile();
    }

    private void codeEditor_TextEditorTextChanged(object sender, EventArgs e) {
      if (Content == null) return;
      Content.Code = codeEditor.UserCode;
    }

    private void errorListView_MouseDoubleClick(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        var item = errorListView.SelectedItems[0];
        var message = (CompilerError)item.Tag;
        codeEditor.ScrollToPosition(message.Line, message.Column);
        codeEditor.Focus();
      }
    }
    #endregion
  }
}