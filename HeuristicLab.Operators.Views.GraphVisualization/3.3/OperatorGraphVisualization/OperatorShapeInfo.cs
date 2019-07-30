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

using System.Collections.Generic;
using System.Drawing;
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [StorableType("354155F1-6C42-42D3-99CB-9290CD11649B")]
  public class OperatorShapeInfo : ShapeInfo, IOperatorShapeInfo {
    [Storable]
    private List<string> labels;
    public IEnumerable<string> Labels {
      get { return labels; }
    }

    private object lockObject = new object();

    [StorableConstructor]
    protected OperatorShapeInfo(StorableConstructorFlag _) : base(_) { }

    protected OperatorShapeInfo(OperatorShapeInfo original, Cloner cloner)
      : base(original, cloner) {
      collapsed = original.collapsed;
      color = original.color;
      lineColor = original.lineColor;
      lineWidth = original.lineWidth;
      title = original.title;
      typeName = original.typeName;

      //mkommend: necessary because cloning a Bitmap is not threadsafe
      //see http://stackoverflow.com/questions/1851292/invalidoperationexception-object-is-currently-in-use-elsewhere for further information
      if (original.icon != null) {
        lock (original.lockObject) {
          icon = (Bitmap)original.icon.Clone();
        }
      }

      connectorNames = new List<string>(original.connectorNames);
      labels = new List<string>(original.labels);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OperatorShapeInfo(this, cloner);
    }

    public OperatorShapeInfo()
      : base() {
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
      if (!this.connectorNames.Contains(connectorName)) {
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
    private List<string> connectorNames;
    public override IEnumerable<string> Connectors {
      get { return this.connectorNames; }
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
    private string typeName;
    public string TypeName {
      get { return this.typeName; }
      set {
        if (this.typeName != value) {
          this.typeName = value;
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
  }
}
