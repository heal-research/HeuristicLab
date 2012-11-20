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
namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionDataTableView {
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
      this.dataTableComboBox = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.label2 = new System.Windows.Forms.Label();
      this.dataRowComboBox = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.groupByComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // dataTableComboBox
      // 
      this.dataTableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataTableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.dataTableComboBox.FormattingEnabled = true;
      this.dataTableComboBox.Location = new System.Drawing.Point(69, 3);
      this.dataTableComboBox.Name = "dataTableComboBox";
      this.dataTableComboBox.Size = new System.Drawing.Size(455, 21);
      this.dataTableComboBox.TabIndex = 0;
      this.dataTableComboBox.SelectedIndexChanged += new System.EventHandler(this.dataTableComboBox_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(60, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "DataTable:";
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(4, 57);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(520, 314);
      this.viewHost.TabIndex = 2;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 33);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "DataRow:";
      // 
      // dataRowComboBox
      // 
      this.dataRowComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataRowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.dataRowComboBox.FormattingEnabled = true;
      this.dataRowComboBox.Location = new System.Drawing.Point(69, 30);
      this.dataRowComboBox.Name = "dataRowComboBox";
      this.dataRowComboBox.Size = new System.Drawing.Size(455, 21);
      this.dataRowComboBox.TabIndex = 4;
      this.dataRowComboBox.SelectedIndexChanged += new System.EventHandler(this.dataRowComboBox_SelectedIndexChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 61);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(53, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Grouping:";
      // 
      // groupByComboBox
      // 
      this.groupByComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupByComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.groupByComboBox.FormattingEnabled = true;
      this.groupByComboBox.Items.AddRange(new object[] {
            "None",
            "Same runs",
            "All"});
      this.groupByComboBox.Location = new System.Drawing.Point(69, 58);
      this.groupByComboBox.Name = "groupByComboBox";
      this.groupByComboBox.Size = new System.Drawing.Size(455, 21);
      this.groupByComboBox.TabIndex = 6;
      this.groupByComboBox.SelectedIndexChanged += new System.EventHandler(this.groupByComboBox_SelectedIndexChanged);
      // 
      // RunCollectionDataTableView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.groupByComboBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.dataRowComboBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.viewHost);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.dataTableComboBox);
      this.Name = "RunCollectionDataTableView";
      this.Size = new System.Drawing.Size(527, 374);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox dataTableComboBox;
    private System.Windows.Forms.Label label1;
    private MainForm.WindowsForms.ViewHost viewHost;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox dataRowComboBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox groupByComboBox;
  }
}
