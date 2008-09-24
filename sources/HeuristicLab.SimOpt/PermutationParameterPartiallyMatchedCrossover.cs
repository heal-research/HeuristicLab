using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  class PermutationParameterPartiallyMatchedCrossover : SimOptCrossoverBase {
    public override string Description {
      get { return @"Applies a PMX on a permutation parameter"; }
    }

    public PermutationParameterPartiallyMatchedCrossover()
      : base() {
    }

    protected override object Cross(IRandom random, object parent1, object parent2, IScope scope) {
      return Permutation.PartiallyMatchedCrossover.Apply(random, (int[])parent1, (int[])parent2);
    }
  }
}
