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
  public class OperatorLibrary : ItemBase, IOperatorLibrary, IEditable {
    private IOperatorGroup myGroup;
    public IOperatorGroup Group {
      get { return myGroup; }
    }

    public OperatorLibrary() {
      myGroup = new OperatorGroup();
    }

    public override IView CreateView() {
      return new OperatorLibraryEditor(this);
    }
    public virtual IEditor CreateEditor() {
      return new OperatorLibraryEditor(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorLibrary clone = new OperatorLibrary();
      clonedObjects.Add(Guid, clone);
      clone.myGroup = (IOperatorGroup)Auxiliary.Clone(Group, clonedObjects);
      return clone;
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("OperatorGroup", Group, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myGroup = (IOperatorGroup)PersistenceManager.Restore(node.SelectSingleNode("OperatorGroup"), restoredObjects);
    }
    #endregion
  }
}
