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
using System.Diagnostics;

namespace HeuristicLab.PluginInfrastructure.GUI {
  [Flags]
  enum ManagerAction {
    Install = 1,
    Remove = 2,
    None = 4,
    Any = Install | Remove
  };

  class PluginAction {
    private ManagerAction action;

    public ManagerAction Action {
      get { return action; }
      set { action = value; }
    }

    private List<PluginTag> hull;

    public List<PluginTag> Hull {
      get { return hull; }
      set { hull = value; }
    }

    private PluginTag plugin;

    public PluginTag Plugin {
      get { return plugin; }
      set { plugin = value; }
    }

    public string GetInverseActionString() {
      switch(action) {
        case ManagerAction.Install:
          return "Don't install";
        case ManagerAction.Remove:
          return "Keep";
        default:
          // this method is not defined for actions other than "Install" and "Remove"
          Trace.Assert(false);
          return "";
      }
    }
  }
}
