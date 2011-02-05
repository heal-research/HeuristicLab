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

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic {
  partial class SimplifiedSymbolicExpressionModelView {
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
      this.simplifiedModelGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.simplifiedModelGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // simplifiedModelGroupBox
      // 
      this.simplifiedModelGroupBox.Controls.Add(this.viewHost);
      this.simplifiedModelGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.simplifiedModelGroupBox.Location = new System.Drawing.Point(0, 0);
      this.simplifiedModelGroupBox.Name = "simplifiedModelGroupBox";
      this.simplifiedModelGroupBox.Size = new System.Drawing.Size(352, 413);
      this.simplifiedModelGroupBox.TabIndex = 0;
      this.simplifiedModelGroupBox.TabStop = false;
      this.simplifiedModelGroupBox.Text = "Simplified Symbolic Expression Model";
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Content = null;
      this.viewHost.Location = new System.Drawing.Point(6, 19);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(340, 388);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewType = null;
      // 
      // SimplifiedSymbolicExpressionModelView
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.simplifiedModelGroupBox);
      this.Name = "SimplifiedSymbolicExpressionModelView";
      this.Size = new System.Drawing.Size(352, 413);
      this.simplifiedModelGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox simplifiedModelGroupBox;
    private HeuristicLab.MainForm.WindowsForms.ViewHost viewHost;

  }
}
