#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.VS2010Wizards {
  partial class ViewWizardForm {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewWizardForm));
      this.baseClassLabel = new System.Windows.Forms.Label();
      this.contentTypeTextBox = new System.Windows.Forms.TextBox();
      this.defaultViewCheckBox = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label4 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.finishButton = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // baseClassLabel
      // 
      this.baseClassLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.baseClassLabel.AutoSize = true;
      this.baseClassLabel.Location = new System.Drawing.Point(74, 157);
      this.baseClassLabel.Name = "baseClassLabel";
      this.baseClassLabel.Size = new System.Drawing.Size(85, 13);
      this.baseClassLabel.TabIndex = 0;
      this.baseClassLabel.Text = "Content Type(s):";
      // 
      // contentTypeTextBox
      // 
      this.contentTypeTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.contentTypeTextBox.Location = new System.Drawing.Point(77, 173);
      this.contentTypeTextBox.Name = "contentTypeTextBox";
      this.contentTypeTextBox.Size = new System.Drawing.Size(313, 20);
      this.contentTypeTextBox.TabIndex = 1;
      this.contentTypeTextBox.Text = "IItem";
      this.contentTypeTextBox.TextChanged += new System.EventHandler(this.contentTypeTextBox_TextChanged);
      // 
      // defaultViewCheckBox
      // 
      this.defaultViewCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.defaultViewCheckBox.AutoSize = true;
      this.defaultViewCheckBox.Location = new System.Drawing.Point(396, 175);
      this.defaultViewCheckBox.Name = "defaultViewCheckBox";
      this.defaultViewCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.defaultViewCheckBox.Size = new System.Drawing.Size(60, 17);
      this.defaultViewCheckBox.TabIndex = 4;
      this.defaultViewCheckBox.Text = "Default";
      this.defaultViewCheckBox.UseVisualStyleBackColor = true;
      this.defaultViewCheckBox.CheckedChanged += new System.EventHandler(this.isDefaultViewCheckBox_CheckedChanged);
      this.defaultViewCheckBox.TextChanged += new System.EventHandler(this.isDefaultViewCheckBox_CheckedChanged);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.label2.Location = new System.Drawing.Point(74, 207);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(382, 32);
      this.label2.TabIndex = 5;
      this.label2.Text = "Warning: Only one view can be default for a certain content type. If there is mor" +
          "e than one default view, the application will crash.";
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.White;
      this.panel1.Controls.Add(this.label4);
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(0, 1);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(533, 81);
      this.panel1.TabIndex = 9;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(24, 21);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(245, 39);
      this.label4.TabIndex = 1;
      this.label4.Text = "New View Wizard";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Image = global::HeuristicLab.VS2010Wizards.Properties.Resources.HL3_Logo;
      this.pictureBox1.Location = new System.Drawing.Point(455, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(75, 75);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(446, 305);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 25);
      this.cancelButton.TabIndex = 12;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // finishButton
      // 
      this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.finishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.finishButton.Location = new System.Drawing.Point(361, 305);
      this.finishButton.Name = "finishButton";
      this.finishButton.Size = new System.Drawing.Size(75, 25);
      this.finishButton.TabIndex = 11;
      this.finishButton.Text = "Finish";
      this.finishButton.UseVisualStyleBackColor = true;
      this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel2.BackColor = System.Drawing.Color.White;
      this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
      this.panel2.Location = new System.Drawing.Point(0, 297);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(533, 2);
      this.panel2.TabIndex = 13;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(74, 112);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 13);
      this.label1.TabIndex = 14;
      this.label1.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.nameTextBox.Location = new System.Drawing.Point(77, 128);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(379, 20);
      this.nameTextBox.TabIndex = 15;
      this.nameTextBox.Text = "My View";
      this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
      // 
      // ViewWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(533, 342);
      this.Controls.Add(this.defaultViewCheckBox);
      this.Controls.Add(this.contentTypeTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.finishButton);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.baseClassLabel);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "ViewWizardForm";
      this.Text = "New View Wizard";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label baseClassLabel;
    private System.Windows.Forms.TextBox contentTypeTextBox;
    private System.Windows.Forms.CheckBox defaultViewCheckBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button finishButton;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox nameTextBox;

  }
}