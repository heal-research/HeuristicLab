using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Clients.Hive.Views {
  [StorableClass]
  public class StateLogList : ItemList<StateLog> {

    [StorableConstructor]
    protected StateLogList(bool deserializing) : base(deserializing) { }
    public StateLogList() : base() { }
    protected StateLogList(StateLogList original, Cloner cloner) : base(original, cloner) { }
    public StateLogList(IEnumerable<StateLog> collection)
      : base(collection) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new StateLogList(this, cloner);
    }

  }
}
