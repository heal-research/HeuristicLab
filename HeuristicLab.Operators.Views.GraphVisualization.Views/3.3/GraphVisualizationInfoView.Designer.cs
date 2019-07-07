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

namespace HeuristicLab.Operators.Views.GraphVisualization.Views {
  partial class GraphVisualizationInfoView {
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
      this.components = new System.ComponentModel.Container();
      this.graphVisualization = new HeuristicLab.Netron.NetronVisualization();
      ((System.ComponentModel.ISupportInitialize)(this.graphVisualization)).BeginInit();
      this.SuspendLayout();
      // 
      // graphVisualization
      // 
      this.graphVisualization.AllowDrop = false;
      this.graphVisualization.BackColor = System.Drawing.Color.DarkGray;
      this.graphVisualization.BackgroundType = global::Netron.Diagramming.Core.CanvasBackgroundTypes.FlatColor;
      this.graphVisualization.Dock = System.Windows.Forms.DockStyle.Fill;
      this.graphVisualization.EnableAddConnection = true;
      this.graphVisualization.FileName = "";
      this.graphVisualization.Location = new System.Drawing.Point(0, 0);
      this.graphVisualization.Magnification = new System.Drawing.SizeF(80F, 80F);
      this.graphVisualization.Name = "graphVisualization";
      this.graphVisualization.Origin = new System.Drawing.Point(0, 0);
      this.graphVisualization.PageSize = new System.Drawing.Size(100000, 100000);
      this.graphVisualization.ShowConnectors = true;
      this.graphVisualization.ShowRulers = false;
      this.graphVisualization.Size = new System.Drawing.Size(665, 444);
      this.graphVisualization.TabIndex = 0;
      this.graphVisualization.OnEntityRemoved += new System.EventHandler<global::Netron.Diagramming.Core.EntityEventArgs>(this.graphVisualization_OnEntityRemoved);
      this.graphVisualization.OnEntityAdded += new System.EventHandler<global::Netron.Diagramming.Core.EntityEventArgs>(this.graphVisualization_OnEntityAdded);

      // 
      // GraphVisualizationInfoView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.graphVisualization);
      this.Name = "GraphVisualizationInfoView";
      this.Size = new System.Drawing.Size(665, 444);
      ((System.ComponentModel.ISupportInitialize)(this.graphVisualization)).EndInit();
      this.ResumeLayout(false);
    }
    #endregion

    private HeuristicLab.Netron.NetronVisualization graphVisualization;
  }
}
