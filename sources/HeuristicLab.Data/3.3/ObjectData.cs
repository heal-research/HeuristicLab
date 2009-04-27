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
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Data {
  /// <summary>
  /// Represents the base class for all base data types.
  /// </summary>
  public class ObjectData : ItemBase, IObjectData {

    [Storable]
    private object myData;
    /// <summary>
    /// Gets or sets the data to represent.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
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
    /// Clones the current instance.
    /// </summary>
    /// <remarks>HeuristicLab data items are cloned with the <see cref="HeuristicLab.Core.Auxiliary.Clone"/> method of 
    /// class <see cref="Auxiliary"/> (deep copy), all other items (like basic data types) 
    /// are cloned with their own <c>Clone</c> methods (shadow copy).</remarks>
    /// <exception cref="InvalidOperationException">Thrown when the current instance is not cloneable.</exception>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The clone instance.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ObjectData clone = (ObjectData)base.Clone(clonedObjects);
      if (Data is IStorable)
        clone.myData = Auxiliary.Clone((IStorable)Data, clonedObjects);
      else if (Data is ICloneable)
        clone.myData = ((ICloneable)Data).Clone();
      else
        throw new InvalidOperationException("contained object is not cloneable");
      return clone;
    }

    /// <summary>
    /// Checks whether the current instance equals the specified <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c>, if both objects are the same or 
    /// the contained data is the same, <c>false</c> otherwise.</returns>
    public override bool Equals(object obj) {
      if(obj == this) return true; // same instance
      IObjectData other = obj as IObjectData;
      if(other != null)
        return Data.Equals(other.Data); // are the contained Data the same?
      else
        return false;
    }

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() {
      return Data.GetHashCode();
    }

    /// <summary>
    /// Compares the current instance to the given <paramref name="obj"/>.
    /// </summary>
    /// <remarks>Can also compare basic data types with their representing HeuristicLab data types 
    /// (e.g. a <see cref="BoolData"/> with a boolean value).</remarks>
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
    /// <returns>"null" if property <see cref="Data"/> is <c>null</c>, else 
    /// the string representation of the current instance.</returns>
    public override string ToString() {
      if (Data == null)
        return "null";
      else
        return Data.ToString();
    }
  }
}
