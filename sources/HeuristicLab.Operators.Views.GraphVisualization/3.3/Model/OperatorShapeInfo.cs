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
using HeuristicLab.Core;
using System.Drawing;
using Netron.Diagramming.Core;
using System.Windows.Forms;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  internal class OperatorShapeInfo : ShapeInfo {

    private List<string> connectorNames;
    public OperatorShapeInfo()
      : base(typeof(OperatorShape)) {
      this.connectorNames = new List<string>();
    }

    public OperatorShapeInfo(IEnumerable<string> connectorNames)
      : this() {
      foreach (string connectorName in connectorNames)
        this.connectorNames.Add(connectorName);
    }

    public override void AddConnector(string connectorName) {
      if (!this.connectorNames.Contains(connectorName) && connectorName != "Successor") {
        this.connectorNames.Add(connectorName);
        this.OnChanged();
      }
    }

    public override void RemoveConnector(string connectorName) {
      if (this.connectorNames.Contains(connectorName)) {
        this.connectorNames.Remove(connectorName);
        if (this.connections.ContainsKey(connectorName))
          this.connections.Remove(connectorName);
        this.OnChanged();
      }
    }

    public override void AddConnection(string fromConnectorName, IShapeInfo toShapeInfo) {
      this.connections.Add(fromConnectorName, toShapeInfo);
    }

    public override void RemoveConnection(string fromConnectorName) {
      if (this.connections.ContainsKey(fromConnectorName))
        this.connections.Remove(fromConnectorName);
    }

    public override void ChangeConnection(string fromConnectorName, IShapeInfo toShapeInfo) {
      this.connections[fromConnectorName] = toShapeInfo;
    }

    private string title;
    public string Title {
      get { return this.title; }
      set {
        if (this.title != value) {
          this.title = value;
          this.OnChanged();
        }
      }
    }

    private string text;
    public string Text {
      get { return this.text; }
      set {
        if (this.text != value) {
          this.text = value;
          this.OnChanged();
        }
      }
    }

    private Color headColor;
    public Color HeadColor {
      get { return this.headColor; }
      set {
        if (this.headColor != value) {
          this.headColor = value;
          this.OnChanged();
        }
      }
    }

    public override IShape CreateShape() {
      OperatorShape shape = (OperatorShape)base.CreateShape();
      shape.Title = this.Title;
      shape.SubTitle = this.Text;
      shape.HeadColor = this.HeadColor;
      foreach (string connectorName in this.connectorNames)
        shape.AddConnector(connectorName);
      return shape;
    }

    public override void UpdateShape(IShape shape) {
      base.UpdateShape(shape);
      OperatorShape operatorShape = shape as OperatorShape;

      if (operatorShape != null) {
        operatorShape.Title = this.Title;
        operatorShape.SubTitle = this.Text;
        operatorShape.HeadColor = this.HeadColor;

        int i = 0;
        int j = 0;
        //remove old connectors and skip correct connectors
        List<string> oldConnectorNames = operatorShape.AdditionalConnectorNames.ToList();
        while (i < this.connectorNames.Count && j < oldConnectorNames.Count) {
          if (this.connectorNames[i] != oldConnectorNames[j]) {
            operatorShape.RemoveConnector(oldConnectorNames[j]);
          } else
            i++;
          j++;
        }
        //remove old connectors
        for (; j < oldConnectorNames.Count; i++)
          operatorShape.RemoveConnector(oldConnectorNames[j]);

        //add new connectors
        for (; i < this.connectorNames.Count; i++)
          operatorShape.AddConnector(this.connectorNames[i]);
      }
    }
  }
}
