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

namespace HeuristicLab.Optimizer {
  partial class NewItemDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.itemsListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptioncolumnHeader = new System.Windows.Forms.ColumnHeader();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.versionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(456, 409);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 2;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(537, 409);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 3;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // itemsListView
      // 
      this.itemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.itemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.versionColumnHeader,
            this.descriptioncolumnHeader});
      this.itemsListView.FullRowSelect = true;
      this.itemsListView.HideSelection = false;
      this.itemsListView.Location = new System.Drawing.Point(12, 12);
      this.itemsListView.MultiSelect = false;
      this.itemsListView.Name = "itemsListView";
      this.itemsListView.Size = new System.Drawing.Size(600, 391);
      this.itemsListView.TabIndex = 1;
      this.itemsListView.UseCompatibleStateImageBehavior = false;
      this.itemsListView.View = System.Windows.Forms.View.Details;
      this.itemsListView.SelectedIndexChanged += new System.EventHandler(this.itemTypesListView_SelectedIndexChanged);
      this.itemsListView.DoubleClick += new System.EventHandler(this.itemTypesListView_DoubleClick);
      // 
      // nameColumnHeader
      // 
      this.nameColumnHeader.DisplayIndex = 0;
      this.nameColumnHeader.Text = "Name";
      this.nameColumnHeader.Width = 91;
      // 
      // descriptioncolumnHeader
      // 
      this.descriptioncolumnHeader.DisplayIndex = 2;
      this.descriptioncolumnHeader.Text = "Description";
      this.descriptioncolumnHeader.Width = 190;
      // 
      // versionColumnHeader
      // 
      this.versionColumnHeader.DisplayIndex = 1;
      this.versionColumnHeader.Text = "Version";
      // 
      // NewItemDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(624, 444);
      this.Controls.Add(this.itemsListView);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "NewItemDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "New Item";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.NewItemDialog_Load);
      this.Shown += new System.EventHandler(this.NewItemDialog_Shown);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.ListView itemsListView;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ColumnHeader descriptioncolumnHeader;
    private System.Windows.Forms.ColumnHeader versionColumnHeader;
  }
}