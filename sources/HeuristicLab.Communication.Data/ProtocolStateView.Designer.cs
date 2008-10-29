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

namespace HeuristicLab.Communication.Data {
  partial class ProtocolStateView {
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
      this.nameLabel = new System.Windows.Forms.Label();
      this.communicationDataTabControl = new System.Windows.Forms.TabControl();
      this.giveTabPage = new System.Windows.Forms.TabPage();
      this.giveBatchCheckBox = new System.Windows.Forms.CheckBox();
      this.giveVariablesLabel = new System.Windows.Forms.Label();
      this.giveBatchLabel = new System.Windows.Forms.Label();
      this.expectTabPage = new System.Windows.Forms.TabPage();
      this.expectVariablesLabel = new System.Windows.Forms.Label();
      this.expectBatchCheckBox = new System.Windows.Forms.CheckBox();
      this.expectBatchLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.giveItemListView = new HeuristicLab.Data.ItemListView<HeuristicLab.Core.IVariable>();
      this.expectItemListView = new HeuristicLab.Data.ItemListView<HeuristicLab.Core.IVariable>();
      this.communicationDataTabControl.SuspendLayout();
      this.giveTabPage.SuspendLayout();
      this.expectTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(4, 17);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 3;
      this.nameLabel.Text = "Name:";
      // 
      // communicationDataTabControl
      // 
      this.communicationDataTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.communicationDataTabControl.Controls.Add(this.giveTabPage);
      this.communicationDataTabControl.Controls.Add(this.expectTabPage);
      this.communicationDataTabControl.Location = new System.Drawing.Point(3, 46);
      this.communicationDataTabControl.Name = "communicationDataTabControl";
      this.communicationDataTabControl.SelectedIndex = 0;
      this.communicationDataTabControl.Size = new System.Drawing.Size(404, 324);
      this.communicationDataTabControl.TabIndex = 5;
      // 
      // giveTabPage
      // 
      this.giveTabPage.Controls.Add(this.giveItemListView);
      this.giveTabPage.Controls.Add(this.giveBatchCheckBox);
      this.giveTabPage.Controls.Add(this.giveVariablesLabel);
      this.giveTabPage.Controls.Add(this.giveBatchLabel);
      this.giveTabPage.Location = new System.Drawing.Point(4, 22);
      this.giveTabPage.Name = "giveTabPage";
      this.giveTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.giveTabPage.Size = new System.Drawing.Size(396, 298);
      this.giveTabPage.TabIndex = 0;
      this.giveTabPage.Text = "Give";
      this.giveTabPage.UseVisualStyleBackColor = true;
      // 
      // giveBatchCheckBox
      // 
      this.giveBatchCheckBox.AutoSize = true;
      this.giveBatchCheckBox.Location = new System.Drawing.Point(58, 20);
      this.giveBatchCheckBox.Name = "giveBatchCheckBox";
      this.giveBatchCheckBox.Size = new System.Drawing.Size(15, 14);
      this.giveBatchCheckBox.TabIndex = 9;
      this.giveBatchCheckBox.UseVisualStyleBackColor = true;
      // 
      // giveVariablesLabel
      // 
      this.giveVariablesLabel.AutoSize = true;
      this.giveVariablesLabel.Location = new System.Drawing.Point(13, 48);
      this.giveVariablesLabel.Name = "giveVariablesLabel";
      this.giveVariablesLabel.Size = new System.Drawing.Size(53, 13);
      this.giveVariablesLabel.TabIndex = 8;
      this.giveVariablesLabel.Text = "Variables:";
      // 
      // giveBatchLabel
      // 
      this.giveBatchLabel.AutoSize = true;
      this.giveBatchLabel.Location = new System.Drawing.Point(13, 20);
      this.giveBatchLabel.Name = "giveBatchLabel";
      this.giveBatchLabel.Size = new System.Drawing.Size(38, 13);
      this.giveBatchLabel.TabIndex = 6;
      this.giveBatchLabel.Text = "Batch:";
      // 
      // expectTabPage
      // 
      this.expectTabPage.Controls.Add(this.expectItemListView);
      this.expectTabPage.Controls.Add(this.expectVariablesLabel);
      this.expectTabPage.Controls.Add(this.expectBatchCheckBox);
      this.expectTabPage.Controls.Add(this.expectBatchLabel);
      this.expectTabPage.Location = new System.Drawing.Point(4, 22);
      this.expectTabPage.Name = "expectTabPage";
      this.expectTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.expectTabPage.Size = new System.Drawing.Size(396, 298);
      this.expectTabPage.TabIndex = 1;
      this.expectTabPage.Text = "Expect";
      this.expectTabPage.UseVisualStyleBackColor = true;
      // 
      // expectVariablesLabel
      // 
      this.expectVariablesLabel.AutoSize = true;
      this.expectVariablesLabel.Location = new System.Drawing.Point(13, 48);
      this.expectVariablesLabel.Name = "expectVariablesLabel";
      this.expectVariablesLabel.Size = new System.Drawing.Size(53, 13);
      this.expectVariablesLabel.TabIndex = 12;
      this.expectVariablesLabel.Text = "Variables:";
      // 
      // expectBatchCheckBox
      // 
      this.expectBatchCheckBox.AutoSize = true;
      this.expectBatchCheckBox.Location = new System.Drawing.Point(58, 20);
      this.expectBatchCheckBox.Name = "expectBatchCheckBox";
      this.expectBatchCheckBox.Size = new System.Drawing.Size(15, 14);
      this.expectBatchCheckBox.TabIndex = 10;
      this.expectBatchCheckBox.UseVisualStyleBackColor = true;
      // 
      // expectBatchLabel
      // 
      this.expectBatchLabel.AutoSize = true;
      this.expectBatchLabel.Location = new System.Drawing.Point(13, 20);
      this.expectBatchLabel.Name = "expectBatchLabel";
      this.expectBatchLabel.Size = new System.Drawing.Size(38, 13);
      this.expectBatchLabel.TabIndex = 6;
      this.expectBatchLabel.Text = "Batch:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Location = new System.Drawing.Point(48, 14);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(137, 20);
      this.nameTextBox.TabIndex = 6;
      // 
      // giveItemListView
      // 
      this.giveItemListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.giveItemListView.Caption = "View";
      this.giveItemListView.ItemList = null;
      this.giveItemListView.Location = new System.Drawing.Point(16, 64);
      this.giveItemListView.Name = "giveItemListView";
      this.giveItemListView.Size = new System.Drawing.Size(377, 231);
      this.giveItemListView.TabIndex = 10;
      // 
      // expectItemListView
      // 
      this.expectItemListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.expectItemListView.Caption = "View";
      this.expectItemListView.ItemList = null;
      this.expectItemListView.Location = new System.Drawing.Point(16, 64);
      this.expectItemListView.Name = "expectItemListView";
      this.expectItemListView.Size = new System.Drawing.Size(377, 231);
      this.expectItemListView.TabIndex = 13;
      // 
      // ProtocolStateView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.communicationDataTabControl);
      this.Controls.Add(this.nameLabel);
      this.Name = "ProtocolStateView";
      this.Size = new System.Drawing.Size(410, 373);
      this.communicationDataTabControl.ResumeLayout(false);
      this.giveTabPage.ResumeLayout(false);
      this.giveTabPage.PerformLayout();
      this.expectTabPage.ResumeLayout(false);
      this.expectTabPage.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TabControl communicationDataTabControl;
    private System.Windows.Forms.TabPage giveTabPage;
    private System.Windows.Forms.TabPage expectTabPage;
    private System.Windows.Forms.Label giveBatchLabel;
    private System.Windows.Forms.Label expectBatchLabel;
    private System.Windows.Forms.CheckBox giveBatchCheckBox;
    private System.Windows.Forms.Label giveVariablesLabel;
    private System.Windows.Forms.Label expectVariablesLabel;
    private System.Windows.Forms.CheckBox expectBatchCheckBox;
    private System.Windows.Forms.TextBox nameTextBox;
    private HeuristicLab.Data.ItemListView<HeuristicLab.Core.IVariable> giveItemListView;
    private HeuristicLab.Data.ItemListView<HeuristicLab.Core.IVariable> expectItemListView;

  }
}
