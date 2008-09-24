using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.SimOpt {
  public class SimOptDiscreteMultiCrossover : MultiCrossoverBase {

    public override string Description {
      get { return @"This operator applies a discrete recombination on the variables defined"; }
    }

    public SimOptDiscreteMultiCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Item", "The item list to be recombined", typeof(ConstrainedItemList), VariableKind.In));
    }

    protected override void Cross(IScope scope, IRandom random, IScope[] parents, IScope child) {
      ICollection<IConstraint> violated;

      ConstrainedItemList[] p = new ConstrainedItemList[parents.Length];
      for (int i = 0; i < p.Length; i++) {
        p[i] = parents[i].GetVariableValue<ConstrainedItemList>("Item", false);
        if (i > 0 && p[i].Count != p[i-1].Count) throw new InvalidOperationException("ERROR: the lists do not contain the same number of items");
      }

      ConstrainedItemList childList = (ConstrainedItemList)p[0].Clone();
      if (childList.Count > 1) {
        int iter = 0;
        do {
          childList.BeginCombinedOperation();
          for (int i = 0; i < childList.Count; i++) {
            int nextParent = random.Next(0, parents.Length);
            if (nextParent > 0) childList.TrySetAt(i, (IItem)p[nextParent].Clone(), out violated);
          }
        } while (!childList.EndCombinedOperation(out violated) && ++iter < 100);
        if (violated.Count == 0) {
          child.AddVariable(new Variable(parents[0].TranslateName("Item"), childList));
        }
      }
    }
  }
}