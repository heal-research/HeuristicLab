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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  /// <summary>
  /// A class representing all basic data types having some constraints.
  /// </summary>
  public class ConstrainedObjectData : ConstrainedItemBase, IObjectData {

    [Storable]
    private object myData;
    /// <summary>
    /// Gets or sets the object with constraints to represent.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/> in the setter.
    /// </remarks>
    public virtual object Data {
      get { return myData; }
      set {
        if (myData != value) {
          myData = value;
          OnChanged();
        }
      }
    }

    /// <summary>
    /// Assigns the new <paramref name="data"/> if it is valid according to the constraints.
    /// </summary>
    /// <param name="data">The data to assign.</param>
    /// <returns><c>true</c> if the new <paramref name="data"/> could be assigned, <c>false</c> otherwise.</returns>
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
    /// <summary>
    /// Assigns the new object <paramref name="data"/> if it is valid according to the constraints.
    /// </summary>
    /// <param name="data">The data to assign.</param>
    /// <param name="violatedConstraints">Output parameter, containing all constraints that could not be fulfilled.</param>
    /// <returns><c>true</c> if the new <paramref name="data"/> could be assigned, <c>false</c> otherwise.</returns>
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

    /// <summary>
    /// Clones the current object.
    /// </summary>
    /// <remarks>HeuristicLab data items are cloned with the <see cref="HeuristicLab.Core.Auxiliary.Clone"/> method of 
    /// class <see cref="Auxiliary"/> (deep copy), all other items (like basic data types) 
    /// are cloned with their own <c>Clone</c> methods (shadow copy).</remarks>
    /// <param name="clonedObjects">All already cloned objects.</param>
    /// <returns>The cloned object as <see cref="ConstrainedObjectData"/>.</returns>
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

    /// <summary>
    /// Compares the current instance to the given <paramref name="obj"/>.
    /// </summary>
    /// <remarks>Can also compare basic data types with their representing HeuristicLab data types 
    /// (e.g. a <see cref="ConstrainedDoubleData"/> with a double value).</remarks>
    /// <exception cref="InvalidOperationException">Thrown when the current 
    /// instance does not implement the interface <see cref="IComparable"/></exception>
    /// <param name="obj">The object to compare the current instance to.</param>
    /// <returns><c>0</c> if the objects are the same, a value smaller than zero when the current instance
    /// is smaller than the given <paramref name="obj"/> and a value greater than zero 
    /// when the current instance is greater than the given <paramref name="obj"/>.</returns>
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

    /// <summary>
    /// The string representation of the current instance.
    /// </summary>
    /// <returns>The current instance as a string.</returns>
    public override string ToString() {
      return Data.ToString();
    }
  }
}
