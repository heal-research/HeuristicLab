#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Problem", "Represents the base class for a problem.")]
  [StorableClass]
  public abstract class Problem : ParameterizedNamedItem, IProblem {
    private const string OperatorsParameterName = "Operators";
    public IFixedValueParameter<ItemCollection<IItem>> OperatorsParameter {
      get { return (IFixedValueParameter<ItemCollection<IItem>>)Parameters[OperatorsParameterName]; }
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    [StorableConstructor]
    protected Problem(bool deserializing) : base(deserializing) { }
    protected Problem(Problem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected Problem()
      : base() {
      Parameters.Add(new FixedValueParameter<ItemCollection<IItem>>(OperatorsParameterName, "The operators and items that the problem provides to the algorithms.", new ItemCollection<IItem>(), false));
      OperatorsParameter.Hidden = true;
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      IParameter operatorsParam;
      if (Parameters.TryGetValue(OperatorsParameterName, out operatorsParam)) {
        var operators = operatorsParam.ActualValue as OperatorCollection;
        if (operators != null) {
          Parameters.Remove(OperatorsParameterName);
          Parameters.Add(new FixedValueParameter<ItemCollection<IItem>>(OperatorsParameterName, "The operators and items that the problem provides to the algorithms.", new ItemCollection<IItem>(operators), false));
          OperatorsParameter.Hidden = true;
        }
      }
      #endregion

      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      Operators.ItemsAdded += new CollectionItemsChangedEventHandler<IItem>(Operators_Changed);
      Operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IItem>(Operators_Changed);
      Operators.CollectionReset += new CollectionItemsChangedEventHandler<IItem>(Operators_Changed);
    }

    #region properties
    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [Storable(Name = "Operators", AllowOneWay = true)]
    private IEnumerable<IOperator> StorableOperators {
      set {
        IParameter operatorsParam;
        if (Parameters.TryGetValue(OperatorsParameterName, out operatorsParam)) {
          var items = operatorsParam.ActualValue as ItemCollection<IItem>;
          if (items == null) Parameters.Remove(operatorsParam);
        }

        //necessary to convert old experiments files where no parameter was used for saving the operators
        if (!Parameters.ContainsKey(OperatorsParameterName)) {
          Parameters.Add(new FixedValueParameter<ItemCollection<IItem>>(OperatorsParameterName, "The operators and items that the problem provides to the algorithms.", new ItemCollection<IItem>(), false));
          OperatorsParameter.Hidden = true;
        }
        if (value != null) Operators.AddRange(value);
      }
    }
    #endregion
    protected ItemCollection<IItem> Operators {
      get {
        // BackwardsCompatibility3.3
        #region Backwards compatible code, remove with 3.4
        if (!Parameters.ContainsKey(OperatorsParameterName)) {
          Parameters.Add(new FixedValueParameter<ItemCollection<IItem>>(OperatorsParameterName, "The operators and items that the problem provides to the algorithms.", new ItemCollection<IItem>(), false));
          OperatorsParameter.Hidden = true;
        }
        #endregion
        return OperatorsParameter.Value;
      }
    }
    IEnumerable<IItem> IProblem.Operators { get { return GetOperators(); } }

    protected virtual IEnumerable<IItem> GetOperators() {
      return Operators;
    }

    public virtual IEnumerable<IParameterizedItem> ExecutionContextItems {
      get { yield return this; }
    }
    #endregion

    protected override IEnumerable<KeyValuePair<string, IItem>> GetCollectedValues(IValueParameter param) {
      var children = base.GetCollectedValues(param);
      foreach (var child in children) {
        if (child.Value is IOperator)
          yield return new KeyValuePair<string, IItem>(child.Key, new StringValue(((IOperator)child.Value).Name));
        else yield return child;
      }
    }

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
