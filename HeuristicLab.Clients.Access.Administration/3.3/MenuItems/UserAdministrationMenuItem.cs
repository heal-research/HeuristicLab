#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Optimizer;

namespace HeuristicLab.Clients.Access.Administration {
  public class UserAdministrationMenuItem : HeuristicLab.MainForm.WindowsForms.MenuItem, IOptimizerUserInterfaceItemProvider {
    public override string Name {
      get { return "&User Administration"; }
    }
    public override IEnumerable<string> Structure {
      get { return new string[] { "&Services", "&Access" }; }
    }
    public override void Execute() {
      if (!UserInformation.Instance.UserExists) {
        MessageBox.Show("Couldn't fetch user information from the server." + Environment.NewLine + "Please verify that you have an existing user and that your user name and password is correct. ", "HeuristicLab Access Service", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      if (UserInformation.Instance.User.Roles.Where(x => x.Name == UserInformation.AdministratorRoleName).Count() > 0) {
        using (UserAdministrationDialog dialog = new UserAdministrationDialog()) {
          dialog.ShowDialog();
        }
      } else {
        MessageBox.Show("You do not seem to have the permissions to use the Access Service Administrator." + Environment.NewLine +
        "If that's not the case or you have any questions please write an email to support@heuristiclab.com",
        "Access Service Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    public override int Position {
      get { return 10000; }
    }
  }
}
