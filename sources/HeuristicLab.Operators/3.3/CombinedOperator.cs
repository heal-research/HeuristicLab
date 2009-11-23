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

namespace HeuristicLab.Operators {
  /// <summary>
  /// Contains an operator graph and automatically injects its sub-operators into the scope it is 
  /// applied on (useful for modularization to assemble complex operators out of simpler ones).
  /// </summary>
  public class CombinedOperator : DelegatingOperator {

    [Storable]
    private string myDescription;
    /// <summary>
    /// Gets the description of the current instance.
    /// </summary>
    public override string Description {
      get { return myDescription; }
    }

    [Storable]
    private IOperatorGraph myOperatorGraph;
    /// <summary>
    /// Gets the operator graph of the current instance.
    /// </summary>
    public IOperatorGraph OperatorGraph {
      get { return myOperatorGraph; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CombinedOperator"/>.
    /// </summary>
    public CombinedOperator()
      : base() {
      myDescription =
        @"A combined operator contains a whole operator graph. It is useful for modularization to assemble complex operators out of simpler ones.

A combined operator automatically inject its sub-operators into the scope it is applied on. Thereby the names of the sub-operators are used as variable names. Those operators can be extracted again in the contained operator graph by using an OperatorExtractor. So it is possible to parameterize a combined operator with custom operators.";
      myOperatorGraph = new OperatorGraph();
    }

    /// <summary>
    /// Sets the description of the current instance.
    /// </summary>
    /// <remarks>Calls <see cref="OnDescriptionChanged"/>.</remarks>
    /// <exception cref="NullReferenceException">Thrown when <paramref name="description"/> is <c>null</c>.</exception>
    /// <param name="description">The description to set.</param>
    public void SetDescription(string description) {
      if (description == null)
        throw new NullReferenceException("description must not be null");

      if (description != myDescription) {
        myDescription = description;
        OnDescriptionChanged();
      }
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBase.Clone
    /// (System.Collections.Generic.IDictionary&lt;System.Guid, object&gt;)"/> 
    /// of base class <see cref="DelegatingOperator"/>.<br/>
    /// Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="CombinedOperator"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      CombinedOperator clone = (CombinedOperator)base.Clone(clonedObjects);
      clone.myDescription = Description;
      clone.myOperatorGraph = (IOperatorGraph)Auxiliary.Clone(OperatorGraph, clonedObjects);
      return clone;
    }

    /// <summary>
    /// Adds all sub operators to the specified <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to inject the sub operators.</param>
    /// <returns><c>null</c> if the initial operator is <c>nulll</c>, else a new 
    /// <see cref="AtomicOperation"/> with the initial operator and the given <paramref name="scope"/>.</returns>
    public override IOperation Apply(IScope scope) {
      if (OperatorGraph.InitialOperator != null) {
        for (int i = 0; i < SubOperators.Count; i++) {
          if (scope.GetVariable(SubOperators[i].Name) != null)
            scope.RemoveVariable(SubOperators[i].Name);
          scope.AddVariable(new Variable(SubOperators[i].Name, SubOperators[i]));
        }
        return new AtomicOperation(OperatorGraph.InitialOperator, scope);
      } else {
        return null;
      }
    }

    /// <summary>
    /// Occurs when the description of the current instance has been changed.
    /// </summary>
    public event EventHandler DescriptionChanged;
    /// <summary>
    /// Fires a new <c>DescriptionChanged</c> event.
    /// </summary>
    protected virtual void OnDescriptionChanged() {
      if (DescriptionChanged != null)
        DescriptionChanged(this, new EventArgs());
    }
  }
}
