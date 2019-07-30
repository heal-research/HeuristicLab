#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.PTSP.Views {
  partial class ProbabilisticTravelingSalesmanProblemView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

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
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.pathPTSPTourView = new HeuristicLab.Problems.PTSP.Views.PathPTSPTourView();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).BeginInit();
      this.problemInstanceSplitContainer.Panel1.SuspendLayout();
      this.problemInstanceSplitContainer.Panel2.SuspendLayout();
      this.problemInstanceSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // problemInstanceSplitContainer
      // 
      // 
      // problemInstanceSplitContainer.Panel2
      // 
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.tabControl);
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.parameterCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.parameterCollectionView.Location = new System.Drawing.Point(3, 3);
      this.parameterCollectionView.Size = new System.Drawing.Size(497, 274);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 27);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(511, 306);
      this.tabControl.TabIndex = 4;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.parametersTabPage.Controls.Add(this.parameterCollectionView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(503, 280);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.BackColor = System.Drawing.SystemColors.Window;
      this.visualizationTabPage.Controls.Add(this.pathPTSPTourView);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(503, 280);
      this.visualizationTabPage.TabIndex = 1;
      this.visualizationTabPage.Text = "Visualization";
      // 
      // pathPTSPTourView
      // 
      this.pathPTSPTourView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pathPTSPTourView.Caption = "PathPTSPTour View";
      this.pathPTSPTourView.Content = null;
      this.pathPTSPTourView.Location = new System.Drawing.Point(6, 6);
      this.pathPTSPTourView.Name = "pathPTSPTourView";
      this.pathPTSPTourView.ReadOnly = false;
      this.pathPTSPTourView.Size = new System.Drawing.Size(491, 268);
      this.pathPTSPTourView.TabIndex = 0;
      // 
      // ProbabilisticTravelingSalesmanProblemView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Name = "ProbabilisticTravelingSalesmanProblemView";
      this.problemInstanceSplitContainer.Panel1.ResumeLayout(false);
      this.problemInstanceSplitContainer.Panel1.PerformLayout();
      this.problemInstanceSplitContainer.Panel2.ResumeLayout(false);
      this.problemInstanceSplitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).EndInit();
      this.problemInstanceSplitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage parametersTabPage;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private PathPTSPTourView pathPTSPTourView;
  }
}
