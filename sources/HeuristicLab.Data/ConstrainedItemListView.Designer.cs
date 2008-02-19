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

namespace HeuristicLab.Data {
  partial class ConstrainedItemListView {
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
      this.itemsSplitContainer = new System.Windows.Forms.SplitContainer();
      this.itemsGroupBox = new System.Windows.Forms.GroupBox();
      this.removeItemButton = new System.Windows.Forms.Button();
      this.addItemButton = new System.Windows.Forms.Button();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.itemsTabPage = new System.Windows.Forms.TabPage();
      this.constraintsTabPage = new System.Windows.Forms.TabPage();
      this.constraintsConstrainedItemBaseView = new HeuristicLab.Core.ConstrainedItemBaseView();
      this.itemsListView = new System.Windows.Forms.ListView();
      this.itemsSplitContainer.Panel1.SuspendLayout();
      this.itemsSplitContainer.Panel2.SuspendLayout();
      this.itemsSplitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.itemsTabPage.SuspendLayout();
      this.constraintsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // itemsSplitContainer
      // 
      this.itemsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemsSplitContainer.Location = new System.Drawing.Point(3, 3);
      this.itemsSplitContainer.Name = "itemsSplitContainer";
      // 
      // itemsSplitContainer.Panel1
      // 
      this.itemsSplitContainer.Panel1.Controls.Add(this.itemsGroupBox);
      // 
      // itemsSplitContainer.Panel2
      // 
      this.itemsSplitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.itemsSplitContainer.Size = new System.Drawing.Size(367, 378);
      this.itemsSplitContainer.SplitterDistance = 162;
      this.itemsSplitContainer.TabIndex = 2;
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Controls.Add(this.itemsListView);
      this.itemsGroupBox.Controls.Add(this.removeItemButton);
      this.itemsGroupBox.Controls.Add(this.addItemButton);
      this.itemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.itemsGroupBox.Name = "itemsGroupBox";
      this.itemsGroupBox.Size = new System.Drawing.Size(162, 378);
      this.itemsGroupBox.TabIndex = 0;
      this.itemsGroupBox.TabStop = false;
      this.itemsGroupBox.Text = "Items";
      // 
      // removeItemButton
      // 
      this.removeItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeItemButton.Location = new System.Drawing.Point(84, 349);
      this.removeItemButton.Name = "removeItemButton";
      this.removeItemButton.Size = new System.Drawing.Size(75, 23);
      this.removeItemButton.TabIndex = 4;
      this.removeItemButton.Text = "Remove";
      this.removeItemButton.UseVisualStyleBackColor = true;
      this.removeItemButton.Click += new System.EventHandler(this.removeItemButton_Click);
      // 
      // addItemButton
      // 
      this.addItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addItemButton.Location = new System.Drawing.Point(3, 349);
      this.addItemButton.Name = "addItemButton";
      this.addItemButton.Size = new System.Drawing.Size(75, 23);
      this.addItemButton.TabIndex = 3;
      this.addItemButton.Text = "Add";
      this.addItemButton.UseVisualStyleBackColor = true;
      this.addItemButton.Click += new System.EventHandler(this.addItemButton_Click);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(201, 378);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.itemsTabPage);
      this.tabControl.Controls.Add(this.constraintsTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(381, 410);
      this.tabControl.TabIndex = 5;
      // 
      // itemsTabPage
      // 
      this.itemsTabPage.Controls.Add(this.itemsSplitContainer);
      this.itemsTabPage.Location = new System.Drawing.Point(4, 22);
      this.itemsTabPage.Name = "itemsTabPage";
      this.itemsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.itemsTabPage.Size = new System.Drawing.Size(373, 384);
      this.itemsTabPage.TabIndex = 0;
      this.itemsTabPage.Text = "Items";
      this.itemsTabPage.UseVisualStyleBackColor = true;
      // 
      // constraintsTabPage
      // 
      this.constraintsTabPage.Controls.Add(this.constraintsConstrainedItemBaseView);
      this.constraintsTabPage.Location = new System.Drawing.Point(4, 22);
      this.constraintsTabPage.Name = "constraintsTabPage";
      this.constraintsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.constraintsTabPage.Size = new System.Drawing.Size(373, 384);
      this.constraintsTabPage.TabIndex = 1;
      this.constraintsTabPage.Text = "Constraints";
      this.constraintsTabPage.UseVisualStyleBackColor = true;
      // 
      // constraintsConstrainedItemBaseView
      // 
      this.constraintsConstrainedItemBaseView.Caption = "Constrained Item";
      this.constraintsConstrainedItemBaseView.ConstrainedItem = null;
      this.constraintsConstrainedItemBaseView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.constraintsConstrainedItemBaseView.Location = new System.Drawing.Point(3, 3);
      this.constraintsConstrainedItemBaseView.Name = "constraintsConstrainedItemBaseView";
      this.constraintsConstrainedItemBaseView.Size = new System.Drawing.Size(367, 378);
      this.constraintsConstrainedItemBaseView.TabIndex = 0;
      // 
      // itemsListView
      // 
      this.itemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.itemsListView.Location = new System.Drawing.Point(6, 19);
      this.itemsListView.Name = "itemsListView";
      this.itemsListView.Size = new System.Drawing.Size(153, 324);
      this.itemsListView.TabIndex = 0;
      this.itemsListView.UseCompatibleStateImageBehavior = false;
      this.itemsListView.SelectedIndexChanged += new System.EventHandler(this.itemsListView_SelectedIndexChanged);
      this.itemsListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.itemsListView_KeyUp);
      // 
      // ConstrainedItemListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "ConstrainedItemListView";
      this.Size = new System.Drawing.Size(381, 410);
      this.itemsSplitContainer.Panel1.ResumeLayout(false);
      this.itemsSplitContainer.Panel2.ResumeLayout(false);
      this.itemsSplitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.itemsTabPage.ResumeLayout(false);
      this.constraintsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer itemsSplitContainer;
    private System.Windows.Forms.Button addItemButton;
    private System.Windows.Forms.Button removeItemButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage itemsTabPage;
    private System.Windows.Forms.TabPage constraintsTabPage;
    private System.Windows.Forms.GroupBox itemsGroupBox;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private HeuristicLab.Core.ConstrainedItemBaseView constraintsConstrainedItemBaseView;
    private System.Windows.Forms.ListView itemsListView;
  }
}
