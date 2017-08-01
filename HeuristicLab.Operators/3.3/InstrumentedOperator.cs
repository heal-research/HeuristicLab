#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  [Item("InstrumentedOperator", "A operator that can execute pre- and post actions.")]
  [StorableClass]
  public abstract class InstrumentedOperator : SingleSuccessorOperator, IInstrumentedOperator {
    private const string BeforeExecutionOperatorsParameterName = "BeforeExecutionOperators";
    private const string AfterExecutionOperatorsParameterName = "AfterExecutionOperators";

    private IFixedValueParameter<OperatorList> BeforeExecutionOperatorsParameter {
      get { return (IFixedValueParameter<OperatorList>)Parameters[BeforeExecutionOperatorsParameterName]; }
    }
    private IFixedValueParameter<OperatorList> AfterExecutionOperatorsParameter {
      get { return (IFixedValueParameter<OperatorList>)Parameters[AfterExecutionOperatorsParameterName]; }
    }


    IEnumerable<IOperator> IInstrumentedOperator.BeforeExecutionOperators { get { return BeforeExecutionOperators; } }
    public OperatorList BeforeExecutionOperators {
      get { return BeforeExecutionOperatorsParameter.Value; }
    }
    IEnumerable<IOperator> IInstrumentedOperator.AfterExecutionOperators { get { return AfterExecutionOperators; } }
    public OperatorList AfterExecutionOperators {
      get { return AfterExecutionOperatorsParameter.Value; }
    }

    [StorableConstructor]
    protected InstrumentedOperator(bool deserializing) : base(deserializing) { }
    protected InstrumentedOperator(InstrumentedOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    protected InstrumentedOperator()
      : base() {
      Parameters.Add(new FixedValueParameter<OperatorList>(BeforeExecutionOperatorsParameterName, "Actions that are executed before the execution of the operator", new OperatorList()));
      Parameters.Add(new FixedValueParameter<OperatorList>(AfterExecutionOperatorsParameterName, "Actions that are executed after the execution of the operator", new OperatorList()));
      BeforeExecutionOperatorsParameter.Hidden = true;
      BeforeExecutionOperatorsParameter.GetsCollected = false;
      AfterExecutionOperatorsParameter.Hidden = true;
      AfterExecutionOperatorsParameter.GetsCollected = false;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(BeforeExecutionOperatorsParameterName)) {
        Parameters.Add(new FixedValueParameter<OperatorList>(BeforeExecutionOperatorsParameterName, "Actions that are executed before the execution of the operator", new OperatorList()));
        BeforeExecutionOperatorsParameter.Hidden = true;
      }
      if (!Parameters.ContainsKey(AfterExecutionOperatorsParameterName)) {
        Parameters.Add(new FixedValueParameter<OperatorList>(AfterExecutionOperatorsParameterName, "Actions that are executed after the execution of the operator", new OperatorList()));
        AfterExecutionOperatorsParameter.Hidden = true;
      }
      #endregion
    }

    protected override IEnumerable<KeyValuePair<string, IItem>> GetCollectedValues(IValueParameter param) {
      foreach (var b in base.GetCollectedValues(param)) yield return b;
      if (param != BeforeExecutionOperatorsParameter && param != AfterExecutionOperatorsParameter) yield break;
      var operatorList = (OperatorList)param.Value;
      var counter = 0;
      foreach (var op in operatorList) {
        yield return new KeyValuePair<string, IItem>(counter.ToString(), op);
        var children = new Dictionary<string, IItem>();
        op.CollectParameterValues(children);
        foreach (var c in children) yield return new KeyValuePair<string, IItem>(counter + "." + c.Key, c.Value);
        counter++;
      }
    }

    public sealed override IOperation Apply() {
      //to speed up the execution call instrumented apply directly if no before operators exists
      if (!BeforeExecutionOperators.Any())
        return InstrumentedApply();

      //build before operations
      var opCol = new OperationCollection();
      foreach (var beforeAction in BeforeExecutionOperators) {
        var beforeActionOperation = ExecutionContext.CreateChildOperation(beforeAction);
        opCol.Add(beforeActionOperation);
      }
      //build operation for the instrumented apply
      opCol.Add(CreateInstrumentedOperation(this));
      return opCol;
    }

    public virtual IOperation InstrumentedApply() {
      if (!AfterExecutionOperators.Any()) {
        if (Successor != null) return ExecutionContext.CreateOperation(Successor);
        return null;
      }

      var opCol = new OperationCollection();
      foreach (var afterAction in AfterExecutionOperators) {
        var afterActionOperation = ExecutionContext.CreateChildOperation(afterAction);
        opCol.Add(afterActionOperation);
      }

      if (Successor != null)
        opCol.Add(ExecutionContext.CreateOperation(Successor));
      return opCol;
    }
  }
}
