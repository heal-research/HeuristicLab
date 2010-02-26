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

    public OperatorShape()
      : base() {
      this.additionalConnectors = new List<IConnector>();
    }

    private List<IConnector> additionalConnectors;
    public IEnumerable<string> AdditionalConnectorNames {
      get { return this.additionalConnectors.Select(c => c.Name); }
    }

    private IConnector predecessor;
    public IConnector Predecessor {
      get { return this.predecessor; }
    }

    private IConnector successor;
    public IConnector Successor {
      get { return this.successor; }
    }

    private IConnector CreateConnector(string connectorName, Point location) {
      Connector connector = new Connector(location, this.Model);
      connector.ConnectorStyle = ConnectorStyle.Square;
      connector.Parent = this;
      connector.Name = connectorName;
      return connector;
    }



    public void AddConnector(string connectorName) {
      IConnector connector = this.CreateConnector(connectorName, this.BottomRightCorner);

      this.additionalConnectors.Add(connector);
      this.Connectors.Add(connector);
      this.UpdateConnectorLocation();
    }

    public void RemoveConnector(string connectorName) {
      IConnector connector = this.additionalConnectors.Where(c => c.Name == connectorName).FirstOrDefault();
      if (connector != null) {
        this.additionalConnectors.Remove(connector);
        this.Connectors.Remove(connector);
        this.UpdateConnectorLocation();
      }
    }

    private void UpdateConnectorLocation() {
      if (this.additionalConnectors.Count == 0)
        return;

      int spacing = this.Rectangle.Width / this.additionalConnectors.Count;
      int margin = spacing / 2;
      int posX = margin + this.Rectangle.X;
      for (int i = 0; i < this.additionalConnectors.Count; i++) {
        this.additionalConnectors[i].MoveBy(new Point(posX - this.additionalConnectors[i].Point.X, 0));
        posX += spacing;
      }
    }


    protected override void Initialize() {
      base.Initialize();

      #region Connectors
      this.Connectors.Clear();

      predecessor = this.CreateConnector("Predecessor", new Point(Rectangle.Left, Center.Y));
      Connectors.Add(predecessor);

      successor = this.CreateConnector("Successor", (new Point(Rectangle.Right, Center.Y)));
      Connectors.Add(successor);
      #endregion
    }
  }
}
