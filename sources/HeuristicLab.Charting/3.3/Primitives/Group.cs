#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.ObjectModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Charting {
  public class Group : PrimitiveBase, IGroup {
    private IList<IPrimitive> myPrimitives;
    public virtual ReadOnlyCollection<IPrimitive> Primitives {
      get { return new ReadOnlyCollection<IPrimitive>(myPrimitives); }
    }
    public virtual ReadOnlyCollection<IPrimitive> SelectedPrimitives {
      get {
        List<IPrimitive> selected = new List<IPrimitive>();
        foreach (IPrimitive primitive in myPrimitives) {
          if (primitive.Selected) selected.Add(primitive);
        }
        return new ReadOnlyCollection<IPrimitive>(selected);
      }
    }
    public override bool Selected {
      get { return base.Selected; }
      set {
        bool updateEnabled = UpdateEnabled;
        UpdateEnabled = false;
        foreach (IPrimitive primitive in myPrimitives)
          primitive.Selected = value;
        UpdateEnabled = updateEnabled;
        base.Selected = value;
      }
    }

    public Group(IChart chart)
      : base(chart) {
      myPrimitives = new List<IPrimitive>();
    }

    public virtual void Add(IPrimitive primitive) {
      if (Contains(primitive))
        throw new ArgumentException("Primitive already added");

      myPrimitives.Insert(0, primitive);
      primitive.Group = this;
      primitive.Update += new EventHandler(primitive_Update);
      OnUpdate();
    }
    public virtual void AddRange(IEnumerable<IPrimitive> primitives) {
      foreach (IPrimitive primitive in primitives) {
        if (Contains(primitive))
          throw new ArgumentException("Primitive already added");

        myPrimitives.Insert(0, primitive);
        primitive.Group = this;
        primitive.Update += new EventHandler(primitive_Update);
      }
      OnUpdate();
    }
    public virtual bool Contains(IPrimitive primitive) {
      return myPrimitives.Contains(primitive);
    }
    public virtual bool Remove(IPrimitive primitive) {
      if (myPrimitives.Remove(primitive)) {
        primitive.Group = null;
        primitive.Update -= new EventHandler(primitive_Update);
        OnUpdate();
        return true;
      } else {
        return false;
      }
    }
    public virtual void Clear() {
      foreach (IPrimitive primitive in myPrimitives) {
        primitive.Group = null;
        primitive.Update -= new EventHandler(primitive_Update);
      }
      myPrimitives.Clear();
      OnUpdate();
    }

    public virtual IPrimitive GetPrimitive(PointD point) {
      int i = 0;
      while ((i < myPrimitives.Count) && (!myPrimitives[i].ContainsPoint(point)))
        i++;
      if (i == myPrimitives.Count) return null;
      else return myPrimitives[i];
    }
    public IPrimitive GetPrimitive(double x, double y) {
      return GetPrimitive(new PointD(x, y));
    }
    public virtual IList<IPrimitive> GetAllPrimitives(PointD point) {
      List<IPrimitive> primitives = new List<IPrimitive>();
      for (int i = 0; i < myPrimitives.Count; i++) {
        if (myPrimitives[i].ContainsPoint(point)) {
          primitives.Add(myPrimitives[i]);
          if (myPrimitives[i] is IGroup) {
            primitives.AddRange(((IGroup)myPrimitives[i]).Primitives);
          }
        }
      }
      return primitives;
    }
    public IList<IPrimitive> GetAllPrimitives(double x, double y) {
      return GetAllPrimitives(new PointD(x, y));
    }

    public override void Move(Offset delta) {
      bool updateEnabled = UpdateEnabled;
      UpdateEnabled = false;
      foreach (IPrimitive primitive in myPrimitives)
        primitive.Move(delta);
      UpdateEnabled = updateEnabled;
      OnUpdate();
    }

    public override bool ContainsPoint(PointD point) {
      return GetPrimitive(point) != null;
    }

    public override Cursor GetCursor(PointD point) {
      IPrimitive primitive = GetPrimitive(point);
      if (primitive != null) return primitive.GetCursor(point);
      else return base.GetCursor(point);
    }
    public override string GetToolTipText(PointD point) {
      IPrimitive primitive = GetPrimitive(point);
      if (primitive != null) return primitive.GetToolTipText(point);
      else return base.GetToolTipText(point);
    }

    public void OneLayerUp(IPrimitive primitive) {
      if (!Contains(primitive))
        throw new ArgumentException("Primitive not found");

      int index = myPrimitives.IndexOf(primitive);
      if (index > 0) {
        myPrimitives.Remove(primitive);
        myPrimitives.Insert(index - 1, primitive);
        OnUpdate();
      }
    }
    public void OneLayerDown(IPrimitive primitive) {
      if (!Contains(primitive))
        throw new ArgumentException("Primitive not found");

      int index = myPrimitives.IndexOf(primitive);
      if (index < myPrimitives.Count - 1) {
        myPrimitives.Remove(primitive);
        myPrimitives.Insert(index + 1, primitive);
        OnUpdate();
      }
    }
    public void IntoForeground(IPrimitive primitive) {
      if (!Contains(primitive))
        throw new ArgumentException("Primitive not found");

      myPrimitives.Remove(primitive);
      myPrimitives.Insert(0, primitive);
      OnUpdate();
    }
    public void IntoBackground(IPrimitive primitive) {
      if (!Contains(primitive))
        throw new ArgumentException("Primitive not found");

      myPrimitives.Remove(primitive);
      myPrimitives.Add(primitive);
      OnUpdate();
    }

    public override void Draw(Graphics graphics) {
      for (int i = myPrimitives.Count - 1; i >= 0; i--) {
        myPrimitives[i].PreDraw(graphics);
        myPrimitives[i].Draw(graphics);
        myPrimitives[i].PostDraw(graphics);
      }
    }

    private void primitive_Update(object sender, EventArgs e) {
      OnUpdate();
    }
  }
}
