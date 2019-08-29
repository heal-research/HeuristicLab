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

using System.ComponentModel;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.ExactOptimization.Views {
  partial class LinearProgrammingProblemView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
      this.components = new System.ComponentModel.Container();
      this.panel1 = new System.Windows.Forms.Panel();
      this.ViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.ViewHost);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(300, 150);
      this.panel1.TabIndex = 7;
      // 
      // ViewHost
      // 
      this.ViewHost.Caption = "View";
      this.ViewHost.Content = null;
      this.ViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ViewHost.Enabled = false;
      this.ViewHost.Location = new System.Drawing.Point(0, 0);
      this.ViewHost.Name = "ViewHost";
      this.ViewHost.ReadOnly = false;
      this.ViewHost.Size = new System.Drawing.Size(300, 150);
      this.ViewHost.TabIndex = 6;
      this.ViewHost.ViewsLabelVisible = false;
      this.ViewHost.ViewType = null;
      // 
      // LinearProgrammingProblemView
      // 
      this.Controls.Add(this.panel1);
      this.Name = "LinearProgrammingProblemView";
      this.Size = new System.Drawing.Size(300, 150);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private MainForm.WindowsForms.ViewHost ViewHost;
    private System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.Button newProblemDefinitionButton;
  }
}
