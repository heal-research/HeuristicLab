#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.VS2010Wizards {
  partial class ParametersControl {
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
      this.parametersListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.typeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.dataTypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.descriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.defaultValueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.downButton = new System.Windows.Forms.Button();
      this.upButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.parameterTypeComboBox = new System.Windows.Forms.ComboBox();
      this.customInputTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // parametersListView
      // 
      this.parametersListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.typeColumnHeader,
            this.dataTypeColumnHeader,
            this.descriptionColumnHeader,
            this.defaultValueColumnHeader});
      this.parametersListView.FullRowSelect = true;
      this.parametersListView.GridLines = true;
      this.parametersListView.HideSelection = false;
      this.parametersListView.Location = new System.Drawing.Point(35, 3);
      this.parametersListView.MultiSelect = false;
      this.parametersListView.Name = "parametersListView";
      this.parametersListView.Size = new System.Drawing.Size(701, 250);
      this.parametersListView.TabIndex = 2;
      this.parametersListView.UseCompatibleStateImageBehavior = false;
      this.parametersListView.View = System.Windows.Forms.View.Details;
      this.parametersListView.SelectedIndexChanged += new System.EventHandler(this.parametersListView_SelectedIndexChanged);
      this.parametersListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.parametersListView_MouseDoubleClick);
      // 
      // nameColumnHeader
      // 
      this.nameColumnHeader.Text = "Name";
      this.nameColumnHeader.Width = 133;
      // 
      // typeColumnHeader
      // 
      this.typeColumnHeader.Text = "Type";
      // 
      // dataTypeColumnHeader
      // 
      this.dataTypeColumnHeader.Text = "Data Type";
      this.dataTypeColumnHeader.Width = 100;
      // 
      // descriptionColumnHeader
      // 
      this.descriptionColumnHeader.Text = "Description";
      this.descriptionColumnHeader.Width = 275;
      // 
      // defaultValueColumnHeader
      // 
      this.defaultValueColumnHeader.Text = "Default Value";
      this.defaultValueColumnHeader.Width = 129;
      // 
      // downButton
      // 
      this.downButton.Enabled = false;
      this.downButton.Image = global::HeuristicLab.VS2010Wizards.Properties.Resources.VS2008ImageLibrary_CommonElements_Objects_Arrow_Down;
      this.downButton.Location = new System.Drawing.Point(3, 90);
      this.downButton.Name = "downButton";
      this.downButton.Size = new System.Drawing.Size(26, 26);
      this.downButton.TabIndex = 4;
      this.downButton.UseVisualStyleBackColor = true;
      this.downButton.Click += new System.EventHandler(this.downButton_Click);
      // 
      // upButton
      // 
      this.upButton.Enabled = false;
      this.upButton.Image = global::HeuristicLab.VS2010Wizards.Properties.Resources.VS2008ImageLibrary_CommonElements_Objects_Arrow_Up;
      this.upButton.Location = new System.Drawing.Point(3, 61);
      this.upButton.Name = "upButton";
      this.upButton.Size = new System.Drawing.Size(26, 26);
      this.upButton.TabIndex = 4;
      this.upButton.UseVisualStyleBackColor = true;
      this.upButton.Click += new System.EventHandler(this.upButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.Enabled = false;
      this.removeButton.Image = global::HeuristicLab.VS2010Wizards.Properties.Resources.VS2008ImageLibrary_Actions_Delete;
      this.removeButton.Location = new System.Drawing.Point(3, 32);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(26, 26);
      this.removeButton.TabIndex = 4;
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Image = global::HeuristicLab.VS2010Wizards.Properties.Resources.VS2008ImageLibrary_CommonElements_Actions_Add;
      this.addButton.Location = new System.Drawing.Point(3, 3);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(26, 26);
      this.addButton.TabIndex = 4;
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // parameterTypeComboBox
      // 
      this.parameterTypeComboBox.FormattingEnabled = true;
      this.parameterTypeComboBox.Items.AddRange(new object[] {
            "",
            "Value",
            "Lookup",
            "ValueLookup",
            "OptionalValue",
            "ConstrainedValue",
            "OptionalConstrainedValue",
            "ScopeTreeLookup"});
      this.parameterTypeComboBox.Location = new System.Drawing.Point(3, 232);
      this.parameterTypeComboBox.Name = "parameterTypeComboBox";
      this.parameterTypeComboBox.Size = new System.Drawing.Size(26, 21);
      this.parameterTypeComboBox.TabIndex = 5;
      this.parameterTypeComboBox.Visible = false;
      this.parameterTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.parameterTypeComboBox_SelectedIndexChanged);
      this.parameterTypeComboBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.parameterTypeComboBox_KeyUp);
      this.parameterTypeComboBox.Leave += new System.EventHandler(this.parameterTypeComboBox_Leave);
      // 
      // customInputTextBox
      // 
      this.customInputTextBox.Location = new System.Drawing.Point(3, 206);
      this.customInputTextBox.Name = "customInputTextBox";
      this.customInputTextBox.Size = new System.Drawing.Size(26, 20);
      this.customInputTextBox.TabIndex = 6;
      this.customInputTextBox.Visible = false;
      this.customInputTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.customInputTextBox_KeyUp);
      this.customInputTextBox.Leave += new System.EventHandler(this.customInputTextBox_Leave);
      // 
      // ParametersControl
      // 
      this.Controls.Add(this.customInputTextBox);
      this.Controls.Add(this.parameterTypeComboBox);
      this.Controls.Add(this.downButton);
      this.Controls.Add(this.upButton);
      this.Controls.Add(this.removeButton);
      this.Controls.Add(this.addButton);
      this.Controls.Add(this.parametersListView);
      this.Name = "ParametersControl";
      this.Size = new System.Drawing.Size(740, 256);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView parametersListView;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ColumnHeader dataTypeColumnHeader;
    private System.Windows.Forms.ColumnHeader defaultValueColumnHeader;
    private System.Windows.Forms.Button addButton;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button upButton;
    private System.Windows.Forms.Button downButton;
    private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
    private System.Windows.Forms.ColumnHeader typeColumnHeader;
    private System.Windows.Forms.ComboBox parameterTypeComboBox;
    private System.Windows.Forms.TextBox customInputTextBox;
  }
}
