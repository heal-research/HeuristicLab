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

namespace HeuristicLab.Operators.Programmable {
  partial class AddVariableInfoDialog {
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
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.formalNameLabel = new System.Windows.Forms.Label();
      this.formalNameTextBox = new System.Windows.Forms.TextBox();
      this.descriptionLabel = new System.Windows.Forms.Label();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.dataTypeLabel = new System.Windows.Forms.Label();
      this.kindLabel = new System.Windows.Forms.Label();
      this.kindNewCheckBox = new System.Windows.Forms.CheckBox();
      this.kindInCheckBox = new System.Windows.Forms.CheckBox();
      this.kindOutCheckBox = new System.Windows.Forms.CheckBox();
      this.kindDeletedCheckBox = new System.Windows.Forms.CheckBox();
      this.dataTypeTextBox = new System.Windows.Forms.TextBox();
      this.setDataTypeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(311, 210);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 13;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(230, 210);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 12;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // formalNameLabel
      // 
      this.formalNameLabel.AutoSize = true;
      this.formalNameLabel.Location = new System.Drawing.Point(12, 9);
      this.formalNameLabel.Name = "formalNameLabel";
      this.formalNameLabel.Size = new System.Drawing.Size(72, 13);
      this.formalNameLabel.TabIndex = 0;
      this.formalNameLabel.Text = "&Formal Name:";
      // 
      // formalNameTextBox
      // 
      this.formalNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.formalNameTextBox.Location = new System.Drawing.Point(90, 6);
      this.formalNameTextBox.Name = "formalNameTextBox";
      this.formalNameTextBox.Size = new System.Drawing.Size(296, 20);
      this.formalNameTextBox.TabIndex = 1;
      this.formalNameTextBox.Text = "Name";
      this.formalNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.formalNameTextBox_Validating);
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.AutoSize = true;
      this.descriptionLabel.Location = new System.Drawing.Point(12, 35);
      this.descriptionLabel.Name = "descriptionLabel";
      this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
      this.descriptionLabel.TabIndex = 2;
      this.descriptionLabel.Text = "&Description:";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(90, 32);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.descriptionTextBox.Size = new System.Drawing.Size(296, 123);
      this.descriptionTextBox.TabIndex = 3;
      // 
      // dataTypeLabel
      // 
      this.dataTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.dataTypeLabel.AutoSize = true;
      this.dataTypeLabel.Location = new System.Drawing.Point(12, 164);
      this.dataTypeLabel.Name = "dataTypeLabel";
      this.dataTypeLabel.Size = new System.Drawing.Size(60, 13);
      this.dataTypeLabel.TabIndex = 4;
      this.dataTypeLabel.Text = "Data &Type:";
      // 
      // kindLabel
      // 
      this.kindLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.kindLabel.AutoSize = true;
      this.kindLabel.Location = new System.Drawing.Point(12, 188);
      this.kindLabel.Name = "kindLabel";
      this.kindLabel.Size = new System.Drawing.Size(72, 13);
      this.kindLabel.TabIndex = 7;
      this.kindLabel.Text = "Variable &Kind:";
      // 
      // kindNewCheckBox
      // 
      this.kindNewCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.kindNewCheckBox.AutoSize = true;
      this.kindNewCheckBox.Location = new System.Drawing.Point(90, 187);
      this.kindNewCheckBox.Name = "kindNewCheckBox";
      this.kindNewCheckBox.Size = new System.Drawing.Size(48, 17);
      this.kindNewCheckBox.TabIndex = 8;
      this.kindNewCheckBox.Text = "&New";
      this.kindNewCheckBox.UseVisualStyleBackColor = true;
      // 
      // kindInCheckBox
      // 
      this.kindInCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.kindInCheckBox.AutoSize = true;
      this.kindInCheckBox.Checked = true;
      this.kindInCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.kindInCheckBox.Location = new System.Drawing.Point(144, 187);
      this.kindInCheckBox.Name = "kindInCheckBox";
      this.kindInCheckBox.Size = new System.Drawing.Size(35, 17);
      this.kindInCheckBox.TabIndex = 9;
      this.kindInCheckBox.Text = "&In";
      this.kindInCheckBox.UseVisualStyleBackColor = true;
      // 
      // kindOutCheckBox
      // 
      this.kindOutCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.kindOutCheckBox.AutoSize = true;
      this.kindOutCheckBox.Location = new System.Drawing.Point(185, 187);
      this.kindOutCheckBox.Name = "kindOutCheckBox";
      this.kindOutCheckBox.Size = new System.Drawing.Size(43, 17);
      this.kindOutCheckBox.TabIndex = 10;
      this.kindOutCheckBox.Text = "&Out";
      this.kindOutCheckBox.UseVisualStyleBackColor = true;
      // 
      // kindDeletedCheckBox
      // 
      this.kindDeletedCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.kindDeletedCheckBox.AutoSize = true;
      this.kindDeletedCheckBox.Location = new System.Drawing.Point(234, 187);
      this.kindDeletedCheckBox.Name = "kindDeletedCheckBox";
      this.kindDeletedCheckBox.Size = new System.Drawing.Size(63, 17);
      this.kindDeletedCheckBox.TabIndex = 11;
      this.kindDeletedCheckBox.Text = "&Deleted";
      this.kindDeletedCheckBox.UseVisualStyleBackColor = true;
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTypeTextBox.Location = new System.Drawing.Point(90, 161);
      this.dataTypeTextBox.Name = "dataTypeTextBox";
      this.dataTypeTextBox.ReadOnly = true;
      this.dataTypeTextBox.Size = new System.Drawing.Size(247, 20);
      this.dataTypeTextBox.TabIndex = 6;
      this.dataTypeTextBox.Text = "HeuristicLab.Data.BoolData";
      // 
      // setDataTypeButton
      // 
      this.setDataTypeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.setDataTypeButton.Location = new System.Drawing.Point(343, 161);
      this.setDataTypeButton.Name = "setDataTypeButton";
      this.setDataTypeButton.Size = new System.Drawing.Size(43, 20);
      this.setDataTypeButton.TabIndex = 5;
      this.setDataTypeButton.Text = "&Set...";
      this.setDataTypeButton.UseVisualStyleBackColor = true;
      this.setDataTypeButton.Click += new System.EventHandler(this.setDataTypeButton_Click);
      // 
      // AddVariableInfoDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(398, 245);
      this.Controls.Add(this.setDataTypeButton);
      this.Controls.Add(this.kindDeletedCheckBox);
      this.Controls.Add(this.kindOutCheckBox);
      this.Controls.Add(this.kindInCheckBox);
      this.Controls.Add(this.kindNewCheckBox);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(this.dataTypeTextBox);
      this.Controls.Add(this.formalNameTextBox);
      this.Controls.Add(this.kindLabel);
      this.Controls.Add(this.dataTypeLabel);
      this.Controls.Add(this.descriptionLabel);
      this.Controls.Add(this.formalNameLabel);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AddVariableInfoDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Add Variable Info";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Label formalNameLabel;
    private System.Windows.Forms.TextBox formalNameTextBox;
    private System.Windows.Forms.Label descriptionLabel;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private System.Windows.Forms.Label dataTypeLabel;
    private System.Windows.Forms.Label kindLabel;
    private System.Windows.Forms.CheckBox kindNewCheckBox;
    private System.Windows.Forms.CheckBox kindInCheckBox;
    private System.Windows.Forms.CheckBox kindOutCheckBox;
    private System.Windows.Forms.CheckBox kindDeletedCheckBox;
    private System.Windows.Forms.TextBox dataTypeTextBox;
    private System.Windows.Forms.Button setDataTypeButton;
  }
}
