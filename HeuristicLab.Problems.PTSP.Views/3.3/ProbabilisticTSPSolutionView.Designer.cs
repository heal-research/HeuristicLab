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

namespace HeuristicLab.Problems.PTSP.Views {
  partial class ProbabilisticTSPSolutionView {
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
      HeuristicLab.Problems.TravelingSalesman.Views.TSPVisualizer tspVisualizer1 = new HeuristicLab.Problems.TravelingSalesman.Views.TSPVisualizer();
      this.tspSolutionView = new HeuristicLab.Problems.TravelingSalesman.Views.TSPSolutionView();
      this.SuspendLayout();
      // 
      // tspSolutionView
      // 
      this.tspSolutionView.Caption = "TSP Solution View";
      this.tspSolutionView.Content = null;
      this.tspSolutionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tspSolutionView.Location = new System.Drawing.Point(0, 0);
      this.tspSolutionView.Name = "tspSolutionView";
      this.tspSolutionView.ReadOnly = false;
      this.tspSolutionView.Size = new System.Drawing.Size(704, 496);
      this.tspSolutionView.TabIndex = 0;
      tspVisualizer1.Coordinates = null;
      tspVisualizer1.Tour = null;
      this.tspSolutionView.Visualizer = tspVisualizer1;
      // 
      // ProbabilisticTSPSolutionView
      // 
      this.Controls.Add(this.tspSolutionView);
      this.Name = "ProbabilisticTSPSolutionView";
      this.Size = new System.Drawing.Size(704, 496);
      this.ResumeLayout(false);

    }
    #endregion

    private TravelingSalesman.Views.TSPSolutionView tspSolutionView;
  }
}
