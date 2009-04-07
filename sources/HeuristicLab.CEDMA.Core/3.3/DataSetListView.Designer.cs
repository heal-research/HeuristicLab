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

namespace HeuristicLab.CEDMA.Core {
  partial class DataSetListView {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.refreshButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.dataSetsGroupBox = new System.Windows.Forms.GroupBox();
      this.dataSetsListView = new System.Windows.Forms.ListView();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.dataSetsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Location = new System.Drawing.Point(81, 161);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(75, 23);
      this.refreshButton.TabIndex = 2;
      this.refreshButton.Text = "&Refresh";
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
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
      this.splitContainer1.Panel1.Controls.Add(this.dataSetsGroupBox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(276, 155);
      this.splitContainer1.SplitterDistance = 135;
      this.splitContainer1.TabIndex = 0;
      // 
      // dataSetsGroupBox
      // 
      this.dataSetsGroupBox.Controls.Add(this.dataSetsListView);
      this.dataSetsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataSetsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.dataSetsGroupBox.Name = "dataSetsGroupBox";
      this.dataSetsGroupBox.Size = new System.Drawing.Size(135, 155);
      this.dataSetsGroupBox.TabIndex = 0;
      this.dataSetsGroupBox.TabStop = false;
      this.dataSetsGroupBox.Text = "&Data Sets";
      // 
      // dataSetsListView
      // 
      this.dataSetsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataSetsListView.Location = new System.Drawing.Point(3, 16);
      this.dataSetsListView.MultiSelect = false;
      this.dataSetsListView.Name = "dataSetsListView";
      this.dataSetsListView.Size = new System.Drawing.Size(129, 136);
      this.dataSetsListView.TabIndex = 0;
      this.dataSetsListView.UseCompatibleStateImageBehavior = false;
      this.dataSetsListView.View = System.Windows.Forms.View.List;
      this.dataSetsListView.SelectedIndexChanged += new System.EventHandler(this.dataSetsListView_SelectedIndexChanged);
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
      // DataSetListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.addButton);
      this.Controls.Add(this.splitContainer1);
      this.Name = "DataSetListView";
      this.Size = new System.Drawing.Size(276, 184);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.dataSetsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.GroupBox dataSetsGroupBox;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.Button addButton;
    private Button refreshButton;
    private ListView dataSetsListView;
  }
}
