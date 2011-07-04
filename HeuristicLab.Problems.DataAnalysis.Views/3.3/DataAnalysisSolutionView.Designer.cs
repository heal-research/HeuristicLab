#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class DataAnalysisSolutionView {
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
      this.dataTabPage = new System.Windows.Forms.TabPage();
      this.dataViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.modelTabPage = new System.Windows.Forms.TabPage();
      this.modelViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.dataTabPage.SuspendLayout();
      this.modelTabPage.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataTabPage
      // 
      this.dataTabPage.Controls.Add(this.dataViewHost);
      this.dataTabPage.Location = new System.Drawing.Point(4, 22);
      this.dataTabPage.Name = "dataTabPage";
      this.dataTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dataTabPage.Size = new System.Drawing.Size(372, 236);
      this.dataTabPage.TabIndex = 1;
      this.dataTabPage.Text = "Data";
      this.dataTabPage.UseVisualStyleBackColor = true;
      // 
      // dataViewHost
      // 
      this.dataViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataViewHost.Caption = "DataAnalysisSolution Data View";
      this.dataViewHost.Content = null;
      this.dataViewHost.Enabled = false;
      this.dataViewHost.Location = new System.Drawing.Point(6, 6);
      this.dataViewHost.Name = "dataViewHost";
      this.dataViewHost.ReadOnly = false;
      this.dataViewHost.Size = new System.Drawing.Size(360, 224);
      this.dataViewHost.TabIndex = 2;
      this.dataViewHost.ViewType = null;
      // 
      // modelTabPage
      // 
      this.modelTabPage.Controls.Add(this.modelViewHost);
      this.modelTabPage.Location = new System.Drawing.Point(4, 22);
      this.modelTabPage.Name = "modelTabPage";
      this.modelTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.modelTabPage.Size = new System.Drawing.Size(372, 236);
      this.modelTabPage.TabIndex = 0;
      this.modelTabPage.Text = "Model";
      this.modelTabPage.UseVisualStyleBackColor = true;
      // 
      // modelViewHost
      // 
      this.modelViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.modelViewHost.Caption = "DataAnalysisSolution Model View";
      this.modelViewHost.Content = null;
      this.modelViewHost.Enabled = false;
      this.modelViewHost.Location = new System.Drawing.Point(6, 6);
      this.modelViewHost.Name = "modelViewHost";
      this.modelViewHost.ReadOnly = false;
      this.modelViewHost.Size = new System.Drawing.Size(360, 224);
      this.modelViewHost.TabIndex = 1;
      this.modelViewHost.ViewType = null;
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.modelTabPage);
      this.tabControl.Controls.Add(this.dataTabPage);
      this.tabControl.Location = new System.Drawing.Point(3, 3);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(380, 262);
      this.tabControl.TabIndex = 1;
      // 
      // DataAnalysisSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "DataAnalysisSolutionView";
      this.Size = new System.Drawing.Size(386, 268);
      this.dataTabPage.ResumeLayout(false);
      this.modelTabPage.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    protected System.Windows.Forms.TabPage dataTabPage;
    protected System.Windows.Forms.TabPage modelTabPage;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost dataViewHost;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost modelViewHost;
  }
}
