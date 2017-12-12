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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class LineChartView {
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
      this.optionsBox = new System.Windows.Forms.GroupBox();
      this.allInOneCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.optionsBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.optionsBox);
      // 
      // optionsBox
      // 
      this.optionsBox.Controls.Add(this.allInOneCheckBox);
      this.optionsBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.optionsBox.Location = new System.Drawing.Point(0, 357);
      this.optionsBox.Name = "optionsBox";
      this.optionsBox.Size = new System.Drawing.Size(180, 46);
      this.optionsBox.TabIndex = 7;
      this.optionsBox.TabStop = false;
      this.optionsBox.Text = "Options";
      // 
      // allInOneCheckBox
      // 
      this.allInOneCheckBox.AutoSize = true;
      this.allInOneCheckBox.Checked = true;
      this.allInOneCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.allInOneCheckBox.Location = new System.Drawing.Point(6, 19);
      this.allInOneCheckBox.Name = "allInOneCheckBox";
      this.allInOneCheckBox.Size = new System.Drawing.Size(69, 17);
      this.allInOneCheckBox.TabIndex = 0;
      this.allInOneCheckBox.Text = "All in one";
      this.allInOneCheckBox.UseVisualStyleBackColor = true;
      this.allInOneCheckBox.CheckedChanged += new System.EventHandler(this.allInOneCheckBox_CheckedChanged);
      // 
      // LineChartView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "LineChartView";
      this.splitContainer.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.optionsBox.ResumeLayout(false);
      this.optionsBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox optionsBox;
    private System.Windows.Forms.CheckBox allInOneCheckBox;
  }
}
