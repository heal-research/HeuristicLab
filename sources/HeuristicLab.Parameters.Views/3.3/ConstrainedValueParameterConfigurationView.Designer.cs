#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Parameters.Views {
  partial class ConstrainedValueParameterConfigurationView<T> {
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
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.valueGroupBox = new System.Windows.Forms.GroupBox();
      this.configureButton = new System.Windows.Forms.CheckBox();
      this.showInRunCheckBox = new System.Windows.Forms.CheckBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.valueComboBox = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.valueGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Location = new System.Drawing.Point(80, 52);
      this.dataTypeTextBox.Size = new System.Drawing.Size(306, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(80, 0);
      this.nameTextBox.Size = new System.Drawing.Size(306, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Location = new System.Drawing.Point(80, 26);
      this.descriptionTextBox.Size = new System.Drawing.Size(306, 20);
      // 
      // valueGroupBox
      // 
      this.valueGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.valueGroupBox.Controls.Add(this.configureButton);
      this.valueGroupBox.Controls.Add(this.showInRunCheckBox);
      this.valueGroupBox.Controls.Add(this.viewHost);
      this.valueGroupBox.Controls.Add(this.valueComboBox);
      this.valueGroupBox.Location = new System.Drawing.Point(0, 78);
      this.valueGroupBox.Name = "valueGroupBox";
      this.valueGroupBox.Size = new System.Drawing.Size(386, 237);
      this.valueGroupBox.TabIndex = 6;
      this.valueGroupBox.TabStop = false;
      this.valueGroupBox.Text = "Value";
      // 
      // configureButton
      // 
      this.configureButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.configureButton.Appearance = System.Windows.Forms.Appearance.Button;
      this.configureButton.AutoSize = true;
      this.configureButton.Location = new System.Drawing.Point(260, 17);
      this.configureButton.MinimumSize = new System.Drawing.Size(24, 24);
      this.configureButton.Name = "configureButton";
      this.configureButton.Size = new System.Drawing.Size(24, 24);
      this.configureButton.TabIndex = 1;
      this.configureButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Edit;
      this.configureButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.toolTip.SetToolTip(this.configureButton, "Edit Values");
      this.configureButton.UseVisualStyleBackColor = true;
      this.configureButton.CheckedChanged += new System.EventHandler(this.configureButton_CheckedChanged);
      // 
      // showInRunCheckBox
      // 
      this.showInRunCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showInRunCheckBox.AutoSize = true;
      this.showInRunCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.showInRunCheckBox.Checked = true;
      this.showInRunCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showInRunCheckBox.Location = new System.Drawing.Point(290, 21);
      this.showInRunCheckBox.Name = "showInRunCheckBox";
      this.showInRunCheckBox.Size = new System.Drawing.Size(90, 17);
      this.showInRunCheckBox.TabIndex = 2;
      this.showInRunCheckBox.Text = "&Show in Run:";
      this.toolTip.SetToolTip(this.showInRunCheckBox, "Check to show the value of this parameter in each run.");
      this.showInRunCheckBox.UseVisualStyleBackColor = true;
      this.showInRunCheckBox.CheckedChanged += new System.EventHandler(this.showInRunCheckBox_CheckedChanged);
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(6, 46);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(374, 185);
      this.viewHost.TabIndex = 3;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // valueComboBox
      // 
      this.valueComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.valueComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.valueComboBox.FormattingEnabled = true;
      this.valueComboBox.Location = new System.Drawing.Point(6, 19);
      this.valueComboBox.Name = "valueComboBox";
      this.valueComboBox.Size = new System.Drawing.Size(248, 21);
      this.valueComboBox.TabIndex = 0;
      this.toolTip.SetToolTip(this.valueComboBox, "Selected Value");
      this.valueComboBox.SelectedIndexChanged += new System.EventHandler(this.valueComboBox_SelectedIndexChanged);
      // 
      // ConstrainedValueParameterConfigurationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.valueGroupBox);
      this.Name = "ConstrainedValueParameterConfigurationView";
      this.Size = new System.Drawing.Size(386, 315);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.dataTypeTextBox, 0);
      this.Controls.SetChildIndex(this.dataTypeLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.valueGroupBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.valueGroupBox.ResumeLayout(false);
      this.valueGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.CheckBox configureButton;
  }
}
