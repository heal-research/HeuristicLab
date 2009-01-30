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

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Enumerator of available file types for a plugin.
  /// </summary>
  public enum PluginFileType {
    Assembly,
    Executable,
    Data,
    License
  };

  /// <summary>
  /// Attribute for plugins providing information about their corresponding files.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
  public class PluginFileAttribute : System.Attribute {
    private string filename;

    /// <summary>
    /// Gets or sets the filename of the plugin.
    /// </summary>
    public string Filename {
      get { return filename; }
      set { filename = value; }
    }

    private PluginFileType filetype = PluginFileType.Data;

    /// <summary>
    /// Gets or sets the filetype of the plugin file.
    /// </summary>
    public PluginFileType Filetype {
      get { return filetype; }
      set { filetype = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PluginFileAttribute"/>.
    /// </summary>
    public PluginFileAttribute() { }
  }
}
