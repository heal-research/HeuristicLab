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
            this.constraintsInput = new System.Windows.Forms.TextBox();
            this.parseBtn = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.errorOutput = new System.Windows.Forms.Label();
            this.shapeConstraintsView = new HeuristicLab.Core.Views.CheckedItemListView<ShapeConstraint>();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.shapeConstraintsView);
            this.splitContainer.Size = new System.Drawing.Size(888, 629);
            this.splitContainer.SplitterDistance = 296;
            this.splitContainer.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox.Controls.Add(this.constraintsInput);
            this.groupBox.Controls.Add(this.parseBtn);
            this.groupBox.Controls.Add(this.infoLabel);
            this.groupBox.Controls.Add(this.errorOutput);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox1";
            this.groupBox.Size = new System.Drawing.Size(296, 629);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Constraints Input";
            // 
            // constraintsInput
            // 
            this.constraintsInput.AcceptsTab = true;
            this.constraintsInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.constraintsInput.Location = new System.Drawing.Point(3, 29);
            this.constraintsInput.Multiline = true;
            this.constraintsInput.Name = "constraintsInput";
            this.constraintsInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.constraintsInput.Size = new System.Drawing.Size(290, 561);
            this.constraintsInput.TabIndex = 2;
            this.constraintsInput.TextChanged += new System.EventHandler(this.constraintsInput_TextChanged);
            // 
            // parseBtn
            // 
            this.parseBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.parseBtn.Location = new System.Drawing.Point(3, 590);
            this.parseBtn.Name = "parseBtn";
            this.parseBtn.Size = new System.Drawing.Size(290, 23);
            this.parseBtn.TabIndex = 1;
            this.parseBtn.Text = "Parse Constraints";
            this.parseBtn.UseVisualStyleBackColor = true;
            this.parseBtn.Click += new System.EventHandler(this.parseBtn_Click);
            // 
            // label1
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.infoLabel.Image = global::HeuristicLab.Problems.DataAnalysis.Views.Properties.Resources.VS2008ImageLibrary_Annotations_Information;
            this.infoLabel.Location = new System.Drawing.Point(3, 16);
            this.infoLabel.Name = "label1";
            this.infoLabel.Size = new System.Drawing.Size(19, 13);
            this.infoLabel.TabIndex = 4;
            this.infoLabel.Text = "    ";
            this.toolTip.SetToolTip(this.infoLabel, "Double-click to open description.");
            this.infoLabel.DoubleClick += new System.EventHandler(this.helpButton_DoubleClick);
            // 
            // errorOutput
            // 
            this.errorOutput.AutoSize = true;
            this.errorOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.errorOutput.ForeColor = System.Drawing.Color.DarkRed;
            this.errorOutput.Location = new System.Drawing.Point(3, 613);
            this.errorOutput.MaximumSize = new System.Drawing.Size(350, 0);
            this.errorOutput.Name = "errorOutput";
            this.errorOutput.Size = new System.Drawing.Size(35, 13);
            this.errorOutput.TabIndex = 3;
            this.errorOutput.Text = "label2";
            // 
            // intervalConstraintsView
            // 
            this.shapeConstraintsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shapeConstraintsView.Location = new System.Drawing.Point(0, 0);
            this.shapeConstraintsView.Name = "intervalConstraintsView";
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.Button parseBtn;
    private HeuristicLab.Core.Views.CheckedItemListView<ShapeConstraint> shapeConstraintsView;
    private System.Windows.Forms.TextBox constraintsInput;
    private Label errorOutput;
    private Label infoLabel;
    protected ToolTip toolTip;
    }
}
