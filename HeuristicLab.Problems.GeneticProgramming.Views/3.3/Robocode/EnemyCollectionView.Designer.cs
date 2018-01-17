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


namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  partial class EnemyCollectionView {
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
      this.reloadButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // reloadButton
      // 
      this.reloadButton.Location = new System.Drawing.Point(151, 3);
      this.reloadButton.Name = "reloadButton";
      this.reloadButton.Size = new System.Drawing.Size(24, 24);
      this.reloadButton.TabIndex = 1;
      this.reloadButton.UseVisualStyleBackColor = true;
      this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
      this.reloadButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      // 
      // EnemyCollectionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      splitContainer.Panel1.Controls.Add(this.reloadButton);
      this.Name = "EnemyCollectionView";
      this.Size = new System.Drawing.Size(440, 275);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button reloadButton;
  }
}
