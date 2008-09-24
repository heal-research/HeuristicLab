using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class RandomDoubleInitializer : SimOptInitializationOperatorBase {
    public override string Description {
      get { return @"Initializes a DoubleData or ConstrainedDoubleData randomly in a given interval"; }
    }

    public RandomDoubleInitializer()
      : base() {
      AddVariableInfo(new VariableInfo("Min", "Minimum of the desired value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Max", "Maximum of the desired value", typeof(DoubleData), VariableKind.In));
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      double min = GetVariableValue<DoubleData>("Min", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Max", scope, true).Data;
      if (item is DoubleData) {
        double r = random.NextDouble();
        ((DoubleData)item).Data = min + (r * max - r * min);
        return;
      } else if (item is ConstrainedDoubleData) {
        ConstrainedDoubleData data = (item as ConstrainedDoubleData);
        for (int tries = 100; tries >= 0; tries--) {
          double r = random.NextDouble();
          double newValue = min + (r * max - r * min);

          if (IsIntegerConstrained(data)) newValue = Math.Round(newValue);
          if (data.TrySetData(newValue)) return;
        }
        throw new InvalidProgramException("ERROR: RandomDoubleInitializer couldn't find a valid value in 100 tries");
      } else throw new InvalidOperationException("ERROR: RandomDoubleInitializer does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }
  }
}
