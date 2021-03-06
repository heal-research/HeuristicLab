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

using System;

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionModifiersListView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.evaluateButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      //
      // splitContainer.Panel1
      //
      this.splitContainer.Panel1.Controls.Add(this.evaluateButton);
      //
      // showDetailsCheckBox 
      //
      this.showDetailsCheckBox.Location = new System.Drawing.Point(153, 3);
      this.showDetailsCheckBox.TabIndex = 5;
      //
      // evaluateButton
      //
      this.evaluateButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.evaluateButton.Location = new System.Drawing.Point(123, 3);
      this.evaluateButton.Name = "evaluateButton";
      this.evaluateButton.Size = new System.Drawing.Size(24, 24);
      this.evaluateButton.TabIndex = 4;
      this.toolTip.SetToolTip(this.evaluateButton, "Evaluate");
      this.evaluateButton.UseVisualStyleBackColor = true;
      this.evaluateButton.Click += new System.EventHandler(this.evaluateButton_Click);
      //
      // itemListView
      //
      this.itemsListView.TabIndex = 6;
      this.ResumeLayout(false);
    }

    protected System.Windows.Forms.Button evaluateButton;

    #endregion
  }
}
