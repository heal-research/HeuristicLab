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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Console {
  public partial class AgentListView : ViewBase {
    private ChooseItemDialog chooseItemDialog;

    public IAgentList AgentList {
      get { return (IAgentList)Item; }
      set { base.Item = value; }
    }

    public AgentListView() {
      InitializeComponent();
      Caption = "Agent View";
    }
    public AgentListView(IAgentList agentList)
      : this() {
      AgentList = agentList;
      agentList.Changed += new EventHandler(agentList_Changed);
    }

    void agentList_Changed(object sender, EventArgs e) {
      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      if(AgentList == null) {
        Caption = "Agents View";
        agentsListView.Enabled = false;
      } else {
        agentsListView.Enabled = true;
        agentsListView.Items.Clear();
        foreach(IAgent agent in AgentList) {
          ListViewItem item = new ListViewItem();
          item.Text = agent.Name;
          item.Tag = agent;
          agentsListView.Items.Add(item);
        }
      }
    }

    private void variablesListView_SelectedIndexChanged(object sender, EventArgs e) {
      if(detailsGroupBox.Controls.Count > 0)
        detailsGroupBox.Controls[0].Dispose();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      if(agentsListView.SelectedItems.Count == 1) {
        IAgent agent = (IAgent)agentsListView.SelectedItems[0].Tag;
        Control control = (Control)new AgentView(agent);
        detailsGroupBox.Controls.Add(control);
        control.Dock = DockStyle.Fill;
        detailsGroupBox.Enabled = true;
      }
    }

    #region Size Changed Events
    private void variablesListView_SizeChanged(object sender, EventArgs e) {
      if(agentsListView.Columns.Count > 0)
        agentsListView.Columns[0].Width = Math.Max(0, agentsListView.Width - 25);
    }
    #endregion

    #region Button Events
    private void addButton_Click(object sender, EventArgs e) {
      AgentList.CreateAgent();
      UpdateControls();
    }
    #endregion
  }
}

