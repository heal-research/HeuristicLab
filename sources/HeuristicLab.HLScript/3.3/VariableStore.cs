using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.HLScript {
  [Item("VariableStore", "Represents a variable store.")]
  [StorableClass]
  public class VariableStore : ObservableDictionary<string, object>, IContent {
    [StorableConstructor]
    protected VariableStore(bool deserializing) : base(deserializing) { }
    public VariableStore() : base() { }
  }
}
