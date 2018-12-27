#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Clients.Access;
using HeuristicLab.MainForm;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Clients.Hive.JobManager {
  public class JobManagerMenuItem : MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&Job Manager"; }
    }

    public override IEnumerable<string> Structure {
      get { return new string[] { "&Services", "&Hive" }; }
    }

    public override void Execute() {
      if (HiveRoles.CheckHiveUserPermissions()) {
        MainFormManager.MainForm.ShowContent(HiveClient.Instance);
      } else if (!UserInformation.Instance.UserExists) {
        MessageBox.Show(
          "Couldn't fetch user information from the server." + Environment.NewLine +
          "Please verify that you have an existing user and that your user name and password is correct.",
          "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
      } else {
        MessageBox.Show(
          "You do not seem to have the permissions to use the Hive Job Manager." + Environment.NewLine +
          "If that's not the case or you have any questions please write an email to support@heuristiclab.com",
          "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public override int Position {
      get { return 10000; }
    }

    public override Keys ShortCutKeys {
      get { return Keys.Control | Keys.H; }
    }
  }
}
