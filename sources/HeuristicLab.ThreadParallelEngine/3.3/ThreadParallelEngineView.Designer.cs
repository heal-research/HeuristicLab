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

namespace HeuristicLab.ThreadParallelEngine {
  partial class ThreadParallelEngineView {
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
      this.workersNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.workersLabel = new System.Windows.Forms.Label();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.operatorGraphGroupBox.SuspendLayout();
      this.globalScopeGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.workersNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.TabIndex = 7;
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.TabIndex = 6;
      // 
      // splitContainer1
      // 
      // 
      // workersNumericUpDown
      // 
      this.workersNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.workersNumericUpDown.Location = new System.Drawing.Point(406, 460);
      this.workersNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.workersNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.workersNumericUpDown.Name = "workersNumericUpDown";
      this.workersNumericUpDown.Size = new System.Drawing.Size(87, 20);
      this.workersNumericUpDown.TabIndex = 5;
      this.workersNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.workersNumericUpDown.ValueChanged += new System.EventHandler(this.workersNumericUpDown_ValueChanged);
      // 
      // workersLabel
      // 
      this.workersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.workersLabel.AutoSize = true;
      this.workersLabel.Location = new System.Drawing.Point(403, 444);
      this.workersLabel.Name = "workersLabel";
      this.workersLabel.Size = new System.Drawing.Size(87, 13);
      this.workersLabel.TabIndex = 4;
      this.workersLabel.Text = "&Worker Threads:";
      // 
      // ThreadParallelEngineEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.workersNumericUpDown);
      this.Controls.Add(this.workersLabel);
      this.Name = "ThreadParallelEngineEditor";
      this.Controls.SetChildIndex(this.workersLabel, 0);
      this.Controls.SetChildIndex(this.workersNumericUpDown, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executeButton, 0);
      this.Controls.SetChildIndex(this.abortButton, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.splitContainer1, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.operatorGraphGroupBox.ResumeLayout(false);
      this.globalScopeGroupBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.workersNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.NumericUpDown workersNumericUpDown;
    private System.Windows.Forms.Label workersLabel;

  }
}
