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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class PreprocessingCheckedVariablesView {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.variablesGroupBox = new System.Windows.Forms.GroupBox();
      this.variablesListView = new System.Windows.Forms.ListView();
      this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.uncheckAllButton = new System.Windows.Forms.Button();
      this.checkAllButton = new System.Windows.Forms.Button();
      this.checkInputsTargetButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.variablesGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.variablesGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(940, 698);
      this.splitContainer.SplitterDistance = 180;
      this.splitContainer.TabIndex = 7;
      // 
      // variablesGroupBox
      // 
      this.variablesGroupBox.Controls.Add(this.uncheckAllButton);
      this.variablesGroupBox.Controls.Add(this.checkAllButton);
      this.variablesGroupBox.Controls.Add(this.checkInputsTargetButton);
      this.variablesGroupBox.Controls.Add(this.variablesListView);
      this.variablesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variablesGroupBox.Name = "variablesGroupBox";
      this.variablesGroupBox.Size = new System.Drawing.Size(180, 698);
      this.variablesGroupBox.TabIndex = 7;
      this.variablesGroupBox.TabStop = false;
      this.variablesGroupBox.Text = "Variables";
      // 
      // variablesListView
      // 
      this.variablesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.variablesListView.CheckBoxes = true;
      this.variablesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
      this.variablesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.variablesListView.HideSelection = false;
      this.variablesListView.Location = new System.Drawing.Point(6, 49);
      this.variablesListView.Name = "variablesListView";
      this.variablesListView.Size = new System.Drawing.Size(168, 643);
      this.variablesListView.TabIndex = 12;
      this.variablesListView.UseCompatibleStateImageBehavior = false;
      this.variablesListView.View = System.Windows.Forms.View.Details;
      // 
      // columnHeader
      // 
      this.columnHeader.Text = "";
      this.columnHeader.Width = 164;
      // 
      // uncheckAllButton
      // 
      this.uncheckAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.None;
      this.uncheckAllButton.Location = new System.Drawing.Point(66, 19);
      this.uncheckAllButton.Name = "uncheckAllButton";
      this.uncheckAllButton.Size = new System.Drawing.Size(24, 24);
      this.uncheckAllButton.TabIndex = 9;
      this.toolTip.SetToolTip(this.uncheckAllButton, "Show None");
      this.uncheckAllButton.UseVisualStyleBackColor = true;
      this.uncheckAllButton.Click += new System.EventHandler(this.uncheckAllButton_Click);
      // 
      // checkAllButton
      // 
      this.checkAllButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.All;
      this.checkAllButton.Location = new System.Drawing.Point(6, 19);
      this.checkAllButton.Name = "checkAllButton";
      this.checkAllButton.Size = new System.Drawing.Size(24, 24);
      this.checkAllButton.TabIndex = 7;
      this.toolTip.SetToolTip(this.checkAllButton, "Show All");
      this.checkAllButton.UseVisualStyleBackColor = true;
      this.checkAllButton.Click += new System.EventHandler(this.checkAllButton_Click);
      // 
      // checkInputsTargetButton
      // 
      this.checkInputsTargetButton.Image = global::HeuristicLab.DataPreprocessing.Views.PreprocessingIcons.Inputs;
      this.checkInputsTargetButton.Location = new System.Drawing.Point(36, 19);
      this.checkInputsTargetButton.Name = "checkInputsTargetButton";
      this.checkInputsTargetButton.Size = new System.Drawing.Size(24, 24);
      this.checkInputsTargetButton.TabIndex = 8;
      this.toolTip.SetToolTip(this.checkInputsTargetButton, "Show Inputs & Target");
      this.checkInputsTargetButton.UseVisualStyleBackColor = true;
      this.checkInputsTargetButton.Click += new System.EventHandler(this.checkInputsTargetButton_Click);
      // 
      // PreprocessingCheckedVariablesView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "PreprocessingCheckedVariablesView";
      this.Size = new System.Drawing.Size(940, 698);
      this.splitContainer.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.variablesGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    protected System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ColumnHeader columnHeader;
    private System.Windows.Forms.ListView variablesListView;
    private System.Windows.Forms.GroupBox variablesGroupBox;
    private System.Windows.Forms.Button checkInputsTargetButton;
    private System.Windows.Forms.Button uncheckAllButton;
    private System.Windows.Forms.Button checkAllButton;
  }
}
