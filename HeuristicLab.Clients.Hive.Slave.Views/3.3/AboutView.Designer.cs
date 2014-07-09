#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  partial class AboutView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutView));
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.mailLinkLabel = new System.Windows.Forms.LinkLabel();
      this.webLinkLabel = new System.Windows.Forms.LinkLabel();
      this.label4 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.licenseTextBox = new System.Windows.Forms.RichTextBox();
      this.txtCopyright = new System.Windows.Forms.TextBox();
      this.versionTextBox = new System.Windows.Forms.TextBox();
      this.productTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(3, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(163, 165);
      this.pictureBox1.TabIndex = 36;
      this.pictureBox1.TabStop = false;
      // 
      // mailLinkLabel
      // 
      this.mailLinkLabel.AutoSize = true;
      this.mailLinkLabel.Location = new System.Drawing.Point(241, 80);
      this.mailLinkLabel.Name = "mailLinkLabel";
      this.mailLinkLabel.Size = new System.Drawing.Size(129, 13);
      this.mailLinkLabel.TabIndex = 34;
      this.mailLinkLabel.TabStop = true;
      this.mailLinkLabel.Text = "support@heuristiclab.com";
      this.mailLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mailLinkLabel_LinkClicked);
      // 
      // webLinkLabel
      // 
      this.webLinkLabel.AutoSize = true;
      this.webLinkLabel.Location = new System.Drawing.Point(241, 61);
      this.webLinkLabel.Name = "webLinkLabel";
      this.webLinkLabel.Size = new System.Drawing.Size(135, 13);
      this.webLinkLabel.TabIndex = 32;
      this.webLinkLabel.TabStop = true;
      this.webLinkLabel.Text = "http://dev.heuristiclab.com";
      this.webLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.webLinkLabel_LinkClicked);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(169, 80);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(47, 13);
      this.label4.TabIndex = 33;
      this.label4.Text = "Contact:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(169, 61);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(49, 13);
      this.label2.TabIndex = 31;
      this.label2.Text = "Website:";
      // 
      // licenseTextBox
      // 
      this.licenseTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.licenseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.licenseTextBox.Location = new System.Drawing.Point(172, 100);
      this.licenseTextBox.Name = "licenseTextBox";
      this.licenseTextBox.ReadOnly = true;
      this.licenseTextBox.Size = new System.Drawing.Size(380, 173);
      this.licenseTextBox.TabIndex = 35;
      this.licenseTextBox.Text = resources.GetString("licenseTextBox.Text");
      this.licenseTextBox.WordWrap = false;
      // 
      // txtCopyright
      // 
      this.txtCopyright.BackColor = System.Drawing.SystemColors.HighlightText;
      this.txtCopyright.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtCopyright.Location = new System.Drawing.Point(244, 42);
      this.txtCopyright.Name = "txtCopyright";
      this.txtCopyright.ReadOnly = true;
      this.txtCopyright.Size = new System.Drawing.Size(308, 13);
      this.txtCopyright.TabIndex = 30;
      this.txtCopyright.Text = "(c) 2002-2014 HEAL";
      // 
      // versionTextBox
      // 
      this.versionTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.versionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.versionTextBox.Location = new System.Drawing.Point(244, 23);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.ReadOnly = true;
      this.versionTextBox.Size = new System.Drawing.Size(308, 13);
      this.versionTextBox.TabIndex = 28;
      // 
      // productTextBox
      // 
      this.productTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.productTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.productTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.productTextBox.Location = new System.Drawing.Point(244, 4);
      this.productTextBox.Name = "productTextBox";
      this.productTextBox.ReadOnly = true;
      this.productTextBox.Size = new System.Drawing.Size(308, 13);
      this.productTextBox.TabIndex = 26;
      this.productTextBox.Text = "HeuristicLab Hive";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(169, 42);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(54, 13);
      this.label3.TabIndex = 29;
      this.label3.Text = "Copyright:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(169, 23);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(45, 13);
      this.label1.TabIndex = 27;
      this.label1.Text = "Version:";
      // 
      // label
      // 
      this.label.AutoSize = true;
      this.label.Location = new System.Drawing.Point(169, 4);
      this.label.Name = "label";
      this.label.Size = new System.Drawing.Size(47, 13);
      this.label.TabIndex = 25;
      this.label.Text = "Product:";
      // 
      // AboutView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.mailLinkLabel);
      this.Controls.Add(this.webLinkLabel);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.licenseTextBox);
      this.Controls.Add(this.txtCopyright);
      this.Controls.Add(this.versionTextBox);
      this.Controls.Add(this.productTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label);
      this.Name = "AboutView";
      this.Size = new System.Drawing.Size(559, 279);
      this.Load += new System.EventHandler(this.AboutView_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.LinkLabel mailLinkLabel;
    private System.Windows.Forms.LinkLabel webLinkLabel;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RichTextBox licenseTextBox;
    private System.Windows.Forms.TextBox txtCopyright;
    private System.Windows.Forms.TextBox versionTextBox;
    private System.Windows.Forms.TextBox productTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label;
  }
}
