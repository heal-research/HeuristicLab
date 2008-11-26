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
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Console {

  public delegate void closeForm(bool cf);

  public partial class HiveServerManagementConsole : Form {

    public event closeForm closeFormEvent;

    List<ClientGroup> clients = null;
    List<Job> jobs = null;
    List<UserGroup> userGroups = null;
    
    public HiveServerManagementConsole() {
      InitializeComponent();

      IClientManager clientManager =
        ServiceLocator.GetClientManager();

      IJobManager jobManager =
        ServiceLocator.GetJobManager();

      IUserRoleManager userRoleManager =
        ServiceLocator.GetUserRoleManager();

     // clients = clientManager.GetAllClientGroups();
      jobs = jobManager.GetAllJobs();
      userGroups = userRoleManager.GetAllUserGroups();
     // foreach (ClientGroup cg in clients) {
     //   tvClientControl.Nodes.Add(cg.Name);
        foreach (ClientInfo ci in clientManager.GetAllClients()) {
          tvClientControl.SelectedNode.Nodes.Add(ci.Name);
        }
     // }
      foreach (Job job in jobs) {
        tvJobControl.Nodes.Add(job.JobId.ToString());
      }
      foreach (UserGroup ug in userGroups) {
        tvUserControl.Nodes.Add(ug.UserGroupId.ToString());
      }

    }

    /// <summary>
    /// Send event to Login-GUI when closing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void close_Click(object sender, EventArgs e) {
      if (closeFormEvent != null) {
        closeFormEvent(true);
      }
      this.Close();
    }

    /// <summary>
    /// Send evnt to Login-GUI when closing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HiveServerConsoleInformation_FormClosing(object sender, FormClosingEventArgs e) {
      if (closeFormEvent != null) {
        closeFormEvent(true);
      }

    }
  }
}
