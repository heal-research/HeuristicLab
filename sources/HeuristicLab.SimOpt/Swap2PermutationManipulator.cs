using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class Swap2PermutationManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Swap two elements of a permutation or IntArray"; }
    }

    public Swap2PermutationManipulator()
      : base() {
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      if (item is Permutation.Permutation || item is IntArrayData) {
        IntArrayData data = (item as IntArrayData);
        int x = random.Next(data.Data.Length);
        int y;
        do {
          y = random.Next(data.Data.Length);
        } while (x == y);

        int h = data.Data[x];
        data.Data[x] = data.Data[y];
        data.Data[y] = h;
      } else throw new InvalidOperationException("ERROR: Swap2PermutationManipulator does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
