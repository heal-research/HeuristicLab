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
using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  partial class ConstrainedItemBaseView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (chooseItemDialog != null) chooseItemDialog.Dispose();
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
      this.removeConstraintButton = new System.Windows.Forms.Button();
      this.addConstraintButton = new System.Windows.Forms.Button();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.constraintsGroupBox = new System.Windows.Forms.GroupBox();
      this.constraintsListView = new System.Windows.Forms.ListView();
      this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
      this.constraintDetailsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.constraintsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // removeConstraintButton
      // 
      this.removeConstraintButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeConstraintButton.Enabled = false;
      this.removeConstraintButton.Location = new System.Drawing.Point(81, 310);
      this.removeConstraintButton.Name = "removeConstraintButton";
      this.removeConstraintButton.Size = new System.Drawing.Size(75, 23);
      this.removeConstraintButton.TabIndex = 2;
      this.removeConstraintButton.Text = "&Remove";
      this.removeConstraintButton.UseVisualStyleBackColor = true;
      this.removeConstraintButton.Click += new System.EventHandler(this.removeConstraintButton_Click);
      // 
      // addConstraintButton
      // 
      this.addConstraintButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addConstraintButton.Location = new System.Drawing.Point(0, 310);
      this.addConstraintButton.Name = "addConstraintButton";
      this.addConstraintButton.Size = new System.Drawing.Size(75, 23);
      this.addConstraintButton.TabIndex = 1;
      this.addConstraintButton.Text = "&Add...";
      this.addConstraintButton.UseVisualStyleBackColor = true;
      this.addConstraintButton.Click += new System.EventHandler(this.addConstraintButton_Click);
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.constraintsGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.constraintDetailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(423, 304);
      this.splitContainer.SplitterDistance = 149;
      this.splitContainer.TabIndex = 0;
      // 
      // constraintsGroupBox
      // 
      this.constraintsGroupBox.Controls.Add(this.constraintsListView);
      this.constraintsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.constraintsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.constraintsGroupBox.Name = "constraintsGroupBox";
      this.constraintsGroupBox.Size = new System.Drawing.Size(149, 304);
      this.constraintsGroupBox.TabIndex = 0;
      this.constraintsGroupBox.TabStop = false;
      this.constraintsGroupBox.Text = "&Constraints";
      // 
      // constraintsListView
      // 
      this.constraintsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
      this.constraintsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.constraintsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.constraintsListView.HideSelection = false;
      this.constraintsListView.Location = new System.Drawing.Point(3, 16);
      this.constraintsListView.Name = "constraintsListView";
      this.constraintsListView.ShowItemToolTips = true;
      this.constraintsListView.Size = new System.Drawing.Size(143, 285);
      this.constraintsListView.TabIndex = 0;
      this.constraintsListView.UseCompatibleStateImageBehavior = false;
      this.constraintsListView.View = System.Windows.Forms.View.Details;
      this.constraintsListView.SelectedIndexChanged += new System.EventHandler(this.constraintsListView_SelectedIndexChanged);
      this.constraintsListView.SizeChanged += new System.EventHandler(this.constraintsListView_SizeChanged);
      this.constraintsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.constraintsListView_KeyDown);
      // 
      // constraintDetailsGroupBox
      // 
      this.constraintDetailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.constraintDetailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.constraintDetailsGroupBox.Name = "constraintDetailsGroupBox";
      this.constraintDetailsGroupBox.Size = new System.Drawing.Size(270, 304);
      this.constraintDetailsGroupBox.TabIndex = 0;
      this.constraintDetailsGroupBox.TabStop = false;
      this.constraintDetailsGroupBox.Text = "&Details";
      // 
      // ConstrainedItemBaseView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.addConstraintButton);
      this.Controls.Add(this.removeConstraintButton);
      this.Controls.Add(this.splitContainer);
      this.Name = "ConstrainedItemBaseView";
      this.Size = new System.Drawing.Size(423, 333);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.constraintsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private ColumnHeader columnHeader3;
    protected Button removeConstraintButton;
    protected Button addConstraintButton;
    protected SplitContainer splitContainer;
    protected GroupBox constraintsGroupBox;
    protected ListView constraintsListView;
    protected GroupBox constraintDetailsGroupBox;


  }
}
