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

using HeuristicLab.Data.Views;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class ProjectJobsView {
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
      this.components = new System.ComponentModel.Container();
      this.refreshButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.matrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(3, 3);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh data");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(33, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.removeButton, "Delete job(s)");
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // matrixView
      // 
      this.matrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.matrixView.Caption = "StringConvertibleMatrix View";
      this.matrixView.Content = null;
      this.matrixView.Location = new System.Drawing.Point(3, 33);
      this.matrixView.Name = "matrixView";
      this.matrixView.ReadOnly = false;
      this.matrixView.ShowRowsAndColumnsTextBox = false;
      this.matrixView.ShowStatisticalInformation = false;
      this.matrixView.Size = new System.Drawing.Size(504, 349);
      this.matrixView.TabIndex = 3;
      // 
      // ProjectJobsView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.matrixView);
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.removeButton);
      this.Name = "ProjectJobsView";
      this.Size = new System.Drawing.Size(510, 385);
      this.Load += new System.EventHandler(this.ProjectJobsView_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button removeButton;
    private StringConvertibleMatrixView matrixView;
  }
}
