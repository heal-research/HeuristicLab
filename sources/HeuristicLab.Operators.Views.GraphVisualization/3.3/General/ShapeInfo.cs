#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using Netron.Diagramming.Core;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [StorableClass]
  public abstract class ShapeInfo : DeepCloneable, IShapeInfo {
    [StorableConstructor]
    protected ShapeInfo(bool deserializing) : base() { }
    protected ShapeInfo(ShapeInfo original, Cloner cloner)
      : base(original, cloner) {
      shapeType = original.shapeType;
      location = original.location;
    }

    protected ShapeInfo(Type shapeType) {
      if (!typeof(IShape).IsAssignableFrom(shapeType))
        throw new ArgumentException("The passed shape type " + shapeType + " must be derived from IShape.");
      this.shapeType = shapeType;
    }

    [Storable]
    private Type shapeType;
    public Type ShapeType {
      get { return this.shapeType; }
    }

    [Storable]
    private Point location;
    public Point Location {
      get { return this.location; }
      set {
        if (this.location != value) {
          this.location = value;
          this.OnChanged();
        }
      }
    }

    public abstract IEnumerable<string> Connectors { get; }

    public event EventHandler Changed;
    protected virtual void OnChanged() {
      EventHandler handler = this.Changed;
      if (handler != null) this.Changed(this, EventArgs.Empty);
    }

    public virtual IShape CreateShape() {
      IShape shape = (IShape)Activator.CreateInstance(this.shapeType);
      shape.Tag = this;
      shape.Location = this.Location;
      return shape;
    }

    public virtual void UpdateShape(IShape shape) {
      shape.Location = this.Location;
    }

    public virtual void UpdateShapeInfo(IShape shape) {
      this.Location = shape.Location;
    }
  }
}
