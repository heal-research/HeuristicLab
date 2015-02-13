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

namespace HeuristicLab.Problems.Knapsack.Views {
  partial class KnapsackSolutionView {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.IsSplitterFixed = true;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.label6);
      this.splitContainer.Panel1.Controls.Add(this.label5);
      this.splitContainer.Panel1.Controls.Add(this.label4);
      this.splitContainer.Panel1.Controls.Add(this.label3);
      this.splitContainer.Panel1.Controls.Add(this.label2);
      this.splitContainer.Panel1.Controls.Add(this.label1);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.pictureBox);
      this.splitContainer.Size = new System.Drawing.Size(392, 310);
      this.splitContainer.SplitterDistance = 140;
      this.splitContainer.TabIndex = 0;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(2, 37);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(116, 13);
      this.label6.TabIndex = 5;
      this.label6.Text = "Filled rectangles - items";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(2, 23);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(125, 13);
      this.label5.TabIndex = 4;
      this.label5.Text = "Big rectangle - knapsack";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(5, 80);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(131, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Rectangle height = weight";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(4, 52);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(134, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Dark color = more valuable";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 10);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(49, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Legend";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(5, 66);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(129, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Light color = less valuable";
      // 
      // pictureBox
      // 
      this.pictureBox.BackColor = System.Drawing.Color.White;
      this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox.Location = new System.Drawing.Point(0, 0);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(248, 310);
      this.pictureBox.TabIndex = 1;
      this.pictureBox.TabStop = false;
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
      // 
      // KnapsackSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.splitContainer);
      this.Name = "KnapsackSolutionView";
      this.Size = new System.Drawing.Size(392, 310);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;

  }
}
