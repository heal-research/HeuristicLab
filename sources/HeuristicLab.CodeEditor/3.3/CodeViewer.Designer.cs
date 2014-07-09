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

namespace HeuristicLab.CodeEditor {
  partial class CodeViewer {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.button1 = new System.Windows.Forms.Button();
      this.textEditorControl1 = new ICSharpCode.TextEditor.TextEditorControl();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(509, 296);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // textEditorControl1
      // 
      this.textEditorControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textEditorControl1.IsReadOnly = false;
      this.textEditorControl1.Location = new System.Drawing.Point(12, 12);
      this.textEditorControl1.Name = "textEditorControl1";
      this.textEditorControl1.ShowEOLMarkers = true;
      this.textEditorControl1.ShowSpaces = true;
      this.textEditorControl1.ShowTabs = true;
      this.textEditorControl1.ShowVRuler = false;
      this.textEditorControl1.Size = new System.Drawing.Size(572, 278);
      this.textEditorControl1.TabIndex = 2;
      this.textEditorControl1.Text = "textEditorControl1";
      // 
      // CodeViewer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(596, 331);
      this.Controls.Add(this.textEditorControl1);
      this.Controls.Add(this.button1);
      this.Icon = HeuristicLab.Common.Resources.HeuristicLab.Icon;
      this.Name = "CodeViewer";
      this.Text = "GeneratedCode";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private ICSharpCode.TextEditor.TextEditorControl textEditorControl1;
  }
}