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
using HeuristicLab.Collections;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  internal abstract class ShapeInfo : Item,IShapeInfo {
    private ShapeInfo() {

    }
    protected ShapeInfo(Type shapeType) {
      if (!typeof(IShape).IsAssignableFrom(shapeType))
        throw new ArgumentException("The passed shape type " + shapeType + " must be derived from IShape.");
      this.shapeType = shapeType;
    }

    public event EventHandler Changed;

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

    protected void OnChanged() {
      if (this.Changed != null)
        this.Changed(this, EventArgs.Empty);
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
  }
}
