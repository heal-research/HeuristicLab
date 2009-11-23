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
using WeifenLuo.WinFormsUI.Docking;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;

namespace HeuristicLab.AdvancedOptimizationFrontend {
  /// <summary>
  /// Displays the used editor.
  /// </summary>
  public partial class EditorForm : DockContent {
    private IEditor myEditor;
    /// <summary>
    /// Gets the editor that is displayed.
    /// </summary>
    public IEditor Editor {
      get { return myEditor; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EditorForm"/>.
    /// </summary>
    public EditorForm() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="EditorForm"/> with the given <paramref name="editor"/>
    /// that should be displayed.
    /// </summary>
    /// <param name="editor">The editor that should be displayed.</param>
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

    private void EditorForm_TextChanged(object sender, EventArgs e) {
      TabText = Text;
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
