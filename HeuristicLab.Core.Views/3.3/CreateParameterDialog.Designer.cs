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

namespace HeuristicLab.Core.Views {
  partial class CreateParameterDialog {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.cancelButton = new System.Windows.Forms.Button();
      this.okButton = new System.Windows.Forms.Button();
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.descriptionLabel = new System.Windows.Forms.Label();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.dataTypeLabel = new System.Windows.Forms.Label();
      this.dataTypeTextBox = new System.Windows.Forms.TextBox();
      this.setDataTypeButton = new System.Windows.Forms.Button();
      this.parameterTypeSelector = new HeuristicLab.Core.Views.TypeSelector();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(455, 450);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 9;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(374, 450);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 8;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // nameLabel
      // 
      this.nameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(9, 289);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 1;
      this.nameLabel.Text = "&Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(78, 286);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(452, 20);
      this.nameTextBox.TabIndex = 2;
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.descriptionLabel.AutoSize = true;
      this.descriptionLabel.Location = new System.Drawing.Point(9, 315);
      this.descriptionLabel.Name = "descriptionLabel";
      this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
      this.descriptionLabel.TabIndex = 3;
      this.descriptionLabel.Text = "&Description:";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(78, 312);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.descriptionTextBox.Size = new System.Drawing.Size(452, 102);
      this.descriptionTextBox.TabIndex = 4;
      // 
      // dataTypeLabel
      // 
      this.dataTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.dataTypeLabel.AutoSize = true;
      this.dataTypeLabel.Location = new System.Drawing.Point(12, 425);
      this.dataTypeLabel.Name = "dataTypeLabel";
      this.dataTypeLabel.Size = new System.Drawing.Size(60, 13);
      this.dataTypeLabel.TabIndex = 5;
      this.dataTypeLabel.Text = "Data &Type:";
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTypeTextBox.Enabled = false;
      this.dataTypeTextBox.Location = new System.Drawing.Point(78, 422);
      this.dataTypeTextBox.Name = "dataTypeTextBox";
      this.dataTypeTextBox.ReadOnly = true;
      this.dataTypeTextBox.Size = new System.Drawing.Size(422, 20);
      this.dataTypeTextBox.TabIndex = 6;
      // 
      // setDataTypeButton
      // 
      this.setDataTypeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.setDataTypeButton.Enabled = false;
      this.setDataTypeButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Edit;
      this.setDataTypeButton.Location = new System.Drawing.Point(506, 420);
      this.setDataTypeButton.Name = "setDataTypeButton";
      this.setDataTypeButton.Size = new System.Drawing.Size(24, 24);
      this.setDataTypeButton.TabIndex = 7;
      this.toolTip.SetToolTip(this.setDataTypeButton, "Set Data Type");
      this.setDataTypeButton.UseVisualStyleBackColor = true;
      this.setDataTypeButton.Click += new System.EventHandler(this.setDataTypeButton_Click);
      // 
      // parameterTypeSelector
      // 
      this.parameterTypeSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parameterTypeSelector.Caption = "Available Parameters";
      this.parameterTypeSelector.Location = new System.Drawing.Point(12, 12);
      this.parameterTypeSelector.Name = "parameterTypeSelector";
      this.parameterTypeSelector.Size = new System.Drawing.Size(518, 268);
      this.parameterTypeSelector.TabIndex = 0;
      this.parameterTypeSelector.SelectedTypeChanged += new System.EventHandler(this.parameterTypeSelector_SelectedTypeChanged);
      // 
      // CreateParameterDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(542, 485);
      this.Controls.Add(this.setDataTypeButton);
      this.Controls.Add(this.parameterTypeSelector);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(this.dataTypeTextBox);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.dataTypeLabel);
      this.Controls.Add(this.descriptionLabel);
      this.Controls.Add(this.nameLabel);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CreateParameterDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Create Parameter";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Button cancelButton;
    protected System.Windows.Forms.Button okButton;
    protected System.Windows.Forms.Label nameLabel;
    protected System.Windows.Forms.TextBox nameTextBox;
    protected System.Windows.Forms.Label descriptionLabel;
    protected System.Windows.Forms.TextBox descriptionTextBox;
    protected TypeSelector parameterTypeSelector;
    protected System.Windows.Forms.Label dataTypeLabel;
    protected System.Windows.Forms.TextBox dataTypeTextBox;
    protected System.Windows.Forms.Button setDataTypeButton;
    protected System.Windows.Forms.ToolTip toolTip;
  }
}
