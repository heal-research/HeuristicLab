#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  [Item("InstrumentedOperator", "A operator that can execute pre- and post actions.")]
  [StorableClass]
  public abstract class InstrumentedOperator : SingleSuccessorOperator {
    private const string BeforeExecutionOperatorsParameterName = "BeforeExecutionOperators";
    private const string AfterExecutionOperatorsParameterName = "AfterExecutionOperators";

    private IFixedValueParameter<ItemList<SingleSuccessorOperator>> BeforeExecutionOperatorsParameter {
      get { return (IFixedValueParameter<ItemList<SingleSuccessorOperator>>)Parameters[BeforeExecutionOperatorsParameterName]; }
    }
    private IFixedValueParameter<ItemList<SingleSuccessorOperator>> AfterExecutionOperatorsParameter {
      get { return (IFixedValueParameter<ItemList<SingleSuccessorOperator>>)Parameters[AfterExecutionOperatorsParameterName]; }
    }

    public ItemList<SingleSuccessorOperator> BeforeExecutionOperators {
      get { return BeforeExecutionOperatorsParameter.Value; }
    }
    public ItemList<SingleSuccessorOperator> AfterExecutionOperators {
      get { return AfterExecutionOperatorsParameter.Value; }
    }

    [StorableConstructor]
    protected InstrumentedOperator(bool deserializing) : base(deserializing) { }
    protected InstrumentedOperator(InstrumentedOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    protected InstrumentedOperator()
      : base() {
      Parameters.Add(new FixedValueParameter<ItemList<SingleSuccessorOperator>>(BeforeExecutionOperatorsParameterName, "Actions that are executed before the execution of the operator", new ItemList<SingleSuccessorOperator>()));
      Parameters.Add(new FixedValueParameter<ItemList<SingleSuccessorOperator>>(AfterExecutionOperatorsParameterName, "Actions that are executed after the execution of the operator", new ItemList<SingleSuccessorOperator>()));
      BeforeExecutionOperatorsParameter.Hidden = true;
      AfterExecutionOperatorsParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(BeforeExecutionOperatorsParameterName)) {
        Parameters.Add(new FixedValueParameter<ItemList<SingleSuccessorOperator>>(BeforeExecutionOperatorsParameterName, "Actions that are executed before the execution of the operator", new ItemList<SingleSuccessorOperator>()));
        BeforeExecutionOperatorsParameter.Hidden = true;
      }
      if (!Parameters.ContainsKey(AfterExecutionOperatorsParameterName)) {
        Parameters.Add(new FixedValueParameter<ItemList<SingleSuccessorOperator>>(AfterExecutionOperatorsParameterName, "Actions that are executed after the execution of the operator", new ItemList<SingleSuccessorOperator>()));
        AfterExecutionOperatorsParameter.Hidden = true;
      }
      #endregion
    }

    public sealed override IOperation Apply() {
      var opCol = new OperationCollection();

      //build before operations
      foreach (var beforeAction in BeforeExecutionOperators) {
        IOperation beforeActionOperation = ExecutionContext.CreateOperation(beforeAction);
        opCol.Add(beforeActionOperation);
      }
      //build operation for the instrumented apply
      opCol.Add(CreateInstrumentedOperation(this));
      return opCol;
    }

    public virtual IOperation InstrumentedApply() {
      var opCol = new OperationCollection();
      foreach (var afterAction in AfterExecutionOperators) {
        IOperation afterActionOperation = ExecutionContext.CreateOperation(afterAction);
        opCol.Add(afterActionOperation);
      }
      if (Successor != null)
        opCol.Add(ExecutionContext.CreateOperation(Successor));
      return opCol;
    }
  }
}
