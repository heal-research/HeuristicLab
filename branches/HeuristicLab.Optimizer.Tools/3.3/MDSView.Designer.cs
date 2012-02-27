#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimizer.Tools {
  partial class MDSView {
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
      this.visualizeCheckBox = new System.Windows.Forms.CheckBox();
      this.mdsPictureBox = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.mdsPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // rowsTextBox
      // 
      this.errorProvider.SetIconAlignment(this.rowsTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.rowsTextBox, 2);
      this.rowsTextBox.Size = new System.Drawing.Size(282, 20);
      // 
      // columnsTextBox
      // 
      this.columnsTextBox.Size = new System.Drawing.Size(282, 20);
      // 
      // visualizeCheckBox
      // 
      this.visualizeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.visualizeCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.visualizeCheckBox.Location = new System.Drawing.Point(353, 0);
      this.visualizeCheckBox.Name = "visualizeCheckBox";
      this.visualizeCheckBox.Size = new System.Drawing.Size(71, 46);
      this.visualizeCheckBox.TabIndex = 7;
      this.visualizeCheckBox.Text = "Visualize";
      this.visualizeCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.visualizeCheckBox.UseVisualStyleBackColor = true;
      this.visualizeCheckBox.CheckedChanged += new System.EventHandler(this.visualizeCheckBox_CheckedChanged);
      // 
      // mdsPictureBox
      // 
      this.mdsPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mdsPictureBox.Location = new System.Drawing.Point(0, 52);
      this.mdsPictureBox.Name = "mdsPictureBox";
      this.mdsPictureBox.Size = new System.Drawing.Size(424, 333);
      this.mdsPictureBox.TabIndex = 8;
      this.mdsPictureBox.TabStop = false;
      this.mdsPictureBox.Visible = false;
      // 
      // MDSView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mdsPictureBox);
      this.Controls.Add(this.visualizeCheckBox);
      this.Name = "MDSView";
      this.Controls.SetChildIndex(this.visualizeCheckBox, 0);
      this.Controls.SetChildIndex(this.mdsPictureBox, 0);
      this.Controls.SetChildIndex(this.statisticsTextBox, 0);
      this.Controls.SetChildIndex(this.rowsLabel, 0);
      this.Controls.SetChildIndex(this.columnsLabel, 0);
      this.Controls.SetChildIndex(this.rowsTextBox, 0);
      this.Controls.SetChildIndex(this.columnsTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.mdsPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox visualizeCheckBox;
    private System.Windows.Forms.PictureBox mdsPictureBox;

  }
}
