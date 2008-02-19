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

namespace HeuristicLab.Core {
  partial class OperatorBaseVariablesView {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.removeVariableButton = new System.Windows.Forms.Button();
      this.addVariableButton = new System.Windows.Forms.Button();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.variablesGroupBox = new System.Windows.Forms.GroupBox();
      this.variablesListView = new System.Windows.Forms.ListView();
      this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
      this.variableDetailsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.variablesGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // removeVariableButton
      // 
      this.removeVariableButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeVariableButton.Enabled = false;
      this.removeVariableButton.Location = new System.Drawing.Point(81, 310);
      this.removeVariableButton.Name = "removeVariableButton";
      this.removeVariableButton.Size = new System.Drawing.Size(75, 23);
      this.removeVariableButton.TabIndex = 2;
      this.removeVariableButton.Text = "&Remove";
      this.removeVariableButton.UseVisualStyleBackColor = true;
      this.removeVariableButton.Click += new System.EventHandler(this.removeVariableButton_Click);
      // 
      // addVariableButton
      // 
      this.addVariableButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addVariableButton.Location = new System.Drawing.Point(0, 310);
      this.addVariableButton.Name = "addVariableButton";
      this.addVariableButton.Size = new System.Drawing.Size(75, 23);
      this.addVariableButton.TabIndex = 1;
      this.addVariableButton.Text = "&Add...";
      this.addVariableButton.UseVisualStyleBackColor = true;
      this.addVariableButton.Click += new System.EventHandler(this.addVariableButton_Click);
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
      this.splitContainer.Panel1.Controls.Add(this.variablesGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.variableDetailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(423, 304);
      this.splitContainer.SplitterDistance = 212;
      this.splitContainer.TabIndex = 0;
      // 
      // variablesGroupBox
      // 
      this.variablesGroupBox.Controls.Add(this.variablesListView);
      this.variablesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variablesGroupBox.Name = "variablesGroupBox";
      this.variablesGroupBox.Size = new System.Drawing.Size(212, 304);
      this.variablesGroupBox.TabIndex = 0;
      this.variablesGroupBox.TabStop = false;
      this.variablesGroupBox.Text = "&Variables";
      // 
      // variablesListView
      // 
      this.variablesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
      this.variablesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.variablesListView.HideSelection = false;
      this.variablesListView.Location = new System.Drawing.Point(3, 16);
      this.variablesListView.Name = "variablesListView";
      this.variablesListView.Size = new System.Drawing.Size(206, 285);
      this.variablesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.variablesListView.TabIndex = 0;
      this.variablesListView.UseCompatibleStateImageBehavior = false;
      this.variablesListView.View = System.Windows.Forms.View.Details;
      this.variablesListView.SelectedIndexChanged += new System.EventHandler(this.variablesListView_SelectedIndexChanged);
      this.variablesListView.SizeChanged += new System.EventHandler(this.variablesListView_SizeChanged);
      this.variablesListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.variablesListView_KeyDown);
      // 
      // variableDetailsGroupBox
      // 
      this.variableDetailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variableDetailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variableDetailsGroupBox.Name = "variableDetailsGroupBox";
      this.variableDetailsGroupBox.Size = new System.Drawing.Size(207, 304);
      this.variableDetailsGroupBox.TabIndex = 0;
      this.variableDetailsGroupBox.TabStop = false;
      this.variableDetailsGroupBox.Text = "&Details";
      // 
      // OperatorBaseVariablesView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.removeVariableButton);
      this.Controls.Add(this.addVariableButton);
      this.Name = "OperatorBaseVariablesView";
      this.Size = new System.Drawing.Size(423, 333);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.variablesGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private ColumnHeader columnHeader2;
    protected SplitContainer splitContainer;
    protected GroupBox variablesGroupBox;
    protected ListView variablesListView;
    protected GroupBox variableDetailsGroupBox;
    protected Button removeVariableButton;
    protected Button addVariableButton;


  }
}
