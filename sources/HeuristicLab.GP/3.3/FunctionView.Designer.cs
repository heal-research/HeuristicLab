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

using System;
namespace HeuristicLab.GP {
  partial class FunctionView {
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
      this.components = new System.ComponentModel.Container();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.subTreesGroupBox = new System.Windows.Forms.GroupBox();
      this.label6 = new System.Windows.Forms.Label();
      this.argumentComboBox = new System.Windows.Forms.ComboBox();
      this.subFunctionsListBox = new System.Windows.Forms.ListBox();
      this.editManipulatorButton = new System.Windows.Forms.Button();
      this.manipulatorTextBox = new System.Windows.Forms.TextBox();
      this.initializerTextBox = new System.Windows.Forms.TextBox();
      this.editInitializerButton = new System.Windows.Forms.Button();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.ticketsTextBox = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.minTreeSizeTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.minTreeHeightTextBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.maxSubTreesTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.minSubTreesTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.nameLabel = new System.Windows.Forms.Label();
      this.functionPropertiesErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.groupBox.SuspendLayout();
      this.subTreesGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.functionPropertiesErrorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.subTreesGroupBox);
      this.groupBox.Controls.Add(this.editManipulatorButton);
      this.groupBox.Controls.Add(this.manipulatorTextBox);
      this.groupBox.Controls.Add(this.initializerTextBox);
      this.groupBox.Controls.Add(this.editInitializerButton);
      this.groupBox.Controls.Add(this.label8);
      this.groupBox.Controls.Add(this.label7);
      this.groupBox.Controls.Add(this.ticketsTextBox);
      this.groupBox.Controls.Add(this.label5);
      this.groupBox.Controls.Add(this.minTreeSizeTextBox);
      this.groupBox.Controls.Add(this.label3);
      this.groupBox.Controls.Add(this.minTreeHeightTextBox);
      this.groupBox.Controls.Add(this.label4);
      this.groupBox.Controls.Add(this.maxSubTreesTextBox);
      this.groupBox.Controls.Add(this.label2);
      this.groupBox.Controls.Add(this.minSubTreesTextBox);
      this.groupBox.Controls.Add(this.label1);
      this.groupBox.Controls.Add(this.nameTextBox);
      this.groupBox.Controls.Add(this.nameLabel);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(432, 514);
      this.groupBox.TabIndex = 0;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "Function";
      // 
      // subTreesGroupBox
      // 
      this.subTreesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.subTreesGroupBox.Controls.Add(this.label6);
      this.subTreesGroupBox.Controls.Add(this.argumentComboBox);
      this.subTreesGroupBox.Controls.Add(this.subFunctionsListBox);
      this.subTreesGroupBox.Location = new System.Drawing.Point(9, 228);
      this.subTreesGroupBox.Name = "subTreesGroupBox";
      this.subTreesGroupBox.Size = new System.Drawing.Size(417, 280);
      this.subTreesGroupBox.TabIndex = 22;
      this.subTreesGroupBox.TabStop = false;
      this.subTreesGroupBox.Text = "Allowed sub trees:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 22);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(76, 13);
      this.label6.TabIndex = 23;
      this.label6.Text = "n-th Argument:";
      // 
      // argumentComboBox
      // 
      this.argumentComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.argumentComboBox.FormattingEnabled = true;
      this.argumentComboBox.Location = new System.Drawing.Point(88, 19);
      this.argumentComboBox.Name = "argumentComboBox";
      this.argumentComboBox.Size = new System.Drawing.Size(129, 21);
      this.argumentComboBox.TabIndex = 20;
      this.argumentComboBox.SelectedIndexChanged += new System.EventHandler(this.argumentComboBox_SelectedIndexChanged);
      // 
      // subFunctionsListBox
      // 
      this.subFunctionsListBox.AllowDrop = true;
      this.subFunctionsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.subFunctionsListBox.FormattingEnabled = true;
      this.subFunctionsListBox.Location = new System.Drawing.Point(6, 46);
      this.subFunctionsListBox.Name = "subFunctionsListBox";
      this.subFunctionsListBox.Size = new System.Drawing.Size(405, 225);
      this.subFunctionsListBox.TabIndex = 12;
      this.subFunctionsListBox.DragOver += new System.Windows.Forms.DragEventHandler(this.subFunctionsListBox_DragOver);
      this.subFunctionsListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.subFunctionsListBox_DragDrop);
      this.subFunctionsListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.subFunctionsListBox_DragEnter);
      this.subFunctionsListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.subFunctionsListBox_KeyUp);
      // 
      // editManipulatorButton
      // 
      this.editManipulatorButton.Location = new System.Drawing.Point(232, 199);
      this.editManipulatorButton.Name = "editManipulatorButton";
      this.editManipulatorButton.Size = new System.Drawing.Size(75, 23);
      this.editManipulatorButton.TabIndex = 19;
      this.editManipulatorButton.Text = "Edit...";
      this.editManipulatorButton.UseVisualStyleBackColor = true;
      this.editManipulatorButton.Click += new System.EventHandler(this.editManipulatorButton_Click);
      // 
      // manipulatorTextBox
      // 
      this.manipulatorTextBox.Location = new System.Drawing.Point(126, 201);
      this.manipulatorTextBox.Name = "manipulatorTextBox";
      this.manipulatorTextBox.ReadOnly = true;
      this.manipulatorTextBox.Size = new System.Drawing.Size(100, 20);
      this.manipulatorTextBox.TabIndex = 18;
      // 
      // initializerTextBox
      // 
      this.initializerTextBox.Location = new System.Drawing.Point(126, 175);
      this.initializerTextBox.Name = "initializerTextBox";
      this.initializerTextBox.ReadOnly = true;
      this.initializerTextBox.Size = new System.Drawing.Size(100, 20);
      this.initializerTextBox.TabIndex = 17;
      // 
      // editInitializerButton
      // 
      this.editInitializerButton.Location = new System.Drawing.Point(232, 173);
      this.editInitializerButton.Name = "editInitializerButton";
      this.editInitializerButton.Size = new System.Drawing.Size(75, 23);
      this.editInitializerButton.TabIndex = 16;
      this.editInitializerButton.Text = "Edit...";
      this.editInitializerButton.UseVisualStyleBackColor = true;
      this.editInitializerButton.Click += new System.EventHandler(this.editInitializerButton_Click);
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(6, 204);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(65, 13);
      this.label8.TabIndex = 15;
      this.label8.Text = "Manipulator:";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 178);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(50, 13);
      this.label7.TabIndex = 14;
      this.label7.Text = "Initializer:";
      // 
      // ticketsTextBox
      // 
      this.ticketsTextBox.Location = new System.Drawing.Point(126, 97);
      this.ticketsTextBox.Name = "ticketsTextBox";
      this.ticketsTextBox.Size = new System.Drawing.Size(100, 20);
      this.ticketsTextBox.TabIndex = 11;
      this.ticketsTextBox.TextChanged += new System.EventHandler(this.ticketsTextBox_TextChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 100);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(45, 13);
      this.label5.TabIndex = 10;
      this.label5.Text = "Tickets:";
      // 
      // minTreeSizeTextBox
      // 
      this.minTreeSizeTextBox.Location = new System.Drawing.Point(126, 149);
      this.minTreeSizeTextBox.Name = "minTreeSizeTextBox";
      this.minTreeSizeTextBox.ReadOnly = true;
      this.minTreeSizeTextBox.Size = new System.Drawing.Size(100, 20);
      this.minTreeSizeTextBox.TabIndex = 9;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 152);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(72, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Min. tree size:";
      // 
      // minTreeHeightTextBox
      // 
      this.minTreeHeightTextBox.Location = new System.Drawing.Point(126, 123);
      this.minTreeHeightTextBox.Name = "minTreeHeightTextBox";
      this.minTreeHeightTextBox.ReadOnly = true;
      this.minTreeHeightTextBox.Size = new System.Drawing.Size(100, 20);
      this.minTreeHeightTextBox.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(4, 126);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(83, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Min. tree height:";
      // 
      // maxSubTreesTextBox
      // 
      this.maxSubTreesTextBox.Location = new System.Drawing.Point(126, 71);
      this.maxSubTreesTextBox.Name = "maxSubTreesTextBox";
      this.maxSubTreesTextBox.Size = new System.Drawing.Size(100, 20);
      this.maxSubTreesTextBox.TabIndex = 5;
      this.maxSubTreesTextBox.TextChanged += new System.EventHandler(this.maxSubTreesTextBox_TextChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 74);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(79, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Max. sub trees:";
      // 
      // minSubTreesTextBox
      // 
      this.minSubTreesTextBox.Location = new System.Drawing.Point(126, 45);
      this.minSubTreesTextBox.Name = "minSubTreesTextBox";
      this.minSubTreesTextBox.Size = new System.Drawing.Size(100, 20);
      this.minSubTreesTextBox.TabIndex = 3;
      this.minSubTreesTextBox.TextChanged += new System.EventHandler(this.minSubTreesTextBox_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 48);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(76, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Min. sub trees:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(126, 19);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(100, 20);
      this.nameTextBox.TabIndex = 1;
      this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(6, 22);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 0;
      this.nameLabel.Text = "Name:";
      // 
      // functionPropertiesErrorProvider
      // 
      this.functionPropertiesErrorProvider.ContainerControl = this;
      // 
      // FunctionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox);
      this.Name = "FunctionView";
      this.Size = new System.Drawing.Size(432, 514);
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.subTreesGroupBox.ResumeLayout(false);
      this.subTreesGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.functionPropertiesErrorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.TextBox minSubTreesTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TextBox maxSubTreesTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox minTreeSizeTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox minTreeHeightTextBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox ticketsTextBox;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ListBox subFunctionsListBox;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button editManipulatorButton;
    private System.Windows.Forms.TextBox manipulatorTextBox;
    private System.Windows.Forms.TextBox initializerTextBox;
    private System.Windows.Forms.Button editInitializerButton;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ComboBox argumentComboBox;
    private System.Windows.Forms.GroupBox subTreesGroupBox;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.ErrorProvider functionPropertiesErrorProvider;

  }
}
