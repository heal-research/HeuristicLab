using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class DefaultInitializer : SimOptInitializationOperatorBase {
    public override string Description {
      get { return @"Does not perform initialization of the variable, but leaves it as it is"; }
    }

    public override IOperation Apply(IScope scope) {
      return null;
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
    }
  }
}
