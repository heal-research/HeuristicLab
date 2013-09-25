#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;


namespace HeuristicLab.Clients.Hive.SlaveCore.SlaveTrayIcon {
  [RunInstaller(true)]
  public partial class TrayIconInstaller : System.Configuration.Install.Installer {

    public TrayIconInstaller() {
      InitializeComponent();
    }

    public override void Commit(IDictionary savedState) {
      base.Commit(savedState);
      //TODO: disable on quiet install (for admins)?
      Process.Start(Context.Parameters["TARGETDIR"].ToString() + "HeuristicLab.Clients.Hive.Slave.SlaveTrayIcon.exe");
    }
  }
}
