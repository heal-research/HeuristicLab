using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class DefaultManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Does not perform any manipulation, instead leaves the data as it is"; }
    }

    public override IOperation Apply(IScope scope) {
      return null;
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
    }
  }
}
