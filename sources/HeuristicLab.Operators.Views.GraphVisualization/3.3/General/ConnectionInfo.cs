#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [StorableClass]
  public class ConnectionInfo : DeepCloneable, IConnectionInfo {
    private ConnectionInfo() {
    }
    public ConnectionInfo(IShapeInfo from, string connectorFrom, IShapeInfo to, string connectorTo)
      : this() {
      if (from == to)
        throw new ArgumentException("Could not create connections between the same shape info.");
      if (!from.Connectors.Contains(connectorFrom) || !to.Connectors.Contains(connectorTo))
        throw new ArgumentException("Could not create connection between not existent connectors.");

      this.from = from;
      this.connectorFrom = connectorFrom;
      this.to = to;
      this.connectorTo = connectorTo;
    }

    [Storable]
    private IShapeInfo from;
    public IShapeInfo From {
      get { return from; }
    }

    [Storable]
    private string connectorFrom;
    public string ConnectorFrom {
      get { return this.connectorFrom; }
    }

    [Storable]
    private IShapeInfo to;
    public IShapeInfo To {
      get { return this.to; }
    }

    [Storable]
    private string connectorTo;
    public string ConnectorTo {
      get { return this.connectorTo; }
    }

    public event EventHandler Changed;
    protected virtual void OnChanged() {
      EventHandler handler = this.Changed;
      if (handler != null)
        this.Changed(this, EventArgs.Empty);
    }

    public override  IDeepCloneable Clone(Cloner cloner) {
      ConnectionInfo clone = (ConnectionInfo)base.Clone(cloner);
      clone.from = (IShapeInfo)cloner.Clone(this.from);
      clone.connectorFrom = this.ConnectorFrom;
      clone.to = (IShapeInfo)cloner.Clone(this.To);
      clone.connectorTo = this.ConnectorTo;
      return clone;
    }
  }
}
