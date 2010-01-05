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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CEDMA.Server {
  [Application("CEDMA", "Cooperative Evolutionary Data Mining.", true)]
  class ServerApplication : ApplicationBase {
    public override void Run() {
      Server server = new Server();
      Form mainForm = new Form();
      UserControl serverControl = (UserControl)server.CreateView();
      serverControl.Dock = DockStyle.Fill;
      mainForm.Controls.Add(serverControl);
      mainForm.Text = "CEDMA";
      Application.Run(mainForm);
    }
  }
}
