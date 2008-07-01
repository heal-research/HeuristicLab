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

namespace HeuristicLab.CEDMA.Console {
  partial class AgentView {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.editorGroupBox = new System.Windows.Forms.GroupBox();
      this.saveButton = new System.Windows.Forms.Button();
      this.activateButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // editorGroupBox
      // 
      this.editorGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.editorGroupBox.Location = new System.Drawing.Point(0, 3);
      this.editorGroupBox.Name = "editorGroupBox";
      this.editorGroupBox.Size = new System.Drawing.Size(274, 119);
      this.editorGroupBox.TabIndex = 0;
      this.editorGroupBox.TabStop = false;
      this.editorGroupBox.Text = "&Editor:";
      // 
      // saveButton
      // 
      this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.saveButton.Location = new System.Drawing.Point(4, 128);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(75, 23);
      this.saveButton.TabIndex = 1;
      this.saveButton.Text = "&Save";
      this.saveButton.UseVisualStyleBackColor = true;
      // 
      // activateButton
      // 
      this.activateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.activateButton.Location = new System.Drawing.Point(85, 128);
      this.activateButton.Name = "activateButton";
      this.activateButton.Size = new System.Drawing.Size(75, 23);
      this.activateButton.TabIndex = 2;
      this.activateButton.Text = "&Activate";
      this.activateButton.UseVisualStyleBackColor = true;
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Location = new System.Drawing.Point(166, 128);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(75, 23);
      this.stopButton.TabIndex = 3;
      this.stopButton.Text = "S&top";
      this.stopButton.UseVisualStyleBackColor = true;
      // 
      // AgentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.activateButton);
      this.Controls.Add(this.saveButton);
      this.Controls.Add(this.editorGroupBox);
      this.Name = "AgentView";
      this.Size = new System.Drawing.Size(274, 154);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox editorGroupBox;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.Button activateButton;
    private System.Windows.Forms.Button stopButton;
  }
}
