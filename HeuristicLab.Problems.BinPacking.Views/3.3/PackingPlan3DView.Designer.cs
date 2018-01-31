#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.BinPacking.Views {
  partial class PackingPlan3DView {
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
      this.binSelection = new System.Windows.Forms.ListBox();
      this.itemSelection = new System.Windows.Forms.ListBox();
      this.elementHost = new System.Windows.Forms.Integration.ElementHost();
      this.packingPlan3D = new HeuristicLab.Problems.BinPacking.Views.Container3DView();
      this.SuspendLayout();
      // 
      // binSelection
      // 
      this.binSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.binSelection.FormattingEnabled = true;
      this.binSelection.Location = new System.Drawing.Point(3, 3);
      this.binSelection.Name = "binSelection";
      this.binSelection.Size = new System.Drawing.Size(54, 290);
      this.binSelection.TabIndex = 4;
      this.binSelection.SelectedIndexChanged += new System.EventHandler(this.binSelection_SelectedIndexChanged);
      // 
      // itemSelection
      // 
      this.itemSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.itemSelection.FormattingEnabled = true;
      this.itemSelection.Location = new System.Drawing.Point(58, 3);
      this.itemSelection.Name = "itemSelection";
      this.itemSelection.Size = new System.Drawing.Size(55, 290);
      this.itemSelection.TabIndex = 5;
      this.itemSelection.SelectedIndexChanged += new System.EventHandler(this.itemSelection_SelectedIndexChanged);
      // 
      // elementHost
      // 
      this.elementHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.elementHost.Location = new System.Drawing.Point(119, 3);
      this.elementHost.Name = "elementHost";
      this.elementHost.Size = new System.Drawing.Size(229, 290);
      this.elementHost.TabIndex = 6;
      this.elementHost.Text = "elementHost";
      this.elementHost.Child = this.packingPlan3D;
      // 
      // PackingPlan3DView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.elementHost);
      this.Controls.Add(this.itemSelection);
      this.Controls.Add(this.binSelection);
      this.Name = "PackingPlan3DView";
      this.Size = new System.Drawing.Size(351, 299);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox binSelection;
    private System.Windows.Forms.ListBox itemSelection;
    private System.Windows.Forms.Integration.ElementHost elementHost;
    private Container3DView packingPlan3D;
  }
}
