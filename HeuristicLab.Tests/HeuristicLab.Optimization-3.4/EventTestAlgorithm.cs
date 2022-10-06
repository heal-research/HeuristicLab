using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Optimization;

namespace HeuristicLab.Optimization.Tests {
  public  class EventTestAlgorithm: BasicAlgorithm {

    public EventTestAlgorithm() {

    } 
    public EventTestAlgorithm(EventTestAlgorithm original, Cloner cloner): base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EventTestAlgorithm(this, cloner);
    }

    protected override void Run(CancellationToken cancellationToken) {
      //Do nothing
    }

    public override bool SupportsPause => false;
  }
}
