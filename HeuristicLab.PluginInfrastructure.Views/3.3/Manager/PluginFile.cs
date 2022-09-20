#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.PluginInfrastructure.Manager {
  /// <summary>
  /// Plugin files have a name and a type.
  /// </summary>
  [Serializable]
  public sealed class PluginFile : IPluginFile {
    #region IPluginFile Members

    private string name;
    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string Name {
      get { return name; }
    }

    private PluginFileType type;
    /// <summary>
    /// Gets the type of the file.
    /// </summary>
    public PluginFileType Type {
      get { return type; }
    }

    #endregion

    /// <summary>
    /// Inizialize a new instance of <see cref="PluginFile" />
    /// </summary>
    /// <param name="name">File name</param>
    /// <param name="type">File type</param>
    public PluginFile(string name, PluginFileType type) {
      this.name = name;
      this.type = type;
    }

    /// <summary>
    /// ToString override for <see cref="PluginFile" />
    /// </summary>
    /// <returns>Plugin file name</returns>
    public override string ToString() {
      return name;
    }
  }
}
