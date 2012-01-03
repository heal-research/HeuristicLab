#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.configureButton = new System.Windows.Forms.CheckBox();
      this.valueGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // valueGroupBox
      // 
      this.valueGroupBox.Controls.Add(this.configureButton);
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
      this.showInRunCheckBox.TabIndex = 2;
      // 
      // viewHost
      // 
      this.viewHost.TabIndex = 3;
      // 
      // valueComboBox
      // 
      this.valueComboBox.Size = new System.Drawing.Size(248, 21);
      this.valueComboBox.TabIndex = 0;
      // 
      // ConstrainedValueParameterConfigurationView
      // 
      this.Name = "ConstrainedValueParameterConfigurationView";
      this.valueGroupBox.ResumeLayout(false);
      this.valueGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.CheckBox configureButton;
  }
}
