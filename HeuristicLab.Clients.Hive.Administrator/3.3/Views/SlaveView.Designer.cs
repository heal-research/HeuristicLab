#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class SlaveView {
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
      this.txtSlaveState = new System.Windows.Forms.TextBox();
      this.txtOS = new System.Windows.Forms.TextBox();
      this.label15 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.txtDetailsDescription = new System.Windows.Forms.TextBox();
      this.label13 = new System.Windows.Forms.Label();
      this.txtName = new System.Windows.Forms.TextBox();
      this.txtCPU = new System.Windows.Forms.TextBox();
      this.txtMemory = new System.Windows.Forms.TextBox();
      this.txtLastHeartbeat = new System.Windows.Forms.TextBox();
      this.label12 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.txtFreeMemory = new System.Windows.Forms.TextBox();
      this.txtId = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txtHbIntervall = new System.Windows.Forms.TextBox();
      this.cbxDisposable = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.cbxPublic = new System.Windows.Forms.CheckBox();
      this.label5 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // txtSlaveState
      // 
      this.txtSlaveState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSlaveState.Enabled = false;
      this.txtSlaveState.Location = new System.Drawing.Point(146, 242);
      this.txtSlaveState.Name = "txtSlaveState";
      this.txtSlaveState.Size = new System.Drawing.Size(390, 20);
      this.txtSlaveState.TabIndex = 27;
      // 
      // txtOS
      // 
      this.txtOS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtOS.Enabled = false;
      this.txtOS.Location = new System.Drawing.Point(146, 216);
      this.txtOS.Name = "txtOS";
      this.txtOS.Size = new System.Drawing.Size(390, 20);
      this.txtOS.TabIndex = 26;
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(3, 245);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(35, 13);
      this.label15.TabIndex = 25;
      this.label15.Text = "State:";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(3, 219);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(93, 13);
      this.label14.TabIndex = 24;
      this.label14.Text = "Operating System:";
      // 
      // txtDetailsDescription
      // 
      this.txtDetailsDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtDetailsDescription.Enabled = false;
      this.txtDetailsDescription.Location = new System.Drawing.Point(146, 112);
      this.txtDetailsDescription.Name = "txtDetailsDescription";
      this.txtDetailsDescription.Size = new System.Drawing.Size(390, 20);
      this.txtDetailsDescription.TabIndex = 23;
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(3, 115);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(63, 13);
      this.label13.TabIndex = 22;
      this.label13.Text = "Description:";
      // 
      // txtName
      // 
      this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtName.Location = new System.Drawing.Point(146, 8);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(390, 20);
      this.txtName.TabIndex = 21;
      this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
      // 
      // txtCPU
      // 
      this.txtCPU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtCPU.Enabled = false;
      this.txtCPU.Location = new System.Drawing.Point(146, 138);
      this.txtCPU.Name = "txtCPU";
      this.txtCPU.Size = new System.Drawing.Size(390, 20);
      this.txtCPU.TabIndex = 20;
      // 
      // txtMemory
      // 
      this.txtMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMemory.Enabled = false;
      this.txtMemory.Location = new System.Drawing.Point(146, 164);
      this.txtMemory.Name = "txtMemory";
      this.txtMemory.Size = new System.Drawing.Size(390, 20);
      this.txtMemory.TabIndex = 19;
      // 
      // txtLastHeartbeat
      // 
      this.txtLastHeartbeat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtLastHeartbeat.Enabled = false;
      this.txtLastHeartbeat.Location = new System.Drawing.Point(146, 268);
      this.txtLastHeartbeat.Name = "txtLastHeartbeat";
      this.txtLastHeartbeat.Size = new System.Drawing.Size(390, 20);
      this.txtLastHeartbeat.TabIndex = 18;
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(3, 271);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(80, 13);
      this.label12.TabIndex = 17;
      this.label12.Text = "Last Heartbeat:";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(3, 167);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(47, 13);
      this.label11.TabIndex = 16;
      this.label11.Text = "Memory:";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(3, 141);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(32, 13);
      this.label10.TabIndex = 15;
      this.label10.Text = "CPU:";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(3, 11);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(38, 13);
      this.label9.TabIndex = 14;
      this.label9.Text = "Name:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 193);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(70, 13);
      this.label1.TabIndex = 28;
      this.label1.Text = "Free memory:";
      // 
      // txtFreeMemory
      // 
      this.txtFreeMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFreeMemory.Enabled = false;
      this.txtFreeMemory.Location = new System.Drawing.Point(146, 190);
      this.txtFreeMemory.Name = "txtFreeMemory";
      this.txtFreeMemory.Size = new System.Drawing.Size(390, 20);
      this.txtFreeMemory.TabIndex = 29;
      // 
      // txtId
      // 
      this.txtId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtId.Enabled = false;
      this.txtId.Location = new System.Drawing.Point(146, 86);
      this.txtId.Name = "txtId";
      this.txtId.Size = new System.Drawing.Size(390, 20);
      this.txtId.TabIndex = 30;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 89);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(19, 13);
      this.label2.TabIndex = 31;
      this.label2.Text = "Id:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 37);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(137, 13);
      this.label3.TabIndex = 32;
      this.label3.Text = "Heartbeat Intervall (in sec) :";
      // 
      // txtHbIntervall
      // 
      this.txtHbIntervall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtHbIntervall.Location = new System.Drawing.Point(146, 34);
      this.txtHbIntervall.Name = "txtHbIntervall";
      this.txtHbIntervall.Size = new System.Drawing.Size(390, 20);
      this.txtHbIntervall.TabIndex = 33;
      this.txtHbIntervall.TextChanged += new System.EventHandler(this.txtHbIntervall_TextChanged);
      // 
      // cbxDisposable
      // 
      this.cbxDisposable.AutoSize = true;
      this.cbxDisposable.Enabled = false;
      this.cbxDisposable.Location = new System.Drawing.Point(146, 297);
      this.cbxDisposable.Name = "cbxDisposable";
      this.cbxDisposable.Size = new System.Drawing.Size(15, 14);
      this.cbxDisposable.TabIndex = 34;
      this.cbxDisposable.UseVisualStyleBackColor = true;
      this.cbxDisposable.CheckedChanged += new System.EventHandler(this.cbxDisposable_CheckedChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 297);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(62, 13);
      this.label4.TabIndex = 35;
      this.label4.Text = "Disposable:";
      // 
      // cbxPublic
      // 
      this.cbxPublic.AutoSize = true;
      this.cbxPublic.Enabled = false;
      this.cbxPublic.Location = new System.Drawing.Point(146, 63);
      this.cbxPublic.Name = "cbxPublic";
      this.cbxPublic.Size = new System.Drawing.Size(15, 14);
      this.cbxPublic.TabIndex = 36;
      this.cbxPublic.UseVisualStyleBackColor = true;
      this.cbxPublic.CheckedChanged += new System.EventHandler(this.cbxPublic_CheckedChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 63);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(39, 13);
      this.label5.TabIndex = 37;
      this.label5.Text = "Public:";
      // 
      // SlaveView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.label5);
      this.Controls.Add(this.cbxPublic);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.cbxDisposable);
      this.Controls.Add(this.txtHbIntervall);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txtId);
      this.Controls.Add(this.txtFreeMemory);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtSlaveState);
      this.Controls.Add(this.txtOS);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.label14);
      this.Controls.Add(this.txtDetailsDescription);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.txtName);
      this.Controls.Add(this.txtCPU);
      this.Controls.Add(this.txtMemory);
      this.Controls.Add(this.txtLastHeartbeat);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label9);
      this.Name = "SlaveView";
      this.Size = new System.Drawing.Size(539, 407);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtSlaveState;
    private System.Windows.Forms.TextBox txtOS;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.TextBox txtDetailsDescription;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.TextBox txtCPU;
    private System.Windows.Forms.TextBox txtMemory;
    private System.Windows.Forms.TextBox txtLastHeartbeat;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtFreeMemory;
    private System.Windows.Forms.TextBox txtId;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtHbIntervall;
    private System.Windows.Forms.CheckBox cbxDisposable;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox cbxPublic;
    private System.Windows.Forms.Label label5;
  }
}
