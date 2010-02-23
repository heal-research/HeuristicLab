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
using Netron.Diagramming.Core;
using System.Drawing;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  public class OperatorShape : ClassShape {

    private BidirectionalLookup<string, IConnector> additionalConnectors;
    public OperatorShape()
      : base() {
      this.additionalConnectors = new BidirectionalLookup<string,IConnector>();
    }

    private Connector predecessor;
    public Connector Predecessor {
      get { return this.predecessor; }
    }

    private Connector successor;
    public Connector Successor {
      get { return this.successor; }
    }


    public void AddConnector(string connectorName) {
      Connector connector = new Connector(new Point(Rectangle.Right, Rectangle.Bottom));
      connector.ConnectorStyle = ConnectorStyle.Square;
      connector.Parent = this;
      this.additionalConnectors.Add(connectorName, connector);
      Connectors.Add(connector);
    }

    public void RemoveConnector(string connectorName) {
      this.additionalConnectors.RemoveByFirst(connectorName);
      this.UpdateConnectorLocation();
    }

    private void UpdateConnectorLocation() { 
      //TODO set x position of connectors
    }


    protected override void Initialize() {
      base.Initialize();

      #region Connectors
      this.Connectors.Clear();

      predecessor = new Connector(new Point(Rectangle.Right, (int)(Rectangle.Top + Rectangle.Height / 2)));
      predecessor.ConnectorStyle = ConnectorStyle.Square;
      predecessor.Name = "Predecessor";
      predecessor.Parent = this;
      Connectors.Add(predecessor);

      successor = new Connector(new Point(Rectangle.Left, (int)(Rectangle.Top + Rectangle.Height / 2)));
      predecessor.ConnectorStyle = ConnectorStyle.Square;
      successor.Name = "Successor";
      successor.Parent = this;
      Connectors.Add(successor);
      #endregion
    }
  }
}
