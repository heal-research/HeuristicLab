#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  partial class EngineView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.logTextBox = new System.Windows.Forms.TextBox();
      this.logLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(92, 0);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(338, 20);
      this.executionTimeTextBox.TabIndex = 1;
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.AutoSize = true;
      this.executionTimeLabel.Location = new System.Drawing.Point(3, 3);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(83, 13);
      this.executionTimeLabel.TabIndex = 0;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // logTextBox
      // 
      this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.logTextBox.Location = new System.Drawing.Point(0, 39);
      this.logTextBox.Multiline = true;
      this.logTextBox.Name = "logTextBox";
      this.logTextBox.ReadOnly = true;
      this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.logTextBox.Size = new System.Drawing.Size(430, 315);
      this.logTextBox.TabIndex = 3;
      this.logTextBox.WordWrap = false;
      // 
      // logLabel
      // 
      this.logLabel.AutoSize = true;
      this.logLabel.Location = new System.Drawing.Point(3, 23);
      this.logLabel.Name = "logLabel";
      this.logLabel.Size = new System.Drawing.Size(28, 13);
      this.logLabel.TabIndex = 2;
      this.logLabel.Text = "&Log:";
      // 
      // EngineView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.logTextBox);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.logLabel);
      this.Controls.Add(this.executionTimeLabel);
      this.Name = "EngineView";
      this.Size = new System.Drawing.Size(430, 354);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TextBox executionTimeTextBox;
    protected System.Windows.Forms.Label executionTimeLabel;
    protected System.Windows.Forms.TextBox logTextBox;
    protected System.Windows.Forms.Label logLabel;

  }
}
