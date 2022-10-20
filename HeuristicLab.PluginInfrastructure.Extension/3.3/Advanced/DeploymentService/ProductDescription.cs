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
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService {
  // extension of auto-generated DataContract class ProductDescription
  /// <summary>
  /// Product description as provided by the deployment service.
  /// A product has a name, a version and a list of plugins that are part of the product.
  /// </summary>
  public partial class ProductDescription {
    /// <summary>
    /// Initializes a new instance of <see cref="ProductDescription" />
    /// </summary>
    /// <param name="name">Name of the product</param>
    /// <param name="version">Version of the product</param>
    public ProductDescription(string name, Version version)
      : this(name, version, new List<PluginDescription>()) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ProductDescription" />
    /// </summary>
    /// <param name="name">Name of the product</param>
    /// <param name="version">Version of the product</param>
    /// <param name="plugins">Enumerable of plugins of the product</param>
    public ProductDescription(string name, Version version, IEnumerable<PluginDescription> plugins) {
      this.Name = name;
      this.Version = version;
      this.Plugins = plugins.ToArray();
    }
  }
}
