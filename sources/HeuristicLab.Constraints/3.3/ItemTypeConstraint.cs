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
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint that limits the type of a given item.
  /// </summary>
  /// <remarks>If the item is a <see cref="ConstrainedItemList"/>, any containing elements are limited to 
  /// the type and not the <see cref="ConstrainedItemList"/> itself.</remarks>
  public class ItemTypeConstraint : ConstraintBase {

    [Storable]
    private Type type;
    /// <summary>
    /// Gets or sets the type to which the items should be limited.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public Type Type {
      get { return type; }
      set {
        type = value;
        OnChanged();
      }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"The ItemTypeConstraint limits the type of a given item.
If the item is a ConstrainedItemList, any containing elements are limited to the type and not the ConstrainedItemList itself.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemTypeConstraint"/> with the <c>Type</c> property
    /// set to <see cref="ItemBase"/> as default.
    /// </summary>
    public ItemTypeConstraint() {
      type = typeof(ItemBase);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemTypeConstraint"/> with the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type the items should be limited to.</param>
    public ItemTypeConstraint(Type type) {
      this.type = type;
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      ConstrainedItemList list = (data as ConstrainedItemList);
      if (list != null) {
        for (int i = 0; i < list.Count; i++)
          if (!list[i].GetType().Equals(type)) return false;
        return true;
      }
      return data.GetType().Equals(type);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ItemTypeConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="ItemTypeConstraintView"/>.</returns>
    public override IView CreateView() {
      return new ItemTypeConstraintView(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="ItemTypeConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemTypeConstraint clone = new ItemTypeConstraint(type);
      clonedObjects.Add(Guid, clone);
      return clone;
    }
  }
}
