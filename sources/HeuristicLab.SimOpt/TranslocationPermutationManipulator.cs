using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.SimOpt {
  public class TranslocationPermutationManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Move a certain number of consecutive elements to a different part in an IntArray or Permutation."; }
    }

    public TranslocationPermutationManipulator()
      : base() {
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      if (item is Permutation.Permutation || item is IntArrayData) {
        IntArrayData data = (item as IntArrayData);
        int l = random.Next(1, data.Data.Length - 2);
        int x = random.Next(data.Data.Length - l);
        int y;
        do {
          y = random.Next(data.Data.Length - l);
        } while (x == y);

        int[] h = new int[l];
        for (int i = 0; i < h.Length; i++)
          h[i] = data.Data[x + i];

        if (x > y) {
          while (x > y) {
            x--;
            data.Data[x + l] = data.Data[x];
          }
        } else {
          while (x < y) {
            data.Data[x] = data.Data[x + l];
            x++;
          }
        }
        for (int i = 0; i < h.Length; i++)
          data.Data[y + i] = h[i];
      } else throw new InvalidOperationException("ERROR: PermutationTranslocationManipulator does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
