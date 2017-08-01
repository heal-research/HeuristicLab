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
using System.Windows.Forms;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HeuristicLab.CodeEditor {
  public partial class CodeViewer : Form {
    private const string DefaultTextEditorSyntaxHighlighting = "C#";
    private const string DefaultTextEditorFontFamily = "Consolas";
    private const double DefaultTextEditorFontSize = 13.0;
    private const bool DefaultTextEditorShowLineNumbers = true;
    private const bool DefaultTextEditorShowEndOfLine = true;
    private const bool DefaultTextEditorShowSpaces = true;
    private const bool DefaultTextEditorShowTabs = true;
    private const bool DefaultTextEditorConvertTabsToSpaces = true;

    private TextEditor TextEditor { get { return avalonEditWrapper.TextEditor; } }

    public CodeViewer() {
      InitializeComponent();
      TextEditor.IsReadOnly = true;
      TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(DefaultTextEditorSyntaxHighlighting);
      TextEditor.FontFamily = new FontFamily(DefaultTextEditorFontFamily);
      TextEditor.FontSize = DefaultTextEditorFontSize;
      TextEditor.ShowLineNumbers = DefaultTextEditorShowLineNumbers;
      TextEditor.Options.ShowEndOfLine = DefaultTextEditorShowEndOfLine;
      TextEditor.Options.ShowSpaces = DefaultTextEditorShowSpaces;
      TextEditor.Options.ShowTabs = DefaultTextEditorShowTabs;
      TextEditor.Options.ConvertTabsToSpaces = DefaultTextEditorConvertTabsToSpaces;
    }

    public CodeViewer(string code)
      : this() {
      TextEditor.Text = code;
    }

    private void okButton_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
