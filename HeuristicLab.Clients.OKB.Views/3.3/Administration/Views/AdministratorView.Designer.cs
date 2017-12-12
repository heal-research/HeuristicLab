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

namespace HeuristicLab.Clients.OKB.Administration {
  partial class AdministratorView {
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
      this.components = new System.ComponentModel.Container();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.platformsTabPage = new System.Windows.Forms.TabPage();
      this.platformCollectionView = new HeuristicLab.Clients.OKB.Administration.PlatformCollectionView();
      this.algorithmClassesTabPage = new System.Windows.Forms.TabPage();
      this.algorithmClassCollectionView = new HeuristicLab.Clients.OKB.Administration.AlgorithmClassCollectionView();
      this.algorithmsTabPage = new System.Windows.Forms.TabPage();
      this.algorithmCollectionView = new HeuristicLab.Clients.OKB.Administration.AlgorithmCollectionView();
      this.problemClassesTabPage = new System.Windows.Forms.TabPage();
      this.problemClassCollectionView = new HeuristicLab.Clients.OKB.Administration.ProblemClassCollectionView();
      this.problemsTabPage = new System.Windows.Forms.TabPage();
      this.problemCollectionView = new HeuristicLab.Clients.OKB.Administration.ProblemCollectionView();
      this.refreshButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.tabControl.SuspendLayout();
      this.platformsTabPage.SuspendLayout();
      this.algorithmClassesTabPage.SuspendLayout();
      this.algorithmsTabPage.SuspendLayout();
      this.problemClassesTabPage.SuspendLayout();
      this.problemsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.platformsTabPage);
      this.tabControl.Controls.Add(this.algorithmClassesTabPage);
      this.tabControl.Controls.Add(this.algorithmsTabPage);
      this.tabControl.Controls.Add(this.problemClassesTabPage);
      this.tabControl.Controls.Add(this.problemsTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 29);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(727, 406);
      this.tabControl.TabIndex = 1;
      // 
      // platformsTabPage
      // 
      this.platformsTabPage.Controls.Add(this.platformCollectionView);
      this.platformsTabPage.Location = new System.Drawing.Point(4, 22);
      this.platformsTabPage.Name = "platformsTabPage";
      this.platformsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.platformsTabPage.Size = new System.Drawing.Size(719, 380);
      this.platformsTabPage.TabIndex = 2;
      this.platformsTabPage.Text = "Platforms";
      this.platformsTabPage.UseVisualStyleBackColor = true;
      // 
      // platformCollectionView
      // 
      this.platformCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.platformCollectionView.Caption = "PlatformCollection View";
      this.platformCollectionView.Content = null;
      this.platformCollectionView.Location = new System.Drawing.Point(3, 3);
      this.platformCollectionView.Name = "platformCollectionView";
      this.platformCollectionView.ReadOnly = false;
      this.platformCollectionView.Size = new System.Drawing.Size(713, 374);
      this.platformCollectionView.TabIndex = 0;
      // 
      // algorithmClassesTabPage
      // 
      this.algorithmClassesTabPage.Controls.Add(this.algorithmClassCollectionView);
      this.algorithmClassesTabPage.Location = new System.Drawing.Point(4, 22);
      this.algorithmClassesTabPage.Name = "algorithmClassesTabPage";
      this.algorithmClassesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.algorithmClassesTabPage.Size = new System.Drawing.Size(719, 380);
      this.algorithmClassesTabPage.TabIndex = 0;
      this.algorithmClassesTabPage.Text = "Algorithm Classes";
      this.algorithmClassesTabPage.UseVisualStyleBackColor = true;
      // 
      // algorithmClassCollectionView
      // 
      this.algorithmClassCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmClassCollectionView.Caption = "AlgorithmClassCollection View";
      this.algorithmClassCollectionView.Content = null;
      this.algorithmClassCollectionView.Location = new System.Drawing.Point(3, 3);
      this.algorithmClassCollectionView.Name = "algorithmClassCollectionView";
      this.algorithmClassCollectionView.ReadOnly = false;
      this.algorithmClassCollectionView.Size = new System.Drawing.Size(713, 374);
      this.algorithmClassCollectionView.TabIndex = 0;
      // 
      // algorithmsTabPage
      // 
      this.algorithmsTabPage.Controls.Add(this.algorithmCollectionView);
      this.algorithmsTabPage.Location = new System.Drawing.Point(4, 22);
      this.algorithmsTabPage.Name = "algorithmsTabPage";
      this.algorithmsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.algorithmsTabPage.Size = new System.Drawing.Size(719, 380);
      this.algorithmsTabPage.TabIndex = 1;
      this.algorithmsTabPage.Text = "Algorithms";
      this.algorithmsTabPage.UseVisualStyleBackColor = true;
      // 
      // algorithmCollectionView
      // 
      this.algorithmCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.algorithmCollectionView.Caption = "AlgorithmCollection View";
      this.algorithmCollectionView.Content = null;
      this.algorithmCollectionView.Location = new System.Drawing.Point(3, 3);
      this.algorithmCollectionView.Name = "algorithmCollectionView";
      this.algorithmCollectionView.ReadOnly = false;
      this.algorithmCollectionView.Size = new System.Drawing.Size(713, 374);
      this.algorithmCollectionView.TabIndex = 0;
      // 
      // problemClassesTabPage
      // 
      this.problemClassesTabPage.Controls.Add(this.problemClassCollectionView);
      this.problemClassesTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemClassesTabPage.Name = "problemClassesTabPage";
      this.problemClassesTabPage.Size = new System.Drawing.Size(719, 380);
      this.problemClassesTabPage.TabIndex = 4;
      this.problemClassesTabPage.Text = "Problem Classes";
      this.problemClassesTabPage.UseVisualStyleBackColor = true;
      // 
      // problemClassCollectionView
      // 
      this.problemClassCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.problemClassCollectionView.Caption = "ProblemClassCollection View";
      this.problemClassCollectionView.Content = null;
      this.problemClassCollectionView.Location = new System.Drawing.Point(3, 3);
      this.problemClassCollectionView.Name = "problemClassCollectionView";
      this.problemClassCollectionView.ReadOnly = false;
      this.problemClassCollectionView.Size = new System.Drawing.Size(713, 374);
      this.problemClassCollectionView.TabIndex = 0;
      // 
      // problemsTabPage
      // 
      this.problemsTabPage.Controls.Add(this.problemCollectionView);
      this.problemsTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemsTabPage.Name = "problemsTabPage";
      this.problemsTabPage.Size = new System.Drawing.Size(719, 380);
      this.problemsTabPage.TabIndex = 5;
      this.problemsTabPage.Text = "Problems";
      this.problemsTabPage.UseVisualStyleBackColor = true;
      // 
      // problemCollectionView
      // 
      this.problemCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.problemCollectionView.Caption = "ProblemCollection View";
      this.problemCollectionView.Content = null;
      this.problemCollectionView.Location = new System.Drawing.Point(3, 3);
      this.problemCollectionView.Name = "problemCollectionView";
      this.problemCollectionView.ReadOnly = false;
      this.problemCollectionView.Size = new System.Drawing.Size(713, 374);
      this.problemCollectionView.TabIndex = 0;
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(0, 0);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh Data");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // AdministratorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.refreshButton);
      this.Name = "AdministratorView";
      this.Size = new System.Drawing.Size(727, 435);
      this.tabControl.ResumeLayout(false);
      this.platformsTabPage.ResumeLayout(false);
      this.algorithmClassesTabPage.ResumeLayout(false);
      this.algorithmsTabPage.ResumeLayout(false);
      this.problemClassesTabPage.ResumeLayout(false);
      this.problemsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage algorithmClassesTabPage;
    private System.Windows.Forms.TabPage algorithmsTabPage;
    private System.Windows.Forms.Button refreshButton;
    private AlgorithmClassCollectionView algorithmClassCollectionView;
    private AlgorithmCollectionView algorithmCollectionView;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.TabPage platformsTabPage;
    private PlatformCollectionView platformCollectionView;
    private System.Windows.Forms.TabPage problemClassesTabPage;
    private System.Windows.Forms.TabPage problemsTabPage;
    private ProblemClassCollectionView problemClassCollectionView;
    private ProblemCollectionView problemCollectionView;

  }
}
