#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using System.Windows.Forms;
namespace HeuristicLab.CEDMA.Server {
  partial class ProblemView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      this.importButton = new System.Windows.Forms.Button();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.datasetView = new HeuristicLab.DataAnalysis.DatasetView();
      this.SuspendLayout();
      // 
      // importButton
      // 
      this.importButton.Location = new System.Drawing.Point(3, 3);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(75, 23);
      this.importButton.TabIndex = 0;
      this.importButton.Text = "Import";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "txt";
      this.openFileDialog.FileName = "txt";
      this.openFileDialog.Filter = "Text files|*.txt|All files|*.*";
      this.openFileDialog.Title = "Import data set from file";
      // 
      // datasetView
      // 
      this.datasetView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.datasetView.Caption = "Editor";
      this.datasetView.Dataset = null;
      this.datasetView.Filename = null;
      this.datasetView.Location = new System.Drawing.Point(3, 32);
      this.datasetView.Name = "datasetView";
      this.datasetView.Size = new System.Drawing.Size(770, 580);
      this.datasetView.TabIndex = 36;
      // 
      // ProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.datasetView);
      this.Controls.Add(this.importButton);
      this.Name = "ProblemView";
      this.Size = new System.Drawing.Size(776, 615);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private HeuristicLab.DataAnalysis.DatasetView datasetView;
  }
}
