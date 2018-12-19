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

using System.ComponentModel;
using HeuristicLab.Common.Resources;

namespace HeuristicLab.MathematicalOptimization.Views {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LinearProgrammingProblemView));
      this.panel1 = new System.Windows.Forms.Panel();
      this.ViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.panel2 = new System.Windows.Forms.Panel();
      this.modelTypeNameLabel = new System.Windows.Forms.Label();
      this.modelTypeLabel = new System.Windows.Forms.Label();
      this.changeModelTypeButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.ViewHost);
      this.panel1.Controls.Add(this.panel2);
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
      this.ViewHost.Location = new System.Drawing.Point(0, 30);
      this.ViewHost.Name = "ViewHost";
      this.ViewHost.ReadOnly = false;
      this.ViewHost.Size = new System.Drawing.Size(300, 120);
      this.ViewHost.TabIndex = 6;
      this.ViewHost.ViewsLabelVisible = false;
      this.ViewHost.ViewType = null;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.modelTypeNameLabel);
      this.panel2.Controls.Add(this.modelTypeLabel);
      this.panel2.Controls.Add(this.changeModelTypeButton);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel2.Location = new System.Drawing.Point(0, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(300, 30);
      this.panel2.TabIndex = 5;
      // 
      // modelTypeNameLabel
      // 
      this.modelTypeNameLabel.AutoSize = true;
      this.modelTypeNameLabel.Location = new System.Drawing.Point(95, 9);
      this.modelTypeNameLabel.Name = "modelTypeNameLabel";
      this.modelTypeNameLabel.Size = new System.Drawing.Size(31, 13);
      this.modelTypeNameLabel.TabIndex = 6;
      this.modelTypeNameLabel.Text = "none";
      // 
      // modelTypeLabel
      // 
      this.modelTypeLabel.AutoSize = true;
      this.modelTypeLabel.Location = new System.Drawing.Point(3, 9);
      this.modelTypeLabel.Name = "modelTypeLabel";
      this.modelTypeLabel.Size = new System.Drawing.Size(39, 13);
      this.modelTypeLabel.TabIndex = 5;
      this.modelTypeLabel.Text = "Model Type:";
      // 
      // changeModelTypeButton
      // 
      this.changeModelTypeButton.Image = VSImageLibrary.RefreshDocument;
      this.changeModelTypeButton.Location = new System.Drawing.Point(68, 3);
      this.changeModelTypeButton.Name = "changeModelType";
      this.changeModelTypeButton.Size = new System.Drawing.Size(24, 24);
      this.changeModelTypeButton.TabIndex = 4;
      this.toolTip.SetToolTip(this.changeModelTypeButton, "Change Model Type");
      this.changeModelTypeButton.UseVisualStyleBackColor = true;
      this.changeModelTypeButton.Click += new System.EventHandler(this.changeModelTypeButton_Click);
      // 
      // LinearProgrammingProblemView
      // 
      this.Controls.Add(this.panel1);
      this.Name = "LinearProgrammingProblemView";
      this.Size = new System.Drawing.Size(300, 150);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private MainForm.WindowsForms.ViewHost ViewHost;
    private System.Windows.Forms.Panel panel2;
    protected System.Windows.Forms.Button changeModelTypeButton;
    private System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.Button newProblemDefinitionButton;
    private System.Windows.Forms.Label modelTypeLabel;
    private System.Windows.Forms.Label modelTypeNameLabel;
  }
}
