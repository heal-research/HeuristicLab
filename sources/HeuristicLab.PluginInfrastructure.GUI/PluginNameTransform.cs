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
using ICSharpCode.SharpZipLib.Core;
using System.Diagnostics;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.GUI {
  /// <summary>
  /// This classes is needed to prune the directory tree of zipped plugin packages by removing the path leading to the plugin directory.
  /// </summary>
  class PluginNameTransform : INameTransform{

    private string pluginDir = Application.StartupPath + "\\" + HeuristicLab.PluginInfrastructure.Properties.Settings.Default.PluginDir + "\\";    
    #region INameTransform Members

    public string TransformDirectory(string name) {
      if(name.StartsWith(pluginDir)) {
      return name.Remove(0, pluginDir.Length);
      } else {
        return name;
      }
    }

    public string TransformFile(string name) {
      if(name.StartsWith(pluginDir)) {
        return name.Remove(0, pluginDir.Length);
      } else {
        return name;
      }
    }

    #endregion
  }
}
