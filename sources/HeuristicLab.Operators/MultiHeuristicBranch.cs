using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  public class MultiHeuristicBranch: OperatorBase {
    public MultiHeuristicBranch()
      : base() {

      AddVariableInfo(new VariableInfo("Probabilities", "The probabilities, that define how likely each suboperator/graph is executed. This array must sum to 1", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "The pseudo random-generator, used for any random-decision.", typeof(IRandom), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleArrayData probabilities = GetVariableValue<DoubleArrayData>("Probabilities", scope, true);
      if(probabilities.Data.Length != SubOperators.Count) {
        throw new InvalidOperationException("MultiHeuristicBranch: The list of probabilities has to match the number of operators");
      }
      double sum = 0;
      foreach(double prob in probabilities.Data) {
        sum+=prob;
      }
      double r = random.NextDouble()*sum;
      sum = 0;
      IOperator successor = null;
      for(int i = 0; i < SubOperators.Count; i++) {
        sum += probabilities.Data[i];
        if(sum > r) {
          successor = SubOperators[i];
          break;
        }
      }
      if(successor == null) {
        throw new InvalidOperationException("MultiHeuristicBranch: There was a problem with the list of probabilities");
      }
      return new AtomicOperation(successor, scope);
    }
  }
}
