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
  partial class CovarianceProdView {
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
      this.itemListView = new HeuristicLab.Core.Views.ItemListView<ICovarianceFunction>();
      this.SuspendLayout();
      // 
      // itemListView
      // 
      this.itemListView.Caption = "ItemList View";
      this.itemListView.Content = null;
      this.itemListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemListView.Location = new System.Drawing.Point(0, 0);
      this.itemListView.Name = "itemListView";
      this.itemListView.ReadOnly = false;
      this.itemListView.Size = new System.Drawing.Size(253, 251);
      this.itemListView.TabIndex = 0;
      // 
      // CovarianceSumView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.itemListView);
      this.Name = "CovarianceProdView";
      this.Size = new System.Drawing.Size(253, 251);
      this.ResumeLayout(false);

    }

    #endregion

    private Core.Views.ItemListView<ICovarianceFunction> itemListView;
  }
}
