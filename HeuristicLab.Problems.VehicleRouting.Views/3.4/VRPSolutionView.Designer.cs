#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.VehicleRouting.Views {
  partial class VRPSolutionView {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.tabControl1 = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.problemInstanceView = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tourGroupBox = new System.Windows.Forms.GroupBox();
      this.valueTextBox = new System.Windows.Forms.TextBox();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.tourGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(468, 415);
      this.tabControl1.TabIndex = 0;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.problemInstanceView);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(460, 389);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "ProblemInstance";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // problemInstanceView
      // 
      this.problemInstanceView.Caption = "View";
      this.problemInstanceView.Content = null;
      this.problemInstanceView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.problemInstanceView.Location = new System.Drawing.Point(3, 3);
      this.problemInstanceView.Name = "problemInstanceView";
      this.problemInstanceView.ReadOnly = false;
      this.problemInstanceView.Size = new System.Drawing.Size(454, 383);
      this.problemInstanceView.TabIndex = 0;
      this.problemInstanceView.ViewType = null;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.tourGroupBox);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(460, 389);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Tours";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // tourGroupBox
      // 
      this.tourGroupBox.Controls.Add(this.valueTextBox);
      this.tourGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tourGroupBox.Location = new System.Drawing.Point(3, 3);
      this.tourGroupBox.Name = "tourGroupBox";
      this.tourGroupBox.Size = new System.Drawing.Size(454, 383);
      this.tourGroupBox.TabIndex = 1;
      this.tourGroupBox.TabStop = false;
      this.tourGroupBox.Text = "Tour";
      // 
      // valueTextBox
      // 
      this.valueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.valueTextBox.Location = new System.Drawing.Point(3, 16);
      this.valueTextBox.Multiline = true;
      this.valueTextBox.Name = "valueTextBox";
      this.valueTextBox.Size = new System.Drawing.Size(403, 507);
      this.valueTextBox.TabIndex = 0;
      this.valueTextBox.ReadOnly = true;
      // 
      // VRPSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tabControl1);
      this.Name = "VRPSolutionView";
      this.Size = new System.Drawing.Size(468, 415);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tourGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private MainForm.WindowsForms.ViewHost problemInstanceView;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.GroupBox tourGroupBox;
    private System.Windows.Forms.TextBox valueTextBox;
  }
}
