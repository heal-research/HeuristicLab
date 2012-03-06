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

namespace HeuristicLab.Problems.TravelingSalesman.Views {
  partial class TravelingSalesmanProblemView {
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
      this.pathTSPTourView = new HeuristicLab.Problems.TravelingSalesman.Views.PathTSPTourView();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // problemInstanceProviderViewHost
      // 
      this.problemInstanceProviderViewHost.Size = new System.Drawing.Size(270, 21);
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
      this.parameterCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.parameterCollectionView.Location = new System.Drawing.Point(3, 3);
      this.parameterCollectionView.Size = new System.Drawing.Size(515, 332);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(454, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(510, 35);
      // 
      // tabControl
      // 
      this.tabControl.AllowDrop = true;
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 58);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(529, 364);
      this.tabControl.TabIndex = 4;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.parameterCollectionView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(521, 338);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.pathTSPTourView);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(521, 338);
      this.visualizationTabPage.TabIndex = 1;
      this.visualizationTabPage.Text = "Visualization";
      this.visualizationTabPage.UseVisualStyleBackColor = true;
      // 
      // pathTSPTourView
      // 
      this.pathTSPTourView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pathTSPTourView.Caption = "PathTSPTour View";
      this.pathTSPTourView.Content = null;
      this.pathTSPTourView.Location = new System.Drawing.Point(6, 6);
      this.pathTSPTourView.Name = "pathTSPTourView";
      this.pathTSPTourView.ReadOnly = false;
      this.pathTSPTourView.Size = new System.Drawing.Size(509, 329);
      this.pathTSPTourView.TabIndex = 0;
      // 
      // TravelingSalesmanProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "TravelingSalesmanProblemView";
      this.Size = new System.Drawing.Size(529, 422);
      this.Controls.SetChildIndex(this.libraryInfoButton, 0);
      this.Controls.SetChildIndex(this.libraryLabel, 0);
      this.Controls.SetChildIndex(this.problemInstanceProviderComboBox, 0);
      this.Controls.SetChildIndex(this.problemInstanceProviderViewHost, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private System.Windows.Forms.TabPage parametersTabPage;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private PathTSPTourView pathTSPTourView;
  }
}
