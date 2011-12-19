using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Clients.Hive.Views {
  [StorableClass]
  public class StateLogListList : ItemList<StateLogList> {

    [StorableConstructor]
    protected StateLogListList(bool deserializing) : base(deserializing) { }
    public StateLogListList() : base() { }
    protected StateLogListList(StateLogListList original, Cloner cloner) : base(original, cloner) { }
    public StateLogListList(IEnumerable<StateLogList> collection)
      : base(collection) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new StateLogListList(this, cloner);
    }

  }
}
