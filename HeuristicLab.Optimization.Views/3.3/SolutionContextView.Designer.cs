#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class SolutionContextView<TEncodedSolution> {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.leftViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.rightViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.showAdditionalDataCheckBox = new System.Windows.Forms.CheckBox();
      this.additionalDataView = new HeuristicLab.Core.Views.ItemCollectionView<HeuristicLab.Core.IItem>();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
      | System.Windows.Forms.AnchorStyles.Left)
      | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(0, 32);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.leftViewHost);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.rightViewHost);
      this.splitContainer.Size = new System.Drawing.Size(748, 454);
      this.splitContainer.SplitterDistance = 374;
      this.splitContainer.TabIndex = 1;
      // 
      // leftViewHost
      // 
      this.leftViewHost.Caption = "View";
      this.leftViewHost.Content = null;
      this.leftViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.leftViewHost.Enabled = false;
      this.leftViewHost.Location = new System.Drawing.Point(0, 0);
      this.leftViewHost.Name = "leftViewHost";
      this.leftViewHost.ReadOnly = false;
      this.leftViewHost.Size = new System.Drawing.Size(374, 454);
      this.leftViewHost.TabIndex = 2;
      this.leftViewHost.ViewsLabelVisible = true;
      this.leftViewHost.ViewType = null;
      // 
      // rightViewHost
      // 
      this.rightViewHost.Caption = "View";
      this.rightViewHost.Content = null;
      this.rightViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rightViewHost.Enabled = false;
      this.rightViewHost.Location = new System.Drawing.Point(0, 0);
      this.rightViewHost.Name = "rightViewHost";
      this.rightViewHost.ReadOnly = false;
      this.rightViewHost.Size = new System.Drawing.Size(370, 454);
      this.rightViewHost.TabIndex = 3;
      this.rightViewHost.ViewsLabelVisible = true;
      this.rightViewHost.ViewType = null;
      // 
      // showAdditionalDataCheckBox
      // 
      this.showAdditionalDataCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showAdditionalDataCheckBox.AutoSize = true;
      this.showAdditionalDataCheckBox.Location = new System.Drawing.Point(0, 3);
      this.showAdditionalDataCheckBox.Name = "showAdditionalDataCheckBox";
      this.showAdditionalDataCheckBox.Size = new System.Drawing.Size(119, 23);
      this.showAdditionalDataCheckBox.TabIndex = 0;
      this.showAdditionalDataCheckBox.Text = "Show Additional Data";
      this.showAdditionalDataCheckBox.UseVisualStyleBackColor = true;
      this.showAdditionalDataCheckBox.CheckedChanged += ShowAdditionalDataCheckBox_CheckedChanged;
      //
      // additionalDataView
      //
      this.additionalDataView.Name = "AdditionalDataView";
      this.additionalDataView.Location = new System.Drawing.Point(0, 32);
      this.additionalDataView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
      | System.Windows.Forms.AnchorStyles.Left)
      | System.Windows.Forms.AnchorStyles.Right)));
      this.additionalDataView.Size = new System.Drawing.Size(748, 454);
      this.additionalDataView.TabIndex = 4;
      this.additionalDataView.Visible = false;
      // 
      // SolutionContextView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.additionalDataView);
      this.Controls.Add(this.showAdditionalDataCheckBox);
      this.Controls.Add(this.splitContainer);
      this.Name = "SolutionContextView";
      this.Size = new System.Drawing.Size(748, 486);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.CheckBox showAdditionalDataCheckBox;
    private MainForm.WindowsForms.ViewHost leftViewHost;
    private MainForm.WindowsForms.ViewHost rightViewHost;
    private Core.Views.ItemCollectionView<Core.IItem> additionalDataView;
  }
}
