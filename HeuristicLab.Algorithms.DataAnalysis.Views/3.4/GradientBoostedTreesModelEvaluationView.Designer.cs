#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class GradientBoostedTreesModelEvaluationView {
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
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.listBox = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(126, 3);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(220, 277);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // listBox
      // 
      this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.listBox.Location = new System.Drawing.Point(3, 3);
      this.listBox.Name = "listBox";
      this.listBox.Size = new System.Drawing.Size(117, 277);
      this.listBox.TabIndex = 1;
      this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
      this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
      // 
      // GradientBoostedTreesModelEvaluationView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.viewHost);
      this.Controls.Add(this.listBox);
      this.Name = "GradientBoostedTreesModelEvaluationView";
      this.Size = new System.Drawing.Size(349, 289);
      this.ResumeLayout(false);

    }

    #endregion

    private MainForm.WindowsForms.ViewHost viewHost;
    private System.Windows.Forms.ListBox listBox;




  }
}
