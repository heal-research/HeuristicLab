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
  partial class ProblemInstanceConsumerView {
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
      this.problemInstanceProviderComboBox = new System.Windows.Forms.ComboBox();
      this.libraryInfoButton = new System.Windows.Forms.Button();
      this.libraryLabel = new System.Windows.Forms.Label();
      this.problemInstanceProviderViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // problemInstanceProviderComboBox
      // 
      this.problemInstanceProviderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.problemInstanceProviderComboBox.FormattingEnabled = true;
      this.problemInstanceProviderComboBox.Location = new System.Drawing.Point(50, 0);
      this.problemInstanceProviderComboBox.Name = "problemInstanceProviderComboBox";
      this.problemInstanceProviderComboBox.Size = new System.Drawing.Size(171, 21);
      this.problemInstanceProviderComboBox.TabIndex = 16;
      this.problemInstanceProviderComboBox.SelectedIndexChanged += new System.EventHandler(this.problemInstanceProviderComboBox_SelectedIndexChanged);
      // 
      // libraryInfoButton
      // 
      this.libraryInfoButton.Location = new System.Drawing.Point(226, -1);
      this.libraryInfoButton.Name = "libraryInfoButton";
      this.libraryInfoButton.Size = new System.Drawing.Size(26, 23);
      this.libraryInfoButton.TabIndex = 17;
      this.libraryInfoButton.Text = "Info";
      this.libraryInfoButton.UseVisualStyleBackColor = true;
      this.libraryInfoButton.Click += new System.EventHandler(this.libraryInfoButton_Click);
      // 
      // libraryLabel
      // 
      this.libraryLabel.AutoSize = true;
      this.libraryLabel.Location = new System.Drawing.Point(3, 3);
      this.libraryLabel.Name = "libraryLabel";
      this.libraryLabel.Size = new System.Drawing.Size(41, 13);
      this.libraryLabel.TabIndex = 15;
      this.libraryLabel.Text = "Library:";
      // 
      // problemInstanceProviderViewHost
      // 
      this.problemInstanceProviderViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemInstanceProviderViewHost.Caption = "View";
      this.problemInstanceProviderViewHost.Content = null;
      this.problemInstanceProviderViewHost.Enabled = false;
      this.problemInstanceProviderViewHost.Location = new System.Drawing.Point(258, 0);
      this.problemInstanceProviderViewHost.Name = "problemInstanceProviderViewHost";
      this.problemInstanceProviderViewHost.ReadOnly = false;
      this.problemInstanceProviderViewHost.Size = new System.Drawing.Size(433, 22);
      this.problemInstanceProviderViewHost.TabIndex = 15;
      this.problemInstanceProviderViewHost.ViewsLabelVisible = false;
      this.problemInstanceProviderViewHost.ViewType = null;
      // 
      // ProblemInstanceConsumerView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.problemInstanceProviderViewHost);
      this.Controls.Add(this.problemInstanceProviderComboBox);
      this.Controls.Add(this.libraryInfoButton);
      this.Controls.Add(this.libraryLabel);
      this.Name = "ProblemInstanceConsumerView";
      this.Size = new System.Drawing.Size(694, 21);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox problemInstanceProviderComboBox;
    private System.Windows.Forms.Button libraryInfoButton;
    private System.Windows.Forms.Label libraryLabel;
    private MainForm.WindowsForms.ViewHost problemInstanceProviderViewHost;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
