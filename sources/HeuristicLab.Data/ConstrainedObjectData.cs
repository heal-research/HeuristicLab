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
using System.Text;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public class ConstrainedObjectData : ConstrainedItemBase, IObjectData {
    private object myData;
    public virtual object Data {
      get { return myData; }
      set {
        if (myData != value) {
          myData = value;
          OnChanged();
        }
      }
    }

    public virtual bool TrySetData(object data) {
      if (myData != data) {
        object backup = myData;
        myData = data;
        if (IsValid()) {
          OnChanged();
          return true;
        } else {
          myData = backup;
          return false;
        }
      }
      return true;
    }
    public virtual bool TrySetData(object data, out ICollection<IConstraint> violatedConstraints) {
      if (myData != data) {
        object backup = myData;
        myData = data;
        if (IsValid(out violatedConstraints)) {
          OnChanged();
          return true;
        } else {
          myData = backup;
          return false;
        }
      }
      violatedConstraints = new List<IConstraint>();
      return true;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedObjectData clone = (ConstrainedObjectData)base.Clone(clonedObjects);
      if (Data is IStorable)
        clone.myData = Auxiliary.Clone((IStorable)Data, clonedObjects);
      else if (Data is ICloneable)
        clone.myData = ((ICloneable)Data).Clone();
      else
        throw new InvalidOperationException("contained object is not cloneable");
      return clone;
    }

    public int CompareTo(object obj) {
      IComparable comparable = Data as IComparable;
      if (comparable != null) {
        IObjectData other = obj as IObjectData;
        if (other != null)
          return comparable.CompareTo(other.Data);
        else
          return comparable.CompareTo(obj);
      }
      throw new InvalidOperationException("Cannot compare as contained object doesn't implement IComparable");
    }

    public override string ToString() {
      return Data.ToString();
    }

    public virtual void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
