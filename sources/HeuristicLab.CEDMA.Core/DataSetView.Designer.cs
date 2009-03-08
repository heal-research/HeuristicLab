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

namespace HeuristicLab.CEDMA.Core {
  partial class DataSetView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.activateButton = new System.Windows.Forms.Button();
      this.resultsButton = new System.Windows.Forms.Button();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.viewComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // editorGroupBox
      // 
      this.editorGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.editorGroupBox.Location = new System.Drawing.Point(0, 3);
      this.editorGroupBox.Name = "editorGroupBox";
      this.editorGroupBox.Size = new System.Drawing.Size(381, 218);
      this.editorGroupBox.TabIndex = 0;
      this.editorGroupBox.TabStop = false;
      this.editorGroupBox.Text = "&Editor:";
      // 
      // activateButton
      // 
      this.activateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.activateButton.Location = new System.Drawing.Point(0, 227);
      this.activateButton.Name = "activateButton";
      this.activateButton.Size = new System.Drawing.Size(75, 23);
      this.activateButton.TabIndex = 2;
      this.activateButton.Text = "&Activate";
      this.activateButton.UseVisualStyleBackColor = true;
      this.activateButton.Click += new System.EventHandler(this.activateButton_Click);
      // 
      // resultsButton
      // 
      this.resultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resultsButton.Location = new System.Drawing.Point(130, 254);
      this.resultsButton.Name = "resultsButton";
      this.resultsButton.Size = new System.Drawing.Size(86, 23);
      this.resultsButton.TabIndex = 3;
      this.resultsButton.Text = "Show results";
      this.resultsButton.UseVisualStyleBackColor = true;
      this.resultsButton.Click += new System.EventHandler(this.resultsButton_Click);
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(222, 254);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(156, 23);
      this.progressBar.TabIndex = 4;
      // 
      // viewComboBox
      // 
      this.viewComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.viewComboBox.FormattingEnabled = true;
      this.viewComboBox.Location = new System.Drawing.Point(3, 256);
      this.viewComboBox.Name = "viewComboBox";
      this.viewComboBox.Size = new System.Drawing.Size(121, 21);
      this.viewComboBox.TabIndex = 5;
      // 
      // DataSetView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.viewComboBox);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.resultsButton);
      this.Controls.Add(this.activateButton);
      this.Controls.Add(this.editorGroupBox);
      this.Name = "DataSetView";
      this.Size = new System.Drawing.Size(381, 280);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox editorGroupBox;
    private System.Windows.Forms.Button activateButton;
    private System.Windows.Forms.Button resultsButton;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.ComboBox viewComboBox;
  }
}
