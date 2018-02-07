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

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class GradientBoostedTreesModelView {
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
      this.groupBoxVisualisation = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.listView = new System.Windows.Forms.ListView();
      this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.groupBoxVisualisation.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBoxVisualisation
      // 
      this.groupBoxVisualisation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBoxVisualisation.Controls.Add(this.viewHost);
      this.groupBoxVisualisation.Location = new System.Drawing.Point(169, 3);
      this.groupBoxVisualisation.Name = "groupBoxVisualisation";
      this.groupBoxVisualisation.Size = new System.Drawing.Size(177, 277);
      this.groupBoxVisualisation.TabIndex = 2;
      this.groupBoxVisualisation.TabStop = false;
      this.groupBoxVisualisation.Text = "Visualization";
      // 
      // viewHost
      // 
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(3, 16);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(171, 258);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // listView
      // 
      this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
      this.listView.FullRowSelect = true;
      this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.listView.LabelWrap = false;
      this.listView.Location = new System.Drawing.Point(3, 3);
      this.listView.MultiSelect = false;
      this.listView.Name = "listView";
      this.listView.ShowGroups = false;
      this.listView.Size = new System.Drawing.Size(160, 283);
      this.listView.TabIndex = 1;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.Details;
      this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
      this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
      // 
      // columnHeader
      // 
      this.columnHeader.Width = 130;
      // 
      // GradientBoostedTreesModelView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.groupBoxVisualisation);
      this.Controls.Add(this.listView);
      this.Name = "GradientBoostedTreesModelView";
      this.Size = new System.Drawing.Size(349, 289);
      this.groupBoxVisualisation.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private MainForm.WindowsForms.ViewHost viewHost;
    private System.Windows.Forms.ListView listView;
    private System.Windows.Forms.GroupBox groupBoxVisualisation;
    private System.Windows.Forms.ColumnHeader columnHeader;
  }
}
