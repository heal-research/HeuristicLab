using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.Core {
  /// <summary>
  /// Holds additional information about a certain permission.
  /// </summary>
  public class PermissionContext {
    private int _priority;
    /// <summary>
    /// Gets or sets the priority of this permission rule.
    /// </summary>
    public int Priority { get { return this._priority; } set { this._priority = value; } }

    private string _elevation;
    /// <summary>
    /// Gets or sets the Rights Elevation Information. (Primary Policy Context)
    /// </summary>
    public string Elevation { get { return this._elevation; } set { this._elevation = value; } }

  }
}
