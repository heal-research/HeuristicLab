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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class ShapeConstraintsView {
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.ConstraintsInputBaseLayout = new System.Windows.Forms.TableLayoutPanel();
            this.constraintsInput = new System.Windows.Forms.TextBox();
            this.parseBtn = new System.Windows.Forms.Button();
            this.infoTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.infoLabel = new System.Windows.Forms.Label();
            this.errorOutput = new System.Windows.Forms.Label();
            this.shapeConstraintsView = new HeuristicLab.Core.Views.CheckedItemListView<ShapeConstraint>();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.ConstraintsInputBaseLayout.SuspendLayout();
            this.infoTableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.shapeConstraintsView);
            this.splitContainer.Size = new System.Drawing.Size(888, 629);
            this.splitContainer.SplitterDistance = 296;
            this.splitContainer.TabIndex = 0;
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.ConstraintsInputBaseLayout);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(7);
            this.groupBox.Size = new System.Drawing.Size(296, 629);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Constraints Input";
            // 
            // ConstraintsInputBaseLayout
            // 
            this.ConstraintsInputBaseLayout.ColumnCount = 1;
            this.ConstraintsInputBaseLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ConstraintsInputBaseLayout.Controls.Add(this.constraintsInput, 0, 1);
            this.ConstraintsInputBaseLayout.Controls.Add(this.parseBtn, 0, 2);
            this.ConstraintsInputBaseLayout.Controls.Add(this.infoTableLayout, 0, 0);
            this.ConstraintsInputBaseLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConstraintsInputBaseLayout.Location = new System.Drawing.Point(7, 20);
            this.ConstraintsInputBaseLayout.Name = "ConstraintsInputBaseLayout";
            this.ConstraintsInputBaseLayout.RowCount = 3;
            this.ConstraintsInputBaseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ConstraintsInputBaseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ConstraintsInputBaseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ConstraintsInputBaseLayout.Size = new System.Drawing.Size(282, 602);
            this.ConstraintsInputBaseLayout.TabIndex = 6;
            // 
            // constraintsInput
            // 
            this.constraintsInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.constraintsInput.Location = new System.Drawing.Point(3, 16);
            this.constraintsInput.Multiline = true;
            this.constraintsInput.Name = "constraintsInput";
            this.constraintsInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.constraintsInput.Size = new System.Drawing.Size(276, 553);
            this.constraintsInput.TabIndex = 0;
            this.constraintsInput.TextChanged += new System.EventHandler(this.constraintsInput_TextChanged);
            // 
            // parseBtn
            // 
            this.parseBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parseBtn.Location = new System.Drawing.Point(3, 575);
            this.parseBtn.Name = "parseBtn";
            this.parseBtn.Size = new System.Drawing.Size(276, 24);
            this.parseBtn.TabIndex = 1;
            this.parseBtn.Text = "Parse Constraints";
            this.parseBtn.UseVisualStyleBackColor = true;
            this.parseBtn.Click += new System.EventHandler(this.parseBtn_Click);
            // 
            // infoTableLayout
            // 
            this.infoTableLayout.AutoSize = true;
            this.infoTableLayout.ColumnCount = 2;
            this.infoTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.infoTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.infoTableLayout.Controls.Add(this.infoLabel, 1, 0);
            this.infoTableLayout.Controls.Add(this.errorOutput, 0, 0);
            this.infoTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoTableLayout.Location = new System.Drawing.Point(0, 0);
            this.infoTableLayout.Margin = new System.Windows.Forms.Padding(0);
            this.infoTableLayout.Name = "infoTableLayout";
            this.infoTableLayout.RowCount = 1;
            this.infoTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.infoTableLayout.Size = new System.Drawing.Size(282, 13);
            this.infoTableLayout.TabIndex = 2;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.AutoSize = true;
            this.infoLabel.Image = global::HeuristicLab.Problems.DataAnalysis.Views.Properties.Resources.VS2008ImageLibrary_Annotations_Information;
            this.infoLabel.Location = new System.Drawing.Point(260, 0);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(19, 13);
            this.infoLabel.TabIndex = 4;
            this.infoLabel.Text = "    ";
            this.toolTip.SetToolTip(this.infoLabel, "Double-click to open description.");
            this.infoLabel.DoubleClick += new System.EventHandler(this.helpButton_DoubleClick);
            // 
            // errorOutput
            // 
            this.errorOutput.AutoSize = true;
            this.errorOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorOutput.Location = new System.Drawing.Point(3, 0);
            this.errorOutput.Name = "errorOutput";
            this.errorOutput.Size = new System.Drawing.Size(236, 13);
            this.errorOutput.TabIndex = 5;
            this.errorOutput.Text = "label1";
            // 
            // box1
            // 
            this.shapeConstraintsView.AutoSize = true;
            this.shapeConstraintsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shapeConstraintsView.Location = new System.Drawing.Point(0, 0);
            this.shapeConstraintsView.Name = "box1";
            this.shapeConstraintsView.Size = new System.Drawing.Size(588, 629);
            this.shapeConstraintsView.TabIndex = 2;
            this.shapeConstraintsView.TabStop = false;
            // 
            // ShapeConstraintsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "ShapeConstraintsView";
            this.Size = new System.Drawing.Size(888, 629);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.ConstraintsInputBaseLayout.ResumeLayout(false);
            this.ConstraintsInputBaseLayout.PerformLayout();
            this.infoTableLayout.ResumeLayout(false);
            this.infoTableLayout.PerformLayout();
            this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox groupBox;
    private HeuristicLab.Core.Views.CheckedItemListView<ShapeConstraint> shapeConstraintsView;
    private Label infoLabel;
    protected ToolTip toolTip;
    private TableLayoutPanel ConstraintsInputBaseLayout;
    private TextBox constraintsInput;
    private Button parseBtn;
    private Label errorOutput;
    private TableLayoutPanel infoTableLayout;
  }
}
