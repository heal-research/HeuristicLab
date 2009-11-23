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
  partial class VariablesScopeView {
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
      foreach (ListViewItem item in variablesListView.Items) {
        ((IVariable)item.Tag).NameChanged -= new EventHandler(Variable_NameChanged);
      }
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
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.variablesGroupBox = new System.Windows.Forms.GroupBox();
      this.variablesListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.addButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.variablesGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.variablesGroupBox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(276, 155);
      this.splitContainer1.SplitterDistance = 135;
      this.splitContainer1.TabIndex = 0;
      // 
      // variablesGroupBox
      // 
      this.variablesGroupBox.Controls.Add(this.variablesListView);
      this.variablesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variablesGroupBox.Name = "variablesGroupBox";
      this.variablesGroupBox.Size = new System.Drawing.Size(135, 155);
      this.variablesGroupBox.TabIndex = 0;
      this.variablesGroupBox.TabStop = false;
      this.variablesGroupBox.Text = "&Variables";
      // 
      // variablesListView
      // 
      this.variablesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.variablesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.variablesListView.HideSelection = false;
      this.variablesListView.Location = new System.Drawing.Point(3, 16);
      this.variablesListView.Name = "variablesListView";
      this.variablesListView.Size = new System.Drawing.Size(129, 136);
      this.variablesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.variablesListView.TabIndex = 0;
      this.variablesListView.UseCompatibleStateImageBehavior = false;
      this.variablesListView.View = System.Windows.Forms.View.Details;
      this.variablesListView.SelectedIndexChanged += new System.EventHandler(this.variablesListView_SelectedIndexChanged);
      this.variablesListView.SizeChanged += new System.EventHandler(this.variablesListView_SizeChanged);
      this.variablesListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.variablesListView_KeyDown);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(137, 155);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "&Details";
      // 
      // addButton
      // 
      this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addButton.Location = new System.Drawing.Point(0, 161);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(75, 23);
      this.addButton.TabIndex = 1;
      this.addButton.Text = "&Add...";
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeButton.Enabled = false;
      this.removeButton.Location = new System.Drawing.Point(81, 161);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(75, 23);
      this.removeButton.TabIndex = 2;
      this.removeButton.Text = "&Remove";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // VariablesScopeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.removeButton);
      this.Controls.Add(this.addButton);
      this.Controls.Add(this.splitContainer1);
      this.Name = "VariablesScopeView";
      this.Size = new System.Drawing.Size(276, 184);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.variablesGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.GroupBox variablesGroupBox;
    private System.Windows.Forms.ListView variablesListView;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.Button addButton;
    private System.Windows.Forms.Button removeButton;
  }
}
