using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class PermutationParameterCyclicCrossover : SimOptCrossoverBase {
    public override string Description {
      get { return @"Applies a CX on a permutation parameter"; }
    }

    public PermutationParameterCyclicCrossover()
      : base() {
    }

    protected override object Cross(IRandom random, object parent1, object parent2, IScope scope) {
      return Permutation.CyclicCrossover.Apply(random, (int[])parent1, (int[])parent2);
    }
  }
}
