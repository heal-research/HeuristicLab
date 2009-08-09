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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HeuristicLab.MainForm {
  public partial class DockingMainForm : MainFormBase {
    public DockingMainForm(Type userInterfaceItemType)
      : base(userInterfaceItemType) {
      InitializeComponent();
      dockPanel = new DockPanel();
      InitializeDockPanel();
      this.IsMdiContainer = true;
      this.Controls.Add(dockPanel);
      this.Controls.SetChildIndex(dockPanel, 0);
    }

    private DockPanel dockPanel;

    public override void ShowView(IView view) {      
      if (InvokeRequired) Invoke((Action<IView>)ShowView, view);
      else {
        base.ShowView(view);
        DockContent dockForm = new DockForm(view);
        dockForm.Activated += new EventHandler(DockFormActivated);
        dockForm.FormClosing += new FormClosingEventHandler(view.FormClosing);
        dockForm.FormClosed += new FormClosedEventHandler(DockFormClosed);
        foreach (IToolStripItem item in ViewChangedToolStripItems)
          view.StateChanged += new EventHandler(item.ViewChanged);
        dockForm.Show(dockPanel);
      }
    }

    private void DockFormClosed(object sender, FormClosedEventArgs e) {
      DockForm dockForm = (DockForm)sender;
      views.Remove(dockForm.View);
      if (views.Count == 0)
        ActiveView = null;
      dockForm.Activated -= new EventHandler(DockFormActivated);
      dockForm.FormClosing -= new FormClosingEventHandler(dockForm.View.FormClosing);
      dockForm.FormClosed -= new FormClosedEventHandler(DockFormClosed);
      foreach (IToolStripItem item in ViewChangedToolStripItems)
        dockForm.View.StateChanged -= new EventHandler(item.ViewChanged);
    }

    private void DockFormActivated(object sender, EventArgs e) {
      base.ActiveView = ((DockForm)sender).View;
      base.StatusStripText = ((DockForm)sender).View.Caption;
    }

    private void InitializeDockPanel() {
      DockPanelSkin dockPanelSkin1 = new DockPanelSkin();
      AutoHideStripSkin autoHideStripSkin1 = new AutoHideStripSkin();
      DockPanelGradient dockPanelGradient1 = new DockPanelGradient();
      TabGradient tabGradient1 = new TabGradient();
      DockPaneStripSkin dockPaneStripSkin1 = new DockPaneStripSkin();
      DockPaneStripGradient dockPaneStripGradient1 = new DockPaneStripGradient();
      TabGradient tabGradient2 = new TabGradient();
      DockPanelGradient dockPanelGradient2 = new DockPanelGradient();
      TabGradient tabGradient3 = new TabGradient();
      DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new DockPaneStripToolWindowGradient();
      TabGradient tabGradient4 = new TabGradient();
      TabGradient tabGradient5 = new TabGradient();
      DockPanelGradient dockPanelGradient3 = new DockPanelGradient();
      TabGradient tabGradient6 = new TabGradient();
      TabGradient tabGradient7 = new TabGradient();

      this.dockPanel.ActiveAutoHideContent = null;
      this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dockPanel.DockBackColor = SystemColors.Control;
      this.dockPanel.DockBottomPortion = 0.33;
      this.dockPanel.DockLeftPortion = 0.33;
      this.dockPanel.DockRightPortion = 0.33;
      this.dockPanel.DockTopPortion = 0.33;
      this.dockPanel.Location = new Point(0, 49);
      this.dockPanel.Name = "dockPanel";
      this.dockPanel.RightToLeftLayout = true;
      dockPanelGradient1.EndColor = SystemColors.ControlLight;
      dockPanelGradient1.StartColor = SystemColors.ControlLight;
      autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
      tabGradient1.EndColor = SystemColors.Control;
      tabGradient1.StartColor = SystemColors.Control;
      tabGradient1.TextColor = SystemColors.ControlDarkDark;
      autoHideStripSkin1.TabGradient = tabGradient1;
      dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
      tabGradient2.EndColor = SystemColors.ControlLightLight;
      tabGradient2.StartColor = SystemColors.ControlLightLight;
      tabGradient2.TextColor = SystemColors.ControlText;
      dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
      dockPanelGradient2.EndColor = SystemColors.Control;
      dockPanelGradient2.StartColor = SystemColors.Control;
      dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
      tabGradient3.EndColor = SystemColors.ControlLight;
      tabGradient3.StartColor = SystemColors.ControlLight;
      tabGradient3.TextColor = SystemColors.ControlText;
      dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
      dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
      tabGradient4.EndColor = SystemColors.ActiveCaption;
      tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
      tabGradient4.StartColor = SystemColors.GradientActiveCaption;
      tabGradient4.TextColor = SystemColors.ActiveCaptionText;
      dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
      tabGradient5.EndColor = SystemColors.Control;
      tabGradient5.StartColor = SystemColors.Control;
      tabGradient5.TextColor = SystemColors.ControlText;
      dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
      dockPanelGradient3.EndColor = SystemColors.ControlLight;
      dockPanelGradient3.StartColor = SystemColors.ControlLight;
      dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
      tabGradient6.EndColor = SystemColors.GradientInactiveCaption;
      tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
      tabGradient6.StartColor = SystemColors.GradientInactiveCaption;
      tabGradient6.TextColor = SystemColors.ControlText;
      dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
      tabGradient7.EndColor = Color.Transparent;
      tabGradient7.StartColor = Color.Transparent;
      tabGradient7.TextColor = SystemColors.ControlDarkDark;
      dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
      dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
      dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
      this.dockPanel.Skin = dockPanelSkin1;
      this.dockPanel.TabIndex = 2;
    }
  }
}
