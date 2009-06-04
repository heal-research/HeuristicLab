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

namespace HeuristicLab.Hive.Engine {
  partial class HiveEngineEditor {
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
      this.urlTextBox = new System.Windows.Forms.TextBox();
      this.urlLabel = new System.Windows.Forms.Label();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.operatorGraphGroupBox.SuspendLayout();
      this.globalScopeGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(686, 460);
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(683, 444);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Size = new System.Drawing.Size(827, 441);
      this.splitContainer1.SplitterDistance = 639;
      // 
      // operatorGraphGroupBox
      // 
      this.operatorGraphGroupBox.Size = new System.Drawing.Size(639, 441);
      // 
      // globalScopeGroupBox
      // 
      this.globalScopeGroupBox.Size = new System.Drawing.Size(184, 441);
      // 
      // operatorGraphView
      // 
      this.operatorGraphView.Size = new System.Drawing.Size(633, 422);
      // 
      // scopeView
      // 
      this.scopeView.Size = new System.Drawing.Size(178, 422);
      // 
      // urlTextBox
      // 
      this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.urlTextBox.Location = new System.Drawing.Point(526, 459);
      this.urlTextBox.Name = "urlTextBox";
      this.urlTextBox.Size = new System.Drawing.Size(154, 20);
      this.urlTextBox.TabIndex = 7;
      // 
      // urlLabel
      // 
      this.urlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.urlLabel.AutoSize = true;
      this.urlLabel.Location = new System.Drawing.Point(523, 444);
      this.urlLabel.Name = "urlLabel";
      this.urlLabel.Size = new System.Drawing.Size(82, 13);
      this.urlLabel.TabIndex = 8;
      this.urlLabel.Text = "Hive Server Url:";
      // 
      // HiveEngineEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.urlTextBox);
      this.Controls.Add(this.urlLabel);
      this.Name = "HiveEngineEditor";
      this.Size = new System.Drawing.Size(827, 480);
      this.Controls.SetChildIndex(this.executeButton, 0);
      this.Controls.SetChildIndex(this.abortButton, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.splitContainer1, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.urlLabel, 0);
      this.Controls.SetChildIndex(this.urlTextBox, 0);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.operatorGraphGroupBox.ResumeLayout(false);
      this.globalScopeGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox urlTextBox;
    private System.Windows.Forms.Label urlLabel;
  }
}
