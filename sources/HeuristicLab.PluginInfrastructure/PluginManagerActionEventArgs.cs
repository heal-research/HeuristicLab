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
  /// Enum for all possible actions the plugin manager can execute.
  /// </summary>
  public enum PluginManagerAction { Initializing, InitializingPlugin, InitializedPlugin, Initialized, Starting }
  /// <summary>
  /// Event handler for all action events of the plugin manager.
  /// </summary>
  /// <param name="sender">The sender of the action event.</param>
  /// <param name="e">The event arguments.</param>
  public delegate void PluginManagerActionEventHandler(object sender, PluginManagerActionEventArgs e);

  // this class must be serializable because EventArgs are transmitted over AppDomain boundaries
  /// <summary>
  /// Class for the event arguments of plugin manager action events.
  /// </summary>
  [Serializable]
  public class PluginManagerActionEventArgs {
    private PluginManagerAction action;
    /// <summary>
    /// Gets or sets the action that has been performed.
    /// </summary>
    public PluginManagerAction Action {
      get { return action; }
      set { this.action = value; }
    }
    private string id;
    /// <summary>
    /// Gets or sets the id of the action event arguments.
    /// </summary>
    public string Id {
      get { return id; }
      set { id = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PluginManagerActionEventArgs"/> with the given 
    /// <paramref name="id"/> and <paramref name="action"/>.
    /// </summary>
    /// <param name="id">The id of the action event arguments.</param>
    /// <param name="action">The action of the plugin manager.</param>
    public PluginManagerActionEventArgs(string id, PluginManagerAction action) {
      this.Id = id;
      this.Action = action;
    }
  }
}
