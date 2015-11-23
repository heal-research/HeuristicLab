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
namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  partial class CodeNodeView {
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
      this.prefixCode = new System.Windows.Forms.TextBox();
      this.prefixLabel = new System.Windows.Forms.Label();
      this.suffixLabel = new System.Windows.Forms.Label();
      this.suffixCode = new System.Windows.Forms.TextBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // prefixCode
      // 
      this.prefixCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.prefixCode.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.prefixCode.Location = new System.Drawing.Point(6, 16);
      this.prefixCode.Multiline = true;
      this.prefixCode.Name = "prefixCode";
      this.prefixCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.prefixCode.Size = new System.Drawing.Size(573, 111);
      this.prefixCode.TabIndex = 3;
      this.prefixCode.Validated += new System.EventHandler(this.prefixCode_Validated);
      // 
      // prefixLabel
      // 
      this.prefixLabel.AutoSize = true;
      this.prefixLabel.Location = new System.Drawing.Point(3, 0);
      this.prefixLabel.Name = "prefixLabel";
      this.prefixLabel.Size = new System.Drawing.Size(33, 13);
      this.prefixLabel.TabIndex = 4;
      this.prefixLabel.Text = "Prefix";
      // 
      // suffixLabel
      // 
      this.suffixLabel.AutoSize = true;
      this.suffixLabel.Location = new System.Drawing.Point(3, 0);
      this.suffixLabel.Name = "suffixLabel";
      this.suffixLabel.Size = new System.Drawing.Size(33, 13);
      this.suffixLabel.TabIndex = 5;
      this.suffixLabel.Text = "Suffix";
      // 
      // suffixCode
      // 
      this.suffixCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.suffixCode.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.suffixCode.Location = new System.Drawing.Point(6, 16);
      this.suffixCode.Multiline = true;
      this.suffixCode.Name = "suffixCode";
      this.suffixCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.suffixCode.Size = new System.Drawing.Size(573, 108);
      this.suffixCode.TabIndex = 6;
      this.suffixCode.Validated += new System.EventHandler(this.suffixCode_Validated);
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(6, 3);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.prefixCode);
      this.splitContainer.Panel1.Controls.Add(this.prefixLabel);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.suffixCode);
      this.splitContainer.Panel2.Controls.Add(this.suffixLabel);
      this.splitContainer.Size = new System.Drawing.Size(582, 261);
      this.splitContainer.SplitterDistance = 130;
      this.splitContainer.TabIndex = 7;
      // 
      // CodeNodeView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "CodeNodeView";
      this.Size = new System.Drawing.Size(591, 267);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox prefixCode;
    private System.Windows.Forms.Label prefixLabel;
    private System.Windows.Forms.Label suffixLabel;
    private System.Windows.Forms.TextBox suffixCode;
    private System.Windows.Forms.SplitContainer splitContainer;
  }
}
