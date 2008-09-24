using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.SimOpt {
  public class SimOptSinglePointCrossover : CrossoverBase {

    public override string Description {
      get { return @"This operator applies a single point crossover on the variables defined"; }
    }

    public SimOptSinglePointCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Item", "The item list to be crossed", typeof(ConstrainedItemList), VariableKind.In));
    }

    protected override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      ICollection<IConstraint> violated;
      ConstrainedItemList p1 = parent1.GetVariableValue<ConstrainedItemList>("Item", false);
      ConstrainedItemList p2 = parent2.GetVariableValue<ConstrainedItemList>("Item", false);

      if (p1.Count != p2.Count) throw new InvalidOperationException("ERROR: the lists do not contain the same number of items");

      ConstrainedItemList childList = (ConstrainedItemList)p1.Clone();
      if (childList.Count > 1) {
        int iter = 0;
        do {
          childList.BeginCombinedOperation();
          int crossPoint = random.Next(1, childList.Count);
          for (int i = crossPoint; i < childList.Count; i++) {
            childList.TrySetAt(i, (IItem)p2[i].Clone(), out violated);
          }
        } while (!childList.EndCombinedOperation(out violated) && ++iter < 100);
        if (violated.Count == 0) {
          child.AddVariable(new Variable(parent1.TranslateName("Item"), childList));
        }
      }
    }
  }
}