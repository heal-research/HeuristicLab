#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class RegressionSolutionResidualAnalysisView {
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
      this.bubbleChartView = new HeuristicLab.Optimization.Views.RunCollectionBubbleChartView();
      this.SuspendLayout();
      // 
      // bubbleChartView
      // 
      this.bubbleChartView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.bubbleChartView.BackColor = System.Drawing.SystemColors.Window;
      this.bubbleChartView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.bubbleChartView.Caption = "Bubble Chart";
      this.bubbleChartView.Content = null;
      this.bubbleChartView.Location = new System.Drawing.Point(3, 3);
      this.bubbleChartView.Name = "bubbleChartView";
      this.bubbleChartView.ReadOnly = false;
      this.bubbleChartView.Size = new System.Drawing.Size(855, 523);
      this.bubbleChartView.TabIndex = 0;
      // 
      // RegressionSolutionResidualAnalysisView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.bubbleChartView);
      this.Name = "RegressionSolutionResidualAnalysisView";
      this.Size = new System.Drawing.Size(861, 529);
      this.ResumeLayout(false);

    }

    #endregion

    private Optimization.Views.RunCollectionBubbleChartView bubbleChartView;
  }
}
