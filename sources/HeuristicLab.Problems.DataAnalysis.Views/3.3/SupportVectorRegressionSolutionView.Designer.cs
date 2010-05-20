#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class SupportVectorRegressionSolutionView {
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
      this.modelTabControl = new System.Windows.Forms.TabControl();
      this.modelTabPage = new System.Windows.Forms.TabPage();
      this.modelPanel = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.dataTabPage = new System.Windows.Forms.TabPage();
      this.dataPanel = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.supportVectorTabControl = new System.Windows.Forms.TabPage();
      this.supportVectorViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.modelTabControl.SuspendLayout();
      this.modelTabPage.SuspendLayout();
      this.dataTabPage.SuspendLayout();
      this.supportVectorTabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // modelTabControl
      // 
      this.modelTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.modelTabControl.Controls.Add(this.modelTabPage);
      this.modelTabControl.Controls.Add(this.dataTabPage);
      this.modelTabControl.Controls.Add(this.supportVectorTabControl);
      this.modelTabControl.Location = new System.Drawing.Point(3, 3);
      this.modelTabControl.Name = "modelTabControl";
      this.modelTabControl.SelectedIndex = 0;
      this.modelTabControl.Size = new System.Drawing.Size(247, 245);
      this.modelTabControl.TabIndex = 0;
      // 
      // modelTabPage
      // 
      this.modelTabPage.Controls.Add(this.modelPanel);
      this.modelTabPage.Location = new System.Drawing.Point(4, 22);
      this.modelTabPage.Name = "modelTabPage";
      this.modelTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.modelTabPage.Size = new System.Drawing.Size(239, 219);
      this.modelTabPage.TabIndex = 0;
      this.modelTabPage.Text = "Model";
      this.modelTabPage.UseVisualStyleBackColor = true;
      // 
      // modelPanel
      // 
      this.modelPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.modelPanel.Caption = "View";
      this.modelPanel.Content = null;
      this.modelPanel.Location = new System.Drawing.Point(6, 6);
      this.modelPanel.Name = "modelPanel";
      this.modelPanel.ReadOnly = false;
      this.modelPanel.Size = new System.Drawing.Size(227, 207);
      this.modelPanel.TabIndex = 0;
      this.modelPanel.ViewType = null;
      // 
      // dataTabPage
      // 
      this.dataTabPage.Controls.Add(this.dataPanel);
      this.dataTabPage.Location = new System.Drawing.Point(4, 22);
      this.dataTabPage.Name = "dataTabPage";
      this.dataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dataTabPage.Size = new System.Drawing.Size(239, 219);
      this.dataTabPage.TabIndex = 1;
      this.dataTabPage.Text = "Data";
      this.dataTabPage.UseVisualStyleBackColor = true;
      // 
      // dataPanel
      // 
      this.dataPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataPanel.Caption = "View";
      this.dataPanel.Content = null;
      this.dataPanel.Location = new System.Drawing.Point(6, 6);
      this.dataPanel.Name = "dataPanel";
      this.dataPanel.ReadOnly = false;
      this.dataPanel.Size = new System.Drawing.Size(227, 207);
      this.dataPanel.TabIndex = 0;
      this.dataPanel.ViewType = null;
      // 
      // supportVectorTabControl
      // 
      this.supportVectorTabControl.Controls.Add(this.supportVectorViewHost);
      this.supportVectorTabControl.Location = new System.Drawing.Point(4, 22);
      this.supportVectorTabControl.Name = "supportVectorTabControl";
      this.supportVectorTabControl.Padding = new System.Windows.Forms.Padding(3);
      this.supportVectorTabControl.Size = new System.Drawing.Size(239, 219);
      this.supportVectorTabControl.TabIndex = 2;
      this.supportVectorTabControl.Text = "Support vectors";
      this.supportVectorTabControl.UseVisualStyleBackColor = true;
      // 
      // supportVectorViewHost
      // 
      this.supportVectorViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.supportVectorViewHost.Caption = "SupportVectors";
      this.supportVectorViewHost.Content = null;
      this.supportVectorViewHost.Location = new System.Drawing.Point(6, 6);
      this.supportVectorViewHost.Name = "supportVectorViewHost";
      this.supportVectorViewHost.ReadOnly = false;
      this.supportVectorViewHost.Size = new System.Drawing.Size(227, 207);
      this.supportVectorViewHost.TabIndex = 1;
      this.supportVectorViewHost.ViewType = null;
      // 
      // SupportVectorRegressionSolutionView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.modelTabControl);
      this.Name = "SupportVectorRegressionSolutionView";
      this.Size = new System.Drawing.Size(253, 251);
      this.modelTabControl.ResumeLayout(false);
      this.modelTabPage.ResumeLayout(false);
      this.dataTabPage.ResumeLayout(false);
      this.supportVectorTabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl modelTabControl;
    private System.Windows.Forms.TabPage modelTabPage;
    private HeuristicLab.MainForm.WindowsForms.ViewHost modelPanel;
    private System.Windows.Forms.TabPage dataTabPage;
    private HeuristicLab.MainForm.WindowsForms.ViewHost dataPanel;
    private System.Windows.Forms.TabPage supportVectorTabControl;
    private HeuristicLab.MainForm.WindowsForms.ViewHost supportVectorViewHost;


  }
}
