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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a graph of operators.
  /// </summary>
  public class OperatorGraph : ItemBase, IOperatorGraph {

    [Storable]
    private IDictionary<IOperator, IOperator> myOperators;
    /// <summary>
    /// Gets all operators of the current instance.
    /// </summary>
    public ICollection<IOperator> Operators {
      get { return myOperators.Values; }
    }

    [Storable]
    private IOperator myInitialOperator;
    /// <summary>
    /// Gets or sets the initial operator (the starting one).
    /// </summary>
    /// <remarks>Calls <see cref="OnInitialOperatorChanged"/> in the setter.</remarks>
    public IOperator InitialOperator {
      get { return myInitialOperator; }
      set {
        if (myInitialOperator != value) {
          myInitialOperator = value;
          OnInitialOperatorChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraph"/>.
    /// </summary>
    public OperatorGraph() {
      myOperators = new Dictionary<IOperator, IOperator>();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="cloner.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorGraph"/>.</returns>
    public override IItem Clone(ICloner cloner) {
      OperatorGraph clone = new OperatorGraph();
      cloner.RegisterClonedObject(this, clone);
      foreach (IOperator op in Operators)
        clone.AddOperator((IOperator)cloner.Clone(op));
      if (InitialOperator != null)
        clone.myInitialOperator = (IOperator)cloner.Clone(InitialOperator);
      return clone;
    }

    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnOperatorAdded"/>.</remarks>
    public void AddOperator(IOperator op) {
      if (!myOperators.ContainsKey(op)) {
        myOperators.Add(op, op);
        OnOperatorAdded(op);

        foreach (IOperator subOperator in op.SubOperators)
          AddOperator(subOperator);
      }
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnOperatorRemoved"/>.</remarks>
    public void RemoveOperator(IOperator op) {
      if (myOperators.ContainsKey(op)) {
        foreach (IOperator o in Operators) {
          int i = 0;
          while (i < o.SubOperators.Count) {
            if (o.SubOperators[i] == op)
              o.RemoveSubOperator(i);
            else
              i++;
          }
        }
        if (InitialOperator == op)
          InitialOperator = null;
        myOperators.Remove(op);
        OnOperatorRemoved(op);
      }
    }
    /// <inheritdoc/>
    public void Clear() {
      IOperator[] ops = new IOperator[Operators.Count];
      int i = 0;
      foreach (IOperator op in Operators) {
        ops[i] = op;
        i++;
      }
      for (int j = 0; j < ops.Length; j++)
        RemoveOperator(ops[j]);
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs<IOperator>> OperatorAdded;
    /// <summary>
    /// Fires a new <c>OperatorAdded</c> event.
    /// </summary>
    /// <param name="op">The operator that has been added.</param>
    protected virtual void OnOperatorAdded(IOperator op) {
      if (OperatorAdded != null)
        OperatorAdded(this, new EventArgs<IOperator>(op));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IOperator>> OperatorRemoved;
    /// <summary>
    /// Fires a new <c>OperatorRemoved</c> event.
    /// </summary>
    /// <param name="op">The operator that has been removed.</param>
    protected virtual void OnOperatorRemoved(IOperator op) {
      if (OperatorRemoved != null)
        OperatorRemoved(this, new EventArgs<IOperator>(op));
    }
    /// <inheritdoc/>
    public event EventHandler InitialOperatorChanged;
    /// <summary>
    /// Fires a new <c>InitialOperatorChanged</c> event.
    /// </summary>
    protected virtual void OnInitialOperatorChanged() {
      if (InitialOperatorChanged != null)
        InitialOperatorChanged(this, new EventArgs());
    }
  }
}
