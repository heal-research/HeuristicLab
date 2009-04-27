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
  /// Represents a single operation with one operator and one scope.
  /// </summary>
  public class AtomicOperation : ItemBase, IOperation {

    [Storable]
    private IOperator myOperator;
    /// <summary>
    /// Gets the current operator as <see cref="IOperator"/>.
    /// </summary>
    public IOperator Operator {
      get { return myOperator; }
    }

    [Storable]
    private IScope myScope;
    /// <summary>
    /// Gets the current scope as <see cref="IScope"/>.
    /// </summary>
    public IScope Scope {
      get { return myScope; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="AtomicOperation"/>.
    /// </summary>
    public AtomicOperation() { }
    /// <summary>
    /// Initializes a new instance of <see cref="AtomicOperation"/> with the given <paramref name="op"/> 
    /// and the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="op">The operator to assign.</param>
    /// <param name="scope">The scope to assign.</param>
    public AtomicOperation(IOperator op, IScope scope) {
      myOperator = op;
      myScope = scope;
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>The operator and the scope objects are cloned with the 
    /// <see cref="HeuristicLab.Core.Auxiliary.Clone"/> method of the <see cref="Auxiliary"/> class.</remarks>
    /// <param name="clonedObjects">All already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="AtomicOperation"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      AtomicOperation clone = new AtomicOperation();
      clonedObjects.Add(Guid, clone);
      clone.myOperator = (IOperator)Auxiliary.Clone(Operator, clonedObjects);
      clone.myScope = (IScope)Auxiliary.Clone(Scope, clonedObjects);
      return clone;
    }
  }
}
