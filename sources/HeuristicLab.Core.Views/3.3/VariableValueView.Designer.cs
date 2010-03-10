#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Core.Views {
  partial class VariableValueView {
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
      this.valuePanel = new System.Windows.Forms.Panel();
      this.viewHost = new HeuristicLab.Core.Views.ViewHost();
      this.valuePanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // valuePanel
      // 
      this.valuePanel.AllowDrop = true;
      this.valuePanel.Controls.Add(this.viewHost);
      this.valuePanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.valuePanel.Location = new System.Drawing.Point(0, 0);
      this.valuePanel.Name = "valuePanel";
      this.valuePanel.Size = new System.Drawing.Size(359, 274);
      this.valuePanel.TabIndex = 0;
      this.valuePanel.DragOver += new System.Windows.Forms.DragEventHandler(this.valuePanel_DragEnterOver);
      this.valuePanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.valuePanel_DragDrop);
      this.valuePanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.valuePanel_DragEnterOver);
      // 
      // viewHost
      // 
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Location = new System.Drawing.Point(0, 0);
      this.viewHost.Name = "viewHost";
      this.viewHost.Size = new System.Drawing.Size(359, 274);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewType = null;
      // 
      // VariableValueView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.valuePanel);
      this.Name = "VariableValueView";
      this.Size = new System.Drawing.Size(359, 274);
      this.valuePanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected ViewHost viewHost;
    protected System.Windows.Forms.Panel valuePanel;
  }
}
