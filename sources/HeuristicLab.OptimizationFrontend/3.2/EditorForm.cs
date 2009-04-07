#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.OptimizationFrontend {
  public partial class EditorForm : Form {
    private IEditor myEditor;
    public IEditor Editor {
      get { return myEditor; }
    }

    public EditorForm() {
      InitializeComponent();
    }
    public EditorForm(IEditor editor)
      : this() {
      myEditor = editor;
      if (Editor != null) {
        Control control = (Control)Editor;
        control.Dock = DockStyle.Fill;
        editorPanel.Controls.Add(control);
        Editor.CaptionChanged += new EventHandler(Editor_CaptionChanged);
        Editor.FilenameChanged += new EventHandler(Editor_FilenameChanged);
        UpdateText();
      } else {
        Label errorLabel = new Label();
        errorLabel.Name = "errorLabel";
        errorLabel.Text = "No editor available";
        errorLabel.AutoSize = false;
        errorLabel.Dock = DockStyle.Fill;
        editorPanel.Controls.Add(errorLabel);
      }
    }

    private void UpdateText() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(UpdateText));
      else {
        if (Editor.Filename == null)
          Text = "Untitled - " + Editor.Caption;
        else
          Text = Editor.Filename + " - " + Editor.Caption;
      }
    }

    #region Editor Events
    private void Editor_FilenameChanged(object sender, EventArgs e) {
      UpdateText();
    }
    private void Editor_CaptionChanged(object sender, EventArgs e) {
      UpdateText();
    }
    #endregion
  }
}
