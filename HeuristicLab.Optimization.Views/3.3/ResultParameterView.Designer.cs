#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class ResultParameterView<T> {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    
    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.resultCollectionNameTextBox = new System.Windows.Forms.TextBox();
      this.resultCollectionNameLabel = new System.Windows.Forms.Label();
      this.defaultValueGroupBox = new System.Windows.Forms.GroupBox();
      this.removeDefaultValueButton = new System.Windows.Forms.Button();
      this.setDefaultValueButton = new System.Windows.Forms.Button();
      this.defaultValueViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.defaultValueGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // actualNameTextBox
      // 
      this.actualNameTextBox.Location = new System.Drawing.Point(98, 26);
      this.actualNameTextBox.Size = new System.Drawing.Size(288, 20);
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Location = new System.Drawing.Point(98, 52);
      this.dataTypeTextBox.Size = new System.Drawing.Size(288, 20);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(98, 0);
      this.nameTextBox.Size = new System.Drawing.Size(263, 20);
      // 
      // resultCollectionNameTextBox
      // 
      this.resultCollectionNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resultCollectionNameTextBox.Location = new System.Drawing.Point(98, 78);
      this.resultCollectionNameTextBox.Name = "resultCollectionNameTextBox";
      this.resultCollectionNameTextBox.Size = new System.Drawing.Size(288, 20);
      this.resultCollectionNameTextBox.TabIndex = 6;
      this.resultCollectionNameTextBox.Validated += new System.EventHandler(this.resultNameTextBox_Validated);
      // 
      // resultCollectionNameLabel
      // 
      this.resultCollectionNameLabel.AutoSize = true;
      this.resultCollectionNameLabel.Location = new System.Drawing.Point(3, 81);
      this.resultCollectionNameLabel.Name = "resultCollectionNameLabel";
      this.resultCollectionNameLabel.Size = new System.Drawing.Size(89, 13);
      this.resultCollectionNameLabel.TabIndex = 5;
      this.resultCollectionNameLabel.Text = "&Result Collection:";
      // 
      // defaultValueGroupBox
      // 
      this.defaultValueGroupBox.AllowDrop = true;
      this.defaultValueGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.defaultValueGroupBox.Controls.Add(this.removeDefaultValueButton);
      this.defaultValueGroupBox.Controls.Add(this.setDefaultValueButton);
      this.defaultValueGroupBox.Controls.Add(this.defaultValueViewHost);
      this.defaultValueGroupBox.Location = new System.Drawing.Point(0, 104);
      this.defaultValueGroupBox.Name = "defaultValueGroupBox";
      this.defaultValueGroupBox.Size = new System.Drawing.Size(386, 222);
      this.defaultValueGroupBox.TabIndex = 7;
      this.defaultValueGroupBox.TabStop = false;
      this.defaultValueGroupBox.Text = "Default Value";
      this.defaultValueGroupBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.defaultValueGroupBox_DragDrop);
      this.defaultValueGroupBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.defaultValueGroupBox_DragEnterOver);
      this.defaultValueGroupBox.DragOver += new System.Windows.Forms.DragEventHandler(this.defaultValueGroupBox_DragEnterOver);
      // 
      // removeDefaultValueButton
      // 
      this.removeDefaultValueButton.Location = new System.Drawing.Point(38, 19);
      this.removeDefaultValueButton.Name = "removeDefaultValueButton";
      this.removeDefaultValueButton.Size = new System.Drawing.Size(26, 23);
      this.removeDefaultValueButton.TabIndex = 1;
      this.removeDefaultValueButton.Text = "Remove";
      this.removeDefaultValueButton.UseVisualStyleBackColor = true;
      this.removeDefaultValueButton.Click += new System.EventHandler(this.removeDefaultValueButton_Click);
      // 
      // setDefaultValueButton
      // 
      this.setDefaultValueButton.Location = new System.Drawing.Point(6, 19);
      this.setDefaultValueButton.Name = "setDefaultValueButton";
      this.setDefaultValueButton.Size = new System.Drawing.Size(26, 23);
      this.setDefaultValueButton.TabIndex = 1;
      this.setDefaultValueButton.Text = "Set";
      this.setDefaultValueButton.UseVisualStyleBackColor = true;
      this.setDefaultValueButton.Click += new System.EventHandler(this.setDefaultValueButton_Click);
      // 
      // defaultValueViewHost
      // 
      this.defaultValueViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.defaultValueViewHost.Caption = "View";
      this.defaultValueViewHost.Content = null;
      this.defaultValueViewHost.Enabled = false;
      this.defaultValueViewHost.Location = new System.Drawing.Point(6, 48);
      this.defaultValueViewHost.Name = "defaultValueViewHost";
      this.defaultValueViewHost.ReadOnly = false;
      this.defaultValueViewHost.Size = new System.Drawing.Size(374, 168);
      this.defaultValueViewHost.TabIndex = 0;
      this.defaultValueViewHost.ViewsLabelVisible = true;
      this.defaultValueViewHost.ViewType = null;
      // 
      // ResultParameterView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.defaultValueGroupBox);
      this.Controls.Add(this.resultCollectionNameTextBox);
      this.Controls.Add(this.resultCollectionNameLabel);
      this.Name = "ResultParameterView";
      this.Size = new System.Drawing.Size(386, 326);
      this.Controls.SetChildIndex(this.dataTypeLabel, 0);
      this.Controls.SetChildIndex(this.dataTypeTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.actualNameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.actualNameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.resultCollectionNameLabel, 0);
      this.Controls.SetChildIndex(this.resultCollectionNameTextBox, 0);
      this.Controls.SetChildIndex(this.defaultValueGroupBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.defaultValueGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TextBox resultCollectionNameTextBox;
    protected System.Windows.Forms.Label resultCollectionNameLabel;
    private System.Windows.Forms.GroupBox defaultValueGroupBox;
    private MainForm.WindowsForms.ViewHost defaultValueViewHost;
    private System.Windows.Forms.Button removeDefaultValueButton;
    private System.Windows.Forms.Button setDefaultValueButton;
  }
}
