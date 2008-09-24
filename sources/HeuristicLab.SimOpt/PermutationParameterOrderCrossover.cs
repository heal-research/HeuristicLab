using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class PermutationParameterOrderCrossover : SimOptCrossoverBase {
    public override string Description {
      get { return @"Applies an OX on a permutation parameter"; }
    }

    public PermutationParameterOrderCrossover()
      : base() {
    }

    protected override object Cross(IRandom random, object parent1, object parent2, IScope scope) {
      return Permutation.OrderCrossover.Apply(random, (int[])parent1, (int[])parent2);
    }
  }
}
