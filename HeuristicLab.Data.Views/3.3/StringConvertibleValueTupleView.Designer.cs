#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Data.Views {
  public partial class StringConvertibleValueTupleView {
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
      this.Item1View = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.Item2View = new HeuristicLab.Data.Views.StringConvertibleValueView();
      this.Item1Label = new System.Windows.Forms.Label();
      this.Item2Label = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // Item1View
      // 
      this.Item1View.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.Item1View.Caption = "StringConvertibleValue View";
      this.Item1View.Content = null;
      this.Item1View.LabelVisible = false;
      this.Item1View.Location = new System.Drawing.Point(45, 0);
      this.Item1View.Name = "Item1View";
      this.Item1View.ReadOnly = false;
      this.Item1View.Size = new System.Drawing.Size(260, 20);
      this.Item1View.TabIndex = 0;
      // 
      // Item2View
      // 
      this.Item2View.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.Item2View.Caption = "StringConvertibleValue View";
      this.Item2View.Content = null;
      this.Item2View.LabelVisible = false;
      this.Item2View.Location = new System.Drawing.Point(45, 26);
      this.Item2View.Name = "Item2View";
      this.Item2View.ReadOnly = false;
      this.Item2View.Size = new System.Drawing.Size(260, 20);
      this.Item2View.TabIndex = 1;
      // 
      // Item1Label
      // 
      this.Item1Label.AutoSize = true;
      this.Item1Label.Location = new System.Drawing.Point(3, 3);
      this.Item1Label.Name = "Item1Label";
      this.Item1Label.Size = new System.Drawing.Size(36, 13);
      this.Item1Label.TabIndex = 2;
      this.Item1Label.Text = "Item1:";
      // 
      // Item2Label
      // 
      this.Item2Label.AutoSize = true;
      this.Item2Label.Location = new System.Drawing.Point(3, 29);
      this.Item2Label.Name = "Item2Label";
      this.Item2Label.Size = new System.Drawing.Size(36, 13);
      this.Item2Label.TabIndex = 3;
      this.Item2Label.Text = "Item2:";
      // 
      // StringConvertibleValueTupleView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.Item1Label);
      this.Controls.Add(this.Item2Label);
      this.Controls.Add(this.Item1View);
      this.Controls.Add(this.Item2View);
      this.Name = "StringConvertibleValueTupleView";
      this.Size = new System.Drawing.Size(308, 47);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected StringConvertibleValueView Item1View;
    protected StringConvertibleValueView Item2View;
    protected System.Windows.Forms.Label Item1Label;
    protected System.Windows.Forms.Label Item2Label;

  }
}
