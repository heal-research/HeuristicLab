#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.MainForm.WindowsForms {
  partial class DockingMainForm {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
      WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
      WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
      WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
      WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
      WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
      WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
      WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
      WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
      WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
      WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
      WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
      WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
      WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
      WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
      this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
      this.SuspendLayout();
      // 
      // dockPanel
      // 
      this.dockPanel.ActiveAutoHideContent = null;
      this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dockPanel.DockBackColor = System.Drawing.SystemColors.Control;
      this.dockPanel.Location = new System.Drawing.Point(0, 49);
      this.dockPanel.Name = "dockPanel";
      this.dockPanel.Size = new System.Drawing.Size(624, 341);
      dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
      dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
      autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
      tabGradient1.EndColor = System.Drawing.SystemColors.Control;
      tabGradient1.StartColor = System.Drawing.SystemColors.Control;
      tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
      autoHideStripSkin1.TabGradient = tabGradient1;
      dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
      tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
      tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
      tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
      dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
      dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
      dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
      dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
      tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
      tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
      tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
      dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
      dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
      tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
      tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
      tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
      tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
      dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
      tabGradient5.EndColor = System.Drawing.SystemColors.Control;
      tabGradient5.StartColor = System.Drawing.SystemColors.Control;
      tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
      dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
      dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
      dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
      dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
      tabGradient6.EndColor = System.Drawing.SystemColors.GradientInactiveCaption;
      tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
      tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
      tabGradient6.TextColor = System.Drawing.SystemColors.ControlText;
      dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
      tabGradient7.EndColor = System.Drawing.Color.Transparent;
      tabGradient7.StartColor = System.Drawing.Color.Transparent;
      tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
      dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
      dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
      dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
      this.dockPanel.Skin = dockPanelSkin1;
      this.dockPanel.TabIndex = 3;
      this.dockPanel.ActiveContentChanged += new System.EventHandler(this.dockPanel_ActiveContentChanged);
      // 
      // DockingMainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(624, 412);
      this.Controls.Add(this.dockPanel);
      this.IsMdiContainer = true;
      this.Name = "DockingMainForm";
      this.Controls.SetChildIndex(this.dockPanel, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
  }
}