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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.Core {
  public partial class AgentView : ViewBase {
    private IAgent agent;
    public IAgent Agent {
      get { return agent; }
      set { agent = value; }
    }

    public AgentView() {
      InitializeComponent();
      Caption = "Agent";
    }
    public AgentView(IAgent agent)
      : this() {
      Agent = agent;
      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      editorGroupBox.Controls.Clear();
      editorGroupBox.Enabled = false;
      if(Agent != null && Agent.OperatorGraph != null) {
        Control editor = (Control)Agent.OperatorGraph.CreateView();
        if(editor != null) {
          editorGroupBox.Controls.Add(editor);
          editor.Dock = DockStyle.Fill;
          editorGroupBox.Enabled = true;
        }

        if(Agent.Status == ProcessStatus.Unknown) activateButton.Enabled = true;
        else activateButton.Enabled = false;
      }
    }

    private void saveButton_Click(object sender, EventArgs e) {
      Agent.Save();
    }

    private void activateButton_Click(object sender, EventArgs e) {
      Agent.Start();
      activateButton.Enabled = false;
    }
  }
}
