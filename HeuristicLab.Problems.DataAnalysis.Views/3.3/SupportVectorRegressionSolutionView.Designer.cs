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
      this.supportVectorTabControl = new System.Windows.Forms.TabPage();
      this.supportVectorViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.supportVectorTabControl.SuspendLayout();
      this.SuspendLayout();
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
      this.Name = "SupportVectorRegressionSolutionView";
      this.tabControl.TabPages.Add(supportVectorTabControl);
      this.supportVectorTabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabPage supportVectorTabControl;
    private HeuristicLab.MainForm.WindowsForms.ViewHost supportVectorViewHost;
  }
}
