using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class StateTransistor : OperatorBase {
    public override string Description {
      get { return @"Evaluates the state transition conditions one by one and advances to the state specified as target state in the first condition that results true.
If the choice on the transition was made and there are suboperators attached, it will call its first suboperator upon successful transition, otherwise the second suboperator is the next to be executed."; }
    }

    public StateTransistor()
      : base() {
      AddVariableInfo(new VariableInfo("CurrentState", "", typeof(ProtocolState), VariableKind.In | VariableKind.Out | VariableKind.New));
      AddVariableInfo(new VariableInfo("ConditionIndex", "", typeof(IntData), VariableKind.New | VariableKind.In | VariableKind.Out | VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("Result", "", typeof(BoolData), VariableKind.New | VariableKind.In | VariableKind.Out | VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      ProtocolState currentState = GetVariableValue<ProtocolState>("CurrentState", scope, true);
      // Terminate as soon as an accepting state is reached
      if (currentState.AcceptingState.Data) return null;

      ItemList<StateTransition> transitionCondition = currentState.StateTransitions;
      if ((transitionCondition == null || transitionCondition.Count < 1) && !currentState.AcceptingState.Data)
        throw new InvalidOperationException("ERROR: A dead-end state (" + currentState.Name.Data + ") has been reached that is not an accepting state");

      IVariableInfo conditionIndexInfo = GetVariableInfo("ConditionIndex");
      IntData conditionIndex = GetVariableValue<IntData>("ConditionIndex", scope, false, false);
      if (conditionIndex == null) {
        Variable conditionIndexVar = null;
        if (conditionIndexInfo.Local) {
          conditionIndexVar = new Variable(conditionIndexInfo.ActualName, new IntData(0));
          AddVariable(conditionIndexVar);
        } else {
          conditionIndexVar = new Variable(scope.TranslateName(conditionIndexInfo.FormalName), new IntData(0));
          scope.AddVariable(conditionIndexVar);
        }
        conditionIndex = (IntData)conditionIndexVar.Value;
      }

      BoolData result = GetVariableValue<BoolData>("Result", scope, false, false);
      if (result != null) {
        bool done = false;
        if (result.Data) {
          IVariable csVar = scope.GetVariable(scope.TranslateName("CurrentState"));
          if (csVar == null) scope.AddVariable(new Variable(scope.TranslateName("CurrentState"), ((StateTransition)transitionCondition[conditionIndex.Data - 1]).TargetState));
          else csVar.Value = ((StateTransition)transitionCondition[conditionIndex.Data - 1]).TargetState;
          done = true;
        }
        if (done || conditionIndex.Data == transitionCondition.Count) {
          //conditionIndex.Data = 0;
          IVariableInfo resultInfo = GetVariableInfo("Result");
          if (resultInfo.Local) RemoveVariable(resultInfo.ActualName);
          else scope.RemoveVariable(scope.TranslateName(resultInfo.FormalName));
          if (conditionIndexInfo.Local) RemoveVariable(conditionIndexInfo.ActualName);
          else scope.RemoveVariable(scope.TranslateName(conditionIndexInfo.FormalName));
        }
        if (!done && conditionIndex.Data == transitionCondition.Count) {
          if (SubOperators.Count > 1) return new AtomicOperation(SubOperators[1], scope);
          else return null;
        } else {
          if (SubOperators.Count > 0) return new AtomicOperation(SubOperators[0], scope);
          else return null;
        }
      }
      SequentialProcessor sp = new SequentialProcessor();
      ProgrammableOperator tmp = (ProgrammableOperator)((StateTransition)transitionCondition[conditionIndex.Data]).TransitionCondition;
      tmp.GetVariableInfo("Result").ActualName = GetVariableInfo("Result").ActualName;
      sp.AddSubOperator(tmp);

      StateTransistor nextTransistor = new StateTransistor();
      nextTransistor.GetVariableInfo("CurrentState").ActualName = GetVariableInfo("CurrentState").ActualName;
      nextTransistor.GetVariableInfo("Result").ActualName = GetVariableInfo("Result").ActualName;
      if (conditionIndexInfo.Local) {
        nextTransistor.AddVariable(new Variable(conditionIndexInfo.ActualName, conditionIndex));
        nextTransistor.GetVariableInfo("ConditionIndex").Local = true;
      }
      nextTransistor.GetVariableInfo("ConditionIndex").ActualName = conditionIndexInfo.ActualName;
      conditionIndex.Data++;

      for (int i = 0 ; i < SubOperators.Count ; i++) {
        nextTransistor.AddSubOperator(SubOperators[i]);
      }

      sp.AddSubOperator(nextTransistor);
      return new AtomicOperation(sp, scope);
    }
  }
}
