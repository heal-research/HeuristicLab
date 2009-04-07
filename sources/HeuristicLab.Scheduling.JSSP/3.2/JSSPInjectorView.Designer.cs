#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Scheduling.JSSP {
  partial class JSSPInjectorView {
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
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tb_jobs = new System.Windows.Forms.TextBox();
      this.tb_machines = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabvariables = new System.Windows.Forms.TabPage();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.descriptionTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseDescriptionView = new HeuristicLab.Core.OperatorBaseDescriptionView();
      this.tabPage1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.descriptionTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.tb_jobs);
      this.tabPage1.Controls.Add(this.tb_machines);
      this.tabPage1.Controls.Add(this.label2);
      this.tabPage1.Controls.Add(this.label1);
      this.tabPage1.Controls.Add(this.button1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(228, 133);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "JSSP Data";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tb_jobs
      // 
      this.tb_jobs.BackColor = System.Drawing.SystemColors.Control;
      this.tb_jobs.Location = new System.Drawing.Point(96, 42);
      this.tb_jobs.Name = "tb_jobs";
      this.tb_jobs.ReadOnly = true;
      this.tb_jobs.Size = new System.Drawing.Size(100, 20);
      this.tb_jobs.TabIndex = 4;
      // 
      // tb_machines
      // 
      this.tb_machines.BackColor = System.Drawing.SystemColors.Control;
      this.tb_machines.Location = new System.Drawing.Point(96, 16);
      this.tb_machines.Name = "tb_machines";
      this.tb_machines.ReadOnly = true;
      this.tb_machines.Size = new System.Drawing.Size(101, 20);
      this.tb_machines.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 45);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Jobs: ";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(11, 19);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(56, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Machines:";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(3, 97);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(216, 25);
      this.button1.TabIndex = 0;
      this.button1.Text = "Import Instance from JSSPLib ";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabvariables);
      this.tabControl1.Controls.Add(this.descriptionTabPage);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(236, 159);
      this.tabControl1.TabIndex = 0;
      // 
      // tabvariables
      // 
      this.tabvariables.Location = new System.Drawing.Point(4, 22);
      this.tabvariables.Name = "tabvariables";
      this.tabvariables.Size = new System.Drawing.Size(228, 133);
      this.tabvariables.TabIndex = 1;
      this.tabvariables.Text = "Variables";
      this.tabvariables.UseVisualStyleBackColor = true;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "jssp";
      this.openFileDialog.Title = "Import JSSP Instance";
      // 
      // descriptionTabPage
      // 
      this.descriptionTabPage.Controls.Add(this.operatorBaseDescriptionView);
      this.descriptionTabPage.Location = new System.Drawing.Point(4, 22);
      this.descriptionTabPage.Name = "descriptionTabPage";
      this.descriptionTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.descriptionTabPage.Size = new System.Drawing.Size(228, 133);
      this.descriptionTabPage.TabIndex = 2;
      this.descriptionTabPage.Text = "Description";
      this.descriptionTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseDescriptionView
      // 
      this.operatorBaseDescriptionView.Caption = "Operator";
      this.operatorBaseDescriptionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseDescriptionView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseDescriptionView.Name = "operatorBaseDescriptionView";
      this.operatorBaseDescriptionView.Operator = null;
      this.operatorBaseDescriptionView.Size = new System.Drawing.Size(222, 127);
      this.operatorBaseDescriptionView.TabIndex = 0;
      // 
      // JSSPInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl1);
      this.Name = "JSSPInjectorView";
      this.Size = new System.Drawing.Size(236, 159);
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.tabControl1.ResumeLayout(false);
      this.descriptionTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TextBox tb_jobs;
    private System.Windows.Forms.TextBox tb_machines;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabPage tabvariables;
    private System.Windows.Forms.TabPage descriptionTabPage;
    private HeuristicLab.Core.OperatorBaseDescriptionView operatorBaseDescriptionView;
  }
}
