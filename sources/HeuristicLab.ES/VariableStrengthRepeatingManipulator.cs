using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.ES {
  public class VariableStrengthRepeatingManipulator : OperatorBase {
    public override string Description {
      get { return @"Applies its suboperator a number of times depending on the ShakingFactor"; }
    }

    public VariableStrengthRepeatingManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShakingFactor", "Determines the strength of the mutation (repeated application of the operator)", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      double shakingFactor = scope.GetVariableValue<DoubleData>("ShakingFactor", true).Data;
      NormalDistributedRandom N = new NormalDistributedRandom(random, 1.0, shakingFactor);
      int strength = (int)Math.Ceiling(Math.Abs(N.NextDouble()));
      if (strength == 0) strength = 1;

      CompositeOperation co = new CompositeOperation();
      co.ExecuteInParallel = false;
      for (int i = 0; i < strength; i++) {
        co.AddOperation(new AtomicOperation(SubOperators[0], scope));
      }

      return co;
    }
  }
}
