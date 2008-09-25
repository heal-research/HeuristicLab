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

namespace HeuristicLab.SimOpt {
  partial class SimOptProblemInjectorView {
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
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.dataTypeTreeView = new System.Windows.Forms.TreeView();
      this.label1 = new System.Windows.Forms.Label();
      this.geneNameStringDataView = new HeuristicLab.Data.StringDataView();
      this.parametersTabControl = new System.Windows.Forms.TabControl();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.objectParameterDataView = new HeuristicLab.Data.ConstrainedItemListView();
      this.variableNameLabel = new System.Windows.Forms.Label();
      this.maximizationBoolDataView = new HeuristicLab.Data.BoolDataView();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.parametersTabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.dataTypeTreeView);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.label1);
      this.splitContainer1.Panel2.Controls.Add(this.geneNameStringDataView);
      this.splitContainer1.Panel2.Controls.Add(this.parametersTabControl);
      this.splitContainer1.Panel2.Controls.Add(this.variableNameLabel);
      this.splitContainer1.Panel2.Controls.Add(this.maximizationBoolDataView);
      this.splitContainer1.Size = new System.Drawing.Size(552, 439);
      this.splitContainer1.SplitterDistance = 184;
      this.splitContainer1.TabIndex = 0;
      // 
      // dataTypeTreeView
      // 
      this.dataTypeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataTypeTreeView.Location = new System.Drawing.Point(0, 0);
      this.dataTypeTreeView.Name = "dataTypeTreeView";
      this.dataTypeTreeView.Size = new System.Drawing.Size(184, 439);
      this.dataTypeTreeView.TabIndex = 0;
      this.dataTypeTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.dataTypeTreeView_ItemDrag);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(245, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(67, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Maximization";
      // 
      // geneNameStringDataView
      // 
      this.geneNameStringDataView.Caption = "View";
      this.geneNameStringDataView.Location = new System.Drawing.Point(47, 7);
      this.geneNameStringDataView.Name = "geneNameStringDataView";
      this.geneNameStringDataView.Size = new System.Drawing.Size(163, 19);
      this.geneNameStringDataView.StringData = null;
      this.geneNameStringDataView.TabIndex = 5;
      // 
      // parametersTabControl
      // 
      this.parametersTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersTabControl.Controls.Add(this.parametersTabPage);
      this.parametersTabControl.Location = new System.Drawing.Point(6, 32);
      this.parametersTabControl.Name = "parametersTabControl";
      this.parametersTabControl.SelectedIndex = 0;
      this.parametersTabControl.Size = new System.Drawing.Size(358, 407);
      this.parametersTabControl.TabIndex = 4;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.objectParameterDataView);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(350, 381);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // objectParameterDataView
      // 
      this.objectParameterDataView.AllowDrop = true;
      this.objectParameterDataView.Caption = "View";
      this.objectParameterDataView.ConstrainedItemList = null;
      this.objectParameterDataView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.objectParameterDataView.Location = new System.Drawing.Point(3, 3);
      this.objectParameterDataView.Name = "objectParameterDataView";
      this.objectParameterDataView.Size = new System.Drawing.Size(344, 375);
      this.objectParameterDataView.TabIndex = 1;
      this.objectParameterDataView.DragDrop += new System.Windows.Forms.DragEventHandler(this.objectParameterDataView_DragDrop);
      this.objectParameterDataView.DragEnter += new System.Windows.Forms.DragEventHandler(this.objectParameterDataView_DragEnter);
      // 
      // variableNameLabel
      // 
      this.variableNameLabel.AutoSize = true;
      this.variableNameLabel.Location = new System.Drawing.Point(3, 9);
      this.variableNameLabel.Name = "variableNameLabel";
      this.variableNameLabel.Size = new System.Drawing.Size(38, 13);
      this.variableNameLabel.TabIndex = 2;
      this.variableNameLabel.Text = "Name:";
      // 
      // maximizationBoolDataView
      // 
      this.maximizationBoolDataView.BoolData = null;
      this.maximizationBoolDataView.Caption = "Maximization";
      this.maximizationBoolDataView.Location = new System.Drawing.Point(225, 9);
      this.maximizationBoolDataView.Name = "maximizationBoolDataView";
      this.maximizationBoolDataView.Size = new System.Drawing.Size(19, 18);
      this.maximizationBoolDataView.TabIndex = 6;
      // 
      // SimOptProblemInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer1);
      this.Name = "SimOptProblemInjectorView";
      this.Size = new System.Drawing.Size(552, 439);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      this.splitContainer1.ResumeLayout(false);
      this.parametersTabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private HeuristicLab.Data.ConstrainedItemListView objectParameterDataView;
    private System.Windows.Forms.TreeView dataTypeTreeView;
    private System.Windows.Forms.Label variableNameLabel;
    private System.Windows.Forms.TabControl parametersTabControl;
    private System.Windows.Forms.TabPage parametersTabPage;
    private HeuristicLab.Data.StringDataView geneNameStringDataView;
    private HeuristicLab.Data.BoolDataView maximizationBoolDataView;
    private System.Windows.Forms.Label label1;
  }
}
