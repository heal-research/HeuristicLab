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
      this.components = new System.ComponentModel.Container();
      this.problemInstanceSplitContainer = new System.Windows.Forms.SplitContainer();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.ContentPanel = new System.Windows.Forms.Panel();
      this.namedItemView = new HeuristicLab.Core.Views.NamedItemView();
      this.problemInstanceProvidersControl = new HeuristicLab.Optimization.Views.ProblemInstanceProvidersControl();
      this.resultsProducingItemView = new HeuristicLab.Optimization.Views.ResultsProducingItemView();
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).BeginInit();
      this.problemInstanceSplitContainer.Panel1.SuspendLayout();
      this.problemInstanceSplitContainer.Panel2.SuspendLayout();
      this.problemInstanceSplitContainer.SuspendLayout();
      this.ContentPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // problemInstanceSplitContainer
      // 
      this.problemInstanceSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.problemInstanceSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.problemInstanceSplitContainer.IsSplitterFixed = true;
      this.problemInstanceSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.problemInstanceSplitContainer.Name = "problemInstanceSplitContainer";
      this.problemInstanceSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // problemInstanceSplitContainer.Panel1
      // 
      this.problemInstanceSplitContainer.Panel1.Controls.Add(this.problemInstanceProvidersControl);
      this.problemInstanceSplitContainer.Panel1MinSize = 10;
      // 
      // problemInstanceSplitContainer.Panel2
      // 
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.namedItemView);
      this.problemInstanceSplitContainer.Panel2.Controls.Add(this.ContentPanel);
      this.problemInstanceSplitContainer.Size = new System.Drawing.Size(960, 709);
      this.problemInstanceSplitContainer.SplitterDistance = 26;
      this.problemInstanceSplitContainer.TabIndex = 13;
      // 
      // ContentPanel
      // 
      this.ContentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ContentPanel.Controls.Add(this.resultsProducingItemView);
      this.ContentPanel.Location = new System.Drawing.Point(0, 33);
      this.ContentPanel.Name = "ContentPanel";
      this.ContentPanel.Size = new System.Drawing.Size(960, 646);
      this.ContentPanel.TabIndex = 2;
      // 
      // namedItemView
      // 
      this.namedItemView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.namedItemView.Caption = "NamedItem View";
      this.namedItemView.Content = null;
      this.namedItemView.Location = new System.Drawing.Point(0, 3);
      this.namedItemView.Name = "namedItemView";
      this.namedItemView.ReadOnly = false;
      this.namedItemView.Size = new System.Drawing.Size(960, 21);
      this.namedItemView.TabIndex = 1;
      // 
      // problemInstanceProvidersControl
      // 
      this.problemInstanceProvidersControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.problemInstanceProvidersControl.Consumer = null;
      this.problemInstanceProvidersControl.Location = new System.Drawing.Point(0, 0);
      this.problemInstanceProvidersControl.Margin = new System.Windows.Forms.Padding(0);
      this.problemInstanceProvidersControl.Name = "problemInstanceProvidersControl";
      this.problemInstanceProvidersControl.Size = new System.Drawing.Size(960, 26);
      this.problemInstanceProvidersControl.TabIndex = 0;
      // 
      // resultsProducingItemView
      // 
      this.resultsProducingItemView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resultsProducingItemView.Caption = "Results Producing Item View";
      this.resultsProducingItemView.Content = null;
      this.resultsProducingItemView.Location = new System.Drawing.Point(3, 3);
      this.resultsProducingItemView.Name = "resultsProducingItemView";
      this.resultsProducingItemView.ReadOnly = false;
      this.resultsProducingItemView.Size = new System.Drawing.Size(954, 640);
      this.resultsProducingItemView.TabIndex = 0;
      // 
      // ProblemView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.problemInstanceSplitContainer);
      this.Name = "ProblemView";
      this.Size = new System.Drawing.Size(960, 709);
      this.problemInstanceSplitContainer.Panel1.ResumeLayout(false);
      this.problemInstanceSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.problemInstanceSplitContainer)).EndInit();
      this.problemInstanceSplitContainer.ResumeLayout(false);
      this.ContentPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected HeuristicLab.Optimization.Views.ResultsProducingItemView resultsProducingItemView;
    protected System.Windows.Forms.SplitContainer problemInstanceSplitContainer;
    private System.Windows.Forms.ToolTip toolTip;
    private ProblemInstanceProvidersControl problemInstanceProvidersControl;
    private Core.Views.NamedItemView namedItemView;
    private System.Windows.Forms.Panel ContentPanel;
  }
}
