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

namespace HeuristicLab.Optimization.Views {
  partial class ProblemInstanceProviderView<T> {
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
      this.loadButton = new System.Windows.Forms.Button();
      this.instanceLabel = new System.Windows.Forms.Label();
      this.instancesComboBox = new System.Windows.Forms.ComboBox();
      this.importButton = new System.Windows.Forms.Button();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // loadButton
      // 
      this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.loadButton.Location = new System.Drawing.Point(549, -1);
      this.loadButton.Name = "loadButton";
      this.loadButton.Size = new System.Drawing.Size(26, 23);
      this.loadButton.TabIndex = 6;
      this.loadButton.Text = "Load";
      this.loadButton.UseVisualStyleBackColor = true;
      this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
      // 
      // instanceLabel
      // 
      this.instanceLabel.AutoSize = true;
      this.instanceLabel.Location = new System.Drawing.Point(-3, 3);
      this.instanceLabel.Name = "instanceLabel";
      this.instanceLabel.Size = new System.Drawing.Size(51, 13);
      this.instanceLabel.TabIndex = 4;
      this.instanceLabel.Text = "Instance:";
      // 
      // instancesComboBox
      // 
      this.instancesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.instancesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.instancesComboBox.FormattingEnabled = true;
      this.instancesComboBox.Location = new System.Drawing.Point(54, 0);
      this.instancesComboBox.Name = "instancesComboBox";
      this.instancesComboBox.Size = new System.Drawing.Size(489, 21);
      this.instancesComboBox.TabIndex = 7;
      this.instancesComboBox.DataSourceChanged += new System.EventHandler(this.comboBox_DataSourceChanged);
      // 
      // importButton
      // 
      this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.importButton.Location = new System.Drawing.Point(581, -1);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(26, 23);
      this.importButton.TabIndex = 6;
      this.importButton.Text = "Import";
      this.importButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "All files|*.*";
      // 
      // ProblemInstanceProviderView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.instancesComboBox);
      this.Controls.Add(this.importButton);
      this.Controls.Add(this.loadButton);
      this.Controls.Add(this.instanceLabel);
      this.Name = "ProblemInstanceProviderView";
      this.Size = new System.Drawing.Size(610, 21);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.Button loadButton;
    protected System.Windows.Forms.Label instanceLabel;
    protected System.Windows.Forms.ComboBox instancesComboBox;
    protected System.Windows.Forms.Button importButton;

  }
}
