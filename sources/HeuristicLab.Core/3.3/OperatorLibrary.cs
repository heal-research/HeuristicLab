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

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a library of operators consisting of one <see cref="IOperatorGroup"/>.
  /// </summary>
  public class OperatorLibrary : ItemBase, IOperatorLibrary, IEditable {
    private IOperatorGroup myGroup;
    /// <summary>
    /// Gets the operator group of the current instance.
    /// </summary>
    public IOperatorGroup Group {
      get { return myGroup; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorLibrary"/>.
    /// </summary>
    public OperatorLibrary() {
      myGroup = new OperatorGroup();
    }

    /// <summary>
    /// Creates a new instance of <see cref="OperatorLibraryEditor"/> to display the current instance.
    /// </summary>
    /// <returns>The created view as <see cref="OperatorLibraryEditor"/>.</returns>
    public override IView CreateView() {
      return new OperatorLibraryEditor(this);
    }
    /// <summary>
    /// Creates a new instance of <see cref="OperatorLibraryEditor"/> to display the current instance.
    /// </summary>
    /// <returns>The created editor as <see cref="OperatorLibraryEditor"/>.</returns>
    public virtual IEditor CreateEditor() {
      return new OperatorLibraryEditor(this);
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorLibrary"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorLibrary clone = new OperatorLibrary();
      clonedObjects.Add(Guid, clone);
      clone.myGroup = (IOperatorGroup)Auxiliary.Clone(Group, clonedObjects);
      return clone;
    }

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.GetXmlNode"/> of base class <see cref="ItemBase"/>.<br/>
    /// The operator group is saved as a child node with the tag name <c>OperatorGroup</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("OperatorGroup", Group, document, persistedObjects));
      return node;
    }
    /// <summary>
    /// Loads the persisted operator library from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.<br/>
    /// See <see cref="GetXmlNode"/> for further information on how the data must be saved.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the operator library is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myGroup = (IOperatorGroup)PersistenceManager.Restore(node.SelectSingleNode("OperatorGroup"), restoredObjects);
    }
    #endregion
  }
}
