using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Bin Packing Solution (3d)", "Represents a solution for a 3D bin packing problem.")]
  [StorableClass]
  public class Solution : PackingPlan<PackingPosition, PackingShape, PackingItem> {
    public Solution(PackingShape binShape) : this(binShape, false, false) { }
    public Solution(PackingShape binShape, bool useExtremePoints, bool stackingConstraints) : base(binShape, useExtremePoints, stackingConstraints) { }
    [StorableConstructor]
    protected Solution(bool deserializing) : base(deserializing) { }
    protected Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Solution(this, cloner);
    }
  }
}