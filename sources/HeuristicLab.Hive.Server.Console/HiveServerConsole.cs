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
using System.Threading;

namespace HeuristicLab.Hive.Server.Console {

  public partial class HiveServerConsole : Form {


    public HiveServerConsole() {
      InitializeComponent();
      tbIp.Text = "010.020.053.006";
    }

    private void tsmiExit_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void btnLogin_Click(object sender, EventArgs e) {
      if (ipIsValid()) {
        this.Visible = false;
        // Form information = new HiveServerConsoleInformation();
        // Application.Run(information);
        this.Visible = true;
      }
    }

    private static bool ipIsValid() {
      return true;
    }
  }
}