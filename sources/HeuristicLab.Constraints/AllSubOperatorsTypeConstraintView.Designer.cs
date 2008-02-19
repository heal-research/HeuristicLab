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

namespace HeuristicLab.Constraints {
  partial class AllSubOperatorsTypeConstraintView {
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
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.listView = new System.Windows.Forms.ListView();
      this.panel = new System.Windows.Forms.Panel();
      this.removeButton = new System.Windows.Forms.Button();
      this.groupBox.SuspendLayout();
      this.panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.listView);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(463, 285);
      this.groupBox.TabIndex = 0;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "Allowed sub-operators";
      // 
      // listView
      // 
      this.listView.AllowDrop = true;
      this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView.Location = new System.Drawing.Point(3, 16);
      this.listView.Name = "listView";
      this.listView.Size = new System.Drawing.Size(457, 266);
      this.listView.TabIndex = 1;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.List;
      this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
      this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_DragDrop);
      this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView_DragEnter);
      this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
      this.listView.DragOver += new System.Windows.Forms.DragEventHandler(this.listView_DragOver);
      // 
      // panel
      // 
      this.panel.Controls.Add(this.removeButton);
      this.panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel.Location = new System.Drawing.Point(0, 291);
      this.panel.Name = "panel";
      this.panel.Size = new System.Drawing.Size(463, 46);
      this.panel.TabIndex = 1;
      // 
      // removeButton
      // 
      this.removeButton.Location = new System.Drawing.Point(3, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(75, 23);
      this.removeButton.TabIndex = 0;
      this.removeButton.Text = "Remove";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // AllSubOperatorsTypeConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.panel);
      this.Controls.Add(this.groupBox);
      this.Name = "AllSubOperatorsTypeConstraintView";
      this.Size = new System.Drawing.Size(463, 337);
      this.groupBox.ResumeLayout(false);
      this.panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.Panel panel;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.ListView listView;

  }
}
