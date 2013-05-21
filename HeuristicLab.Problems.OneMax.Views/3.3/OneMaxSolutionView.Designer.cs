#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.OneMax.Views {
  partial class OneMaxSolutionView {
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
      this.qualityView = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.binaryVectorView = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.SuspendLayout();
      // 
      // qualityView
      // 
      this.qualityView.Content = null;
      this.qualityView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.qualityView.Location = new System.Drawing.Point(3, 16);
      this.qualityView.Name = "qualityView";
      this.qualityView.ReadOnly = false;
      this.qualityView.Size = new System.Drawing.Size(188, 30);
      this.qualityView.TabIndex = 1;
      this.qualityView.ViewType = null;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.IsSplitterFixed = true;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
      this.splitContainer1.Panel1MinSize = 30;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
      this.splitContainer1.Panel2MinSize = 30;
      this.splitContainer1.Size = new System.Drawing.Size(392, 265);
      this.splitContainer1.TabIndex = 7;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.qualityView);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Location = new System.Drawing.Point(0, 0);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(392, 50);
      this.groupBox3.TabIndex = 6;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Quality";
      // 
      // qualityView
      // 
      this.qualityView.Content = null;
      this.qualityView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.qualityView.Location = new System.Drawing.Point(3, 16);
      this.qualityView.Name = "qualityView";
      this.qualityView.ReadOnly = false;
      this.qualityView.Size = new System.Drawing.Size(386, 31);
      this.qualityView.TabIndex = 1;
      this.qualityView.ViewType = null;
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.binaryVectorView);
      this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox4.Location = new System.Drawing.Point(0, 0);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(392, 211);
      this.groupBox4.TabIndex = 6;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Binary vector";
      // 
      // binaryVectorView
      // 
      this.binaryVectorView.Content = null;
      this.binaryVectorView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.binaryVectorView.Location = new System.Drawing.Point(3, 16);
      this.binaryVectorView.Name = "binaryVectorView";
      this.binaryVectorView.ReadOnly = false;
      this.binaryVectorView.Size = new System.Drawing.Size(386, 192);
      this.binaryVectorView.TabIndex = 1;
      this.binaryVectorView.ViewType = null;
      // 
      // OneMaxSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.splitContainer1);
      this.Name = "OneMaxSolutionView";
      this.Size = new System.Drawing.Size(392, 265);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.groupBox4.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.GroupBox groupBox4;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost qualityView;
    private System.Windows.Forms.GroupBox groupBox3;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost binaryVectorView;



  }
}
