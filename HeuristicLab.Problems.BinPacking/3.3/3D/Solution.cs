using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Problems.BinPacking;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Bin Packing Solution (3d)", "Represents a solution for a 3D bin packing problem.")]
  [StorableType("C0620C6F-3882-45CD-976F-4840ABD08BCD")]
  public class Solution : PackingPlan<PackingPosition, PackingShape, PackingItem> {
    public Solution(PackingShape binShape) : this(binShape, false, false) { }
    public Solution(PackingShape binShape, bool useExtremePoints, bool stackingConstraints) : base(binShape, useExtremePoints, stackingConstraints) { }
    [StorableConstructor]
    protected Solution(StorableConstructorFlag _) : base(_) { }
    protected Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Solution(this, cloner);
    }
  }
}