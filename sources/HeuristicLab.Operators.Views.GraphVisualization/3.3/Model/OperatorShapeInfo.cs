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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  internal class OperatorShapeInfo : ShapeInfo, IOperatorShapeInfo {
    [Storable]
    private List<string> connectorNames;
    [Storable]
    private List<string> labels;

    public OperatorShapeInfo()
      : base(typeof(OperatorShape)) {
      this.connectorNames = new List<string>();
      this.labels = new List<string>();
    }

    public OperatorShapeInfo(IEnumerable<string> connectorNames)
      : this() {
      foreach (string connectorName in connectorNames)
        this.connectorNames.Add(connectorName);
    }

    public OperatorShapeInfo(IEnumerable<string> connectorNames, IEnumerable<string> labels)
      : this(connectorNames) {
      foreach (string label in labels)
        this.labels.Add(label);
    }

    public void AddConnector(string connectorName) {
      if (!this.connectorNames.Contains(connectorName) && connectorName != "Successor") {
        this.connectorNames.Add(connectorName);
        this.OnChanged();
      }
    }

    public void RemoveConnector(string connectorName) {
      if (this.connectorNames.Contains(connectorName)) {
        this.connectorNames.Remove(connectorName);
        this.OnChanged();
      }
    }

    public void UpdateLabels(IEnumerable<string> labels) {
      this.labels = new List<string>(labels);
      this.OnChanged();
    }

    [Storable]
    private bool collapsed;
    public bool Collapsed {
      get { return this.collapsed; }
      set {
        if (this.collapsed != value) {
          this.collapsed = value;
          this.OnChanged();
        }
      }
    }

    [Storable]
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

    [Storable]
    private Color color;
    public Color Color {
      get { return this.color; }
      set {
        if (this.color != value) {
          this.color = value;
          this.OnChanged();
        }
      }
    }

    [Storable]
    private Color lineColor;
    public Color LineColor {
      get { return this.lineColor; }
      set {
        if (this.lineColor != value) {
          this.lineColor = value;
          this.OnChanged();
        }
      }
    }

    [Storable]
    private float lineWidth;
    public float LineWidth {
      get { return this.lineWidth; }
      set {
        if (this.lineWidth != value) {
          this.lineWidth = value;
          this.OnChanged();
        }
      }
    }

    private Bitmap icon;
    public Bitmap Icon {
      get { return this.icon; }
      set {
        if (this.icon != value) {
          this.icon = value;
          this.OnChanged();
        }
      }
    }

    public override IShape CreateShape() {
      OperatorShape shape = (OperatorShape)base.CreateShape();
      shape.Title = this.Title;
      shape.Color = this.Color;
      shape.LineColor = this.LineColor;
      shape.LineWidth = this.LineWidth;
      shape.Icon = this.Icon;
      shape.Collapsed = this.Collapsed;
      foreach (string connectorName in this.connectorNames)
        shape.AddConnector(connectorName);

      shape.UpdateLabels(this.labels);
      return shape;
    }

    public override void UpdateShape(IShape shape) {
      base.UpdateShape(shape);
      OperatorShape operatorShape = shape as OperatorShape;
      if (operatorShape != null) {
        operatorShape.Title = this.Title;
        operatorShape.Color = this.Color;
        operatorShape.LineColor = this.LineColor;
        operatorShape.LineWidth = this.LineWidth;
        operatorShape.Collapsed = this.Collapsed;

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
        //remove remaining old connectors
        for (; j < oldConnectorNames.Count; j++)
          operatorShape.RemoveConnector(oldConnectorNames[j]);

        //add new connectors
        for (; i < this.connectorNames.Count; i++)
          operatorShape.AddConnector(this.connectorNames[i]);

        operatorShape.UpdateLabels(this.labels);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      OperatorShapeInfo clone = (OperatorShapeInfo) base.Clone(cloner);
      clone.collapsed = this.collapsed;
      clone.color = this.color;
      clone.lineColor = this.lineColor;
      clone.lineWidth = this.lineWidth;
      clone.title = this.title;
      clone.icon = (Bitmap) this.icon.Clone();

      clone.connectorNames = new List<string>(this.connectorNames);
      clone.labels = new List<string>(this.labels);

      return clone;
    }
  }
}
