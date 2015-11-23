#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DataPreprocessing.Views {
  partial class TransformationView {
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
      this.applyButton = new System.Windows.Forms.Button();
      this.preserveColumnsCheckbox = new System.Windows.Forms.CheckBox();
      this.lblFilterNotice = new System.Windows.Forms.Label();
      this.transformationListView = new HeuristicLab.DataPreprocessing.Views.CheckedTransformationListView();
      this.SuspendLayout();
      // 
      // applyButton
      // 
      this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.applyButton.Location = new System.Drawing.Point(490, 488);
      this.applyButton.Name = "applyButton";
      this.applyButton.Size = new System.Drawing.Size(134, 23);
      this.applyButton.TabIndex = 1;
      this.applyButton.Text = "Apply Transformations";
      this.applyButton.UseVisualStyleBackColor = true;
      this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
      // 
      // preserveColumnsCheckbox
      // 
      this.preserveColumnsCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.preserveColumnsCheckbox.AutoSize = true;
      this.preserveColumnsCheckbox.Location = new System.Drawing.Point(337, 492);
      this.preserveColumnsCheckbox.Name = "preserveColumnsCheckbox";
      this.preserveColumnsCheckbox.Size = new System.Drawing.Size(147, 17);
      this.preserveColumnsCheckbox.TabIndex = 2;
      this.preserveColumnsCheckbox.Text = "Preserve original Columns";
      this.preserveColumnsCheckbox.UseVisualStyleBackColor = true;
      // 
      // lblFilterNotice
      // 
      this.lblFilterNotice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblFilterNotice.AutoSize = true;
      this.lblFilterNotice.Location = new System.Drawing.Point(20, 493);
      this.lblFilterNotice.Name = "lblFilterNotice";
      this.lblFilterNotice.Size = new System.Drawing.Size(274, 13);
      this.lblFilterNotice.TabIndex = 3;
      this.lblFilterNotice.Text = "Transformations cannot be applied since a filter is active!";
      this.lblFilterNotice.Visible = false;
      // 
      // transformationListView
      // 
      this.transformationListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.transformationListView.Caption = "Transformations";
      this.transformationListView.Content = null;
      this.transformationListView.Location = new System.Drawing.Point(0, 0);
      this.transformationListView.Name = "transformationListView";
      this.transformationListView.ReadOnly = false;
      this.transformationListView.Size = new System.Drawing.Size(627, 482);
      this.transformationListView.TabIndex = 0;
      // 
      // TransformationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblFilterNotice);
      this.Controls.Add(this.preserveColumnsCheckbox);
      this.Controls.Add(this.applyButton);
      this.Controls.Add(this.transformationListView);
      this.Name = "TransformationView";
      this.Size = new System.Drawing.Size(627, 514);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private CheckedTransformationListView transformationListView;
    private System.Windows.Forms.Button applyButton;
    private System.Windows.Forms.CheckBox preserveColumnsCheckbox;
    private System.Windows.Forms.Label lblFilterNotice;
  }
}
