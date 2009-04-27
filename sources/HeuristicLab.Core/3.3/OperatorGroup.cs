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
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Representation of a group of operators (can also include subgroups).
  /// </summary>
  public class OperatorGroup : StorableBase, IOperatorGroup {

    [Storable]
    private string myName;
    /// <summary>
    /// Gets or sets the name of the current operator group.
    /// </summary>
    /// <remarks>Calls <see cref="OnNameChanged"/> in the setter.</remarks>
    public string Name {
      get { return myName; }
      set {
        if (myName != value) {
          myName = value;
          OnNameChanged();
        }
      }
    }

    [Storable]
    private List<IOperatorGroup> mySubGroups;
    /// <summary>
    /// Gets all subgroups of the current instance.
    /// <note type="caution"> The subgroups are returned read-only.</note>
    /// </summary>
    public ICollection<IOperatorGroup> SubGroups {
      get { return mySubGroups.AsReadOnly(); }
    }

    [Storable]
    private List<IOperator> myOperators;
    /// <summary>
    /// Gets all operators of the current instance.
    /// <note type="caution"> The operators are returned read-only.</note>
    /// </summary>
    public ICollection<IOperator> Operators {
      get { return myOperators.AsReadOnly(); }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGroup"/> having "Anonymous" as name.
    /// </summary>
    public OperatorGroup() {
      myName = "Anonymous";
      mySubGroups = new List<IOperatorGroup>();
      myOperators = new List<IOperator>();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone with <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorGroup"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorGroup clone = (OperatorGroup)base.Clone(clonedObjects);
      clone.myName = Name;
      foreach (IOperatorGroup group in SubGroups)
        clone.AddSubGroup((IOperatorGroup)Auxiliary.Clone(group, clonedObjects));
      foreach (IOperator op in Operators)
        clone.AddOperator((IOperator)Auxiliary.Clone(op, clonedObjects));
      return clone;
    }

    /// <summary>
    /// Adds the given subgroup (<paramref name="group"/>) to the current instance.
    /// </summary>
    /// <param name="group">The subgroup to add.</param>
    public virtual void AddSubGroup(IOperatorGroup group) {
      mySubGroups.Add(group);
    }
    /// <summary>
    /// Removes the given subgroup (<paramref name="group"/>) from the current instance.
    /// </summary>
    /// <param name="group">The subgroup to remove.</param>
    public virtual void RemoveSubGroup(IOperatorGroup group) {
      mySubGroups.Remove(group);
    }
    /// <summary>
    /// Ads the given operator <paramref name="op"/> to the current instance.
    /// </summary>
    /// <param name="op">The operator to add.</param>
    public virtual void AddOperator(IOperator op) {
      myOperators.Add(op);
    }
    /// <summary>
    /// Removes the given operator <paramref name="op"/> from the current instance.
    /// </summary>
    /// <param name="op">The operator to remove.</param>
    public virtual void RemoveOperator(IOperator op) {
      myOperators.Remove(op);
    }

    /// <summary>
    /// Occurs when the name of the operator was changed.
    /// </summary>
    public event EventHandler NameChanged;
    /// <summary>
    /// Fires a new <c>NameChanged</c> event.
    /// </summary>
    protected virtual void OnNameChanged() {
      if (NameChanged != null) {
        NameChanged(this, new EventArgs());
      }
    }
  }
}
