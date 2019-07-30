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

namespace HeuristicLab.DebugEngine {
  partial class OperatorTraceView {
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
      this.operatorTraceGroupBox = new System.Windows.Forms.GroupBox();
      this.listView = new System.Windows.Forms.ListView();
      this.listViewColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.isEnabledCheckbox = new System.Windows.Forms.CheckBox();
      this.operatorTraceGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // operatorTraceGroupBox
      // 
      this.operatorTraceGroupBox.Controls.Add(this.isEnabledCheckbox);
      this.operatorTraceGroupBox.Controls.Add(this.listView);
      this.operatorTraceGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorTraceGroupBox.Location = new System.Drawing.Point(0, 0);
      this.operatorTraceGroupBox.Name = "operatorTraceGroupBox";
      this.operatorTraceGroupBox.Size = new System.Drawing.Size(152, 489);
      this.operatorTraceGroupBox.TabIndex = 0;
      this.operatorTraceGroupBox.TabStop = false;
      this.operatorTraceGroupBox.Text = "Operator Trace";
      // 
      // listView
      // 
      this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewColumnHeader});
      this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.listView.HideSelection = false;
      this.listView.Location = new System.Drawing.Point(6, 43);
      this.listView.Name = "listView";
      this.listView.ShowItemToolTips = true;
      this.listView.Size = new System.Drawing.Size(140, 440);
      this.listView.SmallImageList = this.imageList;
      this.listView.TabIndex = 0;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.Details;
      this.listView.ItemActivate += new System.EventHandler(this.listView_ItemActivate);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // isEnabledCheckbox
      // 
      this.isEnabledCheckbox.AutoSize = true;
      this.isEnabledCheckbox.Location = new System.Drawing.Point(7, 20);
      this.isEnabledCheckbox.Name = "isEnabledCheckbox";
      this.isEnabledCheckbox.Size = new System.Drawing.Size(65, 17);
      this.isEnabledCheckbox.TabIndex = 1;
      this.isEnabledCheckbox.Text = "Enabled";
      this.isEnabledCheckbox.UseVisualStyleBackColor = true;
      this.isEnabledCheckbox.CheckedChanged += new System.EventHandler(this.isEnabledCheckbox_CheckedChanged);
      // 
      // OperatorTraceView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.operatorTraceGroupBox);
      this.Name = "OperatorTraceView";
      this.Size = new System.Drawing.Size(152, 489);
      this.operatorTraceGroupBox.ResumeLayout(false);
      this.operatorTraceGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox operatorTraceGroupBox;
    private System.Windows.Forms.ListView listView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ColumnHeader listViewColumnHeader;
    private System.Windows.Forms.CheckBox isEnabledCheckbox;
  }
}
