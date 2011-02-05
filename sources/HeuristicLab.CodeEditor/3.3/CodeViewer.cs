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

namespace HeuristicLab.CodeEditor {
  public partial class CodeViewer : Form {

    public CodeViewer() {
      InitializeComponent();
      textEditorControl1.IsReadOnly = true;
      textEditorControl1.SetHighlighting("C#");
    }

    public CodeViewer(string code)
      : this() {
      textEditorControl1.Text = code;
    }

    private void button1_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
