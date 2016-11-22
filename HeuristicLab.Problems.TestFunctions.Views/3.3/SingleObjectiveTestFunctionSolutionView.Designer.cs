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

namespace HeuristicLab.Problems.TestFunctions.Views {
  partial class SingleObjectiveTestFunctionSolutionView {
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
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.realVectorView = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabControl1 = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.textualTabPage = new System.Windows.Forms.TabPage();
      this.graphicalTabPage = new System.Windows.Forms.TabPage();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.textualTabPage.SuspendLayout();
      this.graphicalTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // qualityView
      // 
      this.qualityView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityView.Content = null;
      this.qualityView.Location = new System.Drawing.Point(6, 19);
      this.qualityView.Name = "qualityView";
      this.qualityView.ReadOnly = false;
      this.qualityView.Size = new System.Drawing.Size(479, 32);
      this.qualityView.TabIndex = 0;
      this.qualityView.ViewType = null;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.IsSplitterFixed = true;
      this.splitContainer1.Location = new System.Drawing.Point(3, 3);
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
      this.splitContainer1.Size = new System.Drawing.Size(491, 304);
      this.splitContainer1.SplitterDistance = 60;
      this.splitContainer1.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox3.Controls.Add(this.qualityView);
      this.groupBox3.Location = new System.Drawing.Point(0, 0);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(491, 57);
      this.groupBox3.TabIndex = 0;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Quality";
      // 
      // groupBox4
      // 
      this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox4.Controls.Add(this.realVectorView);
      this.groupBox4.Location = new System.Drawing.Point(0, 3);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(491, 237);
      this.groupBox4.TabIndex = 0;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Real vector";
      // 
      // realVectorView
      // 
      this.realVectorView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.realVectorView.Content = null;
      this.realVectorView.Location = new System.Drawing.Point(6, 19);
      this.realVectorView.Name = "realVectorView";
      this.realVectorView.ReadOnly = false;
      this.realVectorView.Size = new System.Drawing.Size(479, 212);
      this.realVectorView.TabIndex = 0;
      this.realVectorView.ViewType = null;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.textualTabPage);
      this.tabControl1.Controls.Add(this.graphicalTabPage);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(505, 336);
      this.tabControl1.TabIndex = 0;
      // 
      // textualTabPage
      // 
      this.textualTabPage.Controls.Add(this.splitContainer1);
      this.textualTabPage.Location = new System.Drawing.Point(4, 22);
      this.textualTabPage.Name = "textualTabPage";
      this.textualTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.textualTabPage.Size = new System.Drawing.Size(497, 310);
      this.textualTabPage.TabIndex = 0;
      this.textualTabPage.Text = "Textual";
      this.textualTabPage.UseVisualStyleBackColor = true;
      // 
      // graphicalTabPage
      // 
      this.graphicalTabPage.Controls.Add(this.pictureBox);
      this.graphicalTabPage.Controls.Add(this.label1);
      this.graphicalTabPage.Location = new System.Drawing.Point(4, 22);
      this.graphicalTabPage.Name = "graphicalTabPage";
      this.graphicalTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.graphicalTabPage.Size = new System.Drawing.Size(497, 310);
      this.graphicalTabPage.TabIndex = 1;
      this.graphicalTabPage.Text = "Graphical";
      this.graphicalTabPage.UseVisualStyleBackColor = true;
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox.BackColor = System.Drawing.Color.White;
      this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox.Location = new System.Drawing.Point(6, 6);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(485, 298);
      this.pictureBox.TabIndex = 1;
      this.pictureBox.TabStop = false;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(118, 131);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(253, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "The graphical view is only available for 2 dimensions";
      // 
      // SingleObjectiveTestFunctionSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tabControl1);
      this.Name = "SingleObjectiveTestFunctionSolutionView";
      this.Size = new System.Drawing.Size(505, 336);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.groupBox4.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.textualTabPage.ResumeLayout(false);
      this.graphicalTabPage.ResumeLayout(false);
      this.graphicalTabPage.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.GroupBox groupBox4;
    private HeuristicLab.MainForm.WindowsForms.ViewHost qualityView;
    private System.Windows.Forms.GroupBox groupBox3;
    private HeuristicLab.MainForm.WindowsForms.ViewHost realVectorView;
    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl1;
    private System.Windows.Forms.TabPage textualTabPage;
    private System.Windows.Forms.TabPage graphicalTabPage;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox pictureBox;



  }
}
