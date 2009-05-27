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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.GP {
  public class GPOperatorLibrary : ItemBase, IOperatorLibrary, IEditable {
    // constants for variable names
    internal const string MIN_TREE_HEIGHT = "MinTreeHeight";
    internal const string MIN_TREE_SIZE = "MinTreeSize";
    internal const string TICKETS = "Tickets";

    [Storable]
    private GPOperatorGroup group;

    public GPOperatorGroup GPOperatorGroup {
      get { return group; }
    }
    #region IOperatorLibrary Members

    public IOperatorGroup Group {
      get { return group; }
    }

    #endregion


    public GPOperatorLibrary()
      : base() {
      group = new GPOperatorGroup();
    }

    public override IView CreateView() {
      return new GPOperatorLibraryEditor(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      GPOperatorLibrary clone = (GPOperatorLibrary)base.Clone(clonedObjects);
      clone.group = (GPOperatorGroup)group.Clone(clonedObjects);
      return clone;
    }

    #region IEditable Members

    public IEditor CreateEditor() {
      return new GPOperatorLibraryEditor(this);
    }

    #endregion
  }
}
