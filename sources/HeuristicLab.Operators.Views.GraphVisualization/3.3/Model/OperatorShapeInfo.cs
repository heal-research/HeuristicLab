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

namespace HeuristicLab.Operators.Views.GraphVisualization {
  internal class OperatorShapeInfo : ShapeInfo {

    private HashSet<string> connectorNames;
    public OperatorShapeInfo()
      : base(typeof(OperatorShape)) {
      this.connectorNames = new HashSet<string>();
    }

    public OperatorShapeInfo(IEnumerable<string> connectorNames)
      : this() {
      foreach (string connectorName in connectorNames)
        this.connectorNames.Add(connectorName);
    }

    public void AddConnector(string connectorName) {
      if (this.connectorNames.Add(connectorName))
        this.OnChanged();
    }

    public void RemoveConnector(string connectorName) {
      if (this.connectorNames.Remove(connectorName))
        this.OnChanged();
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
  }
}
