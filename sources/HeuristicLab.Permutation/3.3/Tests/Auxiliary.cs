using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Permutation.Tests {
  public static class Auxiliary {
    public static bool PermutationIsEqualByPosition(Permutation p1, Permutation p2) {
      bool equal = (p1.Length == p2.Length);
      if (equal) {
        for (int i = 0; i < p1.Length; i++) {
          if (p1[i] != p2[i]) {
            equal = false;
            break;
          }
        }
      }
      return equal;
    }
  }
}
