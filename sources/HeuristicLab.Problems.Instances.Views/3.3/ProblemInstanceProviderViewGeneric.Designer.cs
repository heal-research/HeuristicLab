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

namespace HeuristicLab.Problems.Instances.Views {
  partial class ProblemInstanceProviderViewGeneric<T> {
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
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.SuspendLayout();
      // 
      // loadButton
      // 
      this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.loadButton.Location = new System.Drawing.Point(667, -1);
      this.loadButton.Name = "loadButton";
      this.loadButton.Size = new System.Drawing.Size(24, 24);
      this.loadButton.TabIndex = 6;
      this.loadButton.Text = "Load";
      this.loadButton.UseVisualStyleBackColor = true;
      this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
      // 
      // instanceLabel
      // 
      this.instanceLabel.AutoSize = true;
      this.instanceLabel.Location = new System.Drawing.Point(3, 4);
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
      this.instancesComboBox.Location = new System.Drawing.Point(60, 1);
      this.instancesComboBox.Name = "instancesComboBox";
      this.instancesComboBox.Size = new System.Drawing.Size(601, 21);
      this.instancesComboBox.TabIndex = 7;
      this.instancesComboBox.DataSourceChanged += new System.EventHandler(this.instancesComboBox_DataSourceChanged);
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "All files|*.*";
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.Filter = "CSV files|*.csv|All files|*.*";
      this.saveFileDialog.Title = "Save RegressionInstance...";
      // 
      // ProblemInstanceProviderViewGeneric
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.instanceLabel);
      this.Controls.Add(this.instancesComboBox);
      this.Controls.Add(this.loadButton);
      this.Name = "ProblemInstanceProviderViewGeneric";
      this.Size = new System.Drawing.Size(694, 21);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.Button loadButton;
    protected System.Windows.Forms.Label instanceLabel;
    protected System.Windows.Forms.ComboBox instancesComboBox;
    protected System.Windows.Forms.SaveFileDialog saveFileDialog;
  }
}
