#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using ICSharpCode.AvalonEdit;

namespace HeuristicLab.CodeEditor {
  internal partial class GoToLineDialog : Form {
    private readonly TextEditor textEditor;
    private readonly int lowerLimit, upperLimit;

    public int Line { get; set; }

    public GoToLineDialog(TextEditor textEditor) {
      InitializeComponent();
      this.textEditor = textEditor;
      lowerLimit = 1;
      upperLimit = textEditor.LineCount;
      lineNumberLabel.Text = string.Format("Line Number ({0}-{1}):", lowerLimit, upperLimit);
    }

    private void GoToLineDialog_Load(object sender, EventArgs e) {
      var caret = textEditor.TextArea.Caret;
      lineNumberTextBox.Text = caret.Line.ToString();
      lineNumberTextBox.SelectAll();
    }

    private void lineNumberTextBox_TextChanged(object sender, EventArgs e) {
      int line;
      if (!int.TryParse(lineNumberTextBox.Text, out line) || line < lowerLimit || line > upperLimit) {
        errorProvider.SetError(lineNumberTextBox, string.Format("Value must be an integer in the range [{0},{1}].", lowerLimit, upperLimit));
        okButton.Enabled = false;
      } else {
        errorProvider.SetError(lineNumberTextBox, null);
        okButton.Enabled = true;
      }
      Line = line;
    }
  }
}
