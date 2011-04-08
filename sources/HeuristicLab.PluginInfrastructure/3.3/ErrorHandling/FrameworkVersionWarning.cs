﻿#region License Information
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
using System;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure {
  public partial class FrameworkVersionWarning : Form {
    public FrameworkVersionWarning() {
      InitializeComponent();
    }

    private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      try {
        System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/en/details.aspx?displaylang=en&FamilyID=0a391abd-25c1-4fc0-919f-b21f31ab88b7");
        linkLabel.LinkVisited = true;
      }
      catch (Exception) { }
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      Application.Exit();
    }
  }
}