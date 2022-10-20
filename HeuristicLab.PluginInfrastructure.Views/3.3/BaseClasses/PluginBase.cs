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


namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Abstract base implementation of the IPlugin interface.
  /// </summary>
  public abstract class PluginBase : IPlugin {
    /// <summary>
    /// Initializes a new instance of <see cref="PluginBase"/>.
    /// </summary>
    protected PluginBase() { }

    private PluginAttribute PluginAttribute {
      get {
        object[] pluginAttributes = this.GetType().GetCustomAttributes(typeof(PluginAttribute), false);
        // exactly one attribute of the type PluginDescriptionAttribute must be given
        if (pluginAttributes.Length == 0) {
          throw new InvalidPluginException("PluginAttribute on type " + this.GetType() + " is missing.");
        } else if (pluginAttributes.Length > 1) {
          throw new InvalidPluginException("Found multiple PluginAttributes on type " + this.GetType());
        }
        return (PluginAttribute)pluginAttributes[0];
      }
    }

    #region IPlugin Members
    /// <inheritdoc />
    public string Name {
      get {
        return PluginAttribute.Name;
      }
    }

    /// <inhertitdoc />
    public virtual void OnLoad() { }
    /// <inhertitdoc />
    public virtual void OnUnload() { }
    #endregion

  }
}
