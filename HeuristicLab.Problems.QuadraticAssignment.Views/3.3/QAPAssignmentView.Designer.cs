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

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  partial class QAPAssignmentView {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.qualityGroupBox = new System.Windows.Forms.GroupBox();
      this.qualityViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.valueTabPage = new System.Windows.Forms.TabPage();
      this.assignmentGroupBox = new System.Windows.Forms.GroupBox();
      this.assignmentViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.qapView = new HeuristicLab.Problems.QuadraticAssignment.Views.QAPVisualizationControl();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.qualityGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.valueTabPage.SuspendLayout();
      this.assignmentGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.qualityGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.tabControl);
      this.splitContainer.Size = new System.Drawing.Size(536, 378);
      this.splitContainer.SplitterDistance = 64;
      this.splitContainer.TabIndex = 1;
      // 
      // qualityGroupBox
      // 
      this.qualityGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityGroupBox.Controls.Add(this.qualityViewHost);
      this.qualityGroupBox.Location = new System.Drawing.Point(0, 0);
      this.qualityGroupBox.Name = "qualityGroupBox";
      this.qualityGroupBox.Size = new System.Drawing.Size(536, 61);
      this.qualityGroupBox.TabIndex = 0;
      this.qualityGroupBox.TabStop = false;
      this.qualityGroupBox.Text = "Quality";
      // 
      // qualityViewHost
      // 
      this.qualityViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityViewHost.Caption = "View";
      this.qualityViewHost.Content = null;
      this.qualityViewHost.Enabled = false;
      this.qualityViewHost.Location = new System.Drawing.Point(6, 19);
      this.qualityViewHost.Name = "qualityViewHost";
      this.qualityViewHost.ReadOnly = false;
      this.qualityViewHost.Size = new System.Drawing.Size(524, 36);
      this.qualityViewHost.TabIndex = 0;
      this.qualityViewHost.ViewsLabelVisible = true;
      this.qualityViewHost.ViewType = null;
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Controls.Add(this.valueTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(536, 307);
      this.tabControl.TabIndex = 0;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.qapView);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(528, 281);
      this.visualizationTabPage.TabIndex = 0;
      this.visualizationTabPage.Text = "Visualization";
      this.visualizationTabPage.UseVisualStyleBackColor = true;
      // 
      // valueTabPage
      // 
      this.valueTabPage.Controls.Add(this.assignmentGroupBox);
      this.valueTabPage.Location = new System.Drawing.Point(4, 22);
      this.valueTabPage.Name = "valueTabPage";
      this.valueTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.valueTabPage.Size = new System.Drawing.Size(528, 281);
      this.valueTabPage.TabIndex = 1;
      this.valueTabPage.Text = "Value";
      this.valueTabPage.UseVisualStyleBackColor = true;
      // 
      // assignmentGroupBox
      // 
      this.assignmentGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.assignmentGroupBox.Controls.Add(this.assignmentViewHost);
      this.assignmentGroupBox.Location = new System.Drawing.Point(6, 6);
      this.assignmentGroupBox.Name = "assignmentGroupBox";
      this.assignmentGroupBox.Size = new System.Drawing.Size(516, 269);
      this.assignmentGroupBox.TabIndex = 0;
      this.assignmentGroupBox.TabStop = false;
      this.assignmentGroupBox.Text = "Assignment";
      // 
      // assignmentViewHost
      // 
      this.assignmentViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.assignmentViewHost.Caption = "View";
      this.assignmentViewHost.Content = null;
      this.assignmentViewHost.Enabled = false;
      this.assignmentViewHost.Location = new System.Drawing.Point(6, 19);
      this.assignmentViewHost.Name = "assignmentViewHost";
      this.assignmentViewHost.ReadOnly = false;
      this.assignmentViewHost.Size = new System.Drawing.Size(504, 244);
      this.assignmentViewHost.TabIndex = 0;
      this.assignmentViewHost.ViewsLabelVisible = true;
      this.assignmentViewHost.ViewType = null;
      // 
      // qapView
      // 
      this.qapView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.qapView.Assignment = null;
      this.qapView.Distances = null;
      this.qapView.Location = new System.Drawing.Point(0, 3);
      this.qapView.Name = "qapView";
      this.qapView.Size = new System.Drawing.Size(526, 278);
      this.qapView.TabIndex = 0;
      this.qapView.Weights = null;
      // 
      // QAPAssignmentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "QAPAssignmentView";
      this.Size = new System.Drawing.Size(536, 378);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.qualityGroupBox.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.valueTabPage.ResumeLayout(false);
      this.assignmentGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox qualityGroupBox;
    private MainForm.WindowsForms.ViewHost qualityViewHost;
    private MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private System.Windows.Forms.TabPage valueTabPage;
    private System.Windows.Forms.GroupBox assignmentGroupBox;
    private MainForm.WindowsForms.ViewHost assignmentViewHost;
    private QAPVisualizationControl qapView;
  }
}
