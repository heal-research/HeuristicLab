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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.PluginAdministrator {
  internal class MainForm : DockingMainForm {
    private System.Windows.Forms.ToolStripProgressBar progressBar;

    public MainForm(Type type)
      : base(type) {
      InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      (new PluginEditor()).Show();
    }

    private void InitializeComponent() {
      this.SuspendLayout();

      progressBar = new System.Windows.Forms.ToolStripProgressBar();
      progressBar.MarqueeAnimationSpeed = 30;
      progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      progressBar.Visible = false;
      statusStrip.Items.Add(progressBar);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.ClientSize = new System.Drawing.Size(770, 550);
      this.Name = "MainForm";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public void ShowProgressBar() {
      progressBar.Visible = true;
    }

    public void HideProgressBar() {
      progressBar.Visible = false;
    }
  }
}
