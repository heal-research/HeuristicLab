#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Problem", "Represents the base class for a problem.")]
  [StorableClass]
  public abstract class Problem : ParameterizedNamedItem, IProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    [StorableConstructor]
    protected Problem(bool deserializing) : base(deserializing) { }
    protected Problem(Problem original, Cloner cloner)
      : base(original, cloner) {
      operators = cloner.Clone(original.operators);
      RegisterEventHandlers();
    }

    protected Problem()
      : base() {
      operators = new OperatorCollection();
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      Operators.ItemsAdded += new CollectionItemsChangedEventHandler<IOperator>(Operators_Changed);
      Operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IOperator>(Operators_Changed);
      Operators.CollectionReset += new CollectionItemsChangedEventHandler<IOperator>(Operators_Changed);
    }

    #region properties
    private OperatorCollection operators;
    [Storable(Name = "Operators")]
    private IEnumerable<IOperator> StorableOperators {
      get { return operators; }
      set { operators = new OperatorCollection(value); }
    }
    protected OperatorCollection Operators {
      get { return this.operators; }
    }
    IEnumerable<IOperator> IProblem.Operators { get { return operators; } }
    #endregion

    #region events
    private void Operators_Changed(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    public event EventHandler OperatorsChanged;
    protected virtual void OnOperatorsChanged() {
      EventHandler handler = OperatorsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public event EventHandler Reset;
    protected virtual void OnReset() {
      EventHandler handler = Reset;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
