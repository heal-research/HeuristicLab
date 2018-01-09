#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  internal class LayoutNode<T> : object where T : class {
    public float Width { get; set; }
    public float Height { get; set; }

    public LayoutNode<T> NextLeft {
      get {
        return Children == null ? Thread : Children.First();
      }
    }
    public LayoutNode<T> NextRight {
      get {
        return Children == null ? Thread : Children.Last();
      }
    }
    public LayoutNode<T> LeftSibling {
      get {
        if (Parent == null) return null;
        return Number == 0 ? null : Parent.Children[Number - 1];
      }
    }
    public LayoutNode<T> LeftmostSibling {
      get {
        if (Parent == null) return null;
        return Number == 0 ? null : Parent.Children[0];
      }
    }

    public LayoutNode<T> Thread { get; set; }
    public LayoutNode<T> Ancestor { get; set; }
    public LayoutNode<T> Parent { get; set; }
    public List<LayoutNode<T>> Children { get; set; }
    public float Mod { get; set; }
    public float Prelim { get; set; }
    public float Change { get; set; }
    public float Shift { get; set; }
    public int Number { get; set; }
    public int Level { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

    public bool IsLeaf {
      get { return Children == null || Children.Count == 0; }
    }

    private T content;
    public T Content {
      get { return content; }
      set {
        if (value == null)
          throw new ArgumentNullException("LayoutNode: Content cannot be null.");
        content = value;
      }
    }
    /// <summary>
    /// Translate the position of the layout node according to the given offsets
    /// </summary>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    public void Translate(float dx, float dy) {
      X += dx;
      Y += dy;
    }

    public void ResetCoordinates() {
      X = 0;
      Y = 0;
    }

    /// <summary>
    /// Reset layout-related parameters
    /// </summary>
    public void Reset() {
      Ancestor = this;
      Thread = null;
      Change = 0;
      Shift = 0;
      Prelim = 0;
      Mod = 0;
      ResetCoordinates();
    }
  }
}
