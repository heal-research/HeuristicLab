#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A base class for operators which apply arbitrary many other operators of a specific type that can be checked or unchecked.
  /// </summary>
  [Item("CheckedMultiOperator", "A base class for operators which apply arbitrary many other operators of a specific type that can be checked or unchecked.")]
  [StorableClass]
  public abstract class CheckedMultiOperator<T> : MultiOperator<T>, ICheckedMultiOperator<T> where T : class, IOperator {
    /// <summary>
    /// Gets the operators of the checked multi operator
    /// </summary>
    public new ICheckedItemList<T> Operators {
      get { return (ICheckedItemList<T>)base.Operators; }
      protected set { base.Operators = value; }
    }

    [StorableConstructor]
    protected CheckedMultiOperator(bool deserializing) : base(deserializing) { }
    protected CheckedMultiOperator(CheckedMultiOperator<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    /// <summary>
    /// Creates a new instance of CheckedMultiOperator
    /// </summary>
    public CheckedMultiOperator()
      : base() {
      Operators = new CheckedItemList<T>();
    }

    public override void CollectParameterValues(IDictionary<string, IItem> values) {
      foreach (var param in Parameters.OfType<IValueParameter>().Except(OperatorParameters)) {
        var children = GetCollectedValues(param);
        foreach (var c in children) {
          if (String.IsNullOrEmpty(c.Key))
            values.Add(param.Name, c.Value);
          else values.Add(param.Name + "." + c.Key, c.Value);
        }
      }
      foreach (var opParam in OperatorParameters) {
        var op = opParam.Value;
        var @checked = Operators.ItemChecked(op);
        if (!@checked) continue;
        var children = GetCollectedValues(opParam);
        foreach (var c in children) {
          if (String.IsNullOrEmpty(c.Key))
            values.Add(opParam.Name, c.Value);
          else values.Add(opParam.Name + "." + c.Key, c.Value);
        }
      }
    }
  }
}
