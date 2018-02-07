using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.Graph {
  [Item("Vertex", "An object representing a vertex in the graph. It can have a text label, a weight, and an additional data object.")]
  [StorableClass]
  public class VertexWithItemId : Vertex {
    [Storable]
    private int _itemId;

    public int ItemId {
      get { return _itemId; }
      protected set { _itemId = value; }
    }

    [StorableConstructor]
    public VertexWithItemId(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public VertexWithItemId(int itemId)
      : base() {
      _itemId = itemId;
    }
        
    protected VertexWithItemId(Vertex original, Cloner cloner)
      : base(original, cloner) {
      if (original is VertexWithItemId) {
        _itemId = ((VertexWithItemId)original).ItemId;
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VertexWithItemId(this, cloner);
    }

    public override string ToString() {
      return $"Vertex: {ItemId}";
    }
  }
}
