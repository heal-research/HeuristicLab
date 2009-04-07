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

using HeuristicLab.Data;

namespace HeuristicLab.Scheduling.JSSP {
  partial class OperationView {
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
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.jobTextBox = new System.Windows.Forms.TextBox();
      this.opTextBox = new System.Windows.Forms.TextBox();
      this.startTextBox = new System.Windows.Forms.TextBox();
      this.durTextBox = new System.Windows.Forms.TextBox();
      this.predTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(5, 3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(30, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Job: ";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(5, 31);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(56, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Operation:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(5, 57);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(32, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Start:";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(5, 84);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(50, 13);
      this.label5.TabIndex = 8;
      this.label5.Text = "Duration:";
      // 
      // jobTextBox
      // 
      this.jobTextBox.Location = new System.Drawing.Point(71, 4);
      this.jobTextBox.Name = "jobTextBox";
      this.jobTextBox.Size = new System.Drawing.Size(38, 20);
      this.jobTextBox.TabIndex = 9;
      this.jobTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.jobTextBox_Validating);
      // 
      // opTextBox
      // 
      this.opTextBox.Location = new System.Drawing.Point(71, 29);
      this.opTextBox.Name = "opTextBox";
      this.opTextBox.Size = new System.Drawing.Size(38, 20);
      this.opTextBox.TabIndex = 10;
      this.opTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.opTextBox_Validating);
      // 
      // startTextBox
      // 
      this.startTextBox.Location = new System.Drawing.Point(71, 55);
      this.startTextBox.Name = "startTextBox";
      this.startTextBox.Size = new System.Drawing.Size(38, 20);
      this.startTextBox.TabIndex = 11;
      this.startTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.startTextBox_Validating);
      // 
      // durTextBox
      // 
      this.durTextBox.Location = new System.Drawing.Point(71, 82);
      this.durTextBox.Name = "durTextBox";
      this.durTextBox.Size = new System.Drawing.Size(38, 20);
      this.durTextBox.TabIndex = 12;
      this.durTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.durTextBox_Validating);
      // 
      // predTextBox
      // 
      this.predTextBox.Location = new System.Drawing.Point(8, 128);
      this.predTextBox.Name = "predTextBox";
      this.predTextBox.Size = new System.Drawing.Size(101, 20);
      this.predTextBox.TabIndex = 13;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 112);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(74, 13);
      this.label1.TabIndex = 14;
      this.label1.Text = "Predecessors:";
      // 
      // OperationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.predTextBox);
      this.Controls.Add(this.durTextBox);
      this.Controls.Add(this.startTextBox);
      this.Controls.Add(this.opTextBox);
      this.Controls.Add(this.jobTextBox);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Name = "OperationView";
      this.Size = new System.Drawing.Size(130, 178);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox jobTextBox;
    private System.Windows.Forms.TextBox opTextBox;
    private System.Windows.Forms.TextBox startTextBox;
    private System.Windows.Forms.TextBox durTextBox;
    private System.Windows.Forms.TextBox predTextBox;
    private System.Windows.Forms.Label label1;

  }
}
